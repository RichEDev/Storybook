using System.Text;
using BusinessLogic.Cache;
using Common.Logging;
using SpendManagementLibrary.Enumerators;

namespace Spend_Management.expenses.code
{
    using System;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Cards;
    using SpendManagementLibrary.Helpers;

    /// <summary>
    /// A class to manage the Corporate Card Import Log data
    /// </summary>
    public class CorporateCardImportLogs
    {
        /// <summary>
        /// A private instance of <see cref="cAccount"/>
        /// </summary>
        private readonly cAccount _currentAccount;

        /// <summary>
        /// An instance of <see cref="ILog"/> for logging <see cref="ICache{T,TK}"/> diagnostics and information.
        /// </summary>
        private static readonly ILog Log = new LogFactory<CorporateCardImportLog>().GetLogger();

        /// <summary>
        /// Create a new instance of <see cref="CorporateCardImportLogs"/>
        /// </summary>
        /// <param name="currentAccount"></param>
        public CorporateCardImportLogs(cAccount currentAccount)
        {
            this._currentAccount = currentAccount;
        }

        /// <summary>
        /// Save an instance of <see cref="CorporateCardImportLog"/> to the database
        /// </summary>
        /// <param name="corporateCardImportLog">The instance of <see cref="CorporateCardImportLog"/> to save</param>
        /// <returns>True if the record has saved.</returns>
        public bool Save(CorporateCardImportLog corporateCardImportLog)
        {
            try
            {
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat($"Saving an instance of CorporateCardImportLog {corporateCardImportLog.ImportId} ");
                }

                int result;
                using (var expdata =
                    new DatabaseConnection(cAccounts.getConnectionString(this._currentAccount.accountid)))
                {
                    var strsql =
                        "INSERT INTO [dbo].[CorporateCardImportLog] ([ImportId] ,[CardProviderId] ,[LogMessage] ,[Date] ,[StatementId], [Status], [NumberOfErrors]) VALUES (@ImportId ,@CardProviderId ,@LogMessage ,@date, @StatementId, @Status, @NumberOfErrors)";


                    expdata.sqlexecute.Parameters.AddWithValue("@ImportId", corporateCardImportLog.ImportId);
                    expdata.sqlexecute.Parameters.AddWithValue("@CardProviderId",
                        corporateCardImportLog.CardProviderId);
                    if (corporateCardImportLog.StatementId == 0)
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@StatementId", DBNull.Value);
                    }
                    else
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@StatementId", corporateCardImportLog.StatementId);
                    }

                    expdata.sqlexecute.Parameters.AddWithValue("@LogMessage", corporateCardImportLog.LogMessage);
                    expdata.sqlexecute.Parameters.AddWithValue("@Date", corporateCardImportLog.Date);
                    expdata.sqlexecute.Parameters.AddWithValue("@Status", corporateCardImportLog.Status);
                    expdata.sqlexecute.Parameters.AddWithValue("@NumberOfErrors", corporateCardImportLog.NumberOfErrors);
                    result = expdata.ExecuteSQL(strsql);
                    expdata.sqlexecute.Parameters.Clear();
                }

                if (result == 0)
                {
                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat($"Saving an instance of CorporateCardImportLog failed.");
                    }
                    return false;
                }

                Log.Debug($"Saved an instance of CorporateCardImportLog.");
            }
            catch (Exception e)
            {
                if (Log.IsErrorEnabled)
                {
                    Log.Error($"Saving an instance of CorporateCardImportLog returned an error.", e);
                }
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets a corporate card import log
        /// </summary>
        /// <param name="importId">Id of a corporate card import</param>
        /// <returns>An corporate card log <see cref="CorporateCardImportLog"/></returns>
        public CorporateCardImportLog GetLogs(Guid importId)
        {
            try
            {
                var cardProviderId = 0;
                var logMessage = "";
                var importDate = DateTime.Now;
                var statementId = 0;
                var numberOfErrors = 0;
                var status = CorporateCardImportStatus.Imported;
                using (var expdata =
                    new DatabaseConnection(cAccounts.getConnectionString(this._currentAccount.accountid)))
                {
                    const string strsql = "SELECT [CardProviderId] ,[LogMessage] ,[Date] ,[StatementId], [Status], [NumberOfErrors] FROM [dbo].[CorporateCardImportLog] WHERE [ImportId] = @ImportId";

                    expdata.sqlexecute.Parameters.AddWithValue("@ImportId", importId);

                    using (var reader = expdata.GetReader(strsql))
                    {
                        expdata.sqlexecute.Parameters.Clear();

                        if (reader.Read())
                        {
                            cardProviderId = reader.GetInt32(reader.GetOrdinal("CardProviderId"));
                            logMessage = reader.GetString(reader.GetOrdinal("LogMessage"));
                            importDate = reader.GetDateTime(reader.GetOrdinal("Date"));
                            statementId = reader.IsDBNull(reader.GetOrdinal("StatementId")) ? -1 : reader.GetInt32(reader.GetOrdinal("StatementId"));
                            status = (CorporateCardImportStatus)reader.GetByte(reader.GetOrdinal("Status"));
                            numberOfErrors = reader.GetInt32(reader.GetOrdinal("NumberOfErrors"));
                        }
                    }
                }

                return new CorporateCardImportLog(cardProviderId, importId, importDate, logMessage, statementId,
                    status, numberOfErrors);
            }
            catch (Exception e)
            {
                if (Log.IsErrorEnabled)
                {
                    Log.Error($"Getting an instance of CorporateCardImportLog returned an error, {e.Message}.", e);
                }

                return new CorporateCardImportLog(-1, new Guid(), DateTime.Now, "", -1, CorporateCardImportStatus.FailedValidation, -1);
            }
        }
    }
}