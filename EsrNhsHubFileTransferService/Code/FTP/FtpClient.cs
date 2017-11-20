namespace EsrNhsHubFileTransferService.Code.FTP
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;

    using EsrNhsHubFileTransferService.Code.Helpers;

    /// <summary>
    /// An FTP Client class
    /// </summary>
    public class FtpClient
    {
        /// <summary>
        /// Method to get a web request to an FTP site with the transfer type (Upload, Download, FileList etc) being passed in 
        /// </summary>
        /// <param name="targetUri"></param>
        /// <param name="username">The FTP site username</param>
        /// <param name="password">The FTP site Password</param>
        /// <param name="ftpTransferType">The type of transfer for the FTP request</param>
        /// <returns></returns>
        public FtpWebRequest GetFtpWebRequest(string targetUri, string username, string password, string ftpTransferType)
        {
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true; // Override any SSL policy by always returning true
            FtpWebRequest ftpRequest = WebRequest.Create(targetUri) as FtpWebRequest;

            if (ftpRequest != null)
            {
                ftpRequest.Credentials = new NetworkCredential(username, password);
                ftpRequest.UsePassive = true;
                ftpRequest.UseBinary = true;
                ftpRequest.KeepAlive = false;
                ftpRequest.EnableSsl = true;
                ftpRequest.Method = ftpTransferType;
            }

            return ftpRequest;
        }

        /// <summary>
        /// Get a list of file names from an FTP directory
        /// </summary>
        /// <returns>A list of filenames</returns>
        public List<string> GetFtpDirectoryFileList(FtpServer ftpServer)
        {
            List<string> lstFilesToDownload = new List<string>();
            if (string.IsNullOrWhiteSpace(ftpServer.Username) == false && string.IsNullOrWhiteSpace(ftpServer.Password) == false)
            {
                FtpWebResponse response = null;
                Stream responseStream = null;
                StreamReader reader = null;
                Uri uriString = new Uri("ftp://" + ftpServer.Hostname + "/" + ftpServer.Path);

                try
                {
                    FtpWebRequest request = this.GetFtpWebRequest(uriString.ToString(), ftpServer.Username, ftpServer.Password, WebRequestMethods.Ftp.ListDirectory);
                    using (response = request.GetResponse() as FtpWebResponse)
                    {
                        if (response == null)
                        {
                            return lstFilesToDownload;
                        }
                        using (responseStream = response.GetResponseStream())
                        {
                            if (responseStream == null)
                            {
                                response.Close();
                                return lstFilesToDownload;
                            }

                            reader = new StreamReader(responseStream);
                            while (!reader.EndOfStream)
                            {
                                lstFilesToDownload.Add(reader.ReadLine());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.ImportantError("Connecting to the FTP location failed with the following exception message - {0}", ex.Message);
                }
                finally
                {
                    #region Clean up

                    if (responseStream != null)
                    {
                        responseStream.Close();
                    }
                    if (response != null)
                    {
                        response.Close();
                    }
                    if (reader != null)
                    {
                        reader.Close();
                    }

                    #endregion
                }
            }

            return lstFilesToDownload;
        }

        /// <summary>
        /// Download a file from an FTP site
        /// </summary>
        /// <param name="ftpServer"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public byte[] DownloadOutboundFile(FtpServer ftpServer, string filename)
        {
            byte[] downloadedData = new byte[0];
            FtpWebResponse response = null;
            Stream reader = null;
            MemoryStream memStream = null;
            Uri uriString = new Uri("ftp://" + ftpServer.Hostname + "/" + ftpServer.Path);

            try
            {
                FtpWebRequest request = this.GetFtpWebRequest(uriString + "/" + filename, ftpServer.Username, ftpServer.Password, WebRequestMethods.Ftp.DownloadFile);

                if (request == null)
                {
                    return downloadedData;
                }

                using (response = request.GetResponse() as FtpWebResponse)
                {
                    if (response == null)
                    {
                        return downloadedData;
                    }

                    using (reader = response.GetResponseStream())
                    {
                        if (reader == null)
                        {
                            response.Close();
                            return downloadedData;
                        }

                        // Download to memory
                        memStream = new MemoryStream();
                        reader.CopyTo(memStream);

                        // Convert the downloaded stream to a byte array
                        downloadedData = memStream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ImportantError("Downloading a file failed with the following exception message - {0}", ex.Message);
            }
            finally
            {
                #region Clean up

                if (response != null)
                {
                    response.Close();
                }
                if (reader != null)
                {
                    reader.Close();
                }
                if (memStream != null)
                {
                    memStream.Close();
                }

                #endregion
            }

            return downloadedData;
        }

        /// <summary>
        /// Delete a file from the ftp site 
        /// </summary>
        /// <param name="ftpServer"></param>
        /// <param name="filename"></param>
        public bool DeleteFileFromFtp(FtpServer ftpServer, string filename)
        {
            FtpWebResponse response = null;
            Uri uriString = new Uri("ftp://" + ftpServer.Hostname + "/" + ftpServer.Path);
            bool deleted;

            try
            {
                FtpWebRequest request = this.GetFtpWebRequest(uriString + "/" + filename, ftpServer.Username, ftpServer.Password, WebRequestMethods.Ftp.DeleteFile);
                using (response = request.GetResponse() as FtpWebResponse)
                {
                    deleted = true;
                }
            }
            catch (WebException ex)
            {
                FtpWebResponse ftpResponse = ((FtpWebResponse)ex.Response);
                if (ftpResponse != null && ftpResponse.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    Log.ImportantInformation("File {0} not found on server. Marking as deleted.", filename);
                    deleted = true;
                }
                else
                {
                    Log.ImportantError("Deleting a file failed with the following exception message - {0}", ex.Message);
                    deleted = false;
                }
            }
            finally
            {
                #region Clean up

                if (response != null)
                {
                    response.Close();
                }

                #endregion
            }
            
            return deleted;
        }
    }
}
