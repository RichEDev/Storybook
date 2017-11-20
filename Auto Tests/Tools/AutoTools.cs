
namespace Auto_Tests
{
    using System;
    using System.ServiceProcess;    
    using System.Text.RegularExpressions;
    using System.Data.SqlClient;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;
    using System.DirectoryServices;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Drawing;
    using Microsoft.VisualStudio.TestTools.UITesting;


    public class AutoTools
    {
        /// <summary>
        /// Returns the web address to use for automated testing.
        /// </summary>
        /// <returns></returns>
        public static string ServerToUse()
        {
            cDatabaseConnection database = new cDatabaseConnection(AutoTools.DatabaseConnectionString(true));
            SqlDataReader reader = database.GetReader("SELECT server FROM ServerToUse");
            string server = "https://testing.sel-expenses.com";
            if (reader.HasRows)
            {
                reader.Read();
                server = reader.GetValue(0).ToString();                
            }
            reader.Close();
            return server;            
        }
        
        public static string DatabaseConnectionString(bool dataSourceDB)
        {
            if (dataSourceDB == true)
            {
                return "Data Source=LITHIUM;Initial Catalog=AutoTestingDataSources;Persist Security Info=True;User ID=spenduser;Password=P3ngu1ns";
            }
            else
            {
                return DatabaseConnectionString();
            }
        }

        public static string DatabaseConnectionString()
        {            
            return "Data Source=TESTDB3;Initial Catalog=expenses_migration;Persist Security Info=True;User ID=spenduser;Password=P3ngu1ns";
        }

        public static string[] UserLevel(UserType userType)
        {
            string[] value = new string[3];
            cDatabaseConnection database = new cDatabaseConnection(AutoTools.DatabaseConnectionString(true));
            SqlDataReader reader = database.GetReader("SELECT CompanyID FROM ServerToUse");
            reader.Read();
            value[0] = reader.GetValue(0).ToString();
            reader.Close();

            switch (userType)
            {
                case UserType.Admin:
                    value[1] = "james";
                    value[2] = "Password1";
                    break;
                case UserType.Claimant:
                    value[1] = "testuser1";
                    value[2] = "Password1";
                    break;
                default:
                    value[1] = "james";
                    value[2] = "Password1";
                    break;
            }
            return value;
        }

        public static string GetID(string extracted)
        {
            return AutoTools.GetID(extracted, "(", ")", 1);
        }

        public static string GetID(string extracted, string firstchar, string lastchar, int distance)
        {
            string id = "";
            string current;
            bool found = false;
            int x = 1;

            while (found == false)
            {
                current = extracted.Substring(extracted.Length - x, 1);
                if (current == firstchar)
                {
                    x = x - distance;
                    while (extracted.Substring(extracted.Length - x, 1) != lastchar)
                    {
                        id += extracted.Substring(extracted.Length - x, 1);
                        x--;
                    }
                    found = true;
                }
                else
                {
                    x++;
                }
                if (x == extracted.Length) { found = true; }
            }
            return id;            
        }

        public static string GetListValue(string values)
        {
            string id = "";
            string current = "";
            bool useDefault = false;
            bool found = false;
            int x = 0;

            while (found == false)
            {
                current = values.Substring(x, 1);
                if (current == "o")
                {
                    // If 'o' has been found, check for "option value="
                    if ((x + 13 < values.Length) && (values.Substring(x, 13) == "option value="))
                    {
                        // Found the first option value

                        // Set x to the start of the id number
                        x = x + 14;

                        while ((values.Substring(x, 1) != "\""))
                        {
                            id += (values.Substring(x, 1));

                            x++;
                        }

                        // Combination boxes do not have a 'Selected' value if nothing (ie '0') is selected
                        // Therefore do not use '0' as a value
                        if (id == "0")
                        {
                            id = "";
                            useDefault = true;                           
                        }
                        // If the ID is "" the first list item is empty and shouldn't be used
                        else if (id != "")
                        {
                            found = true;
                        }
                    }
                }
                x++;
                // Check that the end of the string hasn't been met
                if (x == values.Length) { found = true; }
            }

            if (id == "" && useDefault == true)
            {
                // Return 0 only if there is no other option available to select
                return "0";
            }
            else
            {
                return id;
            }
        }

        /// <summary>
        /// Checks to ensure that all of the Automated User Defined Fields are currently added on the system.
        /// </summary>
        /// <returns></returns>
        public static bool CheckAllUDFs()
        {
            bool Outcome = false;
            cDatabaseConnection database = new cDatabaseConnection(AutoTools.DatabaseConnectionString());
            SqlDataReader reader = database.GetReader("SELECT COUNT(*) as 'Total' FROM userdefined WHERE attribute_name LIKE 'Auto%'");
            reader.Read();
            if (reader.GetValue(reader.GetOrdinal("Total")).ToString() == "136")
            {
                Outcome = true;
            }
            reader.Close();
            return Outcome;
        }

        /// <summary>
        /// Finds all occurances of the given textToMatch within the rawHtml. Returns an int value of the total count. 
        /// </summary>
        /// <param name="rawHtml"></param>
        /// <param name="textToMatch"></param>
        /// <returns></returns>
        public static int FindAll(string rawHtml, string textToMatch)
        {
            Regex reg = new Regex(textToMatch);
            return reg.Matches(rawHtml).Count;            
        }


        /// <summary>
        /// Extract text from the given web request. Specify what the text starts with, ends with, and what the resulting context name should be called
        /// </summary>
        /// <param name="stringStartsWithThisText"></param>
        /// <param name="stringEndsWithThisText"></param>
        /// <param name="contextName"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static WebTestRequest ExtractText(string stringStartsWithThisText, string stringEndsWithThisText, string contextName, WebTestRequest request)
        {
            ExtractText extractionRule2 = new ExtractText();
            extractionRule2.StartsWith = stringStartsWithThisText;
            extractionRule2.EndsWith = stringEndsWithThisText;
            extractionRule2.IgnoreCase = false;
            extractionRule2.UseRegularExpression = false;
            extractionRule2.Required = true;
            extractionRule2.ExtractRandomMatch = false;
            extractionRule2.Index = 0;
            extractionRule2.HtmlDecode = false;
            extractionRule2.ContextParameterName = "";
            extractionRule2.ContextParameterName = contextName;
            request.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            return request;
        }


        /// <summary>
        /// Validates the existence of the given text on a web page specified by the WebTestRequest.
        /// </summary>
        /// <param name="textToFind"></param>
        /// <param name="webRequest"></param>
        /// <returns></returns>
        public static WebTestRequest ValidateText(string textToFind, WebTestRequest webRequest)
        {
            return ValidateText(textToFind, webRequest, false, true);
        }

        /// <summary>
        /// Overload to specify wether Regular Expressions should be used and if the existence of the given text should result in a pass or fail.
        /// </summary>
        /// <param name="textToFind"></param>
        /// <param name="webRequest"></param>
        /// <param name="useRegEx"></param>
        /// <param name="passIfFound"></param>
        /// <returns></returns>
        public static WebTestRequest ValidateText(string textToFind, WebTestRequest webRequest, bool useRegEx, bool passIfFound)
        {
            ValidationRuleFindText newValidationRule = new ValidationRuleFindText();
            newValidationRule.FindText = textToFind;
            newValidationRule.IgnoreCase = false;
            newValidationRule.UseRegularExpression = useRegEx;
            newValidationRule.PassIfTextFound = passIfFound;
            webRequest.ValidateResponse += new EventHandler<ValidationEventArgs>(newValidationRule.Validate);

            return webRequest;            
        }


        /// <summary>
        /// Used to start or stop a service before updating files. 'setStatus' should be passed in as 'Start' or 'Stop'
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="serviceLocation"></param>
        /// <param name="setStatus"></param>
        public void StartStopService(string serviceName, string serviceLocation, string setStatus)
        {
            ServiceController myService = new ServiceController();
            myService.ServiceName = serviceName;
            myService.MachineName = serviceLocation;
            myService.Refresh();

            string svcStatus = myService.Status.ToString();

            DateTime timeStarted = DateTime.Now;
            TimeSpan timePassed = new TimeSpan();

            if (setStatus == "stop" && svcStatus != "Stopped")
            {
                myService.Stop();
                timePassed = (DateTime.Now - timeStarted);
                while (svcStatus != "Stopped")
                {
                    if (timePassed.Minutes > 1) { break; }
                    System.Threading.Thread.Sleep(10000);
                    timePassed = (DateTime.Now - timeStarted);                    
                    myService.Refresh();
                    svcStatus = myService.Status.ToString();
                }
            }
            else if (setStatus == "start" && svcStatus != "Running")
            {
                myService.Start();
            }
        }


        public void RecycleAppPool(string machine, string appPoolName)
        {
            string appPoolPath = "IIS://testweb1/W3SVC/AppPools/" + appPoolName;

            using (DirectoryEntry appPoolEntry = new DirectoryEntry(appPoolPath))
            {
                appPoolEntry.Username.Equals("NIBLEY\\james");
                // appPoolEntry.Password.
                appPoolEntry.Invoke("Recycle", null);
                appPoolEntry.Close();
            }
        }

        /// <summary>
        /// Used to get employeeid from the database by reading the username of the current logged in employee from the app.config
        /// </summary>
        public static int GetEmployeeIDByUsername(ProductType executingProduct, bool isClaimant = false)
        {
            int employeeid;
            string username = string.Empty;
            switch(executingProduct)
            {
                case ProductType.expenses:
                    if (isClaimant == false)
                    {
                        username = Convert.ToString(ConfigurationManager.AppSettings["expensesAdminUsername"].ToString());
                    }
                    else 
                    {
                        username = Convert.ToString(ConfigurationManager.AppSettings["expensesClaimantUsername"].ToString());
                    }
                    break;
                case ProductType.framework:
                    if (isClaimant == false)
                    {
                        username = Convert.ToString(ConfigurationManager.AppSettings["frameworkAdminUsername"].ToString());
                    }
                    else
                    {
                        username = Convert.ToString(ConfigurationManager.AppSettings["frameworkClaimantUsername"].ToString());
                    }
                    break;
                default:
                    if (isClaimant == false)
                    {
                        username = Convert.ToString(ConfigurationManager.AppSettings["expensesAdminUsername"].ToString());
                    }
                    else 
                    {
                        username = Convert.ToString(ConfigurationManager.AppSettings["expensesClaimantUsername"].ToString());
                    }
                    break;
            }
          
            cDatabaseConnection db = new cDatabaseConnection(cGlobalVariables.dbConnectionString(executingProduct));

            string strSQL = "SELECT employeeid FROM employees WHERE username = @username";
            db.sqlexecute.Parameters.AddWithValue("@username", username);
            using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(strSQL)) 
            {
                reader.Read();
                employeeid = reader.GetInt32(0);
                reader.Close();
            }
            db.sqlexecute.Parameters.Clear();
            return employeeid;
        }

        /// <summary>
        /// Used to wait for the page to load until either it finds the specified control or until a specific period of time passes
        /// </summary>
        public static void WaitForPage(UITestControl clickableElement, int millisecondsToWait)
        {
            Point ReturnedPoint = new Point();
            if (clickableElement != null)
            {
                int millisecondsPassed = 0;
                while (!clickableElement.TryGetClickablePoint(out ReturnedPoint) && millisecondsPassed < millisecondsToWait)
                {
                    System.Threading.Thread.Sleep(500);
                    millisecondsPassed += 500;
                }
                System.Diagnostics.EventLog.WriteEntry("Wait for page METHOD", String.Format("Control Coordinates - X:{0} Y:{1}      Time passed:{2}", ReturnedPoint.X.ToString(), ReturnedPoint.Y.ToString(), millisecondsPassed.ToString()));
            }
        }

        /// <summary>
        /// The get sub account id.
        /// </summary>
        /// <param name="executingProduct">
        /// The executing product.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        internal static int GetSubAccountId(ProductType executingProduct)
        {
            DBConnection expdata = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            const string Strsql = "select top 1 subAccountID from accountsSubAccounts ";
            int subAccountId = expdata.getcount(Strsql);
            expdata.sqlexecute.Parameters.Clear();
            return subAccountId;
        }

        /// <summary>
        /// Returns the accountID from registered users
        /// </summary>
        /// <param name="executingProduct">The executing product.</param>
        /// <returns>The account ID</returns>
        internal static int GetAccountID(ProductType executingProduct)
        {
            int accountId = 0;
            var db = new DBConnection(cGlobalVariables.MetabaseConnectionString(executingProduct));
            const string Sql = "SELECT accountid FROM registeredUsers WHERE dbname = @dbname";
            db.sqlexecute.Parameters.AddWithValue("@dbname", GetDatabaseNameFromConfig(executingProduct));
            using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(Sql))
            {
                reader.Read();
                accountId = reader.GetInt32(0);
                reader.Close();
            }
            db.sqlexecute.Parameters.Clear();

            return accountId;
        }

        /// <summary>
        /// Gets the DatabaseName from the connection string in the app.config
        /// </summary>
        /// <param name="product">The executing product.</param>
        /// <returns>The databaseName</returns>
        internal static string GetDatabaseNameFromConfig(ProductType product)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(cGlobalVariables.dbConnectionString(product));
            string databaseName = builder.InitialCatalog;
            return databaseName;
        }

    }

    public enum UserType
    {
        /// <summary>
        /// Use to signon as james
        /// </summary>
        Admin,
        /// <summary>
        /// Use to sign-on as testuser1
        /// </summary>
        Claimant
    }

}
