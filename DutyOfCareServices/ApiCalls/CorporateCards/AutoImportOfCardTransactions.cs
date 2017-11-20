using Common.Logging;

namespace DutyOfCareServices.ApiCalls.CorporateCards
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using ApiClientHelper;
    using ApiClientHelper.Models;
    using ApiClientHelper.Responses;
    using APICallsHelper;

    /// <summary>
    /// A class for uploading a card file to be validated and imported.
    /// </summary>
    public class AutoImportOfCardTransactions
    {
        /// <summary>
        /// A private instance of <see cref="DirectoryScanner"/>
        /// </summary>
        private readonly DirectoryScanner _directoryScanner;

        /// <summary>
        /// a private instance of the current credit card provider id
        /// </summary>
        private readonly string _cardProviderName;

        /// <summary>
        /// A private instance of <see cref="Client"/>
        /// </summary>
        private readonly Client _apiClient;

        /// <summary>
        /// A private instance of <see cref="EventLogger"/>
        /// </summary>
        private readonly EventLogger _logger;

        /// <summary>
        /// Api URL for the card import.
        /// </summary>
        private const string ApiEndPoint = "CorporateCards/AutoImportCardFile/{0}";

        /// <summary>
        /// Common substring of all event log messages for auto import.
        /// </summary>
        private const string LogMessage = "Auto import of card transactions :";

        /// <summary>
        /// An instance of <see cref="ILog"/> for logging <see cref="ICache{T,TK}"/> diagnostics and information.
        /// </summary>
        private static readonly ILog Log = new LogFactory<AutoImportOfCardTransactions>().GetLogger();

        /// <summary>
        /// Create a new instance of <see cref="AutoImportOfCardTransactions"/>
        /// </summary>
        /// <param name="directoryScanner">An instance of <see cref="DirectoryScanner"/></param>
        /// <param name="cardProviderName">The name of the card provider associated with this directory</param>
        /// <param name="logger">An instance of <see cref="EventLogger"/></param>
        /// <param name="apiClient">An instance of <see cref="Client"/></param>
        public AutoImportOfCardTransactions(DirectoryScanner directoryScanner, string cardProviderName, EventLogger logger, Client apiClient)
        {
            this._directoryScanner = directoryScanner;
            this._cardProviderName = cardProviderName;
            this._logger = logger;
            this._apiClient = apiClient;
        }

        /// <summary>
        /// Process any files where the current and previous file size are the same (based on the instance of <see cref="DirectoryScanner"/>.
        /// </summary>
        public void ProcessCompletedFiles()
        {
            Log.Debug("Process completed files.");
            foreach (FileInformation fileInformation in this._directoryScanner.GetInformation())
            {
                if (fileInformation.PreviousFileSize == fileInformation.FileSize)
                {
                    if (Log.IsDebugEnabled)
                    {
                        Log.Debug($"Processing file {fileInformation.FileName}");
                    }
                    
                    var result = this.ProcessFile(fileInformation);
                    if (result == null)
                    {
                        Log.Debug("result == null");
                        this._directoryScanner.MoveFile(fileInformation, ImportStatus.ApiFail);
                        continue;
                    }
                    if (Log.IsDebugEnabled)
                    {
                        Log.Debug($"result == {result}");
                    }
                    
                    switch (result.Item)
                    {
                        case -100:
                        case -1:
                            this._directoryScanner.MoveFile(fileInformation, ImportStatus.IdNotFound);
                            break;
                        case 0:
                            this._directoryScanner.MoveFile(fileInformation, ImportStatus.InvalidFile);
                            break;
                        default:
                            this._directoryScanner.MoveFile(fileInformation, ImportStatus.Sucess);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Read the completed file and send to API.  When complete, move file to new directory depending on result.
        /// </summary>
        /// <param name="fileInformation">An instance of <see cref="FileInformation"/>to process</param>
        private CorporateCardAutoMatchResponse ProcessFile(FileInformation fileInformation)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug($"Submitting file {fileInformation.FileName} to API.");
            }
            Console.WriteLine($"Submitting file {fileInformation.FileName} to API.");
            var endPoint = string.Format(ApiEndPoint, this._cardProviderName);
            try
            {
                var fileData = Convert.ToBase64String(File.ReadAllBytes(fileInformation.FileName));
                var result = this._apiClient.ProcessCorporateCardFile(endPoint, new FileDataResponse(fileData));  

                Log.Debug("Result returned");
                Console.WriteLine("Result returned");
                if (result != null && result.Result != null && result.Result.Data == null)
                {
                    return new CorporateCardAutoMatchResponse();
                }

                Console.WriteLine($"result = {result.Status} {result.Result.ErrorMessage}");
                if (result.Status == TaskStatus.RanToCompletion && result.Result.StatusCode == HttpStatusCode.BadRequest)
                {
                    Log.Error($"{LogMessage} {result.Result.StatusDescription}");
                    this._logger.MakeEventLogEntry(LogMessage + " Error :", endPoint, result.Result.StatusDescription, true);
                    Console.WriteLine(LogMessage + result.Result.StatusDescription);
                    return null;
                }
                return result.Result.Data;
            }
            catch (WebException webException)
            {
                this._logger.MakeEventLogEntry(LogMessage + " Error :", endPoint, webException.Message, true);
                Log.Error($"Error on request to API", webException);
                Console.WriteLine(LogMessage + webException.Message);
            }
            catch (Exception ex)
            {
                Log.Error($"Error on request to API", ex);
                this._logger.MakeEventLogEntry(LogMessage + " Error :", endPoint, ex.Message, true);
                Console.WriteLine(LogMessage + ex.Message);
            }

            return new CorporateCardAutoMatchResponse();
        }
    }

    
}

