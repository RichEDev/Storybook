namespace EsrNhsHubFileTransferService.Code.CommandChannel
{
    using System;
    using System.Linq;

    using EsrGo2FromNhsWcfLibrary.Enum;

    using EsrNhsHubFileTransferService.Code.Database;
    using EsrNhsHubFileTransferService.Code.Helpers;

    /// <summary>
    ///     Class to handle commands received from external services
    /// </summary>
    public class EsrNhsHubCommands : IEsrNhsHubCommands
    {
        /// <summary>
        ///     Marks a file for retransfer by setting the "transfer again" property to true
        /// </summary>
        /// <param name="fileId">The transferred file's file id</param>
        /// <param name="fileName">The full ftp file name (may include full path details)</param>
        /// <returns>Success or failure to set the flag</returns>
        public bool FlagFileForNewTransfer(int fileId, string fileName)
        {
            int rv = 0;
            using (var db = new EsrNhsHubDatabase())
            {
                transferredFile retransferFile = (from file in db.transferredFiles
                                                  where file.fileName.Contains(fileName) && file.fileId == fileId && !file.deleted
                                                  select file).SingleOrDefault();

                if (retransferFile != null)
                {
                    retransferFile.transferAgain = true;
                    retransferFile.processed = false;

                    rv = db.SaveChanges();
                }
            }

            return rv > 0;
        }

        /// <summary>
        ///     Marks a file as having been processed by the file processing service
        /// </summary>
        /// <param name="fileId">The transferred file's file id</param>
        /// <param name="fileName">The full ftp file name (may need to include full path details)</param>
        /// <returns>Success or failure to set the flag</returns>
        public bool SuccessfullyProcessedFile(int fileId, string fileName)
        {
            int rv = 0;
            using (var db = new EsrNhsHubDatabase())
            {
                transferredFile processedFile = (from file in db.transferredFiles
                                                 where file.fileName == fileName && file.fileId == fileId
                                                 select file).SingleOrDefault();

                if (processedFile != null)
                {
                    processedFile.processed = true;
                    processedFile.transferStatus = (byte)EsrHubStatus.EsrHubTransferStatus.Success;
                    processedFile.deleteAfter = DateTime.UtcNow.AddDays(GlobalVariables.DeleteFilesXDaysAfterProcessed);

                    rv = db.SaveChanges();
                }
            }

            return rv > 0;
        }

        /// <summary>
        /// Marks a file as having not been fully processed by the file processing service
        /// </summary>
        /// <param name="fileId">
        /// The transferred file's file id
        /// </param>
        /// <param name="fileName">
        /// The full ftp file name (may need to include full path details)
        /// </param>
        /// <param name="status">
        /// The status of the failure.
        /// </param>
        /// <returns>
        /// Success or failure to set the flag
        /// </returns>
        public bool FailedToProcessFile(int fileId, string fileName, EsrHubStatus.EsrHubTransferStatus status)
        {
            int rv = 0;
            using (var db = new EsrNhsHubDatabase())
            {
                transferredFile processedFile = (from file in db.transferredFiles
                                                 where file.fileName.Contains(fileName) && file.fileId == fileId
                                                 select file).SingleOrDefault();

                if (processedFile != null)
                {
                    bool retry;
                    switch (status)
                    {
                        case EsrHubStatus.EsrHubTransferStatus.ValidationFailedNoFooter:
                            retry = false;
                            break;
                        case EsrHubStatus.EsrHubTransferStatus.ValidationFailedNoHeader:
                            retry = false;
                            break;
                        case EsrHubStatus.EsrHubTransferStatus.ValidationFailedRecordCount:
                            retry = false;
                            break;
                        case EsrHubStatus.EsrHubTransferStatus.ValidationFailedWrongFormat:
                            retry = false;
                            break;
                        case EsrHubStatus.EsrHubTransferStatus.ValidationWrongSequenceNumber:
                            retry = true;
                            break;
                        case EsrHubStatus.EsrHubTransferStatus.NhsVpdNotFound:
                            retry = true;
                            break;
                        case EsrHubStatus.EsrHubTransferStatus.ValidationFailedByteCount:
                            retry = true;
                            break;
                        default:
                            retry = true;
                            break;
                    }

                    processedFile.transferStatus = (byte)status;
                    if (retry)
                    {
                        if (processedFile.transferAttempts >= 5)
                        {
                            // todo: send an email?
                        }
                        else
                        {
                            processedFile.transferAgain = true;
                        }    
                    }
                    else
                    {
                        processedFile.transferAgain = false;
                        processedFile.deleteAfter = DateTime.UtcNow.AddDays(GlobalVariables.DeleteFilesXDaysAfterProcessed);
                    }
                    
                    rv = db.SaveChanges();
                }
            }

            return rv > 0;
        }
    }
}
