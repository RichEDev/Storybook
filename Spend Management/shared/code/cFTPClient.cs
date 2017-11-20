using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using SpendManagementLibrary;
using System.Net.Security;
using System.IO;

namespace Spend_Management
{
    /// <summary>
    /// FTP Transfer status codes
    /// </summary>
    public enum FTP_Status
    {
        /// <summary>
        /// No status set
        /// </summary>
        None = -1,
        /// <summary>
        /// Success state
        /// </summary>
        Success = 0,
        /// <summary>
        /// Fail / Error state
        /// </summary>
        Fail = 1
    }

    /// <summary>
    /// FTP data transfer class
    /// </summary>
    public class cFTPClient
    {
        /// <summary>
        /// The FTP Address
        /// </summary>
        private string sTarget;
        /// <summary>
        /// FTP User Name
        /// </summary>
        private string sUsername;
        /// <summary>
        /// FTP Password
        /// </summary>
        private string sPassword;
        /// <summary>
        /// Use a secure FTP Connection
        /// </summary>

        #region "properties"
        /// <summary>
        /// Gets the FTP Target address
        /// </summary>
        public string FTP_Target
        {
            get { return sTarget; }
        }
        /// <summary>
        /// Gets the FTP User name
        /// </summary>
        public string FTP_Username
        {
            get { return sUsername; }
        }
        /// <summary>
        /// Gets the FTP Password (encrypted)
        /// </summary>
        public string FTP_Password
        {
            get { return sPassword; }
        }
        /// <summary>
        /// Gets or Sets the FTP Root folder
        /// </summary>
        public string FTP_Root
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or Sets whether to use a secure FTP connection
        /// </summary>
        public bool FTP_UseSSL
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or Sets whether to use binary transfer
        /// </summary>
        public bool FTP_UseBinary
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or Sets the FTP KeepAlive property
        /// </summary>
        public bool FTP_KeepAlive
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or Sets whether to use passive authentication
        /// </summary>
        public bool FTP_UsePassive
        {
            get;
            set;
        }
        #endregion

        /// <summary>
        /// Instantiate a new FTP Client Connection
        /// </summary>
        /// <param name="target">FTP target address</param>
        /// <param name="username">FTP User name to use for authentication</param>
        /// <param name="password">FTP Password to use for authentication</param>
        /// <param name="rootFolder">Root folder on target server</param>
        /// <param name="useSSL">Use a secure connection?</param>
        /// <param name="useBinary">Transfers to be binary</param>
        /// <param name="keepAlive">Keep Connection alive</param>
        /// <param name="usePassive">Use passive authentication</param>
        public cFTPClient(string target, string username, string password, string rootFolder = "", bool useSSL = false, bool useBinary = true, bool keepAlive = true, bool usePassive = true)
        {
            sTarget = target;
            sUsername = username;
            sPassword = password;
            FTP_Root = rootFolder;
            FTP_UseBinary = useBinary;
            FTP_UsePassive = usePassive;
            FTP_KeepAlive = keepAlive;
            FTP_UseSSL = useSSL;
        }

        /// <summary>
        /// Method to get a web request to an FTP site with ethe transfer type (Upload, Download, FileList etc) being passed in 
        /// </summary>
        /// <param name="FTPTransferType">The type of transfer for the FTP request</param>
        /// <param name="filename">Filename of file used in action</param>
        /// <returns></returns>
        public FtpWebRequest GetFTPWebRequest(string FTPTransferType, string filename = "")
        {

            ServicePointManager.ServerCertificateValidationCallback = delegate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                //Override any SSL policy by always returning true
                return true;
            };

            Uri uriString = new Uri("ftp://" + FTP_Target);

            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(uriString + (FTP_Root != "" ? "/" + FTP_Root : "") + (filename != "" ? "/" + filename : ""));
            request.Credentials = new NetworkCredential(FTP_Username, FTP_Password);
            request.UsePassive = FTP_UsePassive;
            request.UseBinary = FTP_UseBinary;
            request.KeepAlive = FTP_KeepAlive;
            request.EnableSsl = FTP_UseSSL;

            request.Method = FTPTransferType;

            return request;
        }

        /// <summary>
        /// Get a list of file names from an FTP directory
        /// </summary>
        /// <returns>A list of filenames</returns>
        public List<string> GetFTPDirectoryFileList()
        {
            List<string> lstFilesToDownload = new List<string>();
            FtpWebResponse response = null;
            Stream responseStream = null;
            StreamReader reader = null;

            try
            {
                FtpWebRequest request = GetFTPWebRequest(WebRequestMethods.Ftp.ListDirectory);
                response = request.GetResponse() as FtpWebResponse;
                responseStream = response.GetResponseStream();

                using (reader = new StreamReader(responseStream))
                {
                    while (!reader.EndOfStream)
                    {
                        lstFilesToDownload.Add(reader.ReadLine());
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                //Create Error message
                cEventlog.LogEntry("An error occurred attempting to get FTP Directory File List\n\n" + ex.Message);
                lstFilesToDownload = null; // nullify output to signify error
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

            return lstFilesToDownload;
        }

        /// <summary>
        /// Delete a file from the ftp site 
        /// </summary>
        /// <param name="filename"></param>
        public FTP_Status DeleteFileFromFTP(string filename)
        {
            FtpWebResponse response = null;
            FTP_Status status = FTP_Status.None;

            try
            {
                FtpWebRequest request = GetFTPWebRequest(WebRequestMethods.Ftp.DeleteFile, filename);
                response = request.GetResponse() as FtpWebResponse;

                status = FTP_Status.Success;
            }
            catch (Exception ex)
            {
                cEventlog.LogEntry(ex.Message + " " + ex.StackTrace);
                status = FTP_Status.Fail;
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

            return status;
        }

        /// <summary>
        /// Download a file from an FTP site
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public byte[] DownloadFile(string filename)
        {
            byte[] downloadedData = new byte[0];
            FtpWebResponse response = null;
            Stream reader = null;
            MemoryStream memStream = null;

            try
            {
                FtpWebRequest request = GetFTPWebRequest(WebRequestMethods.Ftp.DownloadFile, filename);

                response = request.GetResponse() as FtpWebResponse;
                reader = response.GetResponseStream();

                //Download to memory
                memStream = new MemoryStream();
                byte[] buffer = new byte[1024]; //downloads in chunks

                while (true)
                {
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
                    cEventlog.LogEntry("FTP File Download complete for file: " + filename + "\n\n**FTP Response**\n" + response.StatusDescription);
                }
                else
                {
                    cEventlog.LogEntry("FTP File Download complete without data for file: " + filename + "\n\n**FTP Response**\n" + response.StatusDescription);
                }
            }
            catch (Exception ex)
            {
                cEventlog.LogEntry("FTP File Download failed\n\n" + ex.Message);
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
        /// Upload file to the FTP server
        /// </summary>
        /// <param name="filename">Filename of the file to be uploaded</param>
        /// <param name="fileData">Byte array of the file</param>
        /// <returns></returns>
        public FTP_Status UploadFile(string filename, byte[] fileData)
        {
            FtpWebResponse response = null;
            FTP_Status status = FTP_Status.None;

            try
            {
                FtpWebRequest request = GetFTPWebRequest(WebRequestMethods.Ftp.UploadFile, filename);
                //request.ContentLength = fileData.Length;

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(fileData, 0, fileData.Length);
                    reqStream.Close();
                }

                response = (FtpWebResponse)request.GetResponse();

                if (response.StatusCode == FtpStatusCode.FileActionOK || response.StatusCode == FtpStatusCode.CommandOK || response.StatusCode == FtpStatusCode.ClosingData)
                {
                    status = FTP_Status.Success;
                    cEventlog.LogEntry("File Upload successful for file: " + filename + "\n\n**FTP Response**\n" + response.StatusDescription);
                }
                else
                {
                    status = FTP_Status.Fail;
                    cEventlog.LogEntry("File Upload failed for file: " + filename + "\n\n**FTP Response**\n" + response.StatusDescription);
                }
            }
            catch (Exception ex)
            {
                status = FTP_Status.Fail;
                cEventlog.LogEntry("An error occurred during upload for file: " + filename + "\n\n" + ex.Message);
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

            return status;
        }
    }
}
