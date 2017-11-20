namespace EsrNhsHubFileTransferService.Code.Transfers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using EsrGo2FromNhsWcfLibrary.Enum;

    using EsrNhsHubFileTransferService.Code.Database;
    using EsrNhsHubFileTransferService.Code.FTP;
    using EsrNhsHubFileTransferService.Code.Helpers;
    using EsrNhsHubFileTransferService.EsrFileProcessor;

    using Utilities.Cryptography;

    /// <summary>
    ///     Class to move files from any store locations to the Esr File Processor
    /// </summary>
    public static class Transfers
    {
        #region Methods

        /// <summary>
        ///     Starts the transfer process
        /// </summary>
        internal static void Begin()
        {
            CheckFtpLocations();
        }

        /// <summary>
        ///     Check an individual ftp server location for ESR files to delete or transfer
        /// </summary>
        /// <param name="ftpServer">An object containing the FTP Host, Path, Username and Password</param>
        private static void CheckFtpLocation(FtpServer ftpServer)
        {
            var ftpClient = new FtpClient();
            
            try
            {
                foreach (KeyValuePair<int, string> file in GetFilesToDeleteFromFtp(ref ftpServer))
                {
                    DeleteFileFromFtp(ref ftpServer, ref ftpClient, file.Key, file.Value);
                }
            }
            catch (Exception exception)
            {
                Log.ImportantError("Error whilst iterating file deletions - {0}\n\n{1}", exception.Message, exception.InnerException.Message);
            }

            try
            {
                foreach (string filename in GetFilesToTransferFromFtp(ref ftpServer, ref ftpClient))
                {
                    TransferFileFromFtp(ref ftpServer, ref ftpClient, filename);
                }
            }
            catch (Exception exception)
            {
                Log.ImportantError("Error whilst iterating file transfers - {0}\n\nInner Exception -\n{1}", exception.Message, exception.InnerException != null ? exception.InnerException.Message : "null");
            }
        }

        /// <summary>
        ///     Obtains the ftp server location list and starts the checks for each
        /// </summary>
        private static void CheckFtpLocations()
        {
            using (var entities = new EsrNhsHubDatabase())
            {
                IQueryable<FtpServer> ftpServerList = (from server in entities.ftpLocations where server.archived == false select new FtpServer { FtpLocationId = server.ftpLocationId, Hostname = server.host, Path = server.path, Username = server.username, Password = server.password });

                foreach (FtpServer ftp in ftpServerList)
                {
                    CheckFtpLocation(ftp);
                }
            }
        }

        /// <summary>
        ///     Remove an individual file from the FTP location
        /// </summary>
        /// <param name="ftpServer">The ftp server details</param>
        /// <param name="ftpClient">The ftp client connection</param>
        /// <param name="fileId">The id of the file to be deleted</param>
        /// <param name="filename">The full ftp name of the file to be deleted (often includes the full FTP path)</param>
        private static void DeleteFileFromFtp(ref FtpServer ftpServer, ref FtpClient ftpClient, int fileId, string filename)
        {
            Log.Information("Attempting to delete '{0}'", filename);

            try
            {
                if (ftpClient.DeleteFileFromFtp(ftpServer, filename.StartsWith(ftpServer.Path) ? filename.Replace(ftpServer.Path + "/", "") : filename))
                {
                    Log.SuccessAudit("'{0}' deleted", filename);
                    LogDeletedFile(ref ftpServer, fileId, filename);
                }
                else
                {
                    Log.FailureAudit("'{0}' did not delete", filename);
                }
            }
            catch (Exception exception)
            {
                Log.ImportantError("Error whilst trying to delete '{0}' - {1}", filename, exception.Message);
            }
        }

        /// <summary>
        ///     Removes any files from a directory listing that should not be attempted to be transferred
        /// </summary>
        /// <param name="ftpServer">The ftp server details</param>
        /// <param name="fileList">The list of filenames retrieved from the FTP location</param>
        /// <returns>The filtered list</returns>
        private static List<string> FilterTransferredFiles(ref FtpServer ftpServer, List<string> fileList)
        {
            int ftpLocationId = ftpServer.FtpLocationId;

            // This is the file extension for files that are currently being constructed in the Hub
            fileList.RemoveAll(s => s.ToUpper().EndsWith("._TXFR"));

            using (var db = new EsrNhsHubDatabase())
            {
                // Remove any files from the listing that have already been transferred
                List<string> removeFiles = (from file in db.transferredFiles
                                            where file.ftpLocationId == ftpLocationId
                                                && !file.transferAgain
                                                && !file.deleted
                                                && fileList.Contains(file.fileName)
                                            select file.fileName).ToList();



                // Establish if any transferAgain files were not in the files available for download
                // This has not been refactored to LINQ as it causes timeout issues in production
                List<transferredFile> unavailableFiles = new List<transferredFile>();

                foreach (var transferredFile in db.transferredFiles)
                {
                    if (transferredFile.ftpLocationId == ftpLocationId && transferredFile.transferAgain && !transferredFile.deleted && !fileList.Contains(transferredFile.fileName))
                    {
                        unavailableFiles.Add(transferredFile);
                    }
                }

                foreach (transferredFile file in unavailableFiles)
                {
                    file.transferStatus = (byte)EsrHubStatus.EsrHubTransferStatus.FileNotAvailableForDownload;
                    file.transferAgain = false;

                    db.SaveChanges();
                }

                fileList.RemoveAll(removeFiles.Contains);

                return fileList;
            }
        }

        /// <summary>
        ///     Checks for any transferred files that are ready for deletion
        /// </summary>
        /// <remarks>
        ///     A file will be sent for deletion if the following -
        ///         it is for the current location that is being checked,
        ///         and it has a Delete After date and time that has passed,
        ///         and it hasn't already been deleted
        /// </remarks>
        /// <param name="ftpServer">The ftp server details</param>
        /// <returns>The file ids and names to delete</returns>
        private static Dictionary<int, string> GetFilesToDeleteFromFtp(ref FtpServer ftpServer)
        {
            int ftpLocationId = ftpServer.FtpLocationId;

            Log.Information("Using FTP server at '{0}' with the path '{1}' (ftpLocationId '{2}')", ftpServer.Hostname, ftpServer.Path, ftpLocationId);

            using (var entities = new EsrNhsHubDatabase())
            {
                Dictionary<int, string> filesToDelete = (from file in entities.transferredFiles
                                                         where file.ftpLocationId == ftpLocationId
                                                             && file.deleteAfter.HasValue
                                                             && file.deleteAfter.Value < DateTime.UtcNow
                                                             && !file.deleted
                                                         select file).ToDictionary(x => x.fileId, x => x.fileName);

                Log.Information("{0} file(s) to delete", filesToDelete.Count);

                return filesToDelete;
            }
        }

        /// <summary>
        ///     Checks for files on the ftp location that are waiting to be downloaded
        /// </summary>
        /// <param name="ftpServer">The ftp server details</param>
        /// <param name="ftpClient">The ftp client connection</param>
        /// <returns>An enumerable collection of ftp file names</returns>
        private static IEnumerable<string> GetFilesToTransferFromFtp(ref FtpServer ftpServer, ref FtpClient ftpClient)
        {
            int ftpLocationId = ftpServer.FtpLocationId;
            Log.Information("Using FTP server at '{0}' with the path '{1}' (ftpLocationId '{2}')", ftpServer.Hostname, ftpServer.Path, ftpLocationId);

            List<string> filesToDownload = FilterTransferredFiles(ref ftpServer, ftpClient.GetFtpDirectoryFileList(ftpServer));

            Log.Information("{0} file(s) to download", filesToDownload.Count);

            return filesToDownload;
        }

        /// <summary>
        ///     Update the transferred file entry to mark it as having been deleted
        /// </summary>
        /// <remarks>
        ///     Will mark one file that matches -
        ///         the location that we are currently processing,
        ///         and the file id that was provided
        ///         and the file name that was provided
        ///     and it will error if it finds more than one file
        /// </remarks>
        /// <param name="ftpServer">The ftp server details</param>
        /// <param name="fileId">The id of the deleted file</param>
        /// <param name="filename">The full ftp name of the deleted file (often includes the full FTP path)</param>
        /// <returns>The number of rows affected or -1 for failure</returns>
        private static int LogDeletedFile(ref FtpServer ftpServer, int fileId, string filename)
        {
            int ftpLocationId = ftpServer.FtpLocationId;

            using (var entities = new EsrNhsHubDatabase())
            {
                transferredFile deletedFile = (from file in entities.transferredFiles
                                               where file.ftpLocationId == ftpLocationId
                                                   && file.fileId == fileId
                                                   && file.fileName == filename
                                               select file).SingleOrDefault();

                if (deletedFile != null)
                {
                    deletedFile.deleted = true;

                    return entities.SaveChanges();
                }
            }

            Log.ImportantError("Error whilst trying to log fileId = '{0}' with filename = '{1}' as deleted", fileId, filename);
            return -1;
        }

        /// <summary>
        ///     Record the details of a downloaded file in the transferredFiles table in the esrNhsHub database
        /// </summary>
        /// <remarks>
        ///     Manages the entry in the transferred files table for the downloaded file, does one of 3 things -
        /// 
        ///         Transmitting a file because it has previously failed to transmit or process properly -
        ///             if the file name has been recorded before in this location
        ///             and it is currently set to "transfer again"
        ///             and hasn't been processed or deleted,
        ///             then it has its "date last collected" updated and the "transfer again" status is removed
        /// 
        ///         Forcing a re-transmittion of a file that has previously been transferred and processed successfully -
        ///             if the file name has been recorded before in this location
        ///             and it is currently set to "transfer again"
        ///             and it has been processed
        ///             and it hasn't been deleted,
        ///             then it has it is marked as having been deleted and the "transfer again" status is removed,
        ///             but a new entry is copied to track the new transfer and processing status of the same file
        ///             and it inherits the "delete after" date of the original
        /// 
        ///         Transferring a newly encountered file -
        ///             if the file name has not been encountered before in this location
        ///             then a new row is added to track the new transfer and processing
        /// </remarks>
        /// <param name="ftpServer">The ftp server details</param>
        /// <param name="filename">The full ftp name of the downloaded file (often includes the full FTP path)</param>
        /// <returns>The file id reference for the transfer</returns>
        private static int LogDownloadedFile(ref FtpServer ftpServer, string filename)
        {
            DateTime dateCollected = DateTime.UtcNow;
            int ftpLocationId = ftpServer.FtpLocationId;

            using (var entities = new EsrNhsHubDatabase())
            {
                transferredFile transferredFile = (from file in entities.transferredFiles
                                                   where file.ftpLocationId == ftpLocationId
                                                       && file.fileName == filename
                                                       && file.transferAgain
                                                       && !file.processed
                                                       && !file.deleted
                                                   orderby file.firstCollectedOn descending
                                                   select file).FirstOrDefault();

                if (transferredFile != null)
                {
                    transferredFile.lastCollectedOn = dateCollected;
                    transferredFile.transferAgain = false;
                    entities.SaveChanges();

                    Log.Information("Downloaded and transferring file id - '{0}'", transferredFile.fileId);

                    return transferredFile.fileId;
                }

                transferredFile retransferredFile = (from file in entities.transferredFiles
                                                     where file.ftpLocationId == ftpLocationId
                                                         && file.fileName == filename
                                                         && file.transferAgain
                                                         && file.processed
                                                         && !file.deleted
                                                     orderby file.firstCollectedOn descending
                                                     select file).FirstOrDefault();

                if (retransferredFile != null)
                {
                    retransferredFile.transferAgain = false;
                    retransferredFile.deleted = true;

                    Log.Information("Downloaded and re-transferring file id - '{0}'", retransferredFile.fileId);
                }

                transferredFile = new transferredFile
                                  {
                                      ftpLocationId = ftpLocationId,
                                      fileName = filename,
                                      firstCollectedOn = dateCollected,
                                      lastCollectedOn = dateCollected,
                                      transferAttempts = 0,
                                      transferAgain = false,
                                      deleteAfter = (retransferredFile != null && retransferredFile.deleteAfter.HasValue)
                                          ? retransferredFile.deleteAfter.Value
                                          : (DateTime?)null,
                                      deleted = false
                                  };

                entities.transferredFiles.Add(transferredFile);

                if (entities.SaveChanges() > 0 && transferredFile.fileId > 0)
                {
                    if (retransferredFile != null)
                    {
                        Log.Information("Adding new entry for re-transfer of file id - '{0}' - as new id - '{1}'", retransferredFile.fileId, transferredFile.fileId);
                    }
                    else
                    {
                        Log.Information("Newly downloaded and attempting transfer of new file id - '{0}'", transferredFile.fileId);
                    }
                    return transferredFile.fileId;
                }
            }

            Log.ImportantError("When trying to log a collected file, the fileId could not be found from a saved file record, a file record may have failed to be saved for filename = '{0}' collected on '{1}'", filename, dateCollected);
            return -1;
        }

        /// <summary>
        ///     Sets a file to "transfer again" if transmission fails
        /// </summary>
        /// <remarks>
        ///     If the file cannot be found in this location by its ID,
        ///     then it looks for an entry with the same file name in this location
        ///         that is not set to "transfer again"
        ///         and has not been processed or deleted
        ///     it then sets it to "transfer again"
        /// </remarks>
        /// <param name="ftpServer">The ftp server details</param>
        /// <param name="fileId">The id of the deleted file</param>
        /// <param name="filename">The full ftp name of the deleted file (often includes the full FTP path)</param>
        /// <returns>The number of rows affected or -1 for an error</returns>
        private static int LogNonTransferredFile(ref FtpServer ftpServer, int fileId, string filename)
        {
            int ftpLocationId = ftpServer.FtpLocationId;

            using (var entities = new EsrNhsHubDatabase())
            {
                transferredFile transferredFile = (from file in entities.transferredFiles
                                                   where file.ftpLocationId == ftpLocationId
                                                        && file.fileId == fileId
                                                   select file).SingleOrDefault()
                                                   ??
                                                   (from file in entities.transferredFiles
                                                    where file.ftpLocationId == ftpLocationId
                                                        && file.fileName == filename
                                                        && !file.transferAgain
                                                        && !file.processed
                                                        && !file.deleted
                                                    select file).FirstOrDefault();

                if (transferredFile != null)
                {
                    transferredFile.transferAgain = true;

                    Log.Information("Marking a file that failed to transfer ready for transfer again with fileId = '{0}', found file id - '{1}'", fileId, transferredFile.fileId);

                    return entities.SaveChanges();
                }
            }

            Log.ImportantError("When trying to log a file that failed to transfer, a file with fileId = '{0}' could not be found, also an unprocessed file of fileName = '{1}' with transferAgain set to false could not be found", fileId, filename);
            return -1;
        }


        /// <summary>
        ///     Sets a file to "transfer again" if transmission fails
        /// </summary>
        /// <remarks>
        ///     If the file cannot be found in this location by its ID,
        ///     then it looks for an entry with the same file name in this location
        ///         that is not set to "transfer again"
        ///         and has not been processed
        ///     it then increments the transfer attempts counter
        /// </remarks>
        /// <param name="ftpServer">The ftp server details</param>
        /// <param name="fileId">The id of the deleted file</param>
        /// <param name="filename">The full ftp name of the deleted file (often includes the full FTP path)</param>
        /// <returns>The number of rows affected or -1 for an error</returns>
        private static int LogTransferredFile(ref FtpServer ftpServer, int fileId, string filename)
        {
            int ftpLocationId = ftpServer.FtpLocationId;

            using (var entities = new EsrNhsHubDatabase())
            {
                transferredFile transferredFile = (from file in entities.transferredFiles
                                                   where file.ftpLocationId == ftpLocationId
                                                        && file.fileId == fileId
                                                   select file).SingleOrDefault()
                                                   ??
                                                   (from file in entities.transferredFiles
                                                    where file.ftpLocationId == ftpLocationId
                                                        && file.fileName == filename
                                                        && !file.transferAgain
                                                        && !file.processed
                                                    select file).FirstOrDefault();

                if (transferredFile != null)
                {
                    transferredFile.transferAttempts++;

                    Log.Information("Marking a file transfer attempt for fileId - '{0}', found file id - '{1}'", fileId, transferredFile.fileId);

                    return entities.SaveChanges();
                }
                
            }

            Log.ImportantError("When trying to log a file that failed to transfer, a file with fileId = '{0}' could not be found, also an unprocessed file of fileName = '{1}' with transferAgain set to false could not be found", fileId, filename);
            return -1;
        }

        /// <summary>
        ///     Takes a file by name from the ftp location, downloads it and performs minimal validation, then transfers it to the file processor
        /// </summary>
        /// <param name="ftpServer">The ftp server details</param>
        /// <param name="ftpClient">The ftp client connection</param>
        /// <param name="filename">The full ftp name of the deleted file (often includes the full FTP path)</param>
        private static void TransferFileFromFtp(ref FtpServer ftpServer, ref FtpClient ftpClient, string filename)
        {
            Log.Information("Starting transfer of '{0}'", filename);

            byte[] fileContent = ftpClient.DownloadOutboundFile(ftpServer, filename.StartsWith(ftpServer.Path) ? filename.Replace(ftpServer.Path + "/", "") : filename);
            int fileId = LogDownloadedFile(ref ftpServer, filename);

            Log.Information("'{0}' is {1} bytes in length", filename, fileContent.Length);
            Log.Information("'{0}' content is -\n\n\n{1}", filename, Encoding.ASCII.GetString(fileContent).Substring(0, (fileContent.Length > 24576 ? 24576 : fileContent.Length)));

            try
            {
                var esrFileProcessorClient = new EsrFileProcessorClient("EsrRouterToProcessorEndpoint");
                string fileContentsString = Encoding.UTF8.GetString(fileContent);
                string encryptedFileContentsString = ExpensesCryptography.Encrypt(fileContentsString);
                byte[] encryptedFileContentsArray = Encoding.UTF8.GetBytes(encryptedFileContentsString);
                Log.Information("'Encrypted {0}' is {1} bytes in length", filename, encryptedFileContentsArray.Length);

                using (var memoryStream = new MemoryStream(encryptedFileContentsArray))
                {
                    string returnMessage;

                    if (esrFileProcessorClient.ProcessStreamedFile(encryptedFileContentsArray.Length, fileId, filename, memoryStream, out returnMessage))
                    {
                        Log.SuccessAudit("'{0}' transferred, the response was '{1}'", filename, returnMessage);
                        LogTransferredFile(ref ftpServer, fileId, filename);
                    }
                    else
                    {
                        Log.FailureAudit("'{0}' did not transfer, the reason was '{1}'", filename, returnMessage);
                        LogNonTransferredFile(ref ftpServer, fileId, filename);
                    }
                }
            }
            catch (Exception exception)
            {
                Log.ImportantError("Error whilst trying to connect to the relay service and send '{0}' - {1}", filename, exception.Message);
                LogNonTransferredFile(ref ftpServer, fileId, filename);
            }
        }

        /// <summary>
        ///     Used to perform a reduced, simple (and quick) validation of an ESR v2 file contents
        /// </summary>
        /// <param name="fileContent">The contents extracted from an ESR file</param>
        /// <param name="returnMessage">A return message indicating the reason for pass/fail</param>
        /// <returns>Pass or fail on validation</returns>
        public static bool ValidateFileBasics(ref byte[] fileContent, out string returnMessage)
        {
            // linefeed/newline 0A
            if (fileContent.Count(b => b == (byte)0x0A) < 2)
            {
                returnMessage = "Invalid file. Minimum number of rows (i.e. 3) are not present.";
                return false;
            }

            if (fileContent.Length < 4)
            {
                returnMessage = "Invalid file. The file does not contain enough characters to find a delimiter.";
                return false;
            }

            // ~ 7E
            if (fileContent[3] != 0x7E)
            {
                returnMessage = "Invalid file. An incorrect delimiter is being used.";
                return false;
            }
            
            /* H 54
             * D 52
             * R 4c
             * */
            if (fileContent[0] != 0x48 || fileContent[1] != 0x44 || fileContent[2] != 0x52)
            {
                returnMessage = "Invalid file. The first row must be a HDR row.";
                return false;
            }

            int lastIndex = fileContent.Length - 1;
            // ~ 7E
            int lastTildeIndex = Array.LastIndexOf(fileContent, (byte)0x7E);

            //EventLog.WriteEntry("_File Transfer Service - ESR NHS Hub", string.Format("TRL - Index of last tilde '{0}' - file length - '{1}'.", lastTildeIndex, fileContent.Length), EventLogEntryType.Warning);

            /* T 54
             * R 52
             * L 4C
             * */
            if (lastTildeIndex <= 2 || fileContent[lastTildeIndex - 3] != 0x54 || fileContent[lastTildeIndex - 2] != 0x52 || fileContent[lastTildeIndex - 1] != 0x4C)
            {
                returnMessage = "Invalid file. The last row must be a TRL row.";

                //EventLog.WriteEntry("_File Transfer Service - ESR NHS Hub", string.Format("TRL values were - Index of last tilde '{0}', T '{1}', R '{2}', L '{3}'.", lastTildeIndex, fileContent[lastTildeIndex - 3], fileContent[lastTildeIndex - 2], fileContent[lastTildeIndex - 1]), EventLogEntryType.Warning);

                return false;
            }

            int lastCharacterIndex = lastIndex;

            /* Space 20
             * linefeed/newline 0A
             * Tab 09
             * */
            for (int i = lastIndex; fileContent[i] == 0x20 || fileContent[i] == 0x0A || fileContent[i] == 0x09; i -= 1)
            {
                lastCharacterIndex = i - 1;
            }

            //EventLog.WriteEntry("_File Transfer Service - ESR NHS Hub", string.Format("TRL values were - Index of last character '{0}'.", lastCharacterIndex), EventLogEntryType.Warning);

            int numRows;
            if (!int.TryParse(new ASCIIEncoding().GetString(new ArraySegment<byte>(fileContent, lastTildeIndex + 1, lastCharacterIndex - lastTildeIndex).ToArray()), out numRows))
            {
                returnMessage = "Invalid file. A rowcount cannot be found in the TRL row.";
                return false;
            }

            if (numRows != new ArraySegment<byte>(fileContent, 0, lastCharacterIndex).Count(b => b == 0x0A) + 1)
            {
                returnMessage = "Invalid file. The rowcount from the last row does not match the number of rows found in the stream.";
                return false;
            }

            returnMessage = "Valid file. Simple file validation passed.";
            return true;
        }

        #endregion
    }
}