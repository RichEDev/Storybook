using System.Linq;
using Common.Cryptography;
using SpendManagementLibrary;
using Spend_Management;
using Utilities.Cryptography;
using static System.Configuration.ConfigurationManager;

namespace PasswordEncryptToHash
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.WriteLine("Connect to metabase");
            Logger.WriteLine("Get list of accounts");
            GlobalVariables.MetabaseConnectionString = ConnectionStrings["metabase"].ToString();
            new cAccounts().CacheList();
            var accounts = cAccounts.CachedAccounts.Values.Where(a=>!a.archived).ToList();
            var encryptor = new HashEncryptor();
            var secureData = new ExpensesCryptography();
            var employeeHash = new EmployeeConvert(encryptor, secureData);
            var previousHash = new PreviousPasswordConvert(encryptor, secureData);
            Logger.WriteLine($"Got list of {accounts.Count} accounts");
            var idx = 1;
            foreach (cAccount cachedAccount in accounts)
            {
                //try
                {
                    Logger.WriteLine($"Processing Employees of {cachedAccount.companyid} - {idx} of {accounts.Count}");
                    employeeHash.Convert(cachedAccount);
                    Logger.WriteLine($"Processing previous passwords {cachedAccount.companyid} - {idx} of {accounts.Count}");
                    previousHash.Convert(cachedAccount);
                }
                //catch (Exception e)
                //{
                //    Logger.WriteLine(e.Message);
                //    Logger.WriteLine("Account not valid");
                //}
                idx++;
            }

            
            Logger.WriteLine("Complete");
        }
    }
}
