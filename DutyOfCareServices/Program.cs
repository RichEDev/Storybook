

namespace DutyOfCareServices
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Threading;

    using APICallsHelper;
    using ApiClientHelper;
    using DutyOfCareServices.ApiCalls;
    
    using DutyOfCareServices.ApiCalls.CorporateCards;
    using Common.Logging;

    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// An instance of <see cref="ILog"/> for logging <see cref="ICache{T,TK}"/> diagnostics and information.
        /// </summary>
        private static readonly ILog Log = new LogFactory<Program>().GetLogger();

        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                PrintDefaultActions();
                return;
            }

            DateTime date = DateTime.Now;
            if (args.Length == 2)
            {
                if (!DateTime.TryParse(args[1].TrimStart('/'), out date))
                {
                    if (Log.IsInfoEnabled)
                    {
                        Log.Info($"The date entered '{args[1]}' was invalid");
                    }
                    Console.WriteLine("Not a valid date. \n");
                    Thread.Sleep(2500);
                    return;
                }
            }

            RunService(args[0].TrimStart('/'), date);
        }

        /// <summary>
        /// The run service.
        /// </summary>
        /// <param name="serviceName">
        /// The service name.
        /// </param>
        /// <param name="date">
        /// The date for which you want the exchange rates populated.
        /// </param>
        private static void RunService(string serviceName, DateTime date)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug($"Service {serviceName} about to be run.");
            }
            var usernametoCheckLicence = ConfigurationManager.AppSettings["LicenceCheckPortalAccessUserName"];
            var passwordToCheckLicence = ConfigurationManager.AppSettings["LicenceCheckPortalAccessPassword"];
            var defaultCompanyId = ConfigurationManager.AppSettings["apiDefaultCompanyId"];
            var apiUrlPath = new RequestHelper().GetHttpWebRequest(string.Empty).Address.AbsoluteUri;
            switch (serviceName.ToLower())
            {
                case "dvlapopulatedrivinglicence": new PopulateDrivingLicence().PopulateEmployeeDrivingLicence(apiUrlPath, defaultCompanyId, passwordToCheckLicence, usernametoCheckLicence, new EventLogger());
                    break;
                case "dvlalookupformanualintervention":
                    new DvlaLookupForManualintervention().PopulateEmployeeDrivingLicences(apiUrlPath, defaultCompanyId, passwordToCheckLicence, usernametoCheckLicence, new EventLogger());
                    break;
                case "consentexpiryreminder":
                    new ConsentReminder().RemindUsersOfExpiringConsents(apiUrlPath, defaultCompanyId, new EventLogger());
                    break;
                case "dutyofcaredocumentsexpiryreminder":
                    var dutyOfCareRemindrs = new DutyOfCareDocumentsExpiryReminder(apiUrlPath, defaultCompanyId, new EventLogger());
                    dutyOfCareRemindrs.RemindUsersOfExpiringDutyOfCareDocuments(true);
                    dutyOfCareRemindrs.RemindUsersOfExpiringDutyOfCareDocuments(false);
                    break;
                case "drivinglicencereviewexpiryreminder":
                    new DrivingLicenceReviewExpiryReminder().NotifyClaimantsOnExpiredDrivingLicenceReviews(apiUrlPath, defaultCompanyId, new EventLogger());
                    break;
                case "populateexchangerates":
                    new PopulateExchangeRates().UpdateDailyExchangeRates(apiUrlPath, date, new EventLogger());
                    break;
                case "corporatecardautoimport":
                    ImportCorporateCards(apiUrlPath);
                    break;
                default:
                    PrintDefaultActions();
                    break;
            }
        }

        private static void ImportCorporateCards(string apiUrlPath)
        {
            var numberOfDirectories = 0;
            try
            {
                numberOfDirectories = int.Parse(ConfigurationManager.AppSettings["NumberOfCardproviders"].ToString());
            }
            catch (Exception e)
            {
                Log.Error("NumberOfCardproviders not set in app Settings.  Must be a whole number.", e);
                Console.WriteLine("NumberOfCardproviders not set in app Settings.  Must be a whole number.");
                throw;
            }

            for (int i = 1; i <= numberOfDirectories; i++)
            {
                try
                {

                    if (!ConfigurationManager.AppSettings.AllKeys.Contains(i.ToString()))
                    {
                        Log.Error($"Expected key {i} missing from AppSettings");
                        throw new ConfigurationErrorsException($"Expected key {i} missing from AppSettings");
                    }

                    var directory = ConfigurationManager.AppSettings[i.ToString()].ToString();
                    if (!ConfigurationManager.AppSettings.AllKeys.Contains("id" + i.ToString()))
                    {
                        Log.Error($"Expected key id{i} missing from AppSettings");
                        throw new ConfigurationErrorsException($"Expected key id{i} missing from AppSettings");
                    }

                    var cardProviderName = ConfigurationManager.AppSettings["id" + i];
                    Console.WriteLine($"Scanning directory '{directory}' for completed files");
                    var autoImport =
                        new AutoImportOfCardTransactions(
                            new DirectoryScanner(directory, new DirectoryDetails(directory)), cardProviderName, new EventLogger(), new Client(apiUrlPath));
                    autoImport.ProcessCompletedFiles();
                }
                catch (ConfigurationErrorsException configurationErrors)
                {
                    Log.Error($"Config error at line { configurationErrors.Line} - { configurationErrors.Message}", configurationErrors);
                    Console.WriteLine($"Config error at line {configurationErrors.Line} - {configurationErrors.Message}");
                    throw;
                }
                catch (Exception e)
                {
                    Log.Error("Failed to import card", e);
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        /// <summary>
        /// The print default actions.
        /// </summary>
        private static void PrintDefaultActions()
        {
            Log.Info("The service name specified was not found");
            Console.WriteLine("Please provide the correct action name to start the application. \n");
            Console.WriteLine("Available actions: \n");
            Console.WriteLine(
                "DvlaPopulateDrivingLicence \n DvlaLookupForManualintervention \n ConsentExpiryReminder \n DutyOfCareDocumentsExpiryReminder \n DrivingLicenceReviewExpiryReminder \n PopulateExchangeRates \n CorporateCardAutoImport");
            Thread.Sleep(2500);
        }
    }
}
