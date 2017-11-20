using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Configuration;
using SpendManagementLibrary;
using SpendManagementLibrary.ESRTransferServiceClasses;
using ESR_File_Service.Database;

namespace ESR_File_Service
{
    public class cFTPClient
    {
        private int dataID = 0;
        private int FileIndex = 0;
        private bool hasInboundFinished = false;

        /// <summary>
        /// Upload the specified Inbound file to the desired FTP directory
        /// </summary>
        /// <param name="ftpLocation">An instance of <see cref="ftpLocation"/>which defines the host and path for the upload</param>
        /// <param name="filename">The name of the file to upload</param>
        /// <param name="fileData">A byte[] holding the file data</param>
        /// <param name="trustId">The <see cref="int"/>ID of the trust this file is from.</param>
        /// <returns>an instance of <see cref="FinancialExportStatus"/>which defines the success or otherwise of the upload.</returns>
        public FinancialExportStatus UploadInboundFile(ftpLocation ftpLocation, string filename, byte[] fileData, int trustId)
        {
            Stream responseStream = null;
            StreamReader readStream = null;
            FtpWebResponse response = null;
            FinancialExportStatus status = FinancialExportStatus.None;
            cESRTransferLogging clsTransferLogging = new cESRTransferLogging(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            cSecureData clsSecure = new cSecureData();
            string decryptedPassword = string.Empty;
            string decryptedUsername = string.Empty;
            Uri uriString = new Uri("ftp://" + ftpLocation.host + "/" + ftpLocation.InboundPath);

            try
            {
                if (ftpLocation.password != "")
                {
                    decryptedPassword = clsSecure.Decrypt(ftpLocation.password);
                }

                if (ftpLocation.username != "")
                {
                    decryptedUsername = clsSecure.Decrypt(ftpLocation.username);
                }

                FtpWebRequest request = this.GetFTPWebRequest(uriString + "/" + filename, decryptedUsername, decryptedPassword, WebRequestMethods.Ftp.UploadFile);

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(fileData, 0, fileData.Length);
                    reqStream.Close();
                }

                using (response = (FtpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == FtpStatusCode.FileActionOK || response.StatusCode == FtpStatusCode.CommandOK || response.StatusCode == FtpStatusCode.ClosingData)
                    {
                        status = FinancialExportStatus.UploadToESRSucceeded;
                        clsTransferLogging.SaveLogItem(new cESRTransferLogItem(0, TransferLogItemType.InboundFileUploadedSuccessfully, AutomatedTransferType.ESRInbound, trustId, filename, false, "\n\n**FTP Response**\n" + response.StatusDescription, null, null));
                    }
                    else
                    {
                        status = FinancialExportStatus.UploadToESRFailed;
                        clsTransferLogging.SaveLogItem(new cESRTransferLogItem(0, TransferLogItemType.InboundFileUploadFailed, AutomatedTransferType.ESRInbound, trustId, filename, false, "\n\n**FTP Response**\n" + response.StatusDescription, null, null));
                    }
                }
            }
            catch(Exception ex)
            {
                status = FinancialExportStatus.UploadToESRFailed;
                clsTransferLogging.SaveLogItem(new cESRTransferLogItem(0, TransferLogItemType.InboundFileUploadFailed, AutomatedTransferType.ESRInbound, trustId, filename, false, "\n\nAn error occurred, please view the error email message", null, null));

                //Create Error message
                SELN3Service clsService = new SELN3Service();
                clsTransferLogging.SaveLogItem(new cESRTransferLogItem(0, TransferLogItemType.ESRServiceErrored, AutomatedTransferType.None, 0, "", false, clsService.CreateErrorTextString(ex), null, null));
            }
            finally
            {
                #region Clean up

                if (readStream != null)
                {
                    readStream.Close();
                }
                if (response != null)
                {
                    response.Close();
                }

                #endregion
            }

            return status;
        }

        /// <summary>
        /// Method to get a web request to an FTP site with ethe transfer type (Upload, Download, FileList etc) being passed in 
        /// </summary>
        /// <param name="UriString">The FTP address</param>
        /// <param name="username">The FTP site username</param>
        /// <param name="password">The FTP site Password</param>
        /// <param name="FTPTransferType">The type of transfer for the FTP request</param>
        /// <returns></returns>
        public FtpWebRequest GetFTPWebRequest(string target, string username, string password, string FTPTransferType)
        {
            
            ServicePointManager.ServerCertificateValidationCallback = delegate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                //Override any SSL policy by always returning true
                return true;
            };

            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(target);
            request.Credentials = new NetworkCredential(username, password);
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;
            request.EnableSsl = true;
            
            request.Method = FTPTransferType;

            return request;
        }

        /// <summary>
        /// Get a list of file names from an FTP directory
        /// </summary>
        /// <param name="target">The FTP address</param>
        /// <param name="username">The FTP site username</param>
        /// <param name="password">The FTP site Password</param>
        /// <returns>A list of filenames</returns>
        public List<string> GetFTPDirectoryFileList(string target, string username, string password)
        {
            List<string> lstFilesToDownload = new List<string>();
            if (string.IsNullOrWhiteSpace(username) == false && string.IsNullOrWhiteSpace(password) == false)
            {
                FtpWebResponse response = null;
                Stream responseStream = null;
                StreamReader reader = null;
                Uri UriString = new Uri("ftp://" + target);

                try
                {
                    FtpWebRequest request = GetFTPWebRequest(UriString + "/goout/", username, password, WebRequestMethods.Ftp.ListDirectory);
                    response = request.GetResponse() as FtpWebResponse;
                    responseStream = response.GetResponseStream();

                    reader = new StreamReader(responseStream);

                    while (!reader.EndOfStream)
                    {
                        lstFilesToDownload.Add(reader.ReadLine());
                    }
                }
                catch (Exception ex)
                {
                    //Create Error message
                    SELN3Service clsService = new SELN3Service();
                    cESRTransferLogging clsTransferLogging = new cESRTransferLogging(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
                    clsTransferLogging.SaveLogItem(new cESRTransferLogItem(0, TransferLogItemType.ESRServiceErrored, AutomatedTransferType.None, 0, "", false, clsService.CreateErrorTextString(ex), null, null));
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
        /// <param name="target"></param>
        /// <param name="filename"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public byte[] DownloadOutboundFile(cESRTrust trust, string filename, string password)
        {
            cESRTransferLogging clsTransferLogging = new cESRTransferLogging(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            byte[] downloadedData = new byte[0];
            FtpWebResponse response = null;
            Stream reader = null;
            MemoryStream memStream = null;
            Uri UriString = new Uri("ftp://" + trust.FTPAddress);

            try
            {
                FtpWebRequest request = GetFTPWebRequest(UriString + "/goout/" + filename, trust.FTPUsername, password, WebRequestMethods.Ftp.DownloadFile);

                response = request.GetResponse() as FtpWebResponse;
                reader = response.GetResponseStream();

                //Download to memory
                memStream = new MemoryStream();
                byte[] buffer = new byte[1024]; //downloads in chunks

                while (true)
                {
                    Application.DoEvents(); //prevent application from crashing

                    //Try to read the data
                    int bytesRead = reader.Read(buffer, 0, buffer.Length);

                    if (bytesRead == 0)
                    {
                        break;
                    }
                    else
                    {
                        //Write the downloaded data
                        memStream.Write(buffer, 0, bytesRead);
                    }
                }

                //Convert the downloaded stream to a byte array
                downloadedData = memStream.ToArray();

                if (downloadedData.Length > 0)
                {
                    clsTransferLogging.SaveLogItem(new cESRTransferLogItem(0, TransferLogItemType.OutboundFileDownloaded, AutomatedTransferType.ESROutbound, trust.TrustID, filename, false, "\n\n**FTP Response**\n" + response.StatusDescription, null, null));
                }
                else
                {
                    clsTransferLogging.SaveLogItem(new cESRTransferLogItem(0, TransferLogItemType.OutboundFileDownloadedWithoutData, AutomatedTransferType.ESROutbound, trust.TrustID, filename, false, "\n\n**FTP Response**\n" + response.StatusDescription, null, null));
                }
            }
            catch (Exception ex)
            {
                clsTransferLogging.SaveLogItem(new cESRTransferLogItem(0, TransferLogItemType.OutboundFileDownloadFailed, AutomatedTransferType.ESROutbound, trust.TrustID, filename, false, "\n\nAn error occurred, please view the error email message", null, null));
                //Create Error message
                SELN3Service clsService = new SELN3Service();
                clsTransferLogging.SaveLogItem(new cESRTransferLogItem(0, TransferLogItemType.ESRServiceErrored, AutomatedTransferType.None, 0, "", false, clsService.CreateErrorTextString(ex), null, null));
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
        /// <param name="target"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="filename"></param>
        public void DeleteFileFromFTP(string target, string username, string password, string filename)
        {
            FtpWebResponse response = null;
            StreamReader reader = null;
            Uri UriString = new Uri("ftp://" + target);

            try
            {
                FtpWebRequest request = GetFTPWebRequest(UriString + "/" + filename, username, password, WebRequestMethods.Ftp.DeleteFile);
                response = request.GetResponse() as FtpWebResponse;
            }
            catch (Exception ex)
            {
                cEventlog.LogEntry(ex.Message + " " + ex.StackTrace);
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

                #endregion
            }
        }
    }
}

