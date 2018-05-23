namespace Spend_Management.shared.code
{
    using System;
    using System.Collections.Generic;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;
    using System.Data;
    using Logon;
    using Utilities.DistributedCaching;
    using System.Linq;

    using BusinessLogic.Modules;

    /// <summary>
    /// Class includes Data access methods related to Logon Messages.
    /// </summary>
    [Serializable]
    public class LogonMessages
    {
        /// <summary>
        /// Audit log.
        /// </summary>
        private readonly cAuditLog _auditLog;

        /// <summary>
        /// Cache object.
        /// </summary>
        private Cache _cache = new Cache();

        /// <summary>
        /// Caching area.
        /// </summary>
        private const string CacheArea = "logonmessages";

        /// <summary>
        /// Logon Message object.
        /// </summary>
        private Dictionary<int, LogonMessage> _logonMessageList;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public LogonMessages()
        {
            this._auditLog = new cAuditLog();
            this.InitialiseData();
        }

        /// <summary>
        /// This method returns a logon message from the cachelist  based on id.
        /// </summary>
        /// <param name="messageId">message Id</param>
        /// <returns></returns>
        public LogonMessage GetActiveLogOnMessagesById(int messageId)
        {
            LogonMessage logonMessage;
            this._logonMessageList.TryGetValue(messageId, out logonMessage);
            return logonMessage;
        }

        /// <summary>
        /// Delete Logon Message by messageId.
        /// </summary>
        /// <param name="messageId">message Id</param>
        public void DeleteLogonMessage(int messageId)
        {
            using (var connection = new DatabaseConnection(GlobalVariables.MetabaseConnectionString))
            {
                connection.sqlexecute.Parameters.AddWithValue("@messageId ", messageId);
                connection.ExecuteProc("DeleteLogonMessages");
                connection.sqlexecute.Parameters.Clear();
                this.InvalidateCache();
                var logonMessage = this.GetActiveLogOnMessagesById(messageId);
                this._auditLog.deleteRecord(SpendManagementElement.LogonMessages, messageId, logonMessage.HeaderText);
            }
        }

        /// <summary>
        /// Archive Logon Message by messageId.
        /// </summary>
        /// <param name="messageId">message Id</param>
        /// <param name="activeStatus">message Id</param>

        public string ChangeLogonMessagesStatus(int messageId, int activeStatus)
        {
            var moduleList = this.CheckCheckLogonMessagesCanArchivedOrDeleted(messageId);

            if (moduleList.Count > 0 && activeStatus == 0)
            {
                return string.Join(",", moduleList.ToArray());
            }

            using (var connection = new DatabaseConnection(GlobalVariables.MetabaseConnectionString))
            {
                connection.sqlexecute.Parameters.AddWithValue("@messageId ", messageId);
                connection.sqlexecute.Parameters.AddWithValue("@archive", activeStatus);
                connection.ExecuteProc("ChangeLogonMessagesStatus");
                connection.sqlexecute.Parameters.Clear();
                this.InvalidateCache();
            }
            var logMessages = this.GetActiveLogOnMessagesById(messageId);
            if (logMessages.Archived != !logMessages.Archived)
            {
                this._auditLog.editRecord(messageId, logMessages.HeaderText, SpendManagementElement.LogonMessages, new Guid("D55A2B7B-4600-4F9E-93C9-DA20421E361F"), (logMessages.Archived).ToString(), (!logMessages.Archived).ToString());
            }

            return null;
        }

        /// <summary>
        /// Check if the message can be activated
        /// </summary>
        /// <param name="messageId">Current message ID</param>
        /// <returns>List of messages which cannot be activated</returns>

        public List<string> CheckCheckLogonMessagesCanArchivedOrDeleted(int messageId=0)
        {
            var moduleList = new List<string>();
            using (var connection = new DatabaseConnection(GlobalVariables.MetabaseConnectionString))
            {
                connection.sqlexecute.Parameters.AddWithValue("@messageId", messageId);
                using (var reader = connection.GetReader("CheckMaxMessageCountForModules", CommandType.StoredProcedure))
                {
                    if (reader != null)
                    {
                        var moduleName = reader.GetOrdinal("modules");
                        while (reader.Read())
                        {
                            moduleList.Add(reader.GetString(moduleName));
                        }
                    }
                }
                connection.sqlexecute.Parameters.Clear();
            }
            return moduleList;
        }

        /// <summary>
        /// Get all  messages for module
        /// </summary>
        /// <returns>List of Logon Messages</returns>
        public List<LogonMessage> GetAllMessages()
        {
            return this._logonMessageList.Values.ToList();
        }

        /// <summary>
        /// Get all active messages for module
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns>List of Logon Messages</returns>
        public List<LogonMessage> GetAllActiveMessages(int moduleId)
        {
            return this._logonMessageList.Values.Where(x => x.Archived == false && x.MessageModules.Contains(moduleId)).ToList();
        }

        /// <summary>
        /// Add or update logon Messages.
        /// </summary>
        /// <param name="logonMessages">Details of logon messages</param>
        /// <param name="employeeId">Details of logon messages</param>
        public int AddOrUpdateLogonMessage(LogonMessage logonMessages, int employeeId)
        {
            int returnCode = 0;
            using (var connection = new DatabaseConnection(GlobalVariables.MetabaseConnectionString))
            {
                connection.sqlexecute.Parameters.AddWithValue("@messageid", logonMessages.MessageId);
                connection.sqlexecute.Parameters.AddWithValue("@CategoryTitle", logonMessages.CategoryTitle);
                connection.sqlexecute.Parameters.AddWithValue("@CategoryTitleColourCode", logonMessages.CategoryTitleColourCode);
                connection.sqlexecute.Parameters.AddWithValue("@HeaderText", logonMessages.HeaderText);
                connection.sqlexecute.Parameters.AddWithValue("@HeaderTextColourCode", logonMessages.HeaderTextColourCode);
                connection.sqlexecute.Parameters.AddWithValue("@ButtonLink", logonMessages.ButtonLink);
                connection.sqlexecute.Parameters.AddWithValue("@ButtonText", logonMessages.ButtonText);
                connection.sqlexecute.Parameters.AddWithValue("@BodyTextColourCode", logonMessages.BodyTextColourCode);
                connection.sqlexecute.Parameters.AddWithValue("@ButtonForeColour", logonMessages.ButtonForeColour);
                connection.sqlexecute.Parameters.AddWithValue("@BackgroundImage", logonMessages.BackgroundImage);
                connection.sqlexecute.Parameters.AddWithValue("@Archived", logonMessages.Archived);
                connection.sqlexecute.Parameters.AddWithValue("@BodyText", logonMessages.BodyText);
                connection.sqlexecute.Parameters.AddWithValue("@ButtonBackGroundColour", logonMessages.ButtonBackGroundColour);
                connection.sqlexecute.Parameters.AddWithValue("@Icon", logonMessages.Icon);
                connection.sqlexecute.Parameters.AddWithValue("@moduleIds", logonMessages.MessageModules != null ? string.Join(", ", logonMessages.MessageModules.ToArray()) : "");
                connection.sqlexecute.Parameters.Add("@returnvalue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                connection.ExecuteProc("AddOrUpdateLogonMessages");
                int returnValue = (int)connection.sqlexecute.Parameters["@returnvalue"].Value;
                connection.sqlexecute.Parameters.Clear();

                if (logonMessages.MessageId == 0)
                {
                    this._auditLog.addRecord(SpendManagementElement.LogonMessages, logonMessages.HeaderText, returnValue);
                }
                else
                {
                    var currentLogonMessage = this.GetActiveLogOnMessagesById(logonMessages.MessageId);

                    if (currentLogonMessage.CategoryTitle.Trim() != logonMessages.CategoryTitle.Trim())
                    {
                        this._auditLog.editRecord(logonMessages.MessageId, currentLogonMessage.HeaderText, SpendManagementElement.LogonMessages, new Guid("A8E6497D-1DCC-4120-B8E7-27D6B8C4A141"), currentLogonMessage.CategoryTitle, logonMessages.CategoryTitle);
                    }
                    if (currentLogonMessage.CategoryTitleColourCode.Trim() != logonMessages.CategoryTitleColourCode.Trim())
                    {
                        this._auditLog.editRecord(logonMessages.MessageId, currentLogonMessage.HeaderText, SpendManagementElement.LogonMessages, new Guid("FFCDC8AE-4F2E-493D-A83D-E8675525CF0E"), currentLogonMessage.CategoryTitleColourCode, logonMessages.CategoryTitleColourCode);
                    }
                    if (currentLogonMessage.HeaderText.Trim() != logonMessages.HeaderText.Trim())
                    {
                        this._auditLog.editRecord(logonMessages.MessageId, currentLogonMessage.HeaderText, SpendManagementElement.LogonMessages, new Guid("6D88F346-2D99-4A6D-8150-59F9B52A353D"), currentLogonMessage.HeaderText, logonMessages.HeaderText);
                    }
                    if (currentLogonMessage.HeaderTextColourCode.Trim() != logonMessages.HeaderTextColourCode.Trim())
                    {
                        this._auditLog.editRecord(logonMessages.MessageId, currentLogonMessage.HeaderText, SpendManagementElement.LogonMessages, new Guid("B132EF19-B919-4809-AE40-94DDF902E2FF"), currentLogonMessage.HeaderTextColourCode, logonMessages.HeaderTextColourCode);
                    }
                    if (currentLogonMessage.BodyText.Trim() != logonMessages.BodyText.Trim())
                    {
                        this._auditLog.editRecord(logonMessages.MessageId, currentLogonMessage.HeaderText, SpendManagementElement.LogonMessages, new Guid("ECE0BD45-3581-4803-B194-B946F8238A1E"), currentLogonMessage.BodyText, logonMessages.BodyText);
                    }
                    if (currentLogonMessage.BodyTextColourCode.Trim() != logonMessages.BodyTextColourCode.Trim())
                    {
                        this._auditLog.editRecord(logonMessages.MessageId, currentLogonMessage.HeaderText, SpendManagementElement.LogonMessages, new Guid("D584D498-18DE-4AB0-AACC-99ABC88C2654"), currentLogonMessage.BodyTextColourCode, logonMessages.BodyTextColourCode);
                    }
                    if (currentLogonMessage.ButtonLink.Trim() != logonMessages.ButtonLink.Trim())
                    {
                        this._auditLog.editRecord(logonMessages.MessageId, currentLogonMessage.HeaderText, SpendManagementElement.LogonMessages, new Guid("08B5E3A1-74C8-48AD-B9F9-55A17A56B63F"), currentLogonMessage.ButtonLink, logonMessages.ButtonLink);
                    }
                    if (currentLogonMessage.ButtonText.Trim() != logonMessages.ButtonText.Trim())
                    {
                        this._auditLog.editRecord(logonMessages.MessageId, currentLogonMessage.HeaderText, SpendManagementElement.LogonMessages, new Guid("0B5D9A57-6BF0-4328-BA5F-058B39B8B8B7"), currentLogonMessage.ButtonText, logonMessages.ButtonText);
                    }
                    if (currentLogonMessage.Archived != logonMessages.Archived)
                    {
                        this._auditLog.editRecord(logonMessages.MessageId, currentLogonMessage.HeaderText, SpendManagementElement.LogonMessages, new Guid("D55A2B7B-4600-4F9E-93C9-DA20421E361F"), currentLogonMessage.Archived.ToString(), logonMessages.Archived.ToString());
                    }
                    if (currentLogonMessage.Icon != logonMessages.Icon)
                    {
                        this._auditLog.editRecord(logonMessages.MessageId, currentLogonMessage.HeaderText, SpendManagementElement.LogonMessages, new Guid("3FABDC54-57EA-4FBE-ABBD-52D1E43D4CC2"), currentLogonMessage.Icon, logonMessages.Icon);
                    }
                    if (currentLogonMessage.BackgroundImage != logonMessages.BackgroundImage)
                    {
                        this._auditLog.editRecord(logonMessages.MessageId, currentLogonMessage.HeaderText, SpendManagementElement.LogonMessages, new Guid("A8E6497D-1DCC-4120-B8E7-27D6B8C4A141"), currentLogonMessage.BackgroundImage, logonMessages.BackgroundImage);
                    }
                    if (currentLogonMessage.ButtonBackGroundColour.Trim() != logonMessages.ButtonBackGroundColour.Trim())
                    {
                        this._auditLog.editRecord(logonMessages.MessageId, currentLogonMessage.HeaderText, SpendManagementElement.LogonMessages, new Guid("1B9E43E1-91F9-4B97-A0CF-26D899E183F8"), currentLogonMessage.ButtonBackGroundColour, logonMessages.ButtonBackGroundColour);
                    }
                    if (currentLogonMessage.ButtonForeColour.Trim() != logonMessages.ButtonForeColour.Trim())
                    {
                        this._auditLog.editRecord(logonMessages.MessageId, currentLogonMessage.HeaderText, SpendManagementElement.LogonMessages, new Guid("AEBAF7D9-2728-4464-9A8F-5FF9D1DFFF6D"), currentLogonMessage.ButtonForeColour, logonMessages.ButtonForeColour);
                    }
                    if (logonMessages.MessageModules != null)
                    {
                        logonMessages.MessageModules.Remove(0);
                        if (!logonMessages.MessageModules.OrderBy(x => x).SequenceEqual(currentLogonMessage.MessageModules.OrderBy(x => x)))
                        {
                            var newModuleList = logonMessages.MessageModules.Select(i => (Modules) i).ToList();
                            var oldModuleList = currentLogonMessage.MessageModules.Select(i => (Modules) i).ToList();

                            this._auditLog.editRecord(logonMessages.MessageId, currentLogonMessage.HeaderText,
                                SpendManagementElement.LogonMessages, new Guid("B05ECE0C-4A6B-4682-B423-2A5DF6FF0D86"),
                                string.Join(", ", oldModuleList.Select(s => s.ToString())),
                                string.Join(", ", newModuleList.Select(s => s.ToString())));
                        }
                    }
                }
            }
            this.InvalidateCache();
            return returnCode;
        }

        /// <summary>
        /// Get all logonmessages to cache list.
        /// </summary>
        public Dictionary<int, LogonMessage> CacheList()
        {
            var logonMessageList = new Dictionary<int, LogonMessage>();
            var lsMessageModules = new List<cMessageModules>();
            var lstModules = this.GetMessageModules(lsMessageModules);

            using (var connection = new DatabaseConnection(GlobalVariables.MetabaseConnectionString))
            using (var reader = connection.GetReader("GetAllLogonMessages ", CommandType.StoredProcedure))
            {
                var messageIdOrd = reader.GetOrdinal("MessageId");
                var categoryTitleOrd = reader.GetOrdinal("CategoryTitle");
                var categoryTitleColourCodeOrd = reader.GetOrdinal("CategoryTitleColourCode");
                var headerTextOrd = reader.GetOrdinal("HeaderText");
                var headerTextColourCodeOrd = reader.GetOrdinal("HeaderTextColourCode");
                var bodyTextOrd = reader.GetOrdinal("BodyText");
                var bodyTextColourCodeOrd = reader.GetOrdinal("BodyTextColourCode");
                var backgroundImageOrd = reader.GetOrdinal("BackgroundImage");
                var iconOrd = reader.GetOrdinal("Icon");
                var buttonTextOrd = reader.GetOrdinal("ButtonText");
                var buttonLinkOrd = reader.GetOrdinal("ButtonLink");
                var buttonForeColourOrd = reader.GetOrdinal("ButtonForeColour");
                var buttonBackGroundColourOrd = reader.GetOrdinal("ButtonBackGroundColour");
                var archivedOrd = reader.GetOrdinal("Archived");

                while (reader.Read())
                {
                    var messageId = reader.GetInt32(messageIdOrd);
                    var categoryTitle = !reader.IsDBNull(categoryTitleOrd) ? reader.GetString(categoryTitleOrd) : string.Empty;
                    var categoryTitleColourCode = !reader.IsDBNull(categoryTitleColourCodeOrd) ? reader.GetString(categoryTitleColourCodeOrd) : string.Empty;
                    var headerText = !reader.IsDBNull(headerTextOrd) ? reader.GetString(headerTextOrd) : string.Empty;
                    var headerTextColourCode = !reader.IsDBNull(headerTextColourCodeOrd) ? reader.GetString(headerTextColourCodeOrd) : string.Empty;
                    var bodyText = !reader.IsDBNull(bodyTextOrd) ? reader.GetString(bodyTextOrd) : string.Empty;
                    var bodyTextColourCode = !reader.IsDBNull(bodyTextColourCodeOrd) ? reader.GetString(bodyTextColourCodeOrd) : string.Empty;
                    var backgroundImage = !reader.IsDBNull(backgroundImageOrd) ? reader.GetString(backgroundImageOrd) : string.Empty;
                    var icon = !reader.IsDBNull(iconOrd) ? reader.GetString(iconOrd) : string.Empty;
                    var buttonText = !reader.IsDBNull(buttonTextOrd) ? reader.GetString(buttonTextOrd) : string.Empty;
                    var buttonLink = !reader.IsDBNull(buttonLinkOrd) ? reader.GetString(buttonLinkOrd) : string.Empty;
                    var buttonForeColour = !reader.IsDBNull(buttonForeColourOrd) ? reader.GetString(buttonForeColourOrd) : string.Empty;
                    var buttonBackgroundColour = !reader.IsDBNull(buttonBackGroundColourOrd) ? reader.GetString(buttonBackGroundColourOrd) : null;
                    var archived = !reader.IsDBNull(archivedOrd) && reader.GetBoolean(archivedOrd);
                    List<int> moduleIds = (from module in lstModules
                                           where module.MessageId == messageId
                                           select module.ModuleId).ToList();
                    logonMessageList.Add(messageId, new LogonMessage(messageId,
                        categoryTitle,
                        categoryTitleColourCode,
                        headerText,
                        headerTextColourCode,
                        bodyText,
                        bodyTextColourCode,
                        backgroundImage,
                        icon,
                        buttonText,
                        buttonLink,
                        buttonForeColour,
                        buttonBackgroundColour,
                        archived,
                        moduleIds));
                }
                reader.Close();
                this._cache.Add(0, CacheArea, "0", logonMessageList);
                return logonMessageList;
            }
        }

        /// <summary>
        /// Cache validation
        /// </summary>
        private void InvalidateCache()
        {
            this._cache.Delete(0, CacheArea, "0");
        }

        /// <summary>
        /// Initialise data.
        /// </summary>
        private void InitialiseData()
        {
            this._logonMessageList = this._cache.Get(0, CacheArea, "0") as Dictionary<int, LogonMessage> ?? this.CacheList();
        }

        /// <summary>
        ///     Get all information from moduleLicencesBase
        /// </summary>
        /// <returns></returns>
        public List<cMessageModules> GetMessageModules(List<cMessageModules> lsMessageModules)
        {
            using (var connection = new DatabaseConnection(GlobalVariables.MetabaseConnectionString))
            using (var reader = connection.GetReader("GetMessagesModules ", CommandType.StoredProcedure))
            {
                var moduleIdOrd = reader.GetOrdinal("moduleId");
                var messageIdOrd = reader.GetOrdinal("messageId");

                while (reader.Read())
                {
                    int moduleId = reader.GetInt32(moduleIdOrd);
                    int messageId = reader.GetInt32(messageIdOrd);
                    lsMessageModules.Add(new cMessageModules(moduleId, messageId));
                }

                reader.Close();
            }

            return lsMessageModules;
        }

    }
}