namespace Spend_Management
{
    using System;
    using System.Data;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Text;
    using System.Linq;

    using BusinessLogic;
    using BusinessLogic.Cache;
    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;

    using Common.Logging;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Cards;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Enumerators;
    using SpendManagementLibrary.Extentions;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Helpers.AuditLogger;
    using SpendManagementLibrary.Interfaces;
    using SpendManagementLibrary.Random;
    using Spend_Management.expenses.code;

    using Convert = System.Convert;
    using NullReferenceException = System.NullReferenceException;

    public class cCardStatements
    {
        private int _accountId;
        private readonly ICurrentUser _currentUser;
        private cGlobalCountries _countries;
        private cGlobalCurrencies _globalCurrencies;
        private cAccountSubAccounts _subAccounts;
        private cCurrencies _currencies;
        SortedList<string, cGlobalCurrency> currencies = new SortedList<string, cGlobalCurrency>();
        SortedList<string, cGlobalCountry> countries = new SortedList<string, cGlobalCountry>();
        SortedList<string, int?> cardnumbers = new SortedList<string, int?>();
        SortedList<int, string> validateCurrencyLst; //List of invalid currencies from the import of credit card transactions
        SortedList<int, string> validateCountryLst; //List of invalid countries from the import of credit card transactions
        Dictionary<int, cCardStatement> list;

        /// <summary>
        /// An instance of <see cref="ILog"/> for logging <see cref="ICache{T,TK}"/> diagnostics and information.
        /// </summary>
        private static readonly ILog Log = new LogFactory<cCardStatements>().GetLogger();
        public cCardStatements()
        {
        }
        public cCardStatements(int accountid, IDBConnection connection = null)
        {
            _accountId = accountid;
            InitialiseData(connection);
        }

        public const string CacheArea = "cardstatements";

        public static readonly string CannotSaveExistingStatementNamesMessage = "The statement name you have entered already exists. Please enter a different name.";

        private void InitialiseData(IDBConnection connection = null)
        {
            var cache = new Utilities.DistributedCaching.Cache();
            list = cache.Get(_accountId, string.Empty, CacheArea) as Dictionary<int, cCardStatement>;
            if (list == null)
            {
                list = CacheList(connection);
                cache.Add(accountid, string.Empty, CacheArea, list);
            }

            this._countries = new cGlobalCountries();
            this._globalCurrencies = new cGlobalCurrencies();
            this._subAccounts = new cAccountSubAccounts(_accountId);
            this._currencies = new cCurrencies(_accountId, this._subAccounts.getFirstSubAccount().SubAccountID);
        }

        private Dictionary<int, cCardStatement> CacheList(IDBConnection connection = null)
        {
            list = new Dictionary<int, cCardStatement>();
            using (IDBConnection connectionObject = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                var clscards = new CorporateCards(accountid, connection);
                const string Sql = "SELECT statementid, name, statementdate, cardproviderid, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy FROM dbo.card_statements_base";

                using (var reader = connectionObject.GetReader(Sql))
                {
                    while (reader.Read())
                    {
                        int statementid = reader.GetInt32(reader.GetOrdinal("statementid"));
                        string name = reader.GetString(reader.GetOrdinal("name"));
                        int cardproviderid = reader.GetInt32(reader.GetOrdinal("cardproviderid"));
                        cCorporateCard corporatecard = clscards.GetCorporateCardById(cardproviderid);
                        DateTime? statementdate;

                        if (reader.IsDBNull(reader.GetOrdinal("statementdate")))
                        {
                            statementdate = null;
                        }
                        else
                        {
                            statementdate = reader.GetDateTime(reader.GetOrdinal("statementdate"));
                        }

                        int createdby = reader.IsDBNull(reader.GetOrdinal("createdby")) ? 0 : reader.GetInt32(reader.GetOrdinal("createdby"));
                        DateTime createdon = reader.IsDBNull(reader.GetOrdinal("createdon")) ? new DateTime(1900, 01, 01) : reader.GetDateTime(reader.GetOrdinal("createdon"));
                        DateTime? modifiedon;

                        if (reader.IsDBNull(reader.GetOrdinal("modifiedon")))
                        {
                            modifiedon = null;
                        }
                        else
                        {
                            modifiedon = reader.GetDateTime(reader.GetOrdinal("modifiedon"));
                        }

                        int? modifiedby;

                        if (reader.IsDBNull(reader.GetOrdinal("modifiedby")))
                        {
                            modifiedby = null;
                        }
                        else
                        {
                            modifiedby = reader.GetInt32(reader.GetOrdinal("modifiedby"));
                        }

                        this.list.Add(statementid, new cCardStatement(this.accountid, corporatecard.cardprovider.cardproviderid, statementid, name, statementdate, createdon, createdby, modifiedon, modifiedby));
                    }

                    reader.Close();
                }
            }

            return list;
        }

        /// <summary>
        /// Force an update of the cache
        /// </summary>
        private void resetCache()
        {
            var cache = new Utilities.DistributedCaching.Cache();
            cache.Delete(accountid, string.Empty, CacheArea);
            list = null;
            InitialiseData();
        }

        #region properties
        public int accountid
        {
            get { return _accountId; }
        }

        #endregion

        /// <summary>
        /// Get the data for the import based on the credit card record type
        /// </summary>
        /// <param name="template"></param>
        /// <param name="recType"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public cImport GetCardRecordTypeData(cCardTemplate template, cCardRecordType recType, byte[] file)
        {
            cImport import = null;

            if (template != null)
            {
                switch (template.filetype)
                {
                    case ImportType.FlatFile:
                        import = new cFlatFileImport(accountid, file, template.delimiter, template.numHeaderRecords, template.numFooterRecords, false);
                        ((cFlatFileImport)import).extractDataFromFile(false);
                        break;
                    case ImportType.Excel:
                        import = new cExcelFileImport(accountid, file, template.numHeaderRecords, template.numFooterRecords, template.sheet);
                        ((cExcelFileImport)import).extractDataFromFile(false);
                        break;
                    case ImportType.FixedWidth:
                        import = new cFixedWidthFileImport(accountid, file, template.numHeaderRecords, template.numFooterRecords, recType.getStartAndEndLengths(), recType.GetSignValues());
                        ((cFixedWidthFileImport)import).extractDataFromFile(false);
                        break;
                }

                if (!import.isvalidfile || validateFile(recType, import.headerrow) == false)
                {
                    return null;
                }
            }
            return import;
        }

        /// <summary>
        /// Add the statement to the database
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public int addStatement(cCardStatement statement)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            int statementid;

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                connection.sqlexecute.Parameters.AddWithValue("@name", statement.name);
                if (statement.statementdate == null)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@statementdate", DBNull.Value);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@statementdate", statement.statementdate);
                }

                connection.sqlexecute.Parameters.AddWithValue("@cardproviderid", statement.Corporatecard.cardprovider.cardproviderid);

                connection.sqlexecute.Parameters.AddWithValue("@date", statement.createdon);

                if (statement.createdby > 0)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@userid", statement.createdby);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@userid", DBNull.Value);
                }

                connection.sqlexecute.Parameters.AddWithValue("@statementid", 0);

                if (currentUser != null && currentUser.isDelegate == true)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }

                connection.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                connection.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
                connection.ExecuteProc("saveCardStatement");
                statementid = (int)connection.sqlexecute.Parameters["@identity"].Value;
                connection.sqlexecute.Parameters.Clear();

                resetCache();
            }

            return statementid;
        }

        public int updateStatement(cCardStatement statement)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            int statementid = 0;
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                connection.sqlexecute.Parameters.AddWithValue("@name", statement.name);

                if (statement.statementdate == null)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@statementdate", DBNull.Value);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@statementdate", statement.statementdate);
                }

                connection.sqlexecute.Parameters.AddWithValue("@cardproviderid", statement.Corporatecard.cardprovider.cardproviderid);

                if (statement.modifiedon != null)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@date", statement.modifiedon);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@date", DBNull.Value);
                }

                if (statement.modifiedby != null)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@userid", statement.modifiedby);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@userid", DBNull.Value);
                }

                connection.sqlexecute.Parameters.AddWithValue("@statementid", statement.statementid);

                if (currentUser.isDelegate)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }

                connection.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                connection.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
                connection.ExecuteProc("saveCardStatement");
                statementid = (int)connection.sqlexecute.Parameters["@identity"].Value;

                connection.sqlexecute.Parameters.Clear();

                resetCache();
            }

            return statementid;
        }

        public bool deleteStatement(int statementid)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            int returnvalue;

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                connection.sqlexecute.Parameters.AddWithValue("@statementid", statementid);
                connection.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);

                if (currentUser.isDelegate)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }

                connection.sqlexecute.Parameters.Add("@returnvalue", SqlDbType.Int);
                connection.sqlexecute.Parameters["@returnvalue"].Direction = ParameterDirection.ReturnValue;
                connection.ExecuteProc("deleteCardStatement");
                returnvalue = (int)connection.sqlexecute.Parameters["@returnvalue"].Value;
                connection.sqlexecute.Parameters.Clear();

                resetCache();
            }

            return returnvalue == 0;

        }

        /// <summary>
        /// Validate the header information of the import file
        /// </summary>
        /// <param name="recType"></param>
        /// <param name="headerrow"></param>
        /// <returns></returns>
        private bool validateFile(cCardRecordType recType, string[] headerrow)
        {
            string value;
            DateTime date;
            foreach (cValidationField field in recType.HeaderFields)
            {
                value = headerrow[recType.HeaderFields.IndexOf(field)].Trim();
                if (field.expectedText != "")
                {
                    if (value != field.expectedText)
                    {
                        return false;
                    }
                }
                if (field.fieldType != "") //check type
                {
                    switch (field.fieldType)
                    {
                        case "D":
                            if (DateTime.TryParse(value, out date) == false)
                            {
                                return false;
                            }
                            break;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Validate the entire import file
        /// </summary>
        /// <param name="recType">Card record</param>
        /// <param name="import">The imported file</param>
        /// <param name="template">Template of the file to be imported</param>
        /// <param name="provider">The card provider</param>
        /// <param name="customerFields">Fields in the customers database</param>
        /// <param name="currencies">A list of currencies</param>
        /// <param name="countries">A list of countries</param>
        /// <returns>A list of error message if there are any otherwise null</returns>
        public List<string> ValidateWholeFile(cCardRecordType recType, cImport import, cCardTemplate template, cCardProvider provider, cFields customerFields, cCurrencies currencies, cCountries countries)
        {
            Guard.ThrowIfNull(recType, nameof(recType));
            Guard.ThrowIfNull(import, nameof(import));
            Guard.ThrowIfNull(template, nameof(template));
            Guard.ThrowIfNull(provider, nameof(provider));
            Guard.ThrowIfNull(customerFields, nameof(customerFields));
            Guard.ThrowIfNull(currencies, nameof(currencies));
            Guard.ThrowIfNull(countries, nameof(countries));
            if (Log.IsInfoEnabled)
            {
                Log.Info($"Validation of the import for the daily automatic import of provider {provider.cardprovider}, with provider Id {provider.cardproviderid} has started.");
            }

            var messagesToReturn = new List<string>();

            if (!import.isvalidfile)
            {
                messagesToReturn.Add("File is invalid");
                if (Log.IsInfoEnabled)
                {
                    Log.Info($"Validation of the import for the daily automatic import of provider {provider.cardprovider}, with provider Id {provider.cardproviderid} has failed due to the file being invalid");
                }

                return messagesToReturn;
            }
        
            
            #region validate transactions

            string currencycode;

            string countrycode;
            int? corporatecardid = null;
            bool orignalAmountFound = false, convertedAmountFound = false;

            int intval;
            decimal decval;
            DateTime dateval;
            bool boolval;
            string additionaltbl = "";
            //List of invalid currencies
            validateCurrencyLst = new SortedList<int, string>();
            //List of invalid countries
            validateCountryLst = new SortedList<int, string>();

            cCardCompanies cardCompanies = null;
            cCardCompany cardCompany = null;

            //Used to specify whether the data row should be read
            bool readFileContents = false;

            bool excludeRow = false;

            //If there are no card company records then there is no selective picking of transaction rows so the boolean value is set to true by default
            if (!template.RecordTypes.ContainsKey(CardRecordType.CardCompany))
            {
                readFileContents = true;
            }
            else
            {
                //only needed if being used for VCF4 files that have company records, so no need to cache the card companies
                cardCompanies = new cCardCompanies(this._accountId);
            }

            try
            {
                for (int currentLine = 0; currentLine < import.filecontents.Count; currentLine++)
                {
                    #region Check for row exclusions

                    //if the row value is equal to the exclusion value then dont import the row
                    if (recType.ExcludeValues.Count > 0)
                    {
                        excludeRow = false;

                        foreach (ExcludeValueObject exObj in recType.ExcludeValues)
                        {
                            if (exObj.excludeValue == import.filecontents[currentLine][exObj.excludeValueIndex].ToString())
                            {
                                excludeRow = true;
                                break;
                            }
                        }

                        if (excludeRow)
                        {
                            continue;
                        }
                    }

                    #endregion

                    //Used to check VCF4 formatted files
                    if (template.RecordTypes.ContainsKey(CardRecordType.CardCompany))
                    {
                        //Find the header record for the transaction record and set the boolean value to true to start the extracting of the transaction data
                        foreach (cValidationField headField in recType.HeaderFields)
                        {
                            if (import.filecontents[currentLine][0].ToString() == headField.TransactionCode &&
                                recType.UniqueValue == import.filecontents[currentLine][headField.RecordTypeColumnIndex]
                                    .ToString())
                            {
                                //Need to check that the card transactions are for a company the user of the system has specified. The lookup column Index 
                                //value is where the company number to compare is contained on the transaction header row
                                cardCompany = cardCompanies.GetCardCompanyByNumber(
                                    import.filecontents[currentLine][headField.LookupColumnIndex].ToString());

                                if (cardCompany != null)
                                {
                                    //if the used for import flag is true then the transactions for this company can be imported
                                    if (cardCompany.usedForImport)
                                    {
                                        readFileContents = true;
                                        currentLine++;
                                        break;
                                    }
                                }
                            }
                        }

                        //Find the footer record for the transaction record and set the boolean value to false to stop the extracting of the transaction data
                        foreach (cValidationField footField in recType.FooterFields)
                        {
                            if (import.filecontents[currentLine][0].ToString() == footField.TransactionCode &&
                                recType.UniqueValue == import.filecontents[currentLine][footField.RecordTypeColumnIndex]
                                    .ToString())
                            {
                                //Need to check that the card transactions footer is for the company the previous rows have been imported for. 
                                //The lookup column Index value is where the company number to compare is contained on the transaction footer row
                                cardCompany = cardCompanies.GetCardCompanyByNumber(
                                    import.filecontents[currentLine][footField.LookupColumnIndex].ToString());

                                if (cardCompany != null)
                                {
                                    //if the used for import flag is true then the transactions for this company will stop being imported
                                    if (cardCompany.usedForImport)
                                    {
                                        readFileContents = false;
                                        break;
                                    }
                                }
                            }
                        }
                    }


                    //Read the company data if the boolean flag is true 
                    if (readFileContents)
                    {
                        orignalAmountFound = false;
                        convertedAmountFound = false;

                        foreach (var field in recType.Fields)
                        {
                            var currentField =
                                customerFields.GetFieldByTableAndFieldName(field.mappedtable.ToLower().Trim(),
                                    field.mappedfield.ToLower().Trim());
                            var columnCount = import.filecontents[currentLine].Count - 1;
                            var valueToCheck = import.filecontents[currentLine][recType.Fields.IndexOf(field)].ToString().TrimEnd(' ');

                            switch (field.mappedfield.ToLower().Trim())
                            {
                                case "billingcurrencycode":
                                case "currency_code":
                                    string fileContentsCurrencyCode = recType.Fields.IndexOf(field) > columnCount
                                        ? null
                                        : valueToCheck;

                                    currencycode =
                                    (field.defaultvalue.Length > 0 &&
                                     string.IsNullOrWhiteSpace(fileContentsCurrencyCode))
                                        ? field.defaultvalue
                                        : fileContentsCurrencyCode;
                                    var globalCurrency = this.GetCurrency(currencycode, field.currencylookup);

                                    if (globalCurrency == null)
                                    {
                                        messagesToReturn.Add($"Global currency does not exist for {currentField.FieldName}. Currency code provided is {currencycode}, on line {currentLine}");
                                        break;
                                    }

                                    var doesCurrencyExists = this.checkCurrencyExists(globalCurrency.globalcurrencyid);
                                    //add the currency if it doesn't already exist
                                    if (!doesCurrencyExists)
                                    {
                                        this.AddNewCurrency(globalCurrency, currencies);
                                    }
                                    break;
                                case "country_code":
                                    string fileContentsCountryCode = recType.Fields.IndexOf(field) > columnCount
                                        ? null
                                        : valueToCheck;

                                    countrycode =
                                    (field.defaultvalue.Length > 0 &&
                                     string.IsNullOrWhiteSpace(fileContentsCountryCode))
                                        ? field.defaultvalue
                                        : fileContentsCountryCode;
                                    var globalCountry = this.GetCountry(countrycode, field.countrylookup);

                                    if (globalCountry == null)
                                    {
                                        messagesToReturn.Add($"Global country does not exist for {currentField.FieldName}. Country code provided is {countrycode}, on line {currentLine}");
                                        break;
                                    }

                                    var doesCountryExists = this.checkCountryExists(globalCountry.GlobalCountryId, countrycode);
                                    //add the country if it doesn't already exist
                                    if (!doesCountryExists)
                                    {
                                        this.AddNewCountry(globalCountry, countries);
                                    }
                                    break;
                                case "exchangerate":
                                    if (!decimal.TryParse(valueToCheck,
                                        out decval))
                                    {
                                        messagesToReturn.Add(
                                            $"The exchange rate value on line {currentLine} is in an invalid format");
                                    }
                                    break;
                                default:
                                    switch (field.fieldtype)
                                    {
                                        case "X":
                                            if (!bool.TryParse(valueToCheck, out boolval) || valueToCheck != "1" ||
                                                valueToCheck != "0" || valueToCheck.ToLower() != "yes" ||
                                                valueToCheck.ToLower() != "no")
                                            {
                                                messagesToReturn.Add(
                                                    $"The value for {currentField.FieldName} on line {currentLine} is in an invalid format");
                                            }
                                            break;

                                        case "S":
                                            if (valueToCheck.Length > currentField.Length)
                                            {
                                                messagesToReturn.Add(
                                                    $"The text length for {currentField.FieldName} on line {currentLine} exceeds the maximum allowed length of {currentField.Length}");
                                            }
                                            break;

                                        case "N":
                                            if (!int.TryParse(valueToCheck, out intval) && !string.IsNullOrEmpty(valueToCheck))
                                            {
                                                messagesToReturn.Add(
                                                    $"The numerical value for {currentField.FieldName} on line {currentLine} is in an invalid format");
                                            }
                                            break;

                                        case "M":
                                        case "C":
                                        case "FD":
                                            if (decimal.TryParse(valueToCheck, out decval))
                                            {
                                                if (field.mappedfield.ToLower().Trim() == "transaction_amount")
                                                {
                                                    convertedAmountFound = true;
                                                }
                                                else if (field.mappedfield.ToLower().Trim() == "original_amount")
                                                {
                                                    orignalAmountFound = true;
                                                }
                                            }
                                            else
                                            {
                                                messagesToReturn.Add(
                                                    $"The numerical value for {currentField.FieldName} on line {currentLine} is in an invalid format");
                                            }
                                            break;

                                        case "D":
                                            var isDefaultValue = (valueToCheck == "00000000"); // To handle default value of automatic HSBC date imports

                                            if (!DateTime.TryParseExact(valueToCheck, field.format, null, System.Globalization.DateTimeStyles.None, out dateval) && !isDefaultValue)
                                            {
                                                messagesToReturn.Add(
                                                    $"The date for {currentField.FieldName} on line {currentLine} is in an invalid format");
                                            }
                                            break;
                                    }
                                    break;
                            }
                        }

                        if (convertedAmountFound == false)
                        {
                            messagesToReturn.Add($"No sterling amount found on line {currentLine}");
                        }

                        if (orignalAmountFound == false)
                        {
                            messagesToReturn.Add($"No original currency amount found on line {currentLine}");
                        }
                    }
                }
            }
            catch (NullReferenceException ex)
            {
                messagesToReturn.Add("Validation for your automatic import has failed");
                if (Log.IsWarnEnabled)
                {
                    Log.Warn($"Validation of the import for the daily automatic import of provider {provider.cardprovider}, with provider Id {provider.cardproviderid} has failed due to a null reference exception.", ex);
                }
            }
            catch (Exception ex)
            {
                messagesToReturn.Add("Validation for your automatic import has failed");
                if (Log.IsWarnEnabled)
                {
                    Log.Warn($"Validation of the import for the daily automatic import of provider {provider.cardprovider}, with provider Id {provider.cardproviderid} has failed due to an exception.", ex);
                }
                throw;
            }
            finally 
            {
                if (messagesToReturn.Count == 0)
                {
                    if (Log.IsInfoEnabled)
                    {
                        Log.Info($"Validation of the import for the daily automatic import of provider {provider.cardprovider}, with provider Id {provider.cardproviderid} has finished.");
                    }
                }
                else
                {
                    if (Log.IsWarnEnabled)
                    {
                        Log.Warn($"Validation of the import for the daily automatic import of provider {provider.cardprovider}, with provider Id {provider.cardproviderid} has failed due to errors in the file");
                    }
                    Log.Warn("The errors are as follows");
                    foreach (var message in messagesToReturn)
                    {
                        Log.Warn(message);
                    }
                }
            }
            #endregion

            return messagesToReturn.Count > 0 ? messagesToReturn : null;
        }

        /// <summary>
        /// Adds a new currency
        /// </summary>
        /// <param name="globalCurrency">Global currency to be added</param>
        /// <param name="currencies">A list of currencies</param>
        public void AddNewCurrency(cGlobalCurrency globalCurrency, cCurrencies currencies)
        {
            Log.Info("Validation of the import has resulted in the need to add a new currency to the account.");
            byte positiveFormat = 1;
            byte negativeFormat = 1;

            var newCurrency = new cCurrency(this._accountId, 0, globalCurrency.globalcurrencyid, positiveFormat,
                negativeFormat, false, DateTime.UtcNow, null, DateTime.UtcNow, null);

            currencies.saveCurrency(newCurrency);
            Log.Info("Adding of a new currency has finished.");
        }

        /// <summary>
        /// Adds a new country
        /// </summary>
        /// <param name="globalCountry">Global country to be added</param>
        /// <param name="countries">A list of countries</param>
        public void AddNewCountry(cGlobalCountry globalCountry, cCountries countries)
        {
            Log.Info("Validation of the import has resulted in the need to add a new country to the account.");

            var newCountry = new cCountry(0, globalCountry.GlobalCountryId, false,
                new Dictionary<int, ForeignVatRate>(), DateTime.UtcNow, null);

            countries.saveCountry(newCountry);
            Log.Info("Adding of a new country has finished.");
        }


        /// <summary>
        /// Get the companies from the VCF4 data 
        /// </summary>
        /// <param name="statementid"></param>
        /// <param name="provider"></param>
        /// <param name="import"></param>
        /// <param name="recType"></param>
        /// <returns></returns>
        public SortedList<string, string> GetVCF4CompanyRecords(cImport import, cCardRecordType recType)
        {
            bool readFileContents = false;
            string companyName;
            string companyNumber;
            SortedList<string, string> lstCompanies = new SortedList<string, string>();

            for (int i = 0; i < import.filecontents.Count; i++)
            {
                companyName = string.Empty;
                companyNumber = string.Empty;

                //Find the first instance of the company header and set the boolean value to true to enable the company data to
                //be extracted
                foreach (cValidationField headField in recType.HeaderFields)
                {
                    if (import.filecontents[i][0].ToString() == headField.TransactionCode && recType.UniqueValue == import.filecontents[i][headField.RecordTypeColumnIndex].ToString())
                    {
                        readFileContents = true;
                        i++;
                        break;
                    }
                }

                //Find the footer record for the company record and set vthe boolean value to false to stop the extracting of the company data
                foreach (cValidationField footField in recType.FooterFields)
                {
                    if (import.filecontents[i][0].ToString() == footField.TransactionCode && recType.UniqueValue == import.filecontents[i][footField.RecordTypeColumnIndex].ToString())
                    {
                        readFileContents = false;
                        break;
                    }
                }

                //Read the company data if the boolean flag is true 
                if (readFileContents)
                {
                    foreach (cCardTemplateField field in recType.Fields)
                    {
                        switch (field.mappedfield)
                        {
                            case "CompanyName":
                                companyName = import.filecontents[i][field.FieldIndex].ToString().TrimEnd(' ');
                                break;

                            case "CompanyNumber":
                                companyNumber = import.filecontents[i][field.FieldIndex].ToString().TrimEnd(' ');
                                break;
                        }
                    }

                    lstCompanies.Add(companyNumber, companyName);
                }
            }

            return lstCompanies;
        }

        /// <summary>
        /// Import the card transactions in to the databsae
        /// </summary>
        /// <param name="template">Template of the file being imported</param>
        /// <param name="statementid">Id of the statement transactions are for</param>
        /// <param name="provider">Type of card provider being used</param>
        /// <param name="file">File to be imported</param>
        /// <param name="import">Imported file</param>
        /// <returns>The number of rows imported</returns>
        public int ImportCardTransactions(cCardTemplate template, int statementid, cCardProvider provider, byte[] file, cImport import)
        {
            var importedRows = 0;
            cCardRecordType recType = template.RecordTypes[CardRecordType.CardTransaction];

            string currencycode, cardnumber;
            
            string countrycode;
            int? corporatecardid = null;
            decimal originalamount = 0;
            decimal convertedamount = 0;

            #region import
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            string values;
            string strval;
            int intval;
            decimal decval;
            DateTime dateval;
            int transactionid;
            bool boolval;
            string additionaltbl = "";
            decimal exchangerate;
            bool exchangerateincluded = false;
            validateCurrencyLst = new SortedList<int, string>(); //List of invalid currencies
            validateCountryLst = new SortedList<int, string>(); //List of invalid countries

            cCardCompanies clsCardComps = null;
            cCardCompany cardComp = null;

            //Used to specify whether the data row should be read
            bool readFileContents = false;
            
            bool excludeRow = false;

            //If there are no card company records then there is no selective picking of transaction rows so the boolean value is set to true by default
            if (!template.RecordTypes.ContainsKey(CardRecordType.CardCompany))
            {
                readFileContents = true;
            }
            else
            {
                //only needed if being used for VCF4 files that have company records, so no need to cache the card companies
                clsCardComps = new cCardCompanies(accountid);
            }

            for (int i = 0; i < import.filecontents.Count; i++)
            {
                #region primary details

                #region Check for row exclusions

                //if the row value is equal to the exclusion value then dont import the rowe

                if (recType.ExcludeValues.Count > 0)
                {
                    excludeRow = false;

                    foreach (ExcludeValueObject exObj in recType.ExcludeValues)
                    {
                        if (exObj.excludeValue == import.filecontents[i][exObj.excludeValueIndex].ToString())
                        {
                            excludeRow = true;
                        }
                    }

                    if (excludeRow)
                    {
                        continue;
                    }
                }

                #endregion

                //Used to check VCF4 formatted files
                if (template.RecordTypes.ContainsKey(CardRecordType.CardCompany))
                {
                    //Find the header record for the transaction record and set the boolean value to true to start the extracting of the transaction data
                    foreach (cValidationField headField in recType.HeaderFields)
                    {
                        if (import.filecontents[i][0].ToString() == headField.TransactionCode && recType.UniqueValue == import.filecontents[i][headField.RecordTypeColumnIndex].ToString())
                        {
                            //Need to check that the card transactions are for a company the user of the system has specified. The lookup column Index 
                            //value is where the company number to compare is contained on the transaction header row
                            cardComp = clsCardComps.GetCardCompanyByNumber(import.filecontents[i][headField.LookupColumnIndex].ToString());

                            if (cardComp == null && provider.AutoImport)
                            {

                                cardComp = new cCardCompany(0, provider.cardprovider,
                                    import.filecontents[i][headField.LookupColumnIndex].ToString(), true, null,
                                    null, null, null);
                                clsCardComps.SaveCardCompany(cardComp);
                                cardComp = clsCardComps.GetCardCompanyByNumber(import.filecontents[i][headField.LookupColumnIndex].ToString());
                            }

                            if (cardComp != null)
                            {
                                //if the used for import flag is true then the transactions for this company can be imported
                                if (cardComp.usedForImport)
                                {
                                    readFileContents = true;
                                    i++;
                                    break;
                                }
                            }
                        }
                    }

                    //Find the footer record for the transaction record and set the boolean value to false to stop the extracting of the transaction data
                    foreach (cValidationField footField in recType.FooterFields)
                    {
                        if (import.filecontents[i][0].ToString() == footField.TransactionCode && recType.UniqueValue == import.filecontents[i][footField.RecordTypeColumnIndex].ToString())
                        {
                            //Need to check that the card transactions footer is for the company the previous rows have been imported for. 
                            //The lookup column Index value is where the company number to compare is contained on the transaction footer row
                            cardComp = clsCardComps.GetCardCompanyByNumber(import.filecontents[i][footField.LookupColumnIndex].ToString());

                            if (cardComp != null)
                            {
                                //if the used for import flag is true then the transactions for this company will stop being imported
                                if (cardComp.usedForImport)
                                {
                                    readFileContents = false;
                                    break;
                                }
                            }
                        }
                    }
                }


                //Read the company data if the boolean flag is true 
                if (readFileContents)
                {
                    strsql = "insert into card_transactions (statementid,";
                    values = " values (@statementid,";
                    expdata.sqlexecute.Parameters.AddWithValue("@statementid", statementid);
                    foreach (cCardTemplateField field in recType.Fields)
                    {
                        #region Check for any enclosed quoted characters

                        if (template.QuotedCharacters != string.Empty)
                        {
                            if ((field.defaultvalue.Length == 0))
                            {
                                import.filecontents[i][recType.Fields.IndexOf(field)] = import.filecontents[i][recType.Fields.IndexOf(field)].ToString().TrimStart(template.QuotedCharacters.ToCharArray());
                                import.filecontents[i][recType.Fields.IndexOf(field)] = import.filecontents[i][recType.Fields.IndexOf(field)].ToString().TrimEnd(template.QuotedCharacters.ToCharArray());
                            }
                        }

                        #endregion

                        if (field.mappedtable.ToLower().Trim() == "card_transactions")
                        {
                            int columnCount = import.filecontents[i].Count - 1;

                            if (field.mappedfield.ToLower().Trim() == "currency_code")
                            {
                                string fileContentsCurrencyCode = recType.Fields.IndexOf(field) > columnCount
                                    ? null
                                    : (string)import.filecontents[i][recType.Fields.IndexOf(field)];

                                currencycode = (field.defaultvalue.Length > 0 && string.IsNullOrWhiteSpace(fileContentsCurrencyCode)) ? field.defaultvalue : fileContentsCurrencyCode;
                                var globalCurrency = this.GetCurrency(currencycode, field.currencylookup);
                                this.checkCurrencyExists(globalCurrency?.globalcurrencyid ?? 0);
                                strsql += "[card_transactions].[currency_code], [card_transactions].[globalcurrencyid],";
                                values += "@currencycode,@globalcurrencyid,";
                                expdata.sqlexecute.Parameters.AddWithValue("@currencycode", currencycode);
                                expdata.sqlexecute.Parameters.AddWithValue("@globalcurrencyid", globalCurrency?.globalcurrencyid ?? 0);
                            }
                            else if (field.mappedfield.ToLower().Trim() == "country_code")
                            {
                                string fileContentsCountryCode = recType.Fields.IndexOf(field) > columnCount
                                    ? null
                                    : (string)import.filecontents[i][recType.Fields.IndexOf(field)];

                                countrycode = (field.defaultvalue.Length > 0 && string.IsNullOrWhiteSpace(fileContentsCountryCode)) ? field.defaultvalue : fileContentsCountryCode;
                                var globalCountry = this.GetCountry(countrycode, field.countrylookup);
                                this.checkCountryExists(globalCountry?.GlobalCountryId ?? 0, countrycode);
                                strsql += "[card_transactions].[country_code], [card_transactions].[globalcountryid],";
                                values += "@countrycode,@globalcountryid,";
                                expdata.sqlexecute.Parameters.AddWithValue("@countrycode", countrycode);
                                
                                    expdata.sqlexecute.Parameters.AddWithValue("@globalcountryid", globalCountry == null ? 0 : globalCountry.GlobalCountryId);    
                            }
                            else if (field.mappedfield.ToLower().Trim() == "exchangerate")
                            {
                                exchangerateincluded = true;
                                strsql += "[card_transactions].[exchangerate],";
                                values += "@exchangerate,";
                                decval = Convert.ToDecimal(import.filecontents[i][recType.Fields.IndexOf(field)]);
                                expdata.sqlexecute.Parameters.AddWithValue("@exchangerate", decval);
                            }
                            else if (field.mappedfield.ToLower().Trim() == "card_number")
                            {
                                cardnumber = Convert.ToString(import.filecontents[i][recType.Fields.IndexOf(field)]).Replace(" ", "");
                                corporatecardid = getCorporateCardID(cardnumber, provider.cardproviderid);
                                strsql += "[card_transactions].[card_number], [card_transactions].[corporatecardid],";
                                values += "@cardnumber, @corporatecardid,";
                                expdata.sqlexecute.Parameters.AddWithValue("@cardnumber", cardnumber);
                                if (corporatecardid == null)
                                {
                                    expdata.sqlexecute.Parameters.AddWithValue("@corporatecardid", DBNull.Value);
                                }
                                else
                                {
                                    expdata.sqlexecute.Parameters.AddWithValue("@corporatecardid", corporatecardid);
                                }
                            }
                            else
                            {
                                strsql += "[" + field.mappedfield + "],";
                                values += "@" + recType.Fields.IndexOf(field) + ",";
                                switch (field.fieldtype)
                                {
                                    case "X":
                                        try
                                        {
                                            boolval = Convert.ToBoolean(import.filecontents[i][recType.Fields.IndexOf(field)]);
                                        }
                                        catch //must be a number
                                        {
                                            if (import.filecontents[i][recType.Fields.IndexOf(field)].ToString() == "")
                                            {
                                                boolval = false;
                                            }
                                            else
                                            {
                                                boolval = Convert.ToBoolean(Convert.ToInt32(import.filecontents[i][recType.Fields.IndexOf(field)]));
                                            }
                                        }
                                        expdata.sqlexecute.Parameters.AddWithValue("@" + recType.Fields.IndexOf(field), Convert.ToByte(boolval));
                                        break;
                                    case "S":
                                        strval = (string)import.filecontents[i][recType.Fields.IndexOf(field)];
                                        expdata.sqlexecute.Parameters.AddWithValue("@" + recType.Fields.IndexOf(field), strval.TrimEnd(' '));
                                        break;
                                    case "N":
                                        intval = Convert.ToInt32(import.filecontents[i][recType.Fields.IndexOf(field)]);
                                        expdata.sqlexecute.Parameters.AddWithValue("@" + recType.Fields.IndexOf(field), intval);
                                        break;
                                    case "M":
                                    case "C":
                                    case "FD":
                                        if (decimal.TryParse(import.filecontents[i][recType.Fields.IndexOf(field)].ToString(), out decval))
                                        {
                                            // If there are a number of decimal places specified as implied for the field
                                            // then the number is parsed as an int and divided by 10 to the power of the 
                                            // number of decimal places
                                            if (field.numDecimalPlaces.HasValue && field.numDecimalPlaces.Value != 0)
                                            {
                                                decval = decval / (decimal)(Math.Pow((double)10, (double)field.numDecimalPlaces.Value));
                                            }
                                        }

                                        if (field.mappedfield.ToLower().Trim() == "transaction_amount")
                                        {
                                            convertedamount = decval;
                                        }
                                        else if (field.mappedfield.ToLower().Trim() == "original_amount")
                                        {
                                            originalamount = decval;
                                        }
                                        expdata.sqlexecute.Parameters.AddWithValue("@" + recType.Fields.IndexOf(field), decval);
                                        break;
                                    case "D":
                                        if (DateTime.TryParseExact((string)import.filecontents[i][recType.Fields.IndexOf(field)], field.format, null, System.Globalization.DateTimeStyles.None, out dateval) == true)
                                        {
                                            expdata.sqlexecute.Parameters.AddWithValue("@" + recType.Fields.IndexOf(field), dateval);
                                        }
                                        else
                                        {
                                            if (import.filecontents[i][recType.Fields.IndexOf(field)].ToString().Length == 8)
                                            {
                                                //Parse a date with no formatting
                                                string sDate = import.filecontents[i][recType.Fields.IndexOf(field)].ToString();
                                                if (DateTime.TryParseExact(sDate.Substring(0, 2) + "/" + sDate.Substring(2, 2) + "/" + sDate.Substring(4, 4), field.format, null, System.Globalization.DateTimeStyles.None, out dateval))
                                                {
                                                    expdata.sqlexecute.Parameters.AddWithValue("@" + recType.Fields.IndexOf(field), dateval);
                                                }
                                                else
                                                {
                                                    expdata.sqlexecute.Parameters.AddWithValue("@" + recType.Fields.IndexOf(field), DBNull.Value);
                                                }
                                            }
                                            else if (import.filecontents[i][recType.Fields.IndexOf(field)].ToString().Length == 10)
                                            {
                                                if (DateTime.TryParseExact(import.filecontents[i][recType.Fields.IndexOf(field)].ToString(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dateval))
                                                {
                                                    expdata.sqlexecute.Parameters.AddWithValue("@" + recType.Fields.IndexOf(field), dateval);
                                                }
                                                else
                                                {
                                                    expdata.sqlexecute.Parameters.AddWithValue("@" + recType.Fields.IndexOf(field), DBNull.Value);
                                                }
                                            }
                                            else
                                            {
                                                expdata.sqlexecute.Parameters.AddWithValue("@" + recType.Fields.IndexOf(field), DBNull.Value);
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    if (!exchangerateincluded && Math.Sign(originalamount) == Math.Sign(convertedamount))
                    {
                        exchangerate = Math.Round(originalamount / convertedamount, 10, MidpointRounding.AwayFromZero);
                        strsql += "[card_transactions].[exchangerate],";
                        values += "@exchangerate,";
                        expdata.sqlexecute.Parameters.AddWithValue("@exchangerate", exchangerate);
                    }

                    strsql = strsql.Remove(strsql.Length - 1, 1);
                    values = values.Remove(values.Length - 1, 1);
                    strsql += ")" + values + "); select @identity = scope_identity()";
                    expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                    expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.Output;
                    expdata.ExecuteSQL(strsql);
                    importedRows++;
                    transactionid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
                    expdata.sqlexecute.Parameters.Clear();

                    #endregion

                    #region additional details

                    strsql = "";
                    values = "values (@transactionid,";
                    expdata.sqlexecute.Parameters.AddWithValue("@transactionid", transactionid);

                    bool bContainsAdditionalFields = false;

                    foreach (cCardTemplateField field in recType.Fields)
                    {
                        if (field.mappedtable.ToLower().Trim() != "card_transactions")
                        {
                            bContainsAdditionalFields = true;
                            additionaltbl = field.mappedtable.ToLower().Trim();
                            strsql += "[" + field.mappedfield + "],";
                            values += "@" + recType.Fields.IndexOf(field) + ",";
                            switch (field.fieldtype)
                            {
                                case "X":
                                    try
                                    {
                                        boolval = Convert.ToBoolean(import.filecontents[i][recType.Fields.IndexOf(field)]);
                                    }
                                    catch //must be a number
                                    {
                                        if (import.filecontents[i][recType.Fields.IndexOf(field)].ToString() == "")
                                        {
                                            boolval = false;
                                        }
                                        else
                                        {
                                            boolval = Convert.ToBoolean(Convert.ToInt32(import.filecontents[i][recType.Fields.IndexOf(field)]));
                                        }
                                    }
                                    expdata.sqlexecute.Parameters.AddWithValue("@" + recType.Fields.IndexOf(field), Convert.ToByte(boolval));
                                    break;
                                case "S":
                                    strval = (string)import.filecontents[i][recType.Fields.IndexOf(field)].ToString().TrimEnd(' ');

                                    if (strval != string.Empty)
                                    {
                                        expdata.sqlexecute.Parameters.AddWithValue("@" + recType.Fields.IndexOf(field), strval);
                                    }
                                    else
                                    {
                                        expdata.sqlexecute.Parameters.AddWithValue("@" + recType.Fields.IndexOf(field), DBNull.Value);
                                    }
                                    break;
                                case "N":
                                    if (!int.TryParse(import.filecontents[i][recType.Fields.IndexOf(field)].ToString(), out intval))
                                    {
                                        intval = 0;
                                    }
                                    expdata.sqlexecute.Parameters.AddWithValue("@" + recType.Fields.IndexOf(field), intval);
                                    break;
                                case "M":
                                case "C":
                                case "FD":
                                    decval = Convert.ToDecimal(import.filecontents[i][recType.Fields.IndexOf(field)]);

                                    if (decimal.TryParse(import.filecontents[i][recType.Fields.IndexOf(field)].ToString(), out decval))
                                    {
                                        //If there are decimal points specified then the number is parsed as an int and divided by 10 to the power of the 
                                        //number of decimal places
                                        if (field.numDecimalPlaces != null)
                                        {
                                            if (decval > 0 && decval.ToString().Length > 2)
                                            {
                                                string tempStr = decval.ToString().Insert(decval.ToString().Length - field.numDecimalPlaces.Value, ".");
                                                decimal.TryParse(tempStr, out decval);
                                            }
                                        }
                                    }

                                    expdata.sqlexecute.Parameters.AddWithValue("@" + recType.Fields.IndexOf(field), decval);
                                    break;
                                case "D":
                                    if (DateTime.TryParseExact((string)import.filecontents[i][recType.Fields.IndexOf(field)], field.format, null, System.Globalization.DateTimeStyles.None, out dateval) == true)
                                    {
                                        expdata.sqlexecute.Parameters.AddWithValue("@" + recType.Fields.IndexOf(field), dateval);
                                    }
                                    else
                                    {
                                        if (import.filecontents[i][recType.Fields.IndexOf(field)].ToString().Length == 8)
                                        {
                                            //Parse a date with no formatting
                                            string sDate = import.filecontents[i][recType.Fields.IndexOf(field)].ToString();
                                            if (DateTime.TryParseExact(sDate.Substring(0, 2) + "/" + sDate.Substring(2, 2) + "/" + sDate.Substring(4, 4), field.format, null, System.Globalization.DateTimeStyles.None, out dateval))
                                            {
                                                expdata.sqlexecute.Parameters.AddWithValue("@" + recType.Fields.IndexOf(field), dateval);
                                            }
                                            else
                                            {
                                                expdata.sqlexecute.Parameters.AddWithValue("@" + recType.Fields.IndexOf(field), DBNull.Value);
                                            }
                                        }
                                        else
                                        {
                                            expdata.sqlexecute.Parameters.AddWithValue("@" + recType.Fields.IndexOf(field), DBNull.Value);
                                        }
                                    }
                                    break;
                            }

                        }
                    }

                    strsql = "insert into [" + additionaltbl + "] (transactionid," + strsql;
                    strsql = strsql.Remove(strsql.Length - 1, 1);
                    values = values.Remove(values.Length - 1, 1);
                    strsql += ")" + values + ")";
                    if (bContainsAdditionalFields == true)
                    {
                        expdata.ExecuteSQL(strsql);
                    }
                    expdata.sqlexecute.Parameters.Clear();
                }
                #endregion
            }
            #endregion

            return importedRows;
        }

        /// <summary>
        /// Send an email to employee/user.
        /// </summary>
        /// <param name="statementid">
        /// Id of statement.
        /// </param>
        public void SendEmployeeMail(int statementid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                int[] recipients = new int[1];

                var notifications = new NotificationTemplates(user);
                string strsql = "select distinct employeeid from employee_corporate_cards inner join card_transactions on card_transactions.corporatecardid = employee_corporate_cards.corporatecardid where statementid = @statementid";
                expdata.sqlexecute.Parameters.AddWithValue("@statementid", statementid);
                using (IDataReader reader = expdata.GetReader(strsql))
                {
                    expdata.sqlexecute.Parameters.Clear();
                    while (reader.Read())
                    {
                        recipients[0] = reader.GetInt32(0);
                        notifications.SendMessage(SendMessageEnum.GetEnumDescription(SendMessageDescription.ThisEmailIsSentToNotifyUsersTheirCreditCardStatementIsReady), user.EmployeeID, recipients);
                    }

                    reader.Close();
                }
            }
        }

        /// <summary>
        ///  Get country.
        /// </summary>
        /// <param name="countrycode">
        /// Country code.
        /// </param>
        /// <param name="countryLookup">
        /// Country lookup.
        /// </param>
        /// <returns>
        /// A global country.
        /// </returns>
        private cGlobalCountry GetCountry(string countrycode, CountryLookup countryLookup)
        {
            cGlobalCountry globalcountry = null;
            int countryid = 0;
            if (countries.Keys.Contains(countrycode))
            {
                globalcountry = countries[countrycode];
            }
            else
            {
                switch (countryLookup)
                {
                    case CountryLookup.Alpha:
                        globalcountry = this._countries.getGlobalCountryByAlphaCode(countrycode);
                        break;
                    case CountryLookup.Label:
                        globalcountry = this._countries.getCountryByName(countrycode);
                        break;
                    case CountryLookup.Alpha3:
                        globalcountry = this._countries.GetGlobalCountryByAlpha3Code(countrycode);
                        break;
                    case CountryLookup.Numeric3:
                        globalcountry = this._countries.GetGlobalCountryByNumeric3Code(int.Parse(countrycode));
                        break;
                }
                if (globalcountry != null)
                {
                    countries.Add(countrycode, globalcountry);
                }
            }

            return globalcountry;
        }

        /// <summary>
        /// Get corporate card id.
        /// </summary>
        /// <param name="cardnumber">
        /// Card Number.
        /// </param>
        /// <param name="cardprovider">
        /// Card provider.
        /// </param>
        /// <returns>
        /// For details <see cref="int?"/>.
        /// </returns>
        private int? getCorporateCardID(string cardnumber, int cardprovider)
        {
            int? corporatecardid = null;
            cardnumber = cardnumber.Replace(" ", "");
            if (cardnumbers.Keys.Contains(cardnumber))
            {
                cardnumbers.TryGetValue(cardnumber, out corporatecardid);
            }
            else
            {
                using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
                {
                    const string Sql = "select corporatecardid from employee_corporate_cards where cardproviderid = @cardprovider and cardnumber = @cardnumber";
                    connection.sqlexecute.Parameters.AddWithValue("@cardprovider", cardprovider);
                    connection.sqlexecute.Parameters.AddWithValue("@cardnumber", cardnumber);
                    using (IDataReader reader = connection.GetReader(Sql))
                    {
                        connection.sqlexecute.Parameters.Clear();
                        while (reader.Read())
                        {
                            corporatecardid = reader.GetInt32(0);
                            cardnumbers.Add(cardnumber, corporatecardid);
                        }
                        reader.Close();
                    }
                }
            }

            return corporatecardid;
        }

        /// <summary>
        /// Get a global currency
        /// </summary>
        /// <param name="currencycode">Currency code</param>
        /// <param name="currencytype">Currency lookup.</param>
        /// <returns>A global currency</returns>
        private cGlobalCurrency GetCurrency(string currencycode, CurrencyLookup currencytype)
        {
            cGlobalCurrency globalcurrency = null;
            if (currencies.Keys.Contains(currencycode))
            {
                globalcurrency = currencies[currencycode];
            }
            else
            {
                switch (currencytype)
                {
                    case CurrencyLookup.Numeric:
                        globalcurrency = this._globalCurrencies.getGlobalCurrencyByNumericCode(Convert.ToInt32(currencycode));
                        break;
                    case CurrencyLookup.Alpha:
                        globalcurrency = this._globalCurrencies.getGlobalCurrencyByAlphaCode(currencycode);
                        break;
                    case CurrencyLookup.Label:
                        globalcurrency = this._globalCurrencies.getGlobalCurrencyByLabel(currencycode);
                        break;
                }
                if (globalcurrency != null)
                {
                    currencies.Add(currencycode, globalcurrency);
                }
            }

            return globalcurrency;
        }

        /// <summary>
        /// Check to make sure the currency from the credit card statement exists;
        /// </summary>
        /// <param name="globalCurrencyID">ID of the currency from the metabase</param>
        private bool checkCurrencyExists(int globalCurrencyID)
        {
            cCurrency curr = this._currencies.getCurrencyByGlobalCurrencyId(globalCurrencyID);

            if (curr == null)
            {
                cGlobalCurrency globCurr = this._globalCurrencies.getGlobalCurrencyById(globalCurrencyID);

                if (globalCurrencyID == 0)
                {
                    if (!validateCurrencyLst.ContainsKey(globalCurrencyID))
                    {
                        validateCurrencyLst.Add(globalCurrencyID, "- The credit card statement imported successfully but there is a problem with the currencies please contact the support desk at Selenity Ltd to resolve.");
                        return false;
                    }
                }
                else
                {
                    if (!validateCurrencyLst.ContainsKey(globalCurrencyID))
                    {
                        validateCurrencyLst.Add(globalCurrencyID, "- Currency '" + globCurr.label + "' does not exist and needs to be added.");
                        return false;
                    }
                }
            }
            return true;

        }

        /// <summary>
        /// Check to make sure the country from the credit card statement exists;
        /// </summary>
        /// <param name="globalCountryID">ID of the country from the metabase</param>
        /// <param name="countryCode">Code of the country from the credit card transaction</param>
        private bool checkCountryExists(int globalCountryID, string countryCode)
        {
            // ONLY WORKS FOR A SINGLE SUBACCOUNT AT PRESENT.

            int subAccountId = this._subAccounts.getFirstSubAccount().SubAccountID;

            var clsCountries = new cCountries(accountid, subAccountId);
            cCountry country = clsCountries.getCountryByGlobalCountryId(globalCountryID);

            if (country == null)
            {
                cGlobalCountry globCountry = this._countries.getGlobalCountryById(globalCountryID);

                if (globalCountryID == 0)
                {
                    if (!validateCountryLst.ContainsKey(globalCountryID))
                    {
                        validateCountryLst.Add(globalCountryID, "- There is a problem with the countries on this credit card statement please contact the support desk at Selenity Ltd to resolve.");
                        return false;
                    }
                }
                else
                {
                    if (!validateCountryLst.ContainsKey(globalCountryID))
                    {
                        this.validateCountryLst.Add(globalCountryID,
                            globCountry == null
                                ? $"- A Country with country code \'{countryCode}\' does not exist and needs to added."
                                : $"- Country \'{globCountry.Country}\' with country code \'{countryCode}\' does not exist and needs to added.");

                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Gets a sorted list of any invalid countries, where they do not exist locally on the cutomer database
        /// </summary>
        /// <returns>Sorted list of string comments of invalid countries</returns>
        public SortedList<int, string> getInvalidCountries()
        {
            return validateCountryLst;
        }

        /// <summary>
        /// Gets a sorted list of any invalid currencies, where they do not exist locally on the cutomer database
        /// </summary>
        /// <returns>Sorted list of string comments of invalid currencies</returns>
        public SortedList<int, string> getInvalidCurrencies()
        {
            return validateCurrencyLst;
        }

        public cCardStatement getStatementById(int id)
        {
            cCardStatement statement = null;
            list.TryGetValue(id, out statement);
            return statement;
        }

        /// <summary>
        /// Check to see if the new statement name isn't alread used
        /// </summary>
        /// <param name="statement">Card statement</param>
        /// <returns>A boolean where true if statement name is found otherwise false</returns>
        public bool checkStatementNames(cCardStatement statement)
        {
            return list.Values.Any(b => b.name == statement.name);
        }

        public DataSet getUnallocatedCardGrid(cCardTemplate template, int statementid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            string additionaltable = "";
            strsql = "select DISTINCT card_transactions.card_number AS CardNumber,";

            foreach (cCardTemplateField field in template.RecordTypes[CardRecordType.CardTransaction].Fields)
            {
                if (field.displayinunallocatedcardgrid)
                {
                    if (field.mappedtable.Trim().ToLower() != "card_transactions")
                    {
                        additionaltable = field.mappedtable.Trim().ToLower();
                    }
                    if (field.mappedfield.Trim().ToLower() != "card_number")
                    {
                        strsql += "[" + field.mappedtable + "].[" + field.mappedfield + "] as [" + field.name + "],";
                    }

                }
            }

            strsql = strsql.Remove(strsql.Length - 1, 1);
            strsql += " FROM card_transactions";
            if (additionaltable != "")
            {
                strsql += " INNER JOIN " + additionaltable + " ON card_transactions.transactionid = [" + additionaltable + "].transactionid";
            }
            strsql += " WHERE corporatecardid is null and statementid = @statementid";
            expdata.sqlexecute.Parameters.AddWithValue("@statementid", statementid);
            DataSet ds = expdata.GetDataSet(strsql);
            expdata.sqlexecute.Parameters.Clear();
            return ds;
        }

        public DataSet getGrid()
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql = "select statementid, name, cardprovider, statementdate, createdon, item_count, unallocated_cards from card_statements order by [name]";
            DataSet ds = expdata.GetDataSet(strsql);
            return ds;
        }

        public void allocateCard(int statementid, int employeeid, string cardnumber)
        {
            cCardStatement statement = getStatementById(statementid);
            cEmployeeCorporateCards clsEmployeeCards = new cEmployeeCorporateCards(accountid);
            cEmployeeCorporateCard card = new cEmployeeCorporateCard(0, employeeid, statement.Corporatecard.cardprovider, cardnumber, true, DateTime.Now.ToUniversalTime(), employeeid, new DateTime(1900, 01, 01), 0);
            clsEmployeeCards.SaveCorporateCard(card);

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql = "update card_transactions set corporatecardid = @corporatecardid, modifiedon = @date, modifiedby = @userid where card_number = @cardnumber";
            expdata.sqlexecute.Parameters.AddWithValue("@corporatecardid", card.corporatecardid);
            expdata.sqlexecute.Parameters.AddWithValue("@cardnumber", cardnumber);
            expdata.sqlexecute.Parameters.AddWithValue("@date", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@userid", employeeid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();



        }

        /// <summary>
        /// Gets a HTML string of transaction details
        /// </summary>
        /// <param name="transactionId">
        /// The transaction Id to get the additional information for.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> of HTML.
        /// </returns>
        public string getTransactionDetails(int transactionId)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cCardTransaction transaction = this.getTransactionById(transactionId);
            cCardStatement statement = this.getStatementById(transaction.statementid);
            var templates = new cCardTemplates(user.AccountID);
            cCardTemplate templateData = templates.getTemplate(statement.Corporatecard.cardprovider.cardprovider);

            Dictionary<string, string> transactionDetails = this.GetTransactionData(templateData, transaction);

            var output = new StringBuilder();
            output.Append("<table>");

            foreach (var transactionField in transactionDetails)
            {
                output.Append("</tr>");
                output.Append("<tr>");
                output.Append("<td class=\"labeltd\">" + transactionField.Key + "</td>");
                output.Append("<td class=\"inputtd\">" + transactionField.Value + "</td>");
                output.Append("</tr>");
            }

            if (templateData == null)
            {
                output.Append("<tr>");
                output.Append(
                    "<td class=\"inputtd\" colspan=\"2\" align=\"center\">Could not load further information. There was a problem loading the card template.</td>");
                output.Append("</tr>");
            }

            output.Append("</table>");

            return output.ToString();
        }


        public string[] generateCardMatchingGrid(int employeeId, int claimId, int transactionId, int itemtype)
        {
            cTables tables = new cTables(accountid);
            cFields fields = new cFields(accountid);
            List<cNewGridColumn> columns = new List<cNewGridColumn>();
            columns.Add(new cFieldColumn(fields.GetFieldByID(Guid.Parse("A528DE93-3037-46F6-974C-A76BD0C8642A")))); //expense id
            columns.Add(new cFieldColumn(fields.GetFieldByID(Guid.Parse("A52B4423-C766-47BB-8BF3-489400946B4C")))); //date
            columns.Add(new cFieldColumn(fields.GetFieldByID(Guid.Parse("ABFE0BB2-E6AC-40D0-88CE-C5F7B043924D")))); //subcat
            columns.Add(new cFieldColumn(fields.GetFieldByID(Guid.Parse("1EE53AE2-2CDF-41B4-9081-1789ADF03459")))); //currency
            columns.Add(new cFieldColumn(fields.GetFieldByID(Guid.Parse("EC527561-DFEE-48C7-A126-0910F8E031B0")))); //country
            columns.Add(new cFieldColumn(fields.GetFieldByID(Guid.Parse("7CF61909-8D25-4230-84A9-F5701268F94B")))); //other details
            columns.Add(new cFieldColumn(fields.GetFieldByID(Guid.Parse("C3C64EB9-C0E1-4B53-8BE9-627128C55011")))); //total


            cGridNew grid = new cGridNew(accountid, employeeId, "gridMatchTransactions", tables.GetTableByID(Guid.Parse("D70D9E5F-37E2-4025-9492-3BCF6AA746A8")), columns);
            grid.WhereClause = "claimid = @claimId and itemtype = @itemType and savedexpenses.transactionid is null and savedexpensesGrid.floatid is null and (subcats.calculation = 1 or subcats.calculation = 2 or subcats.calculation = 5 or subcats.calculation = 6 or subcats.calculation = 8) and subcats.allowance = 0 and ((savedexpenses.convertedtotal > 0 and savedexpenses.convertedtotal <=  (select original_amount from card_transactions where transactionid = @transactionid)) or (savedexpenses.convertedtotal = 0 and savedexpenses.total <= (select transaction_amount from card_transactions where transactionid = @transactionid))) and savedexpenses.currencyid = (select currencyid from currencies inner join card_transactions on card_transactions.globalcurrencyid = currencies.globalcurrencyid where card_transactions.transactionid = @transactionid)";
            grid.EmptyText = "There are no expense items available which match this transaction";
            grid.KeyField = "expenseid";
            grid.getColumnByName("expenseid").hidden = true;
            grid.enablepaging = false;
            grid.EnableSorting = false;
            grid.addFilter(fields.GetFieldByID(Guid.Parse("34012174-7CE8-4F67-8B91-6C44AC1A4845")), "@claimId", claimId);
            grid.addFilter(fields.GetFieldByID(Guid.Parse("975454BF-3269-4903-8C3E-6962C5AFC181")), "@itemType", itemtype);
            grid.addFilter(fields.GetFieldByID(Guid.Parse("82EB7E79-2915-41B7-92CF-95D08B2CFE82")), "@transactionid", transactionId);
            grid.addFilter(fields.GetFieldByID(Guid.Parse("9045C0D2-CEB1-4E5F-8FC4-557B5C60C192")), "@floatid", DBNull.Value);
            grid.addFilter(fields.GetFieldByID(Guid.Parse("D4ED76BD-605C-45CE-B075-4C6018A50B08")), "@subatid", DBNull.Value);
            grid.EnableSelect = true;
            grid.GridSelectType = GridSelectType.RadioButton;
            return grid.generateGrid();
        }
        public string[] generateCardTransactionGrid(int statementId, int employeeId)
        {
            cFields fields = new cFields(accountid);
            cGridNew grid = new cGridNew(accountid, employeeId, "gridTransactions", "select transactionid, transaction_date, description, card_number, transaction_amount, original_amount, label, exchangerate, country, allocated_amount, unallocated_amount from employee_transactions");
            grid.WhereClause = "employee_transactions.statementid = @statementid and employee_corporate_cards.employeeid = @employeeid and (unallocated_amount is null or unallocated_amount > 0)";
            grid.KeyField = "transactionid";
            grid.getColumnByName("transactionid").hidden = true;

            grid.addEventColumn("moreDetailsLink", "../icons/16/plain/zoom_in.png", "javascript:SEL.Claims.ClaimViewer.DisplayTransactionDetails({transactionid})", "View Transaction Details", "View Transaction Details");
            grid.addEventColumn("matchLink", "../icons/16/plain/elements_selection.png", "javascript:SEL.Claims.ClaimViewer.ShowMatchTransactionGrid({transactionid});", "Match Transaction To Expense Item", "Match Transaction To Expense Item");
            grid.addEventColumn("addLink", "../icons/16/plain/add2.png", "javascript:SEL.Claims.ClaimViewer.CheckTransactionCurrencyAndCountry({transactionid});", "Add As New", "Add As New");
            SerializableDictionary<string, object> gridInfo = new SerializableDictionary<string, object> { };
            grid.InitialiseRowGridInfo = gridInfo;
            grid.InitialiseRow += new cGridNew.InitialiseRowEvent(this.transactionGrid_InitialiseRow);
            grid.ServiceClassForInitialiseRowEvent = "Spend_Management.cCardStatements";
            grid.ServiceClassMethodForInitialiseRowEvent = "transactionGrid_InitialiseRow";



            grid.addFilter(fields.GetFieldByID(Guid.Parse("c6a8cdb2-9f38-4f69-ba3b-db6476ae5a89")), "@statementid", statementId);
            grid.addFilter(fields.GetFieldByID(Guid.Parse("C93754A5-0945-4147-9CEF-B3F51F4CC396")), "@employeeid", employeeId);
            return grid.generateGrid();
        }

        private void transactionGrid_InitialiseRow(cNewGridRow row, SerializableDictionary<string, object> gridInfo)
        {
            int transactionId = (int)row.getCellByID("transactionId").Value;
            row.getCellByID("moreDetailsLink").Value = "<a href=\"javascript:javascript:SEL.Claims.ClaimViewer.DisplayTransactionDetails('gridTransactions'," + transactionId + ");\"><img src=\"../icons/16/plain/zoom_in.png\" id=\"viewTransactiongridTransactions" + transactionId + "\" alt=\"View Transaction Details\"/></a>";
        }

        public cCardTransaction getTransactionById(int transactionid, IDBConnection dbConnection = null)
        {
            cCardTransaction transaction = null;
            IDBConnection expdata = dbConnection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid));
            string reference, cardnumber, description;
            DateTime? transactiondate;
            int? corporatecardid, globalcurrencyid;
            int statementid;
            string currencycode;
            decimal transactionamount, originalamount, exchangerate;
            string countrycode;
            int? globalcountryid;
            DateTime createdon;
            int createdby;
            DateTime? modifiedon;
            int? modifiedby;
            string strsql = "SELECT transactionid, statementid, reference, card_number, corporatecardid, transaction_date, description, transaction_amount, original_amount, currency_code, globalcurrencyid, exchangerate, country_code, globalcountryid, createdon, createdby, modifiedon, modifiedby FROM card_transactions where card_transactions.transactionid = @transactionid";
            expdata.sqlexecute.Parameters.AddWithValue("@transactionid", transactionid);
            using (var reader = expdata.GetReader(strsql))
            {
                expdata.sqlexecute.Parameters.Clear();
                while (reader.Read())
                {
                    statementid = reader.GetInt32(reader.GetOrdinal("statementid"));
                    if (reader.IsDBNull(reader.GetOrdinal("reference")) == true)
                    {
                        reference = "";
                    }
                    else
                    {
                        reference = reader.GetString(reader.GetOrdinal("reference"));
                    }
                    cardnumber = reader.GetString(reader.GetOrdinal("card_number"));
                    if (reader.IsDBNull(reader.GetOrdinal("corporatecardid")) == true)
                    {
                        corporatecardid = null;
                    }
                    else
                    {
                        corporatecardid = reader.GetInt32(reader.GetOrdinal("corporatecardid"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("transaction_date")) == true)
                    {
                        transactiondate = null;
                    }
                    else
                    {
                        transactiondate = reader.GetDateTime(reader.GetOrdinal("transaction_date"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("description")) == true)
                    {
                        description = "";
                    }
                    else
                    {
                        description = reader.GetString(reader.GetOrdinal("description"));
                    }
                    transactionamount = reader.GetDecimal(reader.GetOrdinal("transaction_amount"));
                    if (reader.IsDBNull(reader.GetOrdinal("original_amount")) == true)
                    {
                        originalamount = 0;
                    }
                    else
                    {
                        originalamount = reader.GetDecimal(reader.GetOrdinal("original_amount"));
                    }
                    currencycode = reader.GetString(reader.GetOrdinal("currency_code"));
                    if (reader.IsDBNull(reader.GetOrdinal("globalcurrencyid")) == true)
                    {
                        globalcurrencyid = null;
                    }
                    else
                    {
                        globalcurrencyid = reader.GetInt32(reader.GetOrdinal("globalcurrencyid"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("exchangerate")) == true)
                    {
                        exchangerate = 1;
                    }
                    else
                    {
                        exchangerate = reader.GetDecimal(reader.GetOrdinal("exchangerate"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("country_code")) == true)
                    {
                        countrycode = "";
                    }
                    else
                    {
                        countrycode = reader.GetString(reader.GetOrdinal("country_code"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("globalcountryid")) == true)
                    {
                        globalcountryid = null;
                    }
                    else
                    {
                        globalcountryid = reader.GetInt32(reader.GetOrdinal("globalcountryid"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("createdon")) == true)
                    {
                        createdon = new DateTime(1900, 01, 01);
                    }
                    else
                    {
                        createdon = reader.GetDateTime(reader.GetOrdinal("createdon"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("createdby")) == true)
                    {
                        createdby = 0;
                    }
                    else
                    {
                        createdby = reader.GetInt32(reader.GetOrdinal("createdby"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("modifiedon")) == true)
                    {
                        modifiedon = new DateTime(1900, 01, 01);
                    }
                    else
                    {
                        modifiedon = reader.GetDateTime(reader.GetOrdinal("modifiedon"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("modifiedby")) == true)
                    {
                        modifiedby = 0;
                    }
                    else
                    {
                        modifiedby = reader.GetInt32(reader.GetOrdinal("modifiedby"));
                    }
                    transaction = new cCardTransaction(transactionid, statementid, reference, cardnumber, corporatecardid, transactiondate, description, transactionamount, originalamount, currencycode, globalcurrencyid, exchangerate, getMoreDetails(statementid, transactionid), countrycode, globalcountryid, createdon, createdby, modifiedon, modifiedby);
                }
                reader.Close();
            }
            return transaction;
        }

        /// <summary>
        /// The audit corporate card view.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="transaction">
        /// The transaction to audit.
        /// </param>
        /// <param name="auditLogger">
        /// The audit logger.
        /// </param>
        public void AuditTransactionView(ICurrentUserBase user, cCardTransaction transaction, IAuditLogger auditLogger)
        {
            auditLogger.ViewRecordAuditLog(user, SpendManagementElement.CorporateCards, $"{transaction.transactiondate}, {transaction.description}, {transaction.cardnumber.Redact()}, {transaction.transactionamount:0.00}");
        }

        private Dictionary<string, object> getMoreDetails(int statementid, int transactionid)
        {
            cCardStatement statement = getStatementById(statementid);
            cCardTemplates clstemplates = new cCardTemplates(accountid);
            cCardTemplate template = clstemplates.getTemplate(statement.Corporatecard.cardprovider.cardprovider);
            if (template == null)
            {
                return null;
            }
            cCardTemplateField cardfield;
            string othertable = "";
            Dictionary<string, object> moredetails = new Dictionary<string, object>();

            foreach (cCardRecordType recType in template.RecordTypes.Values)
            {
                foreach (cCardTemplateField field in recType.Fields)
                {
                    if (field.mappedtable.ToLower() != "card_transactions" && field.mappedtable.ToLower() != "cardcompanies")
                    {
                        othertable = field.mappedtable;
                        break;
                    }
                }

                if (othertable != "")
                {
                    DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
                    string strsql = "select * from " + othertable + " where transactionid = @transactionid";
                    expdata.sqlexecute.Parameters.AddWithValue("@transactionid", transactionid);
                    using (SqlDataReader reader = expdata.GetReader(strsql))
                    {
                        expdata.sqlexecute.Parameters.Clear();
                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if (reader.GetName(i) != "transactionid")
                                {
                                    if (!reader.IsDBNull(i))
                                    {
                                        cardfield = recType.getFieldByMappedField(reader.GetName(i));
                                        if (cardfield != null)
                                        {
                                            moredetails.Add(cardfield.name, reader.GetValue(i));
                                        }
                                    }
                                }
                            }
                        }
                        reader.Close();
                    }
                }
            }
            return moredetails;
        }

        /// <summary>
        /// Create drop down.
        /// </summary>
        /// <returns>
        /// For details <see cref="ListItem[]"/>.
        /// </returns>
        public ListItem[] CreateDropDown()
        {
            List<ListItem> lst = new List<ListItem>();
            foreach (cCardStatement statement in list.Values)
            {
                lst.Add(new ListItem(statement.name, statement.statementid.ToString()));
            }
            return lst.ToArray();
        }

        /// <summary>
        /// This method is used for creating Card Statements DropDown List
        /// </summary>
        /// <param name="employeeid">Id of the employee</param>
        /// <returns> List Items of Card Statements for given employee</returns>
        public ListItem[] createStatementDropDown(int employeeid)
        {
            var cardStatements = new cEmployeeCorporateCards(this.accountid).GetCardStatementsByEmployee(employeeid);
            var lst = cardStatements.Select(item => new ListItem(item.StatementName, item.StatementId.ToString())).ToList();
            return lst.ToArray();
        }

        public decimal getAllocatedAmount(cCardTransaction transaction)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            decimal allocatedamount = 0;
            if (transaction.originalamount > 0)
            {
                strsql = "select sum(convertedtotal) from savedexpenses where transactionid = @transactionid";
            }
            else
            {
                strsql = "select sum(total) from savedexpenses where transactionid = @transactionid";
            }
            expdata.sqlexecute.Parameters.AddWithValue("@transactionid", transaction.transactionid);
            using (SqlDataReader reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    if (reader.IsDBNull(0) == false)
                    {
                        allocatedamount = reader.GetDecimal(0);
                    }
                }
                reader.Close();
            }
            return allocatedamount;
        }
        public bool matchTransaction(cCardStatement statement, cCardTransaction transaction, cExpenseItem item)
        {
            decimal allocatedamount = getAllocatedAmount(transaction);
            decimal itemtotal;
            decimal transactionamount;

            cCountries clscountries = new cCountries(accountid, null);
            cCountry tranCountry = null;
            if (!transaction.globalcountryid.HasValue || transaction.globalcountryid == 0)
            {
                tranCountry = clscountries.getCountryByCode(transaction.countrycode);
            }
            else
            {
                tranCountry = clscountries.getCountryByGlobalCountryId((int)transaction.globalcountryid);
            }

            if (tranCountry == null || tranCountry.Archived || transaction.globalcurrencyid == 0 || !transaction.globalcurrencyid.HasValue)
            {
                return false;
            }

            var clscurrencies = new cCurrencies(accountid, null);
            cCurrency trancurrency = clscurrencies.getCurrencyByGlobalCurrencyId(transaction.globalcurrencyid.Value);
            if (trancurrency == null || trancurrency.archived)
            {
                return false;
            }
            cClaims clsclaims = new cClaims(accountid);
            cClaim claim = clsclaims.getClaimById(item.claimid);
            if (transaction.originalamount > 0 && transaction.originalamount != transaction.transactionamount)
            {
                //update the exchange rate and total
                if (Math.Round(item.exchangerate, 5, MidpointRounding.AwayFromZero) != Math.Round((double)transaction.exchangerate, 5, MidpointRounding.AwayFromZero))
                {
                    cExpenseItems clsitems = new cExpenseItems(accountid);
                    item.exchangerate = (double)transaction.reverseexchangerate;
                    item.total = item.convertedtotal;
                    clsitems.updateItem(item, claim.employeeid, claim.claimid, false);

                    itemtotal = item.convertedgrandtotal;
                    transactionamount = transaction.originalamount;
                }
                else
                {
                    itemtotal = item.convertedgrandtotal;
                    transactionamount = transaction.originalamount;
                }
            }
            else
            {
                itemtotal = item.grandtotal;
                transactionamount = transaction.transactionamount;
            }

            if (itemtotal > (transactionamount - allocatedamount))
            {
                return false;
            }

            this.RunMatchSQL(transaction.transactionid, item.expenseid);
            return true;
        }

        /// <summary>
        /// Match corporate card transaction to an expense item
        /// </summary>
        /// <param name="transactionid">Id of the corporate card transaction</param>
        /// <param name="expenseId">Id of expense item to be matched</param>
        private void RunMatchSQL(int transactionId, int expenseId)
        {
            using (IDBConnection expdata = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@transactionId", transactionId);
                expdata.sqlexecute.Parameters.AddWithValue("@expenseId", expenseId);
                expdata.ExecuteProc("MatchCorporateCardTransactionToExpenseItem");
                expdata.sqlexecute.Parameters.Clear();
            }
        }

        /// <summary>
        /// Gets the unallocated amount
        /// </summary>
        /// <param name="transactionid">The transaction id</param>
        /// <param name="employeeid">The employee id</param>
        /// <param name="generalOptionsFactory">An instance of <see cref="IDataFactory{IGeneralOptions,Int32}"/> to get a <see cref="IGeneralOptions"/></param>
        /// <returns>The unallocated amount</returns>
        public decimal getUnallocatedAmount(int transactionid, int employeeid, IDataFactory<IGeneralOptions, int> generalOptionsFactory)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            cCardTransaction transaction = getTransactionById(transactionid);

            cEmployees clsemployees = new cEmployees(accountid);
            Employee emp = clsemployees.GetEmployeeById(employeeid);
            int subAccountID = emp.DefaultSubAccount;

            string strsql;
            decimal allocatedamount = 0;
            bool overseasitem = false;
            decimal unallocatedamount;

            cCurrencies clscurrencies = new cCurrencies(accountid, subAccountID);

            var generalOptions = generalOptionsFactory[subAccountID].WithCurrency();

            int basecurrency;
            if (emp.PrimaryCurrency != generalOptions.Currency.BaseCurrency)
            {
                basecurrency = emp.PrimaryCurrency;
            }
            else
            {
                basecurrency = (int)generalOptions.Currency.BaseCurrency;
            }


            if (transaction.globalcurrencyid != null)
            {

                cCurrency currency = clscurrencies.getCurrencyByGlobalCurrencyId((int)transaction.globalcurrencyid);
                if (currency.currencyid == basecurrency)
                {
                    strsql = "select sum(total) from savedexpenses where transactionid = @transactionid";
                }
                else
                {
                    overseasitem = true;
                    strsql = "select sum(convertedtotal) from savedexpenses where transactionid = @transactionid";
                }
            }
            else
            {
                strsql = "select sum(convertedtotal) from savedexpenses where transactionid = @transactionid";
            }
            expdata.sqlexecute.Parameters.AddWithValue("@transactionid", transactionid);
            using (SqlDataReader reader = expdata.GetReader(strsql))
            {
                expdata.sqlexecute.Parameters.Clear();
                while (reader.Read())
                {
                    if (reader.IsDBNull(0) == false)
                    {
                        allocatedamount = reader.GetDecimal(0);
                    }
                }
                reader.Close();
            }

            if (overseasitem)
            {
                unallocatedamount = transaction.originalamount - allocatedamount;
            }
            else
            {
                unallocatedamount = transaction.transactionamount - allocatedamount;
            }
            return unallocatedamount;
        }

        public bool canReconcileItem(int claimid, int statementid, int employeeid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            int count = 0;
            strsql = "select count(*) from savedexpenses where claimid in (select claimid from claims where employeeid = @employeeid) and claimid <> @claimid and transactionid in (select transactionid from card_transactions where statementid = @statementid)";
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            expdata.sqlexecute.Parameters.AddWithValue("@claimid", claimid);
            expdata.sqlexecute.Parameters.AddWithValue("@statementid", statementid);
            count = expdata.getcount(strsql);
            expdata.sqlexecute.Parameters.Clear();

            if (count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the corporate card statement unallocated item count
        /// </summary>
        /// <param name="employeeid">The employeeid</param>
        /// <param name="statementid">The corporate card statement id</param>
        /// <returns>The unallocated item count</returns>
        public int getUnallocatedItemCount(int employeeid, int statementid)
        {
            int count;

            using (var databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@statementId", statementid);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeId", employeeid);
                count = databaseConnection.ExecuteScalar<int>("dbo.GetCorporateCardUnallocatedItemCount", CommandType.StoredProcedure);

                databaseConnection.sqlexecute.Parameters.Clear();
            }

            return count;
        }

        public void unmatchItem(int expenseId)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            expdata.sqlexecute.Parameters.AddWithValue("@expenseId", expenseId);
            expdata.ExecuteProc("unmatchTransaction");
            expdata.sqlexecute.Parameters.Clear();
        }

        public DataSet getTransactionGrid(int statementid, byte transactiontype, int employeeid)
        {
            DataSet ds;
            cCardStatement statement = getStatementById(statementid);
            cCardTemplates clstemplates = new cCardTemplates(accountid);
            cCardTemplate template = clstemplates.getTemplate(statement.Corporatecard.cardprovider.cardprovider);
            if (template == null)
            {
                return null;
            }
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            string additionaltbl = template.RecordTypes[CardRecordType.CardTransaction].additionaltable;
            strsql = "select * from employee_transactions";
            if (additionaltbl != "")
            {
                strsql += " inner join [" + additionaltbl + "] on employee_transactions.transactionid = [" + additionaltbl + "].transactionid";
            }

            strsql += " where statementid = @statementid";
            expdata.sqlexecute.Parameters.AddWithValue("@statementid", statementid);
            if (transactiontype > 0)
            {
                if (transactiontype == 1)
                {
                    strsql += " and allocated_amount = transaction_amount";
                }
                else
                {
                    strsql += " and (allocated_amount <> transaction_amount or allocated_amount is null)";
                }
            }

            if (employeeid > 0)
            {
                strsql += " and corporatecardid in (select corporatecardid from employee_corporate_cards where employeeid = @employeeid)";
                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            }

            ds = expdata.GetDataSet(strsql);
            expdata.sqlexecute.Parameters.Clear();
            return ds;
        }

        /// <summary>
        /// Gets the transaction details for the supplied current user and statementId.
        /// </summary>
        /// <param name="user">
        /// The current user
        /// </param>
        /// <param name="statementId">
        /// The statementId.
        /// </param>
        /// <returns>
        /// A list of <see cref="CreditCardTransaction"/>
        /// </returns>
        public List<CreditCardTransaction> GetTransactionsForStatementId(ICurrentUser user, int statementId)
        {
            var transactionDetails = new List<CreditCardTransaction>();

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                connection.ClearParameters();
                connection.sqlexecute.Parameters.AddWithValue("@employeeid", user.EmployeeID);
                connection.sqlexecute.Parameters.AddWithValue("@statementid", statementId);

                using (var reader = connection.GetReader("GetStatementTransactionsForEmployee", CommandType.StoredProcedure))
                {
                    var transactionIdOrd = reader.GetOrdinal("transactionId");
                    var transactionDateOrd = reader.GetOrdinal("transaction_date");
                    var descriptionOrd = reader.GetOrdinal("description");
                    var cardNumberOrd = reader.GetOrdinal("card_number");
                    var transactionAmountOrd = reader.GetOrdinal("transaction_amount");
                    var originalAmountOrd = reader.GetOrdinal("original_amount");
                    var labelOrd = reader.GetOrdinal("label");
                    var exchangeRateOrd = reader.GetOrdinal("exchangerate");
                    var countryOrd = reader.GetOrdinal("country");
                    var allocatedAmountOrd = reader.GetOrdinal("allocated_amount");
                    var unallocatedAmountOrd = reader.GetOrdinal("unallocated_amount");
                    var currencySymbolOrd = reader.GetOrdinal("currencySymbol");
                    var currencyIdOrd = reader.GetOrdinal("currencyid");

                    while (reader.Read())
                    {

                        var tranactionId = reader.GetInt32(transactionIdOrd);
                        var transactionDateTime = reader.IsDBNull(transactionDateOrd)
                                                      ? (DateTime?)null
                                                      : reader.GetDateTime(transactionDateOrd);
                        var description = reader.IsDBNull(descriptionOrd) ? string.Empty :
                                             reader.GetString(descriptionOrd);
                        var cardNumber = reader.GetString(cardNumberOrd);
                        var transactionAmount = reader.GetDecimal(transactionAmountOrd);                 
                        var orginalAmount = reader.IsDBNull(originalAmountOrd)
                                            ? (Decimal?)null
                                            : reader.GetDecimal(originalAmountOrd);

                        var label = reader.GetString(labelOrd);
                        var exchangeRate = reader.IsDBNull(exchangeRateOrd)
                                               ? (Decimal?)null
                                               : reader.GetDecimal(exchangeRateOrd);
                        var country = reader.GetString(countryOrd);
                        var allocatedAmount = reader.IsDBNull(allocatedAmountOrd)
                                                  ? (Decimal?)null
                                                  : reader.GetDecimal(allocatedAmountOrd);
                        var unallocatedAmount = reader.IsDBNull(unallocatedAmountOrd)
                                                    ? (Decimal?)null
                                                    : reader.GetDecimal(unallocatedAmountOrd);

                        var currencySymbol = reader.GetString(currencySymbolOrd);
                        var currencyId = reader.GetInt32(currencyIdOrd);

                        CreditCardTransaction transaction = new CreditCardTransaction(
                         tranactionId,
                         transactionDateTime,
                         description,
                         cardNumber,
                         transactionAmount,
                         orginalAmount,
                         label,
                         exchangeRate,
                         country,
                         allocatedAmount,
                         unallocatedAmount,
                         currencySymbol,
                         currencyId);

                        transactionDetails.Add(transaction);
                    }

                    return transactionDetails;
                }
            }
        }

        /// <summary>
        /// Builds up a dictionary of transaction details for the provided transaction
        /// </summary>      
        /// <param name="templateData">
        /// An instance of <see cref="cCardTemplate"/>
        /// </param>
        /// <param name="transaction">
        /// An instance of <see cref="cCardTransaction"/>
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/> with the field description and field value.
        /// </returns>
        public Dictionary<string, string> GetTransactionData(cCardTemplate templateData, cCardTransaction transaction)       
        {
            var transactionDetails = new Dictionary<string, string>();

            if (transaction.transactiondate != null)
            {
                transactionDetails.Add("Transaction Date:", ((DateTime)transaction.transactiondate).ToShortDateString());
            }

            transactionDetails.Add("Description:", transaction.description);

            if (transaction.globalcurrencyid != null && transaction.globalcurrencyid != 0)
            {
                cGlobalCurrency currency = this._globalCurrencies.getGlobalCurrencyById((int)transaction.globalcurrencyid);
                transactionDetails.Add("Currency:", currency.label);
            }

            transactionDetails.Add("Transaction Amount:", transaction.transactionamount.ToString("###,###,##0.00"));
            transactionDetails.Add("Original Amount:", transaction.originalamount.ToString("###,###,##0.00"));
            transactionDetails.Add("Exchange Rate:", transaction.exchangerate.ToString());

            if (templateData != null)
            {
                cCardRecordType recordType = templateData.RecordTypes[CardRecordType.CardTransaction];

                foreach (KeyValuePair<string, object> transactionDetail in transaction.moredetails)
                {
                    cCardTemplateField field = recordType.getFieldByName(transactionDetail.Key);

                    if (field != null && field.displayinmoredetailstable)
                    {
                        var value = field.fieldtype == "D"
                                           ? ((DateTime)transactionDetail.Value).ToShortDateString()
                                           : (string)transactionDetail.Value;

                        var key = $"{(string.IsNullOrEmpty(field.label) ? transactionDetail.Key : field.label)}:";
                        transactionDetails.Add(key, value);
                    }
                }
            }

            return transactionDetails;
        }

        /// <summary>
        /// Automatically import a file
        /// </summary>
        /// <param name="fileData">An instance of <see cref="FileDataRequest"/>containing the file to import</param>
        /// <param name="providerName">The name of the card provider</param>
        /// <returns>A positive number is the Statementid
        /// 0 = File failed validation
        /// -1 = Customer Identifier not found
        /// -100 - Customer identifier is not unique</returns>
        public int AutoImportFile(FileDataRequest fileData, string providerName)
        {
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat($"AutoImportFile for provider {providerName} with a file {fileData.FileContent.Length} long.");
            }

            var account = cAccounts.CachedAccounts.Values.FirstOrDefault();
            if (account == null)
            {
                Log.Error("First account not found.");
                throw new NullReferenceException("No accounts loaded");
            }

            var cardTemplates = new cCardTemplates(account.accountid);
            var providers = new CardProviders();
            
            var provider = providers.getProviderByName(providerName);
            if (provider == null)
            {
                if (Log.IsErrorEnabled)
                {
                    Log.ErrorFormat($"Card provider called '{providerName}' not found.");
                }
                throw new NullReferenceException("Provider not found");
            }
            
            var template = cardTemplates.getTemplate(providerName);
            var file = Convert.FromBase64String(fileData.FileContent);
            cImport import = this.GetCardRecordTypeData(template, template.RecordTypes[CardRecordType.CardTransaction], file);
            var customerId = this.FindCustomerId(import, template);
            var accountId = this.ValidateCustomerBankIdentifierAndReturnAccountId(customerId, provider);

            if (accountId == -100)
            {
                if (Log.IsErrorEnabled)
                {
                    Log.ErrorFormat($"Customer identifier '{customerId}' was not unique in all accounts.");
                }

                throw new DuplicateNameException("Customer Identifier not unique");
            }

            if (accountId == -1)
            {
                if (Log.IsErrorEnabled)
                {
                    Log.ErrorFormat($"Customer identifier '{customerId}' was not found in any account.");
                }

                return -1;
            }

            account = cAccounts.CachedAccounts[accountId];
            var cardImportLogs = new CorporateCardImportLogs(account);
            var importLog = new CorporateCardImportLog(provider.cardproviderid);
            this._accountId = account.accountid;
            this.InitialiseData();
            var fields = new cFields(account.accountid);
            var subAccounts =  new cAccountSubAccounts(account.accountid);
            var subAccount = subAccounts.getFirstSubAccount();
            var currencies = new cCurrencies(account.accountid, subAccount.SubAccountID);
            var countries = new cCountries(account.accountid, subAccount.SubAccountID);
            var validateResult = this.ValidateWholeFile(template.RecordTypes[CardRecordType.CardTransaction], import, template, provider, fields, currencies, countries);

            if (validateResult != null && validateResult.Count != 0)
            {
                importLog.NumberOfErrors = validateResult.Count;

                if (Log.IsWarnEnabled)
                {
                    var message = string.Join(", ", validateResult);
                    Log.WarnFormat($"The import for '{providerName}' has failed validation with the following messages: {message}.");
                }

                importLog.Status = CorporateCardImportStatus.FailedValidation;

                foreach (string validateString in validateResult)
                {
                    importLog.AppendToLog(validateString);
                }

                importLog.AppendToLog("Please contact the service desk at Selenity for further assistance.");

                cardImportLogs.Save(importLog);

                var notifications = new NotificationTemplates(account.accountid, 0, string.Empty, 0, Modules.expenses);
                NotificationTemplate reqmsg = notifications.Get(SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToAnAdministratorWhenACardImportFails));
                var recipientTypes = new List<sSendDetails>
                {
                    new sSendDetails
                    {
                        senderType = sendType.employee,
                        sender = subAccount.SubAccountProperties.EmailAdministrator,
                        recType = recipientType.to
                    }
                };

                var emailTemplate = new NotificationTemplate(reqmsg.EmailTemplateId, reqmsg.TemplateName,
                    recipientTypes, reqmsg.Subject, reqmsg.Body, reqmsg.SystemTemplate,
                    reqmsg.Priority, reqmsg.BaseTableId, reqmsg.CreatedOn,
                    reqmsg.CreatedBy, reqmsg.ModifiedOn, reqmsg.ModifiedBy, reqmsg.SendEmail, reqmsg.SendCopyToDelegates,
                    reqmsg.EmailDirection, reqmsg.SendNote, reqmsg.Note, reqmsg.TemplateId);

                var cardProviderIdField =
                    fields.GetFieldByID(new Guid(ReportKeyFields.CardProvidersCardProviderId));
                notifications.processEmail(emailTemplate, recipientTypes, cardProviderIdField,
                    provider.cardproviderid, 0, string.Empty);

                return 0;
            }

            // If good, import file
            var name = $"{provider.cardprovider} statement imported {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
            var statement = new cCardStatement(account.accountid, provider.cardproviderid, 0, name, DateTime.Today, DateTime.Now.ToUniversalTime(), null, DateTime.Now, null);
            int statementid = this.addStatement(statement);
            var rowCount = this.ImportCardTransactions(template, statementid, provider, file, import);
            importLog.StatementId = statementid;
            importLog.AppendToLog($"Imported {rowCount} transactions");
            importLog.Status = CorporateCardImportStatus.Imported;
            importLog.NumberOfErrors = 0;
            cardImportLogs.Save(importLog);
            if (Log.IsInfoEnabled)
            {
                Log.InfoFormat($"Customer identifier '{customerId}' import has completed with {rowCount} transactions.");
            }

            return statementid; 
        }

        /// <summary>
        /// Look in every actve customer database and look for matching parameters
        /// </summary>
        /// <param name="customerId">the customer ID to match</param>
        /// <param name="provider">An instance of <see cref="cCardProvider"/>to match on ID</param>
        /// <returns>The account ID where the customer ID and Provider Id match or
        /// -1 if no match is found
        /// -100 if multiple accounts have the same customer ID for the given provider.</returns>
        private int ValidateCustomerBankIdentifierAndReturnAccountId(string customerId, cCardProvider provider)
        {
            using (var dbConnection = new DatabaseConnection(GlobalVariables.MetabaseConnectionString))
            {
                dbConnection.AddWithValue("@cardProviderId", provider.cardproviderid);
                dbConnection.AddWithValue("@FileIdentifier", customerId);
                return dbConnection.ExecuteScalar<int>("ValidateCustomerBankIdentifier", CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// Extract the Customer ID from the current import file
        /// </summary>
        /// <param name="import">An instance of <see cref="cImport"/>Containing the current import file data</param>
        /// <param name="template">An instance of <see cref="cCardTemplate"/>which is used to decide the file</param>
        /// <returns>Either a valid Customer ID or spaces.</returns>
        private string FindCustomerId(cImport import, cCardTemplate template)
        {
            var recType = template.RecordTypes[CardRecordType.CardCompany];
            var companyNumber = string.Empty;
            for (int i = 0; i < import.filecontents.Count; i++)
            {
                if (template.RecordTypes.ContainsKey(CardRecordType.CardCompany))
                {
                    foreach (cValidationField headField in recType.HeaderFields)
                    {
                        if (import.filecontents[i][0].ToString() == headField.TransactionCode &&
                            recType.UniqueValue == import.filecontents[i][headField.RecordTypeColumnIndex]
                                .ToString())
                        {
                            // Found a company record.
                            foreach (cCardTemplateField cardTemplateField in recType.Fields)
                            {
                                if (cardTemplateField.name == "CompanyNumber")
                                {
                                    return import.filecontents[i][cardTemplateField.FieldIndex].ToString();
                                }   
                            }
                        }
                    }
                }
            }
            return companyNumber;
        }

        /// <summary>
        /// Creates Card Statements grid
        /// </summary>
        /// <param name="accountid">The account id of logged in user</param>
        /// <param name="employeeid">The user id of logged in user</param>
        /// <returns>Returns the html of grid</returns>
        public string[] createGrid(int accountid, int employeeid)
        {
            cTables clstables = new cTables(accountid);
            cFields clsfields = new cFields(accountid);
            List<cNewGridColumn> columns = new List<cNewGridColumn>();
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("ABC8A3E9-FFFD-463F-B957-D641DD049312")))); // statementid
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("CF224FEC-6B1F-46D2-B3A5-6D0554CCAE37")))); // name
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("265140FE-F439-442D-9B0C-8CE8B1D7F0D7")))); // cardprovider
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("7FB17FED-9243-40B5-BDF1-A4AAC1DEE6AF")))); // statementdate
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("D25CB71F-CC31-4D55-B62F-89A6D275FEB1")))); // CreatedOn
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("3C798501-B158-466C-9411-083792451A02")))); // item_count
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("7FB320D5-B1E7-4CC7-85F0-A5C45F055412")))); // unallocated_cards
            cGridNew clsgrid = new cGridNew(accountid, employeeid, "gridCardStatements", clstables.GetTableByID(new Guid("19931AA1-6287-4BD5-936C-1D7ADD367BF8")), columns); // card_statements
            clsgrid.getColumnByName("statementid").hidden = true;
            clsgrid.KeyField = "statementid";
            clsgrid.enabledeleting = true;
            clsgrid.deletelink = "javascript:deleteStatement({statementid});";
            clsgrid.enableupdating = true;
            clsgrid.editlink = "editstatement.aspx?statementid={statementid}";
            clsgrid.getColumnByName("CreatedOn").HeaderText = "Upload Date";
            clsgrid.InitialiseRow += this.statementGrid_InitialiseRow;
            clsgrid.ServiceClassForInitialiseRowEvent = "Spend_Management.cCardStatements";
            clsgrid.ServiceClassMethodForInitialiseRowEvent = "statementGrid_InitialiseRow";
            return clsgrid.generateGrid();
        }

        /// <summary>
        /// The initialise row event of the grid
        /// </summary>
        /// <param name="row">The row in the grid</param>
        /// <param name="gridinfo">The grid information</param>
        public void statementGrid_InitialiseRow(cNewGridRow row, SerializableDictionary<string, object> gridinfo)
        {
            row.getCellByID("name").Value =
                $"<a href=\"transaction_viewer.aspx?statementid={row.getCellByID("statementid").Value}\">{row.getCellByID("name").Value}</a>";
            if ((int)row.getCellByID("unallocated_cards").Value > 0)
            {
                row.getCellByID("unallocated_cards").Value =
                    $"<a href=\"unallocated_cards.aspx?statementid={row.getCellByID("statementid").Value}\">{row.getCellByID("unallocated_cards").Value}</a>";
            }
        }
    }
}

