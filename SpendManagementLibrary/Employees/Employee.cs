namespace SpendManagementLibrary.Employees
{
    /* 
    - THESE ARE NO LONGER USED AND SHOULD BE DROPPED FROM THE WORLD.
    nSupportPortalAccountID
    sSupportPortalPassword
    sAddress1;
    sAddress2;
    sCity;
    sCounty;
    sPostcode;
    sCountry;
    */

    // Make sure Views checks for null in the code.

    /*
     * - Need lazy loading on:
     * UserDefinedFields
     * MobileDevices
     */

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;

    using Microsoft.SqlServer.Server;

    using SpendManagementLibrary.Employees.DutyOfCare;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;
    using SpendManagementLibrary.Mobile;

    using Utilities.DistributedCaching;
    using System.Text;

    /// <summary>
    /// The employee.
    /// </summary>
    [Serializable]
    public class Employee : User
    {
        public const string CacheArea = "employee";

        /// <summary>
        /// A private list of <see cref="EmployeeItemRoles"/> for the employee
        /// </summary>
        private EmployeeItemRoles _itemRoles;

        public Employee()
        {
        }

        /// <summary>
        /// Create a new instance of <see cref="Employee"/>
        /// </summary>
        /// <param name="dvlaResponseCode"></param>
        public Employee(int accountID, int employeeID, string username, string password, string emailAddress, string title, string forename, string middleName, string maidenName,
            string surname, bool active, bool verified, bool archived, bool locked, int logonCount, int logonRetryCount, DateTime createdOn, int createdBy, DateTime? modifiedOn, int? modifiedBy,
            BankAccount bankAccount, int signOffGroupID, string telephoneExtensionNumber, string mobileTelephoneNumber, string pagerNumber, string faxNumber,
            string homeEmailAddress, int lineManager, int advancesSignOffGroup, string preferredName, string gender, DateTime? dateOfBirth, DateTime? hiredDate, DateTime? terminationDate,
            string payrollNumber, string position, string telephoneNumber, string creditor, CreationMethod creationMethod, PasswordEncryptionMethod passwordMethod, bool firstLogon, bool adminOverride,
            int defaultSubAccount, int primaryCurrency, int primaryCountry, int creditCardSignOffGroup, int purchaseCardSignOffGroup, bool hasCustomisedAddItems, int? localeID,
            int? nhsTrustID, string nationalInsuranceNumber, string employeeNumber, string nhsUniqueID, long? esrPersonID, DateTime? esrEffectiveStartDate, DateTime? esrEffectiveEndDate,
            int currentClaimNumber, DateTime lastChange, int currentReferenceNumber, int mileageTotal, DateTime? mileageTotalDate, bool contactHelpDeskAllowed, IDBConnection databaseConnection = null,
            int currency = 0, int? authoriserLevelDetailId = null,bool notifyClaimUnsubmission=false,DateTime? dvlaConsentDate = null,int? driverId=null,Guid? securityCode=null, string drivingLicenceFirstName = null, string drivingLicenceSurname = null, string drivingLicenceSex = null, string drivingLicenceDateOfBirth = null, string drivingLicneceEmail = null, string drivingLicenceNumber = null, string drivingLicenceMiddleName = null, DateTime? dvlaLookUpDate = null, string dvlaResponseCode = null, bool? agreeToProvideConsent = true, double excessMileage = 0)
            : base(accountID, employeeID, username, title, forename, surname)
        {
            this.PopulateEmployee(employeeID, username, password, emailAddress, title, forename, middleName, maidenName, surname, active, verified, archived, locked, logonCount, logonRetryCount, createdOn, createdBy, modifiedOn, modifiedBy, bankAccount, signOffGroupID, telephoneExtensionNumber, mobileTelephoneNumber, pagerNumber, faxNumber, homeEmailAddress, lineManager, advancesSignOffGroup, preferredName, gender, dateOfBirth, hiredDate, terminationDate, payrollNumber, position, telephoneNumber, creditor, creationMethod, passwordMethod, firstLogon, adminOverride, defaultSubAccount, primaryCurrency, primaryCountry, creditCardSignOffGroup, purchaseCardSignOffGroup, hasCustomisedAddItems, localeID, nhsTrustID, nationalInsuranceNumber, employeeNumber, nhsUniqueID, esrPersonID, esrEffectiveStartDate, esrEffectiveEndDate, currentClaimNumber, lastChange, currentReferenceNumber, mileageTotal, mileageTotalDate, contactHelpDeskAllowed, currency, authoriserLevelDetailId, notifyClaimUnsubmission, dvlaConsentDate, driverId, securityCode, drivingLicenceFirstName, drivingLicenceSurname, drivingLicenceSex, drivingLicenceDateOfBirth, drivingLicneceEmail, drivingLicenceNumber, drivingLicenceMiddleName, dvlaLookUpDate, dvlaResponseCode, agreeToProvideConsent, excessMileage);
        }


        /// <summary>
        /// Gets the requested employee from cache if possible, otherwise the database.
        /// </summary>
        /// <param name="employeeID">The employee ID you want retrieving.</param>
        /// <param name="accountID">The account ID the employee belongs to.</param>
        /// <param name="databaseConnection">The database connection to use.</param>
        /// <returns>The <see cref="Employee"/>.</returns>
        public static Employee Get(int employeeID, int accountID, IDBConnection connection = null)
        {
            Employee employee;
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountID)))
            {
                Cache caching = new Cache();

                employee = (Employee)caching.Get(accountID, CacheArea, employeeID.ToString(CultureInfo.InvariantCulture))
                    ?? GetFromDatabase(employeeID, accountID, databaseConnection);
            }

            return employee;
        }

        /// <summary>
        /// Gets an employee from the database
        /// </summary>
        /// <param name="employeeID">The employee ID you want retrieving.</param>
        /// <param name="accountID">The account ID the employee belongs to.</param>
        /// <param name="connection">The database connection to use.</param>
        /// <returns>The <see cref="Employee"/>.</returns>
        private static Employee GetFromDatabase(int employeeID, int accountID, IDBConnection connection = null)
        {
            Employee employee;
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountID)))
            {
                employee = null;

                databaseConnection.sqlexecute.Parameters.Clear();
                const string sql = "select employeeid, username, password, passwordmethod, title, firstname, surname, mileagetotal, mileagetotaldate, email, currefnum, curclaimno, payroll, position, telno, creditor, archived, groupid, roleid, hint, lastchange, additems, cardnum, userole, costcodeid, departmentid, extension, pagerno, mobileno, faxno, homeemail, linemanager, advancegroupid, active, primarycountry, primarycurrency, verified, groupidcc, groupidpc, CreatedOn, CreatedBy, ModifiedOn, modifiedby, ninumber, maidenname, middlenames, gender, dateofbirth, hiredate, terminationdate, homelocationid, officelocationid, [name], accountnumber, accounttype, sortcode, reference, localeID, NHSTrustID, logonCount, retryCount, firstLogon, defaultSubAccountId, supportPortalAccountID, supportPortalPassword, CreationMethod, adminonly, locked, preferredname, employeenumber, esrEffectiveStartDate, esrEffectiveEndDate, nhsUniqueId, EsrPersonid, contactHelpDeskAllowed, AuthoriserLevelDetailId,NotifyClaimUnsubmission, DVLAConsentDate, DriverId, SecurityCode, dbo.getDecryptedValue(drivinglicence_dateofbirth) as DrivingLicenceDateOfBirth, dbo.getDecryptedValue(drivinglicence_sex) as DrivingLicenceSex, dbo.getDecryptedValue(drivinglicence_firstname) as DrivingLicenceFirstName, dbo.getDecryptedValue(drivinglicence_surname) as DrivingLicenceSurname, dbo.getDecryptedValue(drivinglicence_middlename) as DrivingLicenceMiddleName, dbo.getDecryptedValue(drivinglicence_email) as DrivingLicenceEmail, dbo.getDecryptedValue(drivinglicence_licenceNumber) as DrivingLicenceNumber,DvlaLookUpDate as DvlaLookupDate,DvlaResponseCode as DvlaResponseCode, AgreeToProvideConsent, ExcessMileage from dbo.employees where employeeid = @employeeid";
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeid", employeeID);

                using (IDataReader reader = databaseConnection.GetReader(sql))
                {
                    while (reader.Read())
                    {
                        string username = reader.GetString(reader.GetOrdinal("username"));
                        string password = reader.IsDBNull(reader.GetOrdinal("password")) == false ? reader.GetString(reader.GetOrdinal("password")) : string.Empty;
                        string title = reader.IsDBNull(reader.GetOrdinal("title")) == false ? reader.GetString(reader.GetOrdinal("title")) : string.Empty;
                        string forename = reader.IsDBNull(reader.GetOrdinal("firstname")) == false ? reader.GetString(reader.GetOrdinal("firstname")) : string.Empty;
                        string surname = reader.IsDBNull(reader.GetOrdinal("surname")) == false ? reader.GetString(reader.GetOrdinal("surname")) : string.Empty;
                        bool archived = reader.GetBoolean(reader.GetOrdinal("archived"));
                        string creditor = reader.IsDBNull(reader.GetOrdinal("creditor")) == false ? reader.GetString(reader.GetOrdinal("creditor")) : string.Empty;
                        int currentClaimNumber = reader.GetInt32(reader.GetOrdinal("curclaimno"));
                        int currentReferenceNumber = reader.GetInt32(reader.GetOrdinal("currefnum"));
                        string emailAddress = reader.IsDBNull(reader.GetOrdinal("email")) == false ? reader.GetString(reader.GetOrdinal("email")) : string.Empty;
                        int signOffGroupID = (reader.IsDBNull(reader.GetOrdinal("groupid")) == false) ? reader.GetInt32(reader.GetOrdinal("groupid")) : 0;
                        DateTime lastChange = reader.IsDBNull(reader.GetOrdinal("lastchange")) == false ? reader.GetDateTime(reader.GetOrdinal("lastchange")) : DateTime.Today;
                        int mileageTotal = (reader.IsDBNull(reader.GetOrdinal("mileagetotal")) == false) ? reader.GetInt32(reader.GetOrdinal("mileagetotal")) : 0;
                        DateTime? mileageTotalDate = (reader.IsDBNull(reader.GetOrdinal("mileagetotaldate")) == false) ? (DateTime?)reader.GetDateTime(reader.GetOrdinal("mileagetotaldate")) : null;
                        string payrollNumber = reader.IsDBNull(reader.GetOrdinal("payroll")) == false ? reader.GetString(reader.GetOrdinal("payroll")) : string.Empty;
                        string position = reader.IsDBNull(reader.GetOrdinal("position")) == false ? reader.GetString(reader.GetOrdinal("position")) : string.Empty;
                        string telephoneNumber = reader.IsDBNull(reader.GetOrdinal("telno")) == false ? reader.GetString(reader.GetOrdinal("telno")) : string.Empty;
                        string telephoneExtensionNumber = reader.IsDBNull(reader.GetOrdinal("extension")) == false ? reader.GetString(reader.GetOrdinal("extension")) : string.Empty;
                        string homeEmailAddress = reader.IsDBNull(reader.GetOrdinal("homeemail")) == false ? reader.GetString(reader.GetOrdinal("homeemail")) : string.Empty;
                        string pagernNumber = reader.IsDBNull(reader.GetOrdinal("pagerno")) == false ? reader.GetString(reader.GetOrdinal("pagerno")) : string.Empty;
                        string mobileNumber = reader.IsDBNull(reader.GetOrdinal("mobileno")) == false ? reader.GetString(reader.GetOrdinal("mobileno")) : string.Empty;
                        string faxNumber = reader.IsDBNull(reader.GetOrdinal("faxno")) == false ? reader.GetString(reader.GetOrdinal("faxno")) : string.Empty;
                        int linemMnager = (reader.IsDBNull(reader.GetOrdinal("linemanager")) == false) ? reader.GetInt32(reader.GetOrdinal("linemanager")) : 0;
                        int advancesSignOffGroup = (reader.IsDBNull(reader.GetOrdinal("advancegroupid")) == false) ? reader.GetInt32(reader.GetOrdinal("advancegroupid")) : 0;
                        int primaryCountry = reader.IsDBNull(reader.GetOrdinal("primarycountry")) == false ? reader.GetInt32(reader.GetOrdinal("primarycountry")) : 0;
                        int primaryCurrency = reader.IsDBNull(reader.GetOrdinal("primarycurrency")) == false ? reader.GetInt32(reader.GetOrdinal("primarycurrency")) : 0;
                        bool active = reader.GetBoolean(reader.GetOrdinal("active"));
                        bool verified = reader.GetBoolean(reader.GetOrdinal("verified"));
                        int creditCardSignOffGroup = (reader.IsDBNull(reader.GetOrdinal("groupidcc")) == false) ? reader.GetInt32(reader.GetOrdinal("groupidcc")) : 0;
                        int purchaseCardSignOffGroup = (reader.IsDBNull(reader.GetOrdinal("groupidpc")) == false) ? reader.GetInt32(reader.GetOrdinal("groupidpc")) : 0;
                        string nationalInsuranceNumber = reader.IsDBNull(reader.GetOrdinal("ninumber")) == false ? reader.GetString(reader.GetOrdinal("ninumber")) : string.Empty;
                        string middleNames = reader.IsDBNull(reader.GetOrdinal("middlenames")) == false ? reader.GetString(reader.GetOrdinal("middlenames")) : string.Empty;
                        string maidenName = reader.IsDBNull(reader.GetOrdinal("maidenname")) == false ? reader.GetString(reader.GetOrdinal("maidenname")) : string.Empty;
                        string gender = reader.IsDBNull(reader.GetOrdinal("gender")) == false ? reader.GetString(reader.GetOrdinal("gender")) : string.Empty;
                        DateTime? dateOfBirth = (reader.IsDBNull(reader.GetOrdinal("dateofbirth")) == false) ? (DateTime?)reader.GetDateTime(reader.GetOrdinal("dateofbirth")) : null;
                        DateTime? hireDate = (reader.IsDBNull(reader.GetOrdinal("hiredate")) == false) ? (DateTime?)reader.GetDateTime(reader.GetOrdinal("hiredate")) : null;
                        DateTime? terminationDate = (reader.IsDBNull(reader.GetOrdinal("terminationdate")) == false) ? (DateTime?)reader.GetDateTime(reader.GetOrdinal("terminationdate")) : null;
                        string bankAccountName = reader.IsDBNull(reader.GetOrdinal("name")) == false ? reader.GetString(reader.GetOrdinal("name")) : string.Empty;
                        string bankAccountNumber = reader.IsDBNull(reader.GetOrdinal("accountnumber")) == false ? reader.GetString(reader.GetOrdinal("accountnumber")) : string.Empty;
                        string bankAccountType = reader.IsDBNull(reader.GetOrdinal("accounttype")) == false ? reader.GetString(reader.GetOrdinal("accounttype")) : string.Empty;
                        string bankAccountSortCode = reader.IsDBNull(reader.GetOrdinal("sortcode")) == false ? reader.GetString(reader.GetOrdinal("sortcode")) : string.Empty;
                        string bankAccountReference = reader.IsDBNull(reader.GetOrdinal("reference")) == false ? reader.GetString(reader.GetOrdinal("reference")) : string.Empty;
                        DateTime createdOn = (reader.IsDBNull(reader.GetOrdinal("createdon")) == false) ? reader.GetDateTime(reader.GetOrdinal("createdon")) : new DateTime(1900, 01, 01);
                        int createdBy = reader.IsDBNull(reader.GetOrdinal("createdby")) == false ? reader.GetInt32(reader.GetOrdinal("createdby")) : 0;
                        DateTime modifiedOn = (reader.IsDBNull(reader.GetOrdinal("modifiedon")) == false) ? reader.GetDateTime(reader.GetOrdinal("modifiedon")) : new DateTime(1900, 01, 01);
                        int modifiedBy = (reader.IsDBNull(reader.GetOrdinal("modifiedby")) == false) ? reader.GetInt32(reader.GetOrdinal("modifiedby")) : 0;
                        int? nhsTrustID = (reader.IsDBNull(reader.GetOrdinal("NHSTrustID")) == false) ? (int?)reader.GetInt32(reader.GetOrdinal("NHSTrustID")) : null;
                        int? localeID = reader.IsDBNull(reader.GetOrdinal("localeID")) == false ? (int?)reader.GetInt32(reader.GetOrdinal("localeID")) : null;
                        int? authoriserLevelDetailId = reader.IsDBNull(reader.GetOrdinal("AuthoriserLevelDetailId")) == false ? (int?)reader.GetInt32(reader.GetOrdinal("AuthoriserLevelDetailId")) : null;
                        PasswordEncryptionMethod passwordMethod = reader.IsDBNull(reader.GetOrdinal("passwordMethod")) == false ? (PasswordEncryptionMethod)reader.GetByte(reader.GetOrdinal("passwordMethod")) : PasswordEncryptionMethod.RijndaelManaged;
                        int logonCount = reader.GetInt32(reader.GetOrdinal("logonCount"));
                        int logonRetryCount = reader.GetInt32(reader.GetOrdinal("retryCount"));
                        bool firstLogon = reader.GetBoolean(reader.GetOrdinal("firstLogon"));
                        CreationMethod creationMethod = reader.IsDBNull(reader.GetOrdinal("CreationMethod")) == false ? (CreationMethod)reader.GetByte(reader.GetOrdinal("CreationMethod")) : CreationMethod.Manually;
                        bool adminOverride = reader.GetBoolean(reader.GetOrdinal("adminonly"));
                        bool locked = reader.GetBoolean(reader.GetOrdinal("Locked"));
                        string preferredName = reader.IsDBNull(reader.GetOrdinal("preferredname")) == false ? reader.GetString(reader.GetOrdinal("preferredname")) : string.Empty;
                        string nhsUniqueID = reader.IsDBNull(reader.GetOrdinal("nhsUniqueId")) == false ? reader.GetString(reader.GetOrdinal("nhsUniqueId")) : string.Empty;
                        string employeeNumber = reader.IsDBNull(reader.GetOrdinal("employeenumber")) == false ? reader.GetString(reader.GetOrdinal("employeenumber")) : string.Empty;
                        long? esrPersonID = (reader.IsDBNull(reader.GetOrdinal("EsrPersonId")) == false) ? (long?)reader.GetInt64(reader.GetOrdinal("EsrPersonId")) : null;
                        int defaultSubAccountID = (reader.IsDBNull(reader.GetOrdinal("defaultSubAccountId")) == false) ? reader.GetInt32(reader.GetOrdinal("defaultSubAccountId")) : -1;
                        DateTime? esrEffectiveStartDate = (reader.IsDBNull(reader.GetOrdinal("esrEffectiveStartDate")) == false) ? (DateTime?)reader.GetDateTime(reader.GetOrdinal("esrEffectiveStartDate")) : null;
                        DateTime? esrEffectiveEndDate = (reader.IsDBNull(reader.GetOrdinal("esrEffectiveEndDate")) == false) ? (DateTime?)reader.GetDateTime(reader.GetOrdinal("esrEffectiveEndDate")) : null;
                        bool contactHelpDeskAllowed = reader.GetBoolean(reader.GetOrdinal("ContactHelpDeskAllowed"));
                        bool notifyClaimUnsubmission = reader.GetBoolean(reader.GetOrdinal("NotifyClaimUnsubmission"));
                        bool hasCustomisedItems = HasCustomisedItems(employeeID, databaseConnection);
                        DateTime? dvlaConsentDate = (reader.IsDBNull(reader.GetOrdinal("DVLAConsentDate")) == false) ? (DateTime?)reader.GetDateTime(reader.GetOrdinal("DVLAConsentDate")) : null;
                        DateTime? dvlaLookupDate = (reader.IsDBNull(reader.GetOrdinal("DvlaLookupDate")) == false) ? (DateTime?)reader.GetDateTime(reader.GetOrdinal("DvlaLookupDate")) : null; 
                        int? driverId = (reader.IsDBNull(reader.GetOrdinal("DriverId"))==false)? (int?)reader.GetInt32(reader.GetOrdinal("DriverId")) : null;
                        Guid? securityCode = (reader.IsDBNull(reader.GetOrdinal("SecurityCode")) == false) ?reader.GetGuid(reader.GetOrdinal("SecurityCode")) : Guid.Empty;
                        string firstNameInDrivingLicence = reader.IsDBNull(reader.GetOrdinal("drivingLicenceFirstName")) == false ? reader.GetString(reader.GetOrdinal("drivingLicenceFirstName")) : null;
                        string surnameInDrivingLicence = reader.IsDBNull(reader.GetOrdinal("drivingLicencesurname")) == false ? reader.GetString(reader.GetOrdinal("drivingLicencesurname")) : null;
                        string dateOfBirthInDrivingLicence = reader.IsDBNull(reader.GetOrdinal("drivingLicenceDateOfBirth")) == false ? reader.GetString(reader.GetOrdinal("drivingLicenceDateOfBirth")) : null;
                        string sexInDrivinLicence = (reader.IsDBNull(reader.GetOrdinal("DrivingLicenceSex")) == false) ? reader.GetString(reader.GetOrdinal("DrivingLicenceSex")) : null;
                        string emailInDrivingLicence = reader.IsDBNull(reader.GetOrdinal("drivingLicenceEmail")) == false ? reader.GetString(reader.GetOrdinal("drivingLicenceEmail")) : null;
                        string drivingLicenceNumberInDrivinLicence = (reader.IsDBNull(reader.GetOrdinal("DrivingLicenceNumber")) == false) ? reader.GetString(reader.GetOrdinal("DrivingLicenceNumber")) : null;
                        string middleNameInDrivingLicence = reader.IsDBNull(reader.GetOrdinal("DrivingLicenceMiddleName")) == false ? reader.GetString(reader.GetOrdinal("DrivingLicenceMiddleName")) : null; 
                        string dvlaResponseCode = reader.IsDBNull(reader.GetOrdinal("DvlaResponseCode")) == false ? reader.GetString(reader.GetOrdinal("DvlaResponseCode")) : null;
                        // bool? agreeToProvideConsent = reader.IsDBNull(reader.GetOrdinal("AgreeToProvideConsent")) == false ? reader.GetBoolean(reader.GetOrdinal("AgreeToProvideConsent")) : (bool?)null;
                        double excessMileage = reader.IsDBNull(reader.GetOrdinal("ExcessMileage")) ? 0 : reader.GetDouble(reader.GetOrdinal("ExcessMileage"));
                        bool? agreeToProvideConsent;
                        if (reader.IsDBNull(reader.GetOrdinal("AgreeToProvideConsent")))
                        {
                            agreeToProvideConsent = null;
                        }
                        else
                        {
                            agreeToProvideConsent = reader.GetBoolean(reader.GetOrdinal("AgreeToProvideConsent"));
                        }

                        employee = new Employee(accountID, employeeID, username, password, emailAddress, title, forename, middleNames, maidenName, surname, active, verified, archived, locked, logonCount, logonRetryCount, createdOn, createdBy, modifiedOn, modifiedBy, new BankAccount(bankAccountName, bankAccountNumber, bankAccountType, bankAccountSortCode, bankAccountReference), signOffGroupID, telephoneExtensionNumber, mobileNumber, pagernNumber, faxNumber, homeEmailAddress, linemMnager, advancesSignOffGroup, preferredName, gender, dateOfBirth, hireDate, terminationDate, payrollNumber, position, telephoneNumber, creditor, creationMethod, passwordMethod, firstLogon, adminOverride, defaultSubAccountID, primaryCurrency, primaryCountry, creditCardSignOffGroup, purchaseCardSignOffGroup, hasCustomisedItems, localeID, nhsTrustID, nationalInsuranceNumber, employeeNumber, nhsUniqueID, esrPersonID, esrEffectiveStartDate, esrEffectiveEndDate, currentClaimNumber, lastChange, currentReferenceNumber, mileageTotal, mileageTotalDate, contactHelpDeskAllowed, databaseConnection, 0, authoriserLevelDetailId, notifyClaimUnsubmission, dvlaConsentDate, driverId,securityCode, firstNameInDrivingLicence, surnameInDrivingLicence, sexInDrivinLicence, dateOfBirthInDrivingLicence, emailInDrivingLicence, drivingLicenceNumberInDrivinLicence, middleNameInDrivingLicence, dvlaLookupDate, dvlaResponseCode, agreeToProvideConsent, excessMileage);
                    }

                    reader.Close();
                }
            }

            if (employee != null)
            {
                CacheAdd(employee, accountID);
                return employee;
            }

            return null;
        }

        /// <summary>
        /// Gets the requested employee's primary country from database.
        /// </summary>
        /// <param name="employeeID">The employee Id you want retrieving.</param>
        /// <param name="connection">The database connection to use.</param>
        /// <returns>primarycountry.</returns>
        public static string GetPrimaryCountry(int employeeId, IDBConnection connection)
        {
            StringBuilder primaryCountry = new StringBuilder();
            connection.sqlexecute.Parameters.Clear();
            connection.sqlexecute.Parameters.AddWithValue("@employeeid", employeeId);

            using (IDataReader reader = connection.GetReader("GetPrimaryCountry", CommandType.StoredProcedure))
            {
                while (reader.Read())
                {
                    var countryLabel = reader.GetOrdinal("country");
                    var countryId = reader.GetOrdinal("countryid");
                    primaryCountry.Append(reader.IsDBNull(countryLabel) == false ? reader.GetString(countryLabel) : string.Empty);
                    primaryCountry.Append(",");
                    primaryCountry.Append((reader.IsDBNull(countryId) == false ? reader.GetInt32(countryId) : 0).ToString());
                }
                reader.Close();
            }

            return primaryCountry.ToString();
        }

        /// <summary>
        /// Get the driving licence number by using employeeid
        /// </summary>
        /// <param name="employeeId">employeeid</param>
        /// <param name="accountId">accountid</param>
        /// <param name="connection">database connection</param>
        /// <returns>Vehicle Registration Number for the employee</returns>
        public string GetDrivingLicenceNumberByEmployeeId(int employeeId, int accountId, IDBConnection connection = null)
        {
            string drivingLicence = null;
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeid", employeeId);
                using (var reader = databaseConnection.GetReader("GetDrivingLicencNumberByEmployeeId", CommandType.StoredProcedure))
                {
                    var drivingLicenceOrd = reader.GetOrdinal("DrivingLicenceNumber");
                    while (reader.Read())
                    {
                        drivingLicence = reader.IsDBNull(drivingLicenceOrd) == false ? reader.GetString(drivingLicenceOrd) : string.Empty;
                    }
                }
            }

            return drivingLicence;
        }

        /// <summary>
        /// Sets the initial remainder date.
        /// </summary>
        /// <param name="accountId">Account ID</param>
        /// <param name="connection">database connection</param>
        public static void SetInitialClaimantReminderDate(int accountId, IDBConnection connection = null)
        {
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                databaseConnection.ExecuteProc("SetInitialClaimantReminderDate");
            }
        }


        private static bool HasCustomisedItems(int employeeID, IDBConnection connection)
        {
            var databaseConnection = connection;
            databaseConnection.sqlexecute.Parameters.Clear();
            const string SQL = "select count(*) from additems where employeeid = @employeeid";
            databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeid", employeeID);
            int count = databaseConnection.ExecuteScalar<int>(SQL);
            databaseConnection.sqlexecute.Parameters.Clear();
            return count > 0;
        }

        #region properties

        /// <summary>
        /// Gets or access roles this employee has.
        /// </summary>
        public EmployeeAccessRoles GetAccessRoles()
        {
            return new EmployeeAccessRoles(this.AccountID, this.EmployeeID);
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not this employee record is active or not.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this employee overrides all access role checks.
        /// </summary>
        public bool AdminOverride { get; set; }

        /// <summary>
        /// Gets or sets the advances sign off group ID for this employee.
        /// </summary>
        public int AdvancesSignOffGroup { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this employee is archived or not.
        /// </summary>
        public bool Archived { get; set; }

        /// <summary>
        /// Gets or sets the bank account details for this employee.
        /// </summary>
        public BankAccount BankAccountDetails { get; set; }

        /// <summary>
        /// Gets or sets the broadcast messages this employee is marked as having read.
        /// </summary>
        public EmployeeBroadcastMessages GetBroadcastMessagesRead()
        {
            return new EmployeeBroadcastMessages(this.AccountID, this.EmployeeID);
        }

        /// <summary>
        /// Gets
        /// </summary>
        public bool ContactHelpDeskAllowed { get; set; }

        /// <summary>
        /// Gets the cost breakdown list for this employee.
        /// </summary>
        public EmployeeCostBreakdown GetCostBreakdown()
        {
            return new EmployeeCostBreakdown(this.AccountID, this.EmployeeID);
        }

        /// <summary>
        /// Gets or sets the employee ID who created this employee record.
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the date this employee record was created.
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the method which was used to create this employee.
        /// </summary>
        public CreationMethod CreationMethod { get; set; }

        /// <summary>
        /// Gets or sets the credit card sign off group.
        /// </summary>
        public int CreditCardSignOffGroup { get; set; }

        /// <summary>
        /// Gets or sets the creditor for this employee.
        /// </summary>
        public string Creditor { get; set; }

        public int CurrentClaimNumber { get; set; }

        public int CurrentReferenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the date of birth of this employee.
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the default sub account for this employee.
        /// </summary>
        public int DefaultSubAccount { get; set; }

        /// <summary>
        /// Gets or sets the email address for this employee.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the date of birth of this employee as per the dvla record.
        /// </summary>
        public string DrivingLicenceDateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the surname of this employee as per the dvla record.
        /// </summary>
        public string DrivingLicenceSurname { get; set; }

        /// <summary>
        /// Gets or sets the middle name of this employee as per the dvla record.
        /// </summary>
        public string DrivingLicenceMiddleName { get; set; }

        /// <summary>
        /// Gets or sets the sex of this employee as per the dvla record.
        /// </summary>
        public string DrivingLicenceSex { get; set; }

        /// <summary>
        /// Gets or sets the firstname of this employee as per the dvla record.
        /// </summary>
        public string DrivingLicenceFirstname { get; set; }

        /// <summary>
        /// Gets or sets the email of this employee as per the dvla record.
        /// </summary>
        public string DrivingLicenceEmail { get; set; }

        /// <summary>
        /// Gets or sets the driving licence number of this employee as per the dvla record.
        /// </summary>
        public string DrivingLicenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the email notifications this employee is subscribed too
        /// </summary>
        public EmployeeEmailNotifications GetEmailNotificationList()
        {
            return new EmployeeEmailNotifications(this.AccountID, this.EmployeeID);
        }

        /// <summary>
        /// Gets or sets this employees number
        /// </summary>
        public string EmployeeNumber { get; set; }

        public DateTime? EsrEffectiveEndDate { get; set; }

        public DateTime? EsrEffectiveStartDate { get; set; }

        /// <summary>
        /// Gets or sets the employees unique Esr Person ID
        /// </summary>
        public long? EsrPersonID { get; set; }

        /// <summary>
        /// Gets or sets the fax number for this employee.
        /// </summary>
        public string FaxNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this employee has logged on yet or not.
        /// </summary>
        public bool FirstLogon { get; set; }

        /// <summary>
        /// Gets or sets the gender of this employee.
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// Gets or sets the grid sort orders for this employee.
        /// </summary>
        public EmployeeGridSorts GetGridSortOrders()
        {
            return new EmployeeGridSorts(this.AccountID, this.EmployeeID);
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not this employee has customised what expense items are selected when adding an expense.
        /// </summary>
        public bool HasCustomisedAddItems { get; set; }

        /// <summary>
        /// Gets or sets the date this employee was hired.
        /// </summary>
        public DateTime? HiredDate { get; set; }

        /// <summary>
        /// Gets or sets the home addresses for this employee.
        /// </summary>
        public EmployeeHomeAddresses GetHomeAddresses()
        {
            return new EmployeeHomeAddresses(this.AccountID, this.EmployeeID);
        }

        /// <summary>
        /// Gets or sets the home email address for this employee.
        /// </summary>
        public string HomeEmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the item roles this employee has.
        /// </summary>
        public EmployeeItemRoles GetItemRoles()
        {
            return this._itemRoles ?? (this._itemRoles = new EmployeeItemRoles(this.AccountID, this.EmployeeID));
        }

        public DateTime LastChange { get; set; }

        /// <summary>
        /// Gets or sets the line manager ID for this employee.
        /// </summary>
        public int LineManager { get; set; }

        /// <summary>
        /// Gets or sets the locale this employee uses.
        /// </summary>
        public int? LocaleID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this employee is locked out or not.
        /// </summary>
        public bool Locked { get; set; }

        public int LogonCount { get; set; }

        /// <summary>
        /// Gets or sets the number of logon attempts this employee record has left before locking.
        /// </summary>
        public int LogonRetryCount { get; set; }

        /// <summary>
        /// Gets or sets the maiden name of this individual.
        /// </summary>
        public string MaidenName { get; set; }

        /// <summary>
        /// Gets or sets the middle name of this individual.
        /// </summary>
        public string MiddleNames { get; set; }

        public int MileageTotal { get; set; }

        public DateTime? MileageTotalDate { get; set; }

        /// <summary>
        /// Gets or sets the mobile devices associated to this employee.
        /// </summary>
        [Obsolete("not used?", true)]
        public Dictionary<int, MobileDevice> MobileDevices { get; set; }

        /// <summary>
        /// Gets or sets the mobile telephone number for this employee.
        /// </summary>
        public string MobileTelephoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the employee ID who modified this employee record last.
        /// </summary>
        public int? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the date this employee record was modified last.
        /// </summary>
        public DateTime? ModifiedOn { get; set; }

        /// <summary>
        /// Gets or sets the National Insurance Number for this employee
        /// </summary>
        public string NationalInsuranceNumber { get; set; }

        /// <summary>
        /// Gets or sets the new grid sort orders for this employee.
        /// </summary>
        public EmployeeNewGridSorts GetNewGridSortOrders()
        {
            return new EmployeeNewGridSorts(this.AccountID, this.EmployeeID);
        }

        /// <summary>
        /// Gets or sets the Nhs Trust this employee works for.
        /// </summary>
        public int? NhsTrustID { get; set; }

        /// <summary>
        /// Gets or sets this employees unique Nhs ID
        /// </summary>
        public string NhsUniqueID { get; set; }

        /// <summary>
        /// Gets or sets the pager number for this employee.
        /// </summary>
        public string PagerNumber { get; set; }

        /// <summary>
        /// Gets or sets the password for this employee.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the encryption method this employees password uses.
        /// </summary>
        public PasswordEncryptionMethod PasswordMethod { get; set; }

        /// <summary>
        /// Gets or sets the payroll number for this employee.
        /// </summary>
        public string PayrollNumber { get; set; }

        /// <summary>
        /// Gets or sets the position title for this employee.
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// Gets or sets the preferred name for this employee.
        /// </summary>
        public string PreferredName { get; set; }

        /// <summary>
        /// Gets or sets the primary country for this employee.
        /// </summary>
        public int PrimaryCountry { get; set; }

        /// <summary>
        /// Gets or sets the primary currency for this employee.
        /// </summary>
        public int PrimaryCurrency { get; set; }

        /// <summary>
        /// Gets or sets the purchase card sign off group.
        /// </summary>
        public int PurchaseCardSignOffGroup { get; set; }

        /// <summary>
        /// Gets or sets the sign off group ID for this employee.
        /// </summary>
        public int SignOffGroupID { get; set; }

        /// <summary>
        /// Gets or sets the telephone extension number for this employee.
        /// </summary>
        public string TelephoneExtensionNumber { get; set; }

        /// <summary>
        /// Gets or sets the telephone number for this employee.
        /// </summary>
        public string TelephoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the termination date for this employee.
        /// </summary>
        public DateTime? TerminationDate { get; set; }

        /// <summary>
        /// Gets or sets the user defined fields for this employee.
        /// </summary>
        public SortedList<int, object> UserDefinedFields { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this employee is verified or not.
        /// </summary>
        public bool Verified { get; set; }

        /// <summary>
        /// Gets or sets authoriser level id
        /// </summary>
        public int? AuthoriserLevelDetailId { get; set; }

        /// <summary>
        /// Gets and sets curencyid
        /// </summary>
        public int Currency { get; set; }


        /// <summary>
        /// Gets or sets NotifyClaimUnsubmission flag
        /// </summary>
        public bool NotifyClaimUnsubmission { get; set; }

        /// <summary>
        /// Gets or sets DVLA consent date
        /// </summary>
        public DateTime? DvlaConsentDate { get; set; }

        /// <summary>
        /// Gets or sets DVLA Response Code
        /// </summary>
        public string DvlaResponseCode { get; set; }

        /// <summary>
        /// Gets or sets whether the user has agreed to provide the consent
        /// </summary>
        public bool? AgreeToProvideConsent { get; set; }

        /// <summary>
        /// Driverid from DVLA consent portal using driving licence
        /// </summary>
        public int? DriverId { get; set; }

        /// <summary>
        /// Gets or sets Dvla lookup date
        /// </summary>
        public DateTime? DvlaLookUpDate { get; set; }
        
        /// <summary>
        /// Security code from DVLA consent portal
        /// </summary>
        public Guid? SecurityCode { get; set; }

        /// <summary>
        /// Excess Mileage that can claimed when moving from one work address to another
        /// </summary>
        public double ExcessMileage { get; set; }

        public EmployeeViews GetViews()
        {
            return new EmployeeViews(this.AccountID, this.EmployeeID);
        }

        public EmployeeSubCategories GetSubCategories()
        {
            return new EmployeeSubCategories(this.AccountID, this.EmployeeID);
        }

        /// <summary>
        /// Gets or sets the work addresses for this employee.
        /// </summary>
        public EmployeeWorkAddresses GetWorkAddresses()
        {
            return new EmployeeWorkAddresses(this.AccountID, this.EmployeeID);
        }

        #endregion properties

        /// <summary>
        /// The change status.
        /// </summary>
        /// <param name="archive">Whether to archive or not.</param>
        /// <param name="currentUser">The current user executing this method.</param>
        /// /// <param name="employeeId">Id of an employee.</param>
        public int ChangeArchiveStatus(bool archive, ICurrentUserBase currentUser)
        {
            int result = 0;
            if (this.EmployeeID > 0)
            {
                using (var databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(currentUser == null ? this.AccountID : currentUser.AccountID)))
                {
                    databaseConnection.sqlexecute.Parameters.Clear();
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeId", this.EmployeeID);
                    // Following stored procedure checks if any Authoriser Level assigned to current approver. Returns NULL for no AL, -1 for DAL, or actual amount if assigned.
                    decimal? authoriserLevelAmount = databaseConnection.ExecuteScalar<decimal?>("dbo.GetAuthoriserLevelAmountByEmployee", CommandType.StoredProcedure);
                    databaseConnection.sqlexecute.Parameters.Clear();
                    if (authoriserLevelAmount != null && authoriserLevelAmount == -1)
                    {
                        // It is assigned Default Authoriser Level
                        return -1;
                    }

                    this.Archived = archive;
                    if (!archive)
                    {
                        this.LogonRetryCount = 0;
                    }
                }
            }

            result = this.Save(currentUser);
            return result;
        }

        /// <summary>
        /// Changed "Locked" status on account
        /// </summary>
        /// <param name="locked">value to set lock status.</param> 
        /// <param name="currentUser">The current user executing this method.</param>
        public void ChangeLockedStatus(bool locked, ICurrentUserBase currentUser)
        {
            this.Locked = locked;

            if (!locked)
            {
                this.LogonRetryCount = 0;
            }

            this.Save(currentUser);
        }

        /// <summary>
        /// Changes an employee password.
        /// </summary>
        /// <param name="currentPassword">
        /// The current password.
        /// </param>
        /// <param name="newPassword">
        /// The new password.
        /// </param>
        /// <param name="checkOldPassword">
        /// Whether to check the old password parameter matches the employee's current password.
        /// </param>
        /// <param name="checkNewPassword">
        /// Whether to check the new password parameter matches the employee's current password.
        /// </param>
        /// <param name="passwordHistoryNumber">
        /// The password history number.
        /// </param>
        /// <param name="currentUser">
        /// The current user.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// 0 on success, 1 if the old password doesn't match, 2 if it doesn't conform to the account password policy.
        /// </returns>
        public byte ChangePassword(string currentPassword, string newPassword, bool checkOldPassword, byte checkNewPassword, int passwordHistoryNumber, ICurrentUserBase currentUser, IDBConnection connection = null)
        {
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            cSecureData secureData = new cSecureData();

            string oldPasswordHashed = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(currentPassword, System.Web.Configuration.FormsAuthPasswordFormat.MD5.ToString());
            string oldPasswordEncrypted = secureData.Encrypt(currentPassword);
            string newPasswordEncryped = secureData.Encrypt(newPassword);

            if (checkOldPassword)
            {
                if (this.Password != oldPasswordEncrypted && this.Password != oldPasswordHashed && this.Password != currentPassword)
                {
                    return 1;
                }
            }

            if (checkNewPassword != 0)
            {
                return 2;
            }

            this.Password = newPasswordEncryped;
            this.PasswordMethod = PasswordEncryptionMethod.RijndaelManaged;
            this.LastChange = DateTime.UtcNow;
            this.LogonRetryCount = 0;
            this.Save(currentUser);
            this.AddPreviousPassword(passwordHistoryNumber);

            if (this.Locked)
            {
                this.ChangeLockedStatus(false, currentUser);
            }

            return 0;
        }

        public bool CheckPasswordExpiry(cAccountProperties accountProperties)
        {
            if (!accountProperties.PwdExpires)
            {
                return false;
            }

            DateTime lastdate = this.LastChange;

            lastdate = lastdate.AddDays(accountProperties.PwdExpiryDays);

            return lastdate < DateTime.Today;
        }

        public byte CheckPasswordMeetsRequirements(cAccountProperties accountProperties, string password, IDBConnection connection = null)
        {
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();

                /*
                return codes
                0 = password fine
                1 = password is incorrect length
                2 = capital letter
                3 = number
                4 = previous
                5 = symbol
                */

                char[] nums = "0123456789".ToCharArray();
                char[] symbols = "!¦£$%^&*€@#~?|".ToCharArray();

                bool isPrevious = false;
                PasswordLength plength = accountProperties.PwdConstraint;
                if (plength != PasswordLength.AnyLength)
                {
                    int length1 = accountProperties.PwdLength1;
                    int length2 = accountProperties.PwdLength2;
                    switch (plength)
                    {
                        case PasswordLength.EqualTo:
                            if (password.Length != length1)
                            {
                                return 1;
                            }

                            break;
                        case PasswordLength.GreaterThan:
                            if (password.Length <= length1)
                            {
                                return 1;
                            }

                            break;
                        case PasswordLength.LessThan:
                            if (password.Length >= length1)
                            {
                                return 1;
                            }

                            break;
                        case PasswordLength.Between:
                            if (password.Length < length1 || password.Length > length2)
                            {
                                return 1;
                            }

                            break;
                    }
                }

                if (accountProperties.PwdMustContainUpperCase)
                {
                    if (password.ToLower() == password)
                    {
                        return 2;
                    }
                }

                if (accountProperties.PwdMustContainNumbers)
                {
                    if (password.IndexOfAny(nums, 0, password.Length) == -1)
                    {
                        return 3;
                    }
                }

                if (accountProperties.PwdMustContainSymbol)
                {
                    if (password.IndexOfAny(symbols, 0, password.Length) == -1)
                    {
                        return 5;
                    }
                }

                if (accountProperties.PwdHistoryNum != 0)
                {
                    if (this.EmployeeID != 0)
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeid", this.EmployeeID);
                        const string SQL = "select password from previouspasswords where employeeid = @employeeid";
                        string oldPassword;

                        using (IDataReader prevreader = databaseConnection.GetReader(SQL))
                        {
                            switch (this.PasswordMethod)
                            {
                                case PasswordEncryptionMethod.FWBasic:
                                    oldPassword = cPassword.Crypt(password, "2");
                                    break;
                                case PasswordEncryptionMethod.Hash:
                                case PasswordEncryptionMethod.MD5:
                                    oldPassword = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(password, System.Web.Configuration.FormsAuthPasswordFormat.MD5.ToString());
                                    break;
                                case PasswordEncryptionMethod.RijndaelManaged:
                                    cSecureData clsSecureData = new cSecureData();
                                    oldPassword = clsSecureData.Encrypt(password);
                                    break;
                                case PasswordEncryptionMethod.ShaHash:
                                    oldPassword = cPassword.SHA_HashPassword(password);
                                    break;
                                default:
                                    oldPassword = string.Empty;
                                    break;
                            }

                            while (prevreader.Read())
                            {
                                if (prevreader.GetString(0) == oldPassword)
                                {
                                    isPrevious = true;
                                    break;
                                }
                            }
                            prevreader.Close();
                        }

                        // Check against current password
                        if (this.Password == oldPassword)
                        {
                            isPrevious = true;
                        }

                        if (isPrevious)
                        {
                            return 4;
                        }
                    }
                }

                databaseConnection.sqlexecute.Parameters.Clear();
            }

            return 0;
        }

        /// <summary>
        /// Increments the current claim number.
        /// </summary>
        /// <param name="currentUser">The user calling this method.</param>
        /// <returns></returns>
        public int IncrementClaimNumber(ICurrentUserBase currentUser)
        {
            this.CurrentClaimNumber++;
            this.Save(currentUser);
            return this.CurrentClaimNumber;
        }

        /// <summary>
        /// Increments the current reference number.
        /// </summary>
        /// <param name="currentUser">The current user executing this method.</param>
        public void IncrementAndSaveCurrentExpenseItemReferenceNumber(ICurrentUserBase currentUser)
        {
            if (this.EmployeeID <= 0)
            {
                return;
            }

            DBConnection database = new DBConnection(cAccounts.getConnectionString(currentUser.AccountID));
            database.sqlexecute.Parameters.AddWithValue("@employeeId", this.EmployeeID);
            database.ExecuteProc("IncrementEmployeesCurrentReferenceNumberForExpenseItems");
            database.sqlexecute.Parameters.Clear();

            User.CacheRemove(this.EmployeeID, this.AccountID);
        }

        public void MarkFirstLogonComplete(ICurrentUserBase currentUser)
        {
            this.FirstLogon = false;
            this.Save(currentUser);
        }


        /// <summary>
        /// Checks to see if the password has expired.
        /// </summary>
        /// <param name="passwordExpires">True if the password is set to expire.</param>
        /// <param name="daysValidFor">How many days a password can be valid for.</param>
        /// <returns>A <see cref="bool"/> indicating whether or not this employees password has expired.</returns>
        public bool PasswordExpired(bool passwordExpires, int daysValidFor)
        {
            if (passwordExpires == false)
            {
                return false;
            }

            return this.LastChange.AddDays(daysValidFor) < DateTime.Today;
        }

        /// <summary>
        /// Populate the employee details 
        /// </summary>
        /// <param name="employeeID">
        /// The employee ID.
        /// </param>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="emailAddress">
        /// The email Address.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="forename">
        /// The forename.
        /// </param>
        /// <param name="middleName">
        /// The middle Name.
        /// </param>
        /// <param name="maidenName">
        /// The maiden Name.
        /// </param>
        /// <param name="surname">
        /// The surname.
        /// </param>
        /// <param name="active">
        /// The active.
        /// </param>
        /// <param name="verified">
        /// The verified.
        /// </param>
        /// <param name="archived">
        /// The archived.
        /// </param>
        /// <param name="locked">
        /// The locked.
        /// </param>
        /// <param name="logonCount">
        /// The logon Count.
        /// </param>
        /// <param name="logonRetryCount">
        /// The logon Retry Count.
        /// </param>
        /// <param name="createdOn">
        /// The created On.
        /// </param>
        /// <param name="createdBy">
        /// The created By.
        /// </param>
        /// <param name="modifiedOn">
        /// The modified On.
        /// </param>
        /// <param name="modifiedBy">
        /// The modified By.
        /// </param>
        /// <param name="bankAccount">
        /// The bank Account.
        /// </param>
        /// <param name="signOffGroupID">
        /// The sign Off Group ID.
        /// </param>
        /// <param name="telephoneExtensionNumber">
        /// The telephone Extension Number.
        /// </param>
        /// <param name="mobileTelephoneNumber">
        /// The mobile Telephone Number.
        /// </param>
        /// <param name="pagerNumber">
        /// The pager Number.
        /// </param>
        /// <param name="faxNumber">
        /// The fax Number.
        /// </param>
        /// <param name="homeEmailAddress">
        /// The home Email Address.
        /// </param>
        /// <param name="lineManager">
        /// The line Manager.
        /// </param>
        /// <param name="advancesSignOffGroup">
        /// The advances Sign Off Group.
        /// </param>
        /// <param name="preferredName">
        /// The preferred Name.
        /// </param>
        /// <param name="gender">
        /// The gender.
        /// </param>
        /// <param name="dateOfBirth">
        /// The date Of Birth.
        /// </param>
        /// <param name="hiredDate">
        /// The hired Date.
        /// </param>
        /// <param name="terminationDate">
        /// The termination Date.
        /// </param>
        /// <param name="payrollNumber">
        /// The payroll Number.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="telephoneNumber">
        /// The telephone Number.
        /// </param>
        /// <param name="creditor">
        /// The creditor.
        /// </param>
        /// <param name="creationMethod">
        /// The creation Method.
        /// </param>
        /// <param name="passwordMethod">
        /// The password Method.
        /// </param>
        /// <param name="firstLogon">
        /// The first Logon.
        /// </param>
        /// <param name="adminOverride">
        /// The admin Override.
        /// </param>
        /// <param name="defaultSubAccount">
        /// The default Sub Account.
        /// </param>
        /// <param name="primaryCurrency">
        /// The primary Currency.
        /// </param>
        /// <param name="primaryCountry">
        /// The primary Country.
        /// </param>
        /// <param name="creditCardSignOffGroup">
        /// The credit Card Sign Off Group.
        /// </param>
        /// <param name="purchaseCardSignOffGroup">
        /// The purchase Card Sign Off Group.
        /// </param>
        /// <param name="hasCustomisedAddItems">
        /// The has Customised Add Items.
        /// </param>
        /// <param name="localeID">
        /// The locale ID.
        /// </param>
        /// <param name="nhsTrustID">
        /// The nhs Trust ID.
        /// </param>
        /// <param name="nationalInsuranceNumber">
        /// The national Insurance Number.
        /// </param>
        /// <param name="employeeNumber">
        /// The employee Number.
        /// </param>
        /// <param name="nhsUniqueID">
        /// The nhs Unique ID.
        /// </param>
        /// <param name="esrPersonID">
        /// The esr Person ID.
        /// </param>
        /// <param name="esrEffectiveStartDate">
        /// The esr Effective Start Date.
        /// </param>
        /// <param name="esrEffectiveEndDate">
        /// The esr Effective End Date.
        /// </param>
        /// <param name="currentClaimNumber">
        /// The current Claim Number.
        /// </param>
        /// <param name="lastChange">
        /// The last Change.
        /// </param>
        /// <param name="currentReferenceNumber">
        /// The current Reference Number.
        /// </param>
        /// <param name="mileageTotal">
        /// The mileage Total.
        /// </param>
        /// <param name="mileageTotalDate">
        /// The mileage Total Date.
        /// </param>
        /// <param name="contactHelpDeskAllowed">
        /// The contact Help Desk Allowed.
        /// </param>
        /// <param name="currency">
        /// The currency.
        /// </param>
        /// <param name="authoriserLevelDetailId">
        /// The authoriser Level Detail Id.
        /// </param>
        /// <param name="notifyClaimUnsubmission">
        /// The notify Claim Unsubmission.
        /// </param>
        /// <param name="dvlaConsentDate">
        /// The dvla Consent Date.
        /// </param>
        /// <param name="driverId">
        /// The driver Id.
        /// </param>
        /// <param name="securityCode">
        /// The security Code.
        /// </param>
        /// <param name="drivingLicenceFirstName">
        /// The driving Licence First Name.
        /// </param>
        /// <param name="drivingLicenceSurname">
        /// The driving Licence Surname.
        /// </param>
        /// <param name="drivingLicenceSex">
        /// The driving Licence Sex.
        /// </param>
        /// <param name="drivingLicenceDateofBirth">
        /// The driving Licence Dateof Birth.
        /// </param>
        /// <param name="drivingLicenceEmailAddress">
        /// The driving Licence Email Address.
        /// </param>
        /// <param name="drivingLicenceNumber">
        /// The driving Licence Number.
        /// </param>
        /// <param name="drivingLicenceMiddleName">
        /// The driving Licence Middle Name.
        /// </param>
        /// <param name="dvlaLookUpDate">
        /// Date on which the  driving licence details are updated through dvla lookup for the employee
        /// </param>
        /// <param name="dvlaResponseCode">Response code returned by the Dvla licence check api when creating the driver or when doing a licence lookup for the employee</param>
        public void PopulateEmployee(int employeeID, string username, string password, string emailAddress, string title, string forename, string middleName, string maidenName, string surname, bool active, bool verified, bool archived, bool locked, int logonCount, int logonRetryCount, DateTime createdOn, int createdBy, DateTime? modifiedOn, int? modifiedBy, BankAccount bankAccount, int signOffGroupID, string telephoneExtensionNumber, string mobileTelephoneNumber, string pagerNumber, string faxNumber, string homeEmailAddress, int lineManager, int advancesSignOffGroup, string preferredName, string gender, DateTime? dateOfBirth, DateTime? hiredDate, DateTime? terminationDate, string payrollNumber, string position, string telephoneNumber, string creditor, CreationMethod creationMethod, PasswordEncryptionMethod passwordMethod, bool firstLogon, bool adminOverride, int defaultSubAccount, int primaryCurrency, int primaryCountry, int creditCardSignOffGroup, int purchaseCardSignOffGroup, bool hasCustomisedAddItems, int? localeID, int? nhsTrustID, string nationalInsuranceNumber, string employeeNumber, string nhsUniqueID, long? esrPersonID, DateTime? esrEffectiveStartDate, DateTime? esrEffectiveEndDate, int currentClaimNumber, DateTime lastChange, int currentReferenceNumber, int mileageTotal, DateTime? mileageTotalDate, bool contactHelpDeskAllowed, int currency, int? authoriserLevelDetailId,bool notifyClaimUnsubmission,DateTime? dvlaConsentDate, int? driverId,Guid? securityCode, string drivingLicenceFirstName, string drivingLicenceSurname, string drivingLicenceSex, string drivingLicenceDateofBirth, string drivingLicenceEmailAddress, string drivingLicenceNumber, string drivingLicenceMiddleName,DateTime? dvlaLookUpDate,string dvlaResponseCode, bool? agreeToProvideConsent, double excessMileage)
        {
            this.EmployeeID = employeeID;
            this.Username = username;
            this.Password = password;
            this.EmailAddress = emailAddress;
            this.Title = title;
            this.Forename = forename;
            this.MiddleNames = middleName;
            this.Surname = surname;
            this.Active = active;
            this.Verified = verified;
            this.Archived = archived;
            this.Locked = locked;
            this.LogonCount = logonCount;
            this.LogonRetryCount = logonRetryCount;
            this.CreatedOn = createdOn;
            this.CreatedBy = createdBy;
            this.ModifiedOn = modifiedOn;
            this.ModifiedBy = modifiedBy;
            this.BankAccountDetails = bankAccount;
            this.SignOffGroupID = signOffGroupID;
            this.TelephoneExtensionNumber = telephoneExtensionNumber;
            this.MobileTelephoneNumber = mobileTelephoneNumber;
            this.PagerNumber = pagerNumber;
            this.FaxNumber = faxNumber;
            this.HomeEmailAddress = homeEmailAddress;
            this.LineManager = lineManager;
            this.AdvancesSignOffGroup = advancesSignOffGroup;
            this.PreferredName = preferredName;
            this.Gender = gender;
            this.DateOfBirth = dateOfBirth;
            this.HiredDate = hiredDate;
            this.TerminationDate = terminationDate;
            this.PayrollNumber = payrollNumber;
            this.Position = position;
            this.TelephoneNumber = telephoneNumber;
            this.Creditor = creditor;
            this.PasswordMethod = passwordMethod;
            this.CreationMethod = creationMethod;
            this.FirstLogon = firstLogon;
            this.AdminOverride = adminOverride;
            this.DefaultSubAccount = defaultSubAccount;
            this.PrimaryCurrency = primaryCurrency;
            this.PrimaryCountry = primaryCountry;
            this.CreditCardSignOffGroup = creditCardSignOffGroup;
            this.PurchaseCardSignOffGroup = purchaseCardSignOffGroup;
            this.HasCustomisedAddItems = hasCustomisedAddItems;
            this.LocaleID = localeID;
            this.NhsTrustID = nhsTrustID;
            this.NationalInsuranceNumber = nationalInsuranceNumber;
            this.EmployeeNumber = employeeNumber;
            this.NhsUniqueID = nhsUniqueID;
            this.EsrPersonID = esrPersonID;
            this.EsrEffectiveStartDate = esrEffectiveStartDate;
            this.EsrEffectiveEndDate = esrEffectiveEndDate;
            this.CurrentClaimNumber = currentClaimNumber;
            this.LastChange = lastChange;
            this.CurrentReferenceNumber = currentReferenceNumber;
            this.MileageTotal = mileageTotal;
            this.MileageTotalDate = mileageTotalDate;
            this.MaidenName = maidenName;
            this.ContactHelpDeskAllowed = contactHelpDeskAllowed;
            this.Currency = currency;
            this.AuthoriserLevelDetailId = authoriserLevelDetailId;
            this.NotifyClaimUnsubmission = notifyClaimUnsubmission;
            this.DvlaConsentDate = dvlaConsentDate;
            this.DriverId = driverId;
            this.SecurityCode = securityCode;
            this.DrivingLicenceFirstname = drivingLicenceFirstName;
            this.DrivingLicenceSurname = drivingLicenceSurname;
            this.DrivingLicenceSex = drivingLicenceSex;
            this.DrivingLicenceDateOfBirth = drivingLicenceDateofBirth;
            this.DrivingLicenceEmail = drivingLicenceEmailAddress;
            this.DrivingLicenceNumber = drivingLicenceNumber;
            this.DrivingLicenceMiddleName = drivingLicenceMiddleName;
            this.DvlaLookUpDate = dvlaLookUpDate;
            this.DvlaResponseCode = dvlaResponseCode;
            this.AgreeToProvideConsent = agreeToProvideConsent;
            this.ExcessMileage = excessMileage;
        }

        /// <summary>
        /// Check to see if the broadcast has already been read.
        ///  - GetReadBroadcasts in cEmployees MUST have been called for the user before using this check
        /// </summary>
        /// <param name="broadcastID">Broadcast message ID to check</param>
        /// <returns>True if the employee has read the broadcast already or false otherwise</returns>
        public bool ReadBroadcast(int broadcastID)
        {
            return this.GetBroadcastMessagesRead().Contains(broadcastID);
        }

        /// <summary>
        /// Save any changes made to the Employee.
        /// </summary>
        /// <param name="currentUser">The current user executing this method.</param>
        /// <returns>An <see cref="int"/> indicating the outcome of the save.</returns>
        public int Save(ICurrentUserBase currentUser, IDBConnection connection = null)
        {
            int returnValue = 0;
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeid", this.EmployeeID);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@title", this.Title);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@firstname", this.Forename);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@surname", this.Surname);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@telno", this.TelephoneNumber);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@email", this.EmailAddress);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@creditor", this.Creditor);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@payroll", this.PayrollNumber);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@position", this.Position);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@verified", this.Verified);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@locked", this.Locked);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@logonRetryCount", this.LogonRetryCount);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@password", this.Password);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@passwordMethod", (int)this.PasswordMethod);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@archived", this.Archived);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@currclaimno", this.CurrentClaimNumber);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@lastchange", this.LastChange);

                if (this.SignOffGroupID == 0)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@groupid", DBNull.Value);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@groupid", this.SignOffGroupID);
                }

                if (this.LineManager == 0)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@linemanager", DBNull.Value);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@linemanager", this.LineManager);
                }

                databaseConnection.sqlexecute.Parameters.AddWithValue("@mileagetotal", this.MileageTotal);

                if (this.MileageTotalDate.HasValue)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@mileagetotaldate", this.MileageTotalDate.Value);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@mileagetotaldate", DBNull.Value);
                }

                databaseConnection.sqlexecute.Parameters.AddWithValue("@homeemail", this.HomeEmailAddress);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@faxno", this.FaxNumber);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@pagerno", this.PagerNumber);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@mobileno", this.MobileTelephoneNumber);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@extension", this.TelephoneExtensionNumber);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@advancegroup", this.AdvancesSignOffGroup);

                if (this.PrimaryCurrency == 0)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@primarycurrency", DBNull.Value);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@primarycurrency", this.PrimaryCurrency);
                }

                if (this.PrimaryCountry == 0)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@primarycountry", DBNull.Value);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@primarycountry", this.PrimaryCountry);
                }
                if (this.CreditCardSignOffGroup == 0)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@groupidcc", DBNull.Value);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@groupidcc", this.CreditCardSignOffGroup);
                }

                if (this.PurchaseCardSignOffGroup == 0)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@groupidpc", DBNull.Value);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@groupidpc", this.PurchaseCardSignOffGroup);
                }

                if (this.NationalInsuranceNumber == string.Empty)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@ninumber", DBNull.Value);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@ninumber", this.NationalInsuranceNumber);
                }

                if (this.MiddleNames == string.Empty)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@middlenames", DBNull.Value);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@middlenames", this.MiddleNames);
                }

                if (this.MaidenName == string.Empty)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@maidenname", DBNull.Value);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@maidenname", this.MaidenName);
                }

                if (this.Gender == string.Empty)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@gender", DBNull.Value);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@gender", this.Gender);
                }

                if (this.DateOfBirth == null)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@dateofbirth", DBNull.Value);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@dateofbirth", this.DateOfBirth);
                }

                if (this.HiredDate == null)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@hiredate", DBNull.Value);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@hiredate", this.HiredDate);
                }

                if (this.TerminationDate == null)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@terminationdate", DBNull.Value);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@terminationdate", this.TerminationDate);
                }

                databaseConnection.sqlexecute.Parameters.AddWithValue("@active", this.Active);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@username", this.Username);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@name", this.BankAccountDetails.AccountHolderName);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@accountnumber", this.BankAccountDetails.AccountNumber);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@accounttype", this.BankAccountDetails.AccountType);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@sortcode", this.BankAccountDetails.SortCode);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@reference", this.BankAccountDetails.AccountReference);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@currencyid", this.Currency);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@countryid", this.BankAccountDetails.CountryId);

                if (this.AuthoriserLevelDetailId == null)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@authoriserLevelDetail", DBNull.Value);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@authoriserLevelDetail", this.AuthoriserLevelDetailId);
                }

                if (this.EmployeeID > 0)
                {
                    this.ModifiedOn = this.ModifiedOn ?? DateTime.Now;

                    int? modifiedBy = null;
                    if (currentUser != null)
                    {
                        modifiedBy = currentUser.EmployeeID;
                    }

                    this.ModifiedBy = this.ModifiedBy ?? modifiedBy;
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@date", this.ModifiedOn);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@userid", this.ModifiedBy);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@date", this.CreatedOn);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@userid", this.CreatedBy);
                }

                if (this.LocaleID == null || this.LocaleID == 0)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@localeID", DBNull.Value);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@localeID", this.LocaleID);
                }

                if (this.NhsTrustID.HasValue == false || this.NhsTrustID == 0)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@NHSTrustID", DBNull.Value);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@NHSTrustID", this.NhsTrustID.Value);
                }

                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeNumber", this.EmployeeNumber);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@nhsuniqueId", this.NhsUniqueID);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@preferredName", this.PreferredName);

                if (this.DefaultSubAccount >= 0)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@defaultSubAccountId", this.DefaultSubAccount);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@defaultSubAccountId", DBNull.Value);
                }

                databaseConnection.sqlexecute.Parameters.AddWithValue("@CreationMethod", this.CreationMethod);

                if (currentUser != null)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                    if (currentUser.isDelegate)
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                    }
                    else
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                    }
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@CUemployeeID", -1);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }

                if (!this.DvlaConsentDate.HasValue || string.IsNullOrEmpty(this.DvlaConsentDate.ToString()))
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@DvlaConsentDate", DBNull.Value);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@DvlaConsentDate", this.DvlaConsentDate);
                }

                if (!this.DriverId.HasValue || string.IsNullOrEmpty(this.DriverId.ToString()))
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@DriverId", DBNull.Value);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@DriverId", this.DriverId);
                }

                databaseConnection.sqlexecute.Parameters.AddWithValue("@notifyClaimUnsubmission", this.NotifyClaimUnsubmission);

                if (this.ExcessMileage <= 0)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@ExcessMileage", DBNull.Value);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@ExcessMileage", Math.Round(this.ExcessMileage, 2, MidpointRounding.AwayFromZero));
                }

                databaseConnection.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
                databaseConnection.ExecuteProc("saveEmployee");

                try
                {
                    returnValue = (int)databaseConnection.sqlexecute.Parameters["@identity"].Value;
                    databaseConnection.sqlexecute.Parameters.Clear();
                }
                catch (Exception ex)
                {
                    return -3;
                }
            }


            if (returnValue <= 0)
            {
                return returnValue;
            }

            this.EmployeeID = returnValue;

            User.CacheRemove(this.EmployeeID, this.AccountID);
            return this.EmployeeID;
        }

        /// <summary>
        /// Adds the existing password to the previous passwords table.
        /// </summary>
        /// <param name="passwordHistoryNumber">The number of passwords to keep in history for this employee.</param>
        private void AddPreviousPassword(int passwordHistoryNumber, IDBConnection connection = null)
        {
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();

                int previous = 0;

                if (passwordHistoryNumber != 0)
                {
                    previous = passwordHistoryNumber;
                }

                if (previous == 0)
                {
                    return;
                }

                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeid", this.EmployeeID);

                string strsql = "select isnull(max([order]),0) from previouspasswords where employeeid = @employeeid";
                int order = databaseConnection.ExecuteScalar<int>(strsql);

                order++;

                // Insert the old password into the previous passwords table.
                strsql = "insert into previouspasswords (employeeid, password, [order], passwordMethod) values (@employeeid, @password, @order, @passwordMethod)";
                databaseConnection.AddWithValue("@password", this.Password, 250);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@order", order);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@passwordMethod", (int)this.PasswordMethod);
                databaseConnection.ExecuteSQL(strsql);

                // Check if we are storing too many passwords, if so delete the oldest one.
                if (order > previous)
                {
                    strsql = "delete from previouspasswords where employeeid = @employeeid and [order] = 1";
                    databaseConnection.ExecuteSQL(strsql);

                    // Re-order the password history.
                    strsql = "update previouspasswords set [order] = [order] - 1 where employeeid = @employeeid";
                    databaseConnection.ExecuteSQL(strsql);
                }

                databaseConnection.sqlexecute.Parameters.Clear();
            }
        }

        /// <summary>
        /// This function will return the line manager heirarchy for the employee.
        /// If this employee is not a line manager, or has no employees underneath having this employee as line manager, then an empty list is returned.
        /// </summary>
        /// <param name="claimsHierarchy">Specifies whether to use access permissions for claims (groups / teams / matrix etc.)</param>
        /// <param name="connection">
        /// The connectionm object.
        /// </param>
        /// <returns>
        /// All employees in the heirarchy represented as a list of integers.
        /// </returns>
        public List<int> Hierarchy(bool claimsHierarchy = false, IDBConnection connection = null)
        {
            const string LINE_MANAGER_MATCH_FIELD = "96F11C6D-7615-4ABD-94EC-0E4D34E187A0";
            var heirarchyList = new List<int>();
            string procedureName = claimsHierarchy ? "GetHeirachy" : "GetClaimHierarchy";
            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();
                databaseConnection.sqlexecute.Parameters.AddWithValue("employeeid", this.EmployeeID);
                databaseConnection.sqlexecute.Parameters.AddWithValue("linemanagerfield", LINE_MANAGER_MATCH_FIELD);
                string sql = string.Format("exec {0} @employeeid, @linemanagerfield", procedureName);
                using (IDataReader reader = databaseConnection.GetReader(sql))
                {
                    while (reader.Read())
                    {
                        heirarchyList.Add(reader.GetInt32(reader.GetOrdinal("employeeid")));
                    }

                    reader.Close();
                }
            }

            return heirarchyList;
        }

        /// <summary>
        /// Returns a list of employee ids that have the given access roles.
        /// </summary>
        /// <param name="accountId">The account id.</param>
        /// <param name="accessRoleList">Array of access roles.</param>
        /// <param name="connection">Override database connection</param>
        /// <returns>An array of employeeIds</returns>
        public static List<int> GetEmployeesWithAccessRoles(int accountId, List<int> accessRoleList, IDBConnection connection = null)
        {
            var returnList = new List<int>();
            if (accessRoleList != null)
            {
                using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountId)))
                {
                    databaseConnection.AddWithValue("@accessRoleIds", accessRoleList);

                    databaseConnection.sqlexecute.Parameters.Add("@returnValue", SqlDbType.Int);
                    databaseConnection.sqlexecute.Parameters["@returnValue"].Direction = ParameterDirection.ReturnValue;

                    using (var reader = databaseConnection.GetReader("GetEmployeesWithMyAccessRoles", CommandType.StoredProcedure))
                    {
                        while (reader.Read())
                        {
                            returnList.Add(reader.GetInt32(0));
                        }

                        databaseConnection.sqlexecute.Parameters.Clear();
                        reader.Close();
                    }
                }
            }

            return returnList;
        }

        /// <summary>
        /// Obtains a list of email addresses for required employees to reduce unnecessary employee caching
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="employeeIds">List of employeeIds to obtain email addresses for</param>
        /// <param name="connection">An optional database connection parameter</param>
        /// <returns>A list of email addresses</returns>
        public static Dictionary<int, string> GetEmailAddresses(int accountId, List<int> employeeIds, IDBConnection connection = null)
        {
            Dictionary<int, string> addresses;
            using (connection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                connection.sqlexecute.Parameters.Clear();
                addresses = new Dictionary<int, string>();
                const string Sql = "SELECT [employeeId], [email] FROM [employees] WHERE [employeeId] IN (SELECT c1 FROM @employeeIds);";

                connection.AddWithValue("@employeeIds", employeeIds);
                using (var reader = connection.GetReader(Sql))
                {
                    while (reader.Read())
                    {
                        if (reader.IsDBNull(1))
                        {
                            continue;
                        }

                        var emailAddress = reader.GetString(1);
                        if (string.IsNullOrWhiteSpace(emailAddress))
                        {
                            continue;
                        }

                        addresses.Add(reader.GetInt32(0), reader.GetString(1));
                    }
                }
            }

            return addresses;
        }

        /// <summary>
        /// Gets the gender value of current Employee.
        /// </summary>
        /// <returns>
        /// The Gender value as an integer <see cref="int"/>.
        /// </returns>
        public int GetGenderValue()
        {
            
            if (string.IsNullOrEmpty(this.Gender))
            {
                return 0;
            }

            return this.Gender == "Male" ? 1 : 2;
        }

    }
}