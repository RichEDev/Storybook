using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using SpendManagementApi.Interfaces;
using SpendManagementApi.Utilities;
using SpendManagementLibrary;
using SpendManagementLibrary.Helpers;
using SpendManagementLibrary.Interfaces;
using Spend_Management;
using Utilities.DistributedCaching;

namespace SpendManagementApi.Models.Types
{
    /// <summary>
    /// Represents a set of details / a table for validating a user's access to the API.
    /// Rather than the user sending usernames and password, they get an AuthToken, and they must use this.
    /// </summary>
    [Serializable]
    internal class ApiDetails
    {
        #region Constants

        public static string EncryptionValueSeparator = "|";
       
        private const string CacheAreaKey = "apiDetails";
        private const string AppSettingExpiryKey = "AuthTokenExpiryMinutes";
        private const string AppSettingExpiryDefault = "120";

        private const string SqlSpGet = "SMApiGetApiDetails";
        private const string SqlSpGetByEmployee = "SMApiGetApiDetailsByEmployeeId";
        private const string SqlSpAdd = "SMApiAddApiDetails";
        private const string SqlSpUpdate = "SMApiUpdateApiDetails";
        private const string SqlSpUpdateExpiry = "SMApiUpdateApiDetailsExpiry";
        private const string SqlSpDelete = "SMApiDeleteApiDetails";
        
        private const string SqlParamKeyReturn = "@return";
        private const string SqlParamKeyApiDetailsId = "@apiDetailsId";
        private const string SqlParamKeyEmployeeId = "@employeeId";
        private const string SqlParamKeyGenerationTime = "@generationTime";
        private const string SqlParamKeyExpiryTime = "@expiryTime";
        private const string SqlParamKeyCertificateInfo = "@certificateInfo";

        private const string SqlTableKeyApiDetailsId = "ApiDetailsId";
        private const string SqlTableKeyEmployeeId = "EmployeeId";
        private const string SqlTableKeyGenerationTime = "GenerationTime";
        private const string SqlTableKeyExpiryTime = "ExpiryTime";
        private const string SqlTableKeyCertificateInfo = "CertificateInfo";

        private const string ExceptionMessageAddingToDb = "There was an error adding the ApiDetails to the Database";
        private const string ExceptionMessageUpdateDb = "There was an error updating the ApiDetails in the Database";
        private const string ExceptionMessageAccountEmployee = "Invalid AccountId or EmployeeId";
        //private const string ExceptionMessageAuthTokenBad = "The authToken doesn't contain the correct information.";

        #endregion Constants

        #region Constructors and Destructors

        /// <summary>
        /// Basic ApiDetails constructor. Use the static methods to create and affect instances.
        /// </summary>
        private ApiDetails() { }

        #endregion Constructors and Destructors

        #region Enums and related objects

        /// <summary>
        /// The possible types of result when validating a user.
        /// </summary>
        public enum ApiDetailsValidity
        {
            /// <summary>
            /// The User is Valid
            /// </summary>
            Valid = 1,

            /// <summary>
            /// Invalid because the token has expired.
            /// </summary>
            InvalidAuthTokenExpired = 2,
            
            /// <summary>
            /// Invalid because the user was not found.
            /// </summary>
            InvalidNoUser = 3,

            /// <summary>
            /// Invalid because the account was not found.
            /// </summary>
            InvalidNoAccount = 4,

            /// <summary>
            /// Invalid because the certificate info is bad.
            /// </summary>
            InvalidBadCertificateInfo = 5,

            /// <summary>
            /// Invalid because the Auth Token was bad.
            /// </summary>
            InvalidAuthTokenBad = 6
        }

        /// <summary>
        /// Represents the result of validating a user. 
        /// The <see cref="User"/> property will be populated only if the Result is <see cref="ApiDetailsValidity.Valid"/>.s
        /// </summary>
        public class ApiDetailsValidityResult
        {
            /// <summary>
            /// The Result of the Validation.
            /// </summary>
            public ApiDetailsValidity Result { get; set; }

            /// <summary>
            /// The current user, if validation succeeded.
            /// </summary>
            public ICurrentUser User { get; set; }

        }

        /// <summary>
        /// AuthToken elements is the result of a call to <see cref="ApiDetails.GetAuthTokenElements">ApiDetails.GetAuthTokenElements</see>.
        /// It contains either the constituent parts of the AuthToken and a successful <see cref="ApiDetailsValidity">ApiDetailsValidity</see>, or
        /// or a failure, where all the parts will be default and the Result will be BadAuthToken.
        /// </summary>
        public class AuthTokenElements
        {
            /// <summary>
            /// The Result of attempted get. 
            /// </summary>
            public ApiDetailsValidity Result { get; set; }

            /// <summary>
            /// The account Id part of the token.
            /// </summary>
            public int AccountId { get; set; }

            /// <summary>
            /// The apiDetails Id part of the token
            /// </summary>
            public int ApiDetailsId { get; set; }

            /// <summary>
            /// The Auth token itself.
            /// </summary>
            public string AuthToken { get; set; }
        }


        #endregion Enums and related objects

        #region Public Properties

        /// <summary>
        /// The Key for this object in the DB, purely for quick reference.
        /// </summary>
        public int ApiDetailsId { get; private set; }

        /// <summary>
        /// The AccountId, purely for quick reference.
        /// </summary>
        public int AccountId { get; private set; }

        /// <summary>
        /// The Employeed Id to whom this ApiDetails object applies.
        /// </summary>
        public int EmployeeId { get; private set; }
        
        /// <summary>
        /// The generated security key used to decrypt a public key.
        /// </summary>
        public string CertificateInfo { get; private set; }

        /// <summary>
        /// The generated authentication token. This will be set during a Regenerate or Create.
        /// </summary>
        public string AuthToken { get; private set; }

        /// <summary>
        /// The time at which the AuthToken was generated. This will be set during a Regenerate or Create.
        /// </summary>
        public DateTime GenerationTime { get; private set; }

        /// <summary>
        /// The time at which the AuthToken will expire. This will be set during a Regenerate or Create.
        /// It is also updated on a call to RecreateAuthToken.
        /// </summary>
        public DateTime ExpiryTime { get; private set; }
        
        #endregion Public Properties

        #region Public Methods and Operators


        /// <summary>
        /// Parses the AuthToken to get the elements out of it.
        /// </summary>
        /// <param name="authToken"></param>
        /// <returns></returns>
        public static AuthTokenElements GetAuthTokenElements(string authToken)
        {
            var result = new AuthTokenElements();

            // split out the contents of the auth token
            var authTokenArray = authToken.Split(char.Parse(EncryptionValueSeparator));
            if (authTokenArray.Length != 3)
            {
                result.Result = ApiDetailsValidity.InvalidAuthTokenBad;
                return result;
            }
            
            // get the values
            result.AccountId = Convert.ToInt32(authTokenArray[0]);
            result.ApiDetailsId = Convert.ToInt32(authTokenArray[1]);
            result.AuthToken = authTokenArray[2];
            if (result.ApiDetailsId < 1 || result.AccountId < 1 || string.IsNullOrEmpty(result.AuthToken))
            {
                result.Result = ApiDetailsValidity.InvalidAuthTokenBad;
                return result;
            }

            result.Result = ApiDetailsValidity.Valid;
            return result;
        }

        /// <summary>
        /// Attempts to get the supplied user's ApiDetails object. If this doesn't exist, null is returned.
        /// Important: This method does not check if the expiry is out of date for an existing user.
        /// </summary>
        /// <param name="currentUser">The current user object, needed for the account, subaccount and employee identifiers</param>
        /// <param name="connection">Database connection override, usually for unit testing</param>
        /// <returns>An ApiDetails object representing the current user's Api Access, or null</returns>
        public static ApiDetails Get(ICurrentUser currentUser, IDBConnection connection = null)
        {
            //EnusureApiUserIsGood(currentUser);

            var cache = new Cache();
            var apiDetails = CacheGet(currentUser, cache);

            if (apiDetails != null) return apiDetails;

            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                databaseConnection.AddWithValue(SqlParamKeyEmployeeId, currentUser.EmployeeID);

                using (var reader = databaseConnection.GetReader(SqlSpGetByEmployee, CommandType.StoredProcedure))
                {
                    if (reader.Read())
                    {
                        int apiDetailsIdIndex = reader.GetOrdinal(SqlTableKeyApiDetailsId),
                            employeeIdIndex = reader.GetOrdinal(SqlTableKeyEmployeeId),
                            privateKeyIndex = reader.GetOrdinal(SqlTableKeyCertificateInfo),
                            generationIndex = reader.GetOrdinal(SqlTableKeyGenerationTime),
                            expiryIndex = reader.GetOrdinal(SqlTableKeyExpiryTime);


                        var stringToEncrypt = reader.GetInt32(employeeIdIndex) + EncryptionValueSeparator + new DateTime(reader.GetInt64(generationIndex)).Ticks;
                        apiDetails = new ApiDetails
                        {
                            ApiDetailsId = reader.GetInt32(apiDetailsIdIndex),
                            AccountId = currentUser.AccountID,
                            EmployeeId = reader.GetInt32(employeeIdIndex),
                            CertificateInfo = reader.GetString(privateKeyIndex),
                            GenerationTime = new DateTime(reader.GetInt64(generationIndex)),
                            ExpiryTime = reader.GetDateTime(expiryIndex),
                            AuthToken = currentUser.AccountID + EncryptionValueSeparator + reader.GetInt32(apiDetailsIdIndex) + EncryptionValueSeparator + new ApiAuthTokenProvider().Encrypt(reader.GetString(privateKeyIndex), stringToEncrypt)
                        };
                    }

                    reader.Close();
                }
            }

            // update cache
            if (apiDetails != null) CacheAddOrUpdate(apiDetails, cache);

            return apiDetails;
        }


        /// <summary>
        /// Creates a new ApiDetails object in the database, for the specified user.
        /// </summary>
        /// <param name="currentUser">The current user object, needed for the account, subaccount and employee identifiers</param>
        /// <param name="authTokenProvider">An instance of <see cref="IAuthTokenProvider"/> to use to encrypt / decrypt tokens</param>
        /// <param name="connection">Database connection override, usually for unit testing</param>
        /// <returns></returns>
        public static ApiDetails Create(ICurrentUser currentUser, IAuthTokenProvider authTokenProvider, IDBConnection connection = null)
        {
            //EnusureApiUserIsGood(currentUser);

            // if the get doesn't work, create a new one
            var apiDetails = new ApiDetails
            {
                AccountId = currentUser.AccountID,
                EmployeeId = currentUser.EmployeeID
            };

            return PopulateApiDetails(apiDetails, authTokenProvider, true, connection);
        }


        /// <summary>
        /// Finds an ApiDetails record with the corresponding account + employee IDs, then deletes it from the database.
        /// </summary>
        /// <param name="currentUser">The current user object, needed for the account, subaccount and employee identifiers</param>
        /// <param name="connection">Database connection override, usually for unit testing</param>
        /// <returns>The database result</returns>
        public static bool Delete(ICurrentUserBase currentUser, IDBConnection connection = null)
        {
            int deleteResult;

            // remove for database
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                databaseConnection.AddWithValue(SqlParamKeyEmployeeId, currentUser.EmployeeID);
                databaseConnection.AddReturn(SqlParamKeyReturn, SqlDbType.Int);
                databaseConnection.ExecuteProc(SqlSpDelete);
                deleteResult = databaseConnection.GetReturnValue<int>(SqlParamKeyReturn);
            }

            if (deleteResult <= 0) return false;

            // remove from cache
            CacheDelete(currentUser);
            return true;
        }


        /// <summary>
        /// Using an AccountId and AuthToken, looks up the appropriate ApiDetails, and attempts to validate the supplied AuthToken.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="apiDetailsId">The ApiDetailsId</param>
        /// <param name="authToken">The AuthToken</param>
        /// <param name="authTokenProvider">An instance of <see cref="IAuthTokenProvider"/> to use to encrypt / decrypt tokens</param>
        /// <param name="connection">Database connection override, usually for unit testing</param>
        /// <returns>An <see cref="ApiDetailsValidity"/> with the appropriate status.</returns>
        public static ApiDetailsValidityResult Authenticate(int accountId, int apiDetailsId, string authToken, IAuthTokenProvider authTokenProvider, IDBConnection connection = null)
        {
            ApiDetails apiDetails = null;

            // attempt to authenticate
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                databaseConnection.AddWithValue(SqlParamKeyApiDetailsId, apiDetailsId);

                using (var reader = databaseConnection.GetReader(SqlSpGet, CommandType.StoredProcedure))
                {
                    int     apiDetailsIdIndex = reader.GetOrdinal(SqlTableKeyApiDetailsId),
                            employeeIdIndex = reader.GetOrdinal(SqlTableKeyEmployeeId),
                            privateKeyIndex = reader.GetOrdinal(SqlTableKeyCertificateInfo),
                            generationIndex = reader.GetOrdinal(SqlTableKeyGenerationTime),
                            expiryIndex = reader.GetOrdinal(SqlTableKeyExpiryTime);

                        if (reader.Read())
                        {
                            apiDetails = new ApiDetails
                            {
                                ApiDetailsId = reader.GetInt32(apiDetailsIdIndex),
                                AccountId = accountId,
                                EmployeeId = reader.GetInt32(employeeIdIndex),
                                CertificateInfo = reader.GetString(privateKeyIndex),
                                AuthToken = accountId + EncryptionValueSeparator + apiDetailsId + EncryptionValueSeparator + authToken,
                                GenerationTime = new DateTime(reader.GetInt64(generationIndex)),
                                ExpiryTime = reader.GetDateTime(expiryIndex)
                            };
                        }

                        reader.Close();
                    }
                }

            // null check here, to return appropriate result if failed
            if (apiDetails == null) return new ApiDetailsValidityResult {Result = ApiDetailsValidity.InvalidAuthTokenBad};
            
            // validate
            var result = apiDetails.Validate(authTokenProvider);
            
            // return now if result is bad...
            if (result.Result != ApiDetailsValidity.Valid) return result;
            
            // result is okay, so update expiry and drop instance in cache
            apiDetails.UpdateExpiryTime(connection);
            CacheAddOrUpdate(apiDetails);

            return result;
        }


        /// <summary>
        /// Causes this ApiDetails to validate the token.
        /// </summary>
        /// <param name="authTokenProvider">An instance of <see cref="IAuthTokenProvider"/> to use to encrypt / decrypt tokens</param>
        /// <returns>An <see cref="ApiDetailsValidity"/> corresponding to the result of the validation.</returns>
        public ApiDetailsValidityResult Validate(IAuthTokenProvider authTokenProvider)
        {
            // do all basic error checking first.
            if (string.IsNullOrEmpty(CertificateInfo))
                return new ApiDetailsValidityResult {Result = ApiDetailsValidity.InvalidBadCertificateInfo};
            if (ExpiryTime < DateTime.Now) 
                return new ApiDetailsValidityResult {Result = ApiDetailsValidity.InvalidAuthTokenExpired}; 
            if (string.IsNullOrEmpty(AuthToken))
                return new ApiDetailsValidityResult {Result = ApiDetailsValidity.InvalidAuthTokenBad};
            try
            {
                // now if everything above is okay, decrypt and validate the token.
                var parts = authTokenProvider.Decrypt(CertificateInfo, AuthToken, EncryptionValueSeparator)
                                             .Split(char.Parse(EncryptionValueSeparator));

                if (parts.Length != 2)
                {
                    return new ApiDetailsValidityResult {Result = ApiDetailsValidity.InvalidAuthTokenBad};
                }

                var id = int.Parse(parts[0]);
                var creation = new DateTime(long.Parse(parts[1]));

                // make sure the two token parts match the actual record
                if (id != EmployeeId || creation != GenerationTime)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                return new ApiDetailsValidityResult { Result = ApiDetailsValidity.InvalidAuthTokenBad };
            }
            
            // we have a valid user. 
            var creds = AccountId + "," + EmployeeId;
            var employees = new cEmployees(AccountId);
            var employee = employees.GetEmployeeById(EmployeeId);
            var result = new ApiDetailsValidityResult
            {
                Result = ApiDetailsValidity.Valid,
                User = cMisc.GetCurrentUser(creds)
            };

            
            result.User.Employee = employee; // fetching the CurrentUser above returns an Employee with incorrect info.

            return result;
        }


        /// <summary>
        /// Updates the expiry time of this instance. This is to achieve a kind of 'rolling session' for the API user.
        /// <param name="connection">Database connection override, usually for unit testing</param>
        /// </summary>
        public void UpdateExpiryTime(IDBConnection connection = null)
        {
            // get expiry
            ExpiryTime = GetNewExpiryFromDateTime(DateTime.Now);

            int result;
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(AccountId)))
            {
                databaseConnection.AddWithValue(SqlParamKeyApiDetailsId, ApiDetailsId);
                databaseConnection.AddWithValue(SqlParamKeyExpiryTime, ExpiryTime);
                databaseConnection.AddReturn(SqlParamKeyReturn, SqlDbType.Int);
                databaseConnection.ExecuteProc(SqlSpUpdateExpiry);
                result = databaseConnection.GetReturnValue<int>(SqlParamKeyReturn);
            }

            // check for error
            if (result != 0) throw new DataException(ExceptionMessageAddingToDb);

            // update cache
            CacheAddOrUpdate(this);
        }


        /// <summary>
        /// Updates the <see cref="GenerationTime"/> of this instance, and sets the <see cref="AuthToken"/> to a newly generated token.
        /// <param name="connection">Database connection override, usually for unit testing</param>
        /// </summary>
        public void RecreateAuthToken(IAuthTokenProvider authTokenProvider, IDBConnection connection = null)
        {
            var updated = PopulateApiDetails(this, authTokenProvider, false, connection);
            GenerationTime = updated.GenerationTime;
            ExpiryTime = updated.ExpiryTime;
            CertificateInfo = updated.CertificateInfo;
            AccountId = updated.AccountId;
            ApiDetailsId = updated.ApiDetailsId;
            AuthToken = updated.AuthToken;
            EmployeeId = updated.EmployeeId;
        }
       
        #endregion Public Methods and Operators

        #region Methods

        /// <summary>
        /// Given an ApiDetails instance, (re)populates the GenerationTime, ExpiryTime and CertificateInfo properties.
        /// Depending on the createNotUpdate argument supplied, this will attempt to create a new ApiDetails
        /// or update an existing ApiDetails in the Database.
        /// </summary>
        /// <param name="instance">The instance of ApiDetails to operate on.</param>
        /// <param name="authTokenProvider">An instance of <see cref="IAuthTokenProvider"/> to use to encrypt / decrypt tokens</param>
        /// <param name="createNotUpate">Whether to attempt to add a new record, or update an existing one.</param>
        /// <param name="connection">Database connection override, usually for unit testing</param>
        /// <returns></returns>
        private static ApiDetails PopulateApiDetails(ApiDetails instance, IAuthTokenProvider authTokenProvider, bool createNotUpate = false, IDBConnection connection = null)
        {
            // handle bad input
            if (instance.AccountId < 1 || instance.EmployeeId < 1) throw new Exception(ExceptionMessageAccountEmployee);

            var now = DateTime.Now;
            var tokenExpiry = GetNewExpiryFromDateTime(now);

            // generate new data
            instance.GenerationTime = now;
            instance.ExpiryTime = tokenExpiry;
            instance.CertificateInfo = authTokenProvider.Generate();

            // attempt to add the ApiDetails to the DB
            int result;
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(instance.AccountId)))
            {
                databaseConnection.AddWithValue(SqlParamKeyEmployeeId, instance.EmployeeId);
                databaseConnection.AddWithValue(SqlParamKeyCertificateInfo, instance.CertificateInfo, -1);
                databaseConnection.AddWithValue(SqlParamKeyGenerationTime, instance.GenerationTime.Ticks);
                databaseConnection.AddWithValue(SqlParamKeyExpiryTime, instance.ExpiryTime);
                databaseConnection.AddReturn(SqlParamKeyReturn, SqlDbType.Int);
                
                if (createNotUpate)
                {
                    databaseConnection.ExecuteProc(SqlSpAdd);
                }
                else
                {
                    databaseConnection.AddWithValue(SqlParamKeyApiDetailsId, instance.ApiDetailsId);
                    databaseConnection.ExecuteProc(SqlSpUpdate);
                }
                
                result = databaseConnection.GetReturnValue<int>(SqlParamKeyReturn);
            }


            // check for error
            if (result < 1)
            {
                throw new DataException(createNotUpate
                    ? ExceptionMessageAddingToDb
                    : ExceptionMessageUpdateDb);
            }
            
            // set ApiDetailsId
            instance.ApiDetailsId = result;

            // generate the AuthToken.
            var stringToEncrypt = instance.EmployeeId + EncryptionValueSeparator + instance.GenerationTime.Ticks;
            instance.AuthToken = instance.AccountId + EncryptionValueSeparator + instance.ApiDetailsId + EncryptionValueSeparator + authTokenProvider.Encrypt(instance.CertificateInfo, stringToEncrypt);
            
            // update cache now
            CacheAddOrUpdate(instance);

            return instance;
        }


        /// <summary>
        /// Uses the web.config or a default constant to add the appropriate amount of minutes to the supplied DateTime.
        /// </summary>
        /// <param name="dateTime">The DateTime instance to create an expiry time from.</param>
        /// <returns>A new Expiry Time.</returns>
        private static DateTime GetNewExpiryFromDateTime(DateTime dateTime)
        {
            // work out when the new expiry will be
            var tokenExpiryFromConfig = ConfigurationManager.AppSettings[AppSettingExpiryKey] ?? AppSettingExpiryDefault;
            return dateTime.AddMinutes(int.Parse(tokenExpiryFromConfig));
        }


        /// <summary>
        /// Gets an ApiDetails instance from the cache object.
        /// </summary>
        /// <param name="currentUser">The user from which to identify the ApiDetails</param>
        /// <param name="cache">Pass in a cache object to use.</param>
        /// <returns></returns>
        private static ApiDetails CacheGet(ICurrentUserBase currentUser, Cache cache = null)
        {
            var caching = cache ?? new Cache();
            return (ApiDetails)caching.Get(currentUser.AccountID, CacheAreaKey, currentUser.EmployeeID.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Adds to or updates an instance in the cache object.
        /// <param name="instance">The instance to add or update.</param>
        /// <param name="cache">Pass in a cache object to use.</param>
        /// </summary>
        private static void CacheAddOrUpdate(ApiDetails instance, Cache cache = null)
        {
            var caching = cache ?? new Cache();
            if (caching.Contains(instance.AccountId, CacheAreaKey, instance.EmployeeId.ToString(CultureInfo.InvariantCulture)))
            {
                caching.Set(instance.AccountId, CacheAreaKey, instance.EmployeeId.ToString(CultureInfo.InvariantCulture), instance);
            }
            else
            {
                caching.Add(instance.AccountId, CacheAreaKey, instance.EmployeeId.ToString(CultureInfo.InvariantCulture),instance);
            }
        }

        /// <summary>
        /// Deletes an instance from the cache.
        /// </summary>
        /// <param name="currentUser">The user from which to identify the ApiDetails</param>
        /// <param name="cache">Pass in a cache object to use.</param>
        private static void CacheDelete(ICurrentUserBase currentUser, Cache cache = null)
        {
            var caching = cache ?? new Cache();
            caching.Delete(currentUser.AccountID, CacheAreaKey, currentUser.EmployeeID.ToString(CultureInfo.InvariantCulture));
        }

        #endregion Methods
    }
}
