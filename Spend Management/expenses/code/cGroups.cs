namespace Spend_Management
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Enumerators;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    using ValidationException = SpendManagementLibrary.Exceptions.ValidationException;
    using Utilities.DistributedCaching;

    /// <summary>
    /// Summary description for groups.
    /// </summary>

    public class cGroups
    {
        string strsql;
        SortedList list;

        public int AccountId { get; private set; }

        /// <summary>
        /// The cache area.
        /// </summary>
        public const string CacheArea = "groups";

        /// <summary>
        /// The _Caching object .
        /// </summary>
        private readonly Cache _Caching = new Cache();

        public cGroups(int accountid)
        {
            AccountId = accountid;

            InitialiseData();
        }

        /// <summary>
        /// Exposes the internal (cached) list of cGroups, 
        /// initialising the list if necessary
        /// </summary>
        public SortedList groupList
        {
            get
            {
                if (this.list == null)
                {
                    this.InitialiseData();
                }

                return this.list;
            }
        }

        private void InitialiseData()
        {
            this.list = this._Caching.Get(this.AccountId, CacheArea, "0") as SortedList
                            ?? this.CacheList();
        }

        /// <summary>
        /// The invalidate cache.
        /// </summary>
        private void InvalidateCache()
        {
            this._Caching.Delete(this.AccountId, CacheArea, "0");
            this.list = null;
        }

        private SortedList CacheList()
        {
            SerializableDictionary<int, SerializableDictionary<int, cStage>> allStages = this.GetStages();
            var sortedList = new SortedList();

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
            {
                using (IDataReader reader = connection.GetReader("dbo.GetGroups", CommandType.StoredProcedure))
                {
                    connection.sqlexecute.Parameters.Clear();

                    while (reader.Read())
                    {
                        int groupid = reader.GetInt32(0);
                        string groupname = reader.GetString(2);
                        string description = reader.IsDBNull(1) == false ? reader.GetString(1) : string.Empty;
                        bool oneclickauthorisation = reader.GetBoolean(3);
                        DateTime createdon = reader.IsDBNull(reader.GetOrdinal("createdon"))
                                                    ? new DateTime(1900, 01, 01)
                                                    : reader.GetDateTime(reader.GetOrdinal("createdon"));
                        int createdby = reader.IsDBNull(reader.GetOrdinal("createdby"))
                                            ? 0
                                            : reader.GetInt32(reader.GetOrdinal("createdby"));
                        DateTime modifiedon = reader.IsDBNull(reader.GetOrdinal("modifiedon"))
                                                    ? new DateTime(1900, 01, 01)
                                                    : reader.GetDateTime(reader.GetOrdinal("modifiedon"));
                        int modifiedby = reader.IsDBNull(reader.GetOrdinal("modifiedby"))
                                                ? 0
                                                : reader.GetInt32(reader.GetOrdinal("modifiedby"));
                        bool? envReceived = reader.GetNullable<bool>("MailClaimantWhenEnvelopeReceived");
                        bool? envNotReceived = reader.GetNullable<bool>("MailClaimantWhenEnvelopeNotReceived");

                        SerializableDictionary<int, cStage> groupStages = allStages.ContainsKey(groupid)
                                                                                ? allStages[groupid]
                                                                                : new SerializableDictionary<int, cStage>();

                        var reqgroup = new cGroup(
                            this.AccountId,
                            groupid,
                            groupname,
                            description,
                            oneclickauthorisation,
                            createdon,
                            createdby,
                            modifiedon,
                            modifiedby,
                            groupStages,
                            envReceived,
                            envNotReceived);
                        sortedList.Add(groupid, reqgroup);
                    }

                    reader.Close();
                }
            }
            this._Caching.Add(this.AccountId, CacheArea, "0", sortedList);
            return sortedList;
        }

        public static string GetGridSQL()
        {
            return "SELECT groupid, groupname, description from dbo.groups order by groupname ASC";
        }

        public virtual int getCountOfClaimsInProcessByGroupID(int groupID)
        {
            DBConnection data = new DBConnection(cAccounts.getConnectionString(this.AccountId));
            string sql = "select count(*) from claims_base inner join employees on employees.employeeid = claims_base.employeeid inner join groups on groups.groupid = employees.groupid where claims_base.submitted = 1 and claims_base.paid = 0 and groups.groupid = @groupid";
            data.sqlexecute.Parameters.AddWithValue("@groupid", groupID);
            int count = data.getcount(sql);
            data.sqlexecute.Parameters.Clear();
            return count;
        }

        private bool alreadyExists(string groupname, int groupid, int action)
        {
            cGroup reqgroup;
            int i;

            for (i = 0; i < list.Count; i++)
            {
                reqgroup = (cGroup)list.GetByIndex(i);
                if (action == 0) //add
                {
                    if (reqgroup.groupname.ToLower().Trim() == groupname.ToLower().Trim())
                    {
                        return true;
                    }
                }
                else
                {
                    if (reqgroup.groupname.ToLower().Trim() == groupname.ToLower().Trim() && reqgroup.groupid != groupid)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// Saves a group.
        /// </summary>
        /// <param name="groupId">The Id of the group.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="description">The description of the group.</param>
        /// <param name="oneClickAuth">Whether the group has one click auth turned on.</param>
        /// <param name="currentUser">The current user.</param>
        /// <param name="notifyClaimantWhenEnvelopeReceived">Whether to notify the claimant when their envelope is received. Leave as null if the group has no Scan & Attach stage, or if the Account doesn't have ReceiptsServiceEnabled set.</param>
        /// <param name="notifyClaimantWhenEnvelopeNotReceived">Whether to notify the claimant when their envelope has not been received after the number of days configured in teir account. Leave as null if the group has no Scan & Attach stage, or if the Account doesn't have ReceiptsServiceEnabled set.</param>
        /// <returns>The result of the Save</returns>
        public int SaveGroup(int groupId, string groupName, string description, bool oneClickAuth, ICurrentUser currentUser, int action, bool? notifyClaimantWhenEnvelopeReceived = false, bool? notifyClaimantWhenEnvelopeNotReceived = false)
        {
            if (alreadyExists(groupName, groupId, action))
            {
                return -1;
            }

            using (var dbConnection = new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
            {
                dbConnection.AddWithValue("@groupName", groupName);
                dbConnection.AddWithValue("@oneClickAuth", oneClickAuth);
                dbConnection.AddWithValue("@groupId", groupId);
                dbConnection.AddWithValue("@mailClaimantWhenEnvelopeReceived", notifyClaimantWhenEnvelopeReceived);
                dbConnection.AddWithValue("@mailClaimantWhenEnvelopeNotReceived", notifyClaimantWhenEnvelopeNotReceived);
                dbConnection.AddWithValue("@modifiedOn", DateTime.Now.ToUniversalTime());
                dbConnection.AddWithValue("@modifiedBy", currentUser.EmployeeID);

                if (description != string.Empty)
                {
                    dbConnection.AddWithValue("@description", description.Length > 4000 ? description.Substring(0, 3999) : description);
                }
                else
                {
                    dbConnection.AddWithValue("@description", DBNull.Value);
                }

                if (currentUser.isDelegate == true)
                {
                    dbConnection.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    dbConnection.AddWithValue("@delegateID", DBNull.Value);
                }

                dbConnection.AddReturn("@identity", SqlDbType.Int);
                dbConnection.ExecuteProc("SaveSignOffGroup");
                var returnValue = dbConnection.GetReturnValue<int>("@identity");
                this.InvalidateCache();
                return returnValue;
            }
        }

        /// <summary>
        /// Attempts to delete a group
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="currentUser"></param>
        /// <param name="connection"></param>
        /// <returns>The result of the attempted sign off group deletion </returns>
        public int DeleteGroup(int groupId, ICurrentUser currentUser, IDBConnection connection = null)
        {
            var count = 0;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
            {
                var clsTables = new cTables(this.AccountId);
                var groupTableId = clsTables.GetTableByName("groups").TableID;

                expdata.sqlexecute.Parameters.AddWithValue("@currentTableID", groupTableId);
                expdata.sqlexecute.Parameters.AddWithValue("@currentRecordID", groupId);
                expdata.sqlexecute.Parameters.Add("@retCode", SqlDbType.Int);
                expdata.sqlexecute.Parameters["@retCode"].Direction = ParameterDirection.ReturnValue;
                expdata.ExecuteProc("checkReferencedBy");
                count = (int)expdata.sqlexecute.Parameters["@retCode"].Value;
                expdata.sqlexecute.Parameters.Clear();

                if (count == -10)
                {
                    return -10;
                }

                expdata.sqlexecute.Parameters.AddWithValue("@groupId", groupId);
                expdata.sqlexecute.Parameters.AddWithValue("@modifiedBy", currentUser.EmployeeID);

                if (currentUser.isDelegate == true)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }

                expdata.sqlexecute.Parameters.Add("@retCode", SqlDbType.Int);
                expdata.sqlexecute.Parameters["@retCode"].Direction = ParameterDirection.ReturnValue;
                expdata.ExecuteProc("DeleteSignOffGroup");
                count = (int)expdata.sqlexecute.Parameters["@retCode"].Value;
                expdata.sqlexecute.Parameters.Clear();
            }

            if (count == 2) //Return code is 2 for deletion failure for advance groups
            {
                return -2;
            }            

            if (count != 0)
            {
                return -1;
            }

            list.Remove(groupId);
            deleteStages(groupId);

            this.InvalidateCache();
            return groupId;
        }

        /// <summary>
        /// Get a group by its id.
        /// </summary>
        /// <param name="groupid">The groups Id</param>
        /// <returns>The <see cref="cGroup"/>.</returns>
        public virtual cGroup GetGroupById(int groupid)
        {
            return (cGroup)this.groupList[groupid];
        }

        public virtual cGroup getGroupByName(string name)
        {
            cGroup reqGroup = null;
            foreach (cGroup clsGroup in list.Values)
            {
                if (clsGroup.groupname == name)
                {
                    reqGroup = clsGroup;
                    break;
                }
            }
            return reqGroup;
        }

        private System.Collections.SortedList sortList()
        {
            System.Collections.SortedList orderedlst = new System.Collections.SortedList();
            cGroup reqgroup;
            int i;
            for (i = 0; i < list.Count; i++)
            {
                reqgroup = (cGroup)list.GetByIndex(i);
                orderedlst.Add(reqgroup.groupname, reqgroup);
            }
            return orderedlst;
        }
        public System.Web.UI.WebControls.ListItem[] CreateDropDown(int groupid)
        {
            System.Collections.SortedList orderedlst = sortList();
            System.Web.UI.WebControls.ListItem[] tempItems = new System.Web.UI.WebControls.ListItem[list.Count + 1];
            cGroup reqgroup;
            int i = 0;
            tempItems[0] = new System.Web.UI.WebControls.ListItem();
            tempItems[0].Text = "[None]";
            tempItems[0].Value = "";
            for (i = 0; i < list.Count; i++)
            {
                reqgroup = (cGroup)orderedlst.GetByIndex(i);
                tempItems[i + 1] = new System.Web.UI.WebControls.ListItem();
                tempItems[i + 1].Text = reqgroup.groupname;
                tempItems[i + 1].Value = reqgroup.groupid.ToString();
                if (groupid == reqgroup.groupid)
                {
                    tempItems[i + 1].Selected = true;
                }
            }
            return tempItems;
        }

        #region stage functions

        private SerializableDictionary<int, SerializableDictionary<int, cStage>> GetStages()
        {
            SerializableDictionary<int, SerializableDictionary<int, cStage>> allStages;
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
            {
                allStages = new SerializableDictionary<int, SerializableDictionary<int, cStage>>();

                using (var reader = connection.GetReader("GetSignoffStages", CommandType.StoredProcedure))
                {
                    #region Ordinals

                    int groupid_ord = reader.GetOrdinal("groupid");
                    int amount_ord = reader.GetOrdinal("amount");
                    int holidayid_ord = reader.GetOrdinal("holidayid");
                    int holidaytype_ord = reader.GetOrdinal("holidaytype");
                    int include_ord = reader.GetOrdinal("include");
                    int notify_ord = reader.GetOrdinal("notify");
                    int onholiday_ord = reader.GetOrdinal("onholiday");
                    int relid_ord = reader.GetOrdinal("relid");
                    int extraApprovalLevels_ord = reader.GetOrdinal("extraApprovalLevels");
                    int signoffid_ord = reader.GetOrdinal("signoffid");
                    int signofftype_ord = reader.GetOrdinal("signofftype");
                    int stage_ord = reader.GetOrdinal("stage");
                    int claimantemail_ord = reader.GetOrdinal("claimantmail");
                    int includeid_ord = reader.GetOrdinal("includeid");
                    int singlesignoff_ord = reader.GetOrdinal("singlesignoff");
                    int sendmail_ord = reader.GetOrdinal("sendmail");
                    int displaydeclaration_ord = reader.GetOrdinal("displaydeclaration");
                    int createdon_ord = reader.GetOrdinal("createdon");
                    int createdby_ord = reader.GetOrdinal("createdby");
                    int modifiedon_ord = reader.GetOrdinal("modifiedon");
                    int modifiedby_ord = reader.GetOrdinal("modifiedby");
                    int approveHigherLevelsOnly_ord = reader.GetOrdinal("approveHigherLevelsOnly");
                    int nhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner_ord =
                        reader.GetOrdinal("nhsassignmentsupervisorapproveswhenmissingcostcodeowner");
                    int approverJustificationRequired_ord = reader.GetOrdinal("approverJustificationsRequired");
                    int claimPercentageToValidate_ord = reader.GetOrdinal("claimPercentageToValidate");
                    #endregion Ordinals

                    while (reader.Read())
                    {
                        int groupid = reader.GetInt32(groupid_ord);
                        decimal amount = reader.GetDecimal(amount_ord);
                        int holidayid = reader.GetInt32(holidayid_ord);
                        SignoffType holidaytype = (SignoffType)reader.GetInt32(holidaytype_ord);
                        StageInclusionType include = (StageInclusionType)reader.GetInt32(include_ord);
                        int notify = reader.GetInt32(notify_ord);
                        byte onholiday = reader.GetByte(onholiday_ord);
                        int relid = reader.GetInt32(relid_ord);
                        int extraApprovalLevels = reader.GetInt32(extraApprovalLevels_ord);
                        int signoffid = reader.GetInt32(signoffid_ord);
                        SignoffType signofftype = (SignoffType)reader.GetByte(signofftype_ord);
                        byte stage = reader.GetByte(stage_ord);
                        bool claimantmail = reader.GetBoolean(claimantemail_ord);
                        int includeid = !reader.IsDBNull(includeid_ord) ? reader.GetInt32(includeid_ord) : 0;
                        bool singlesignoff = reader.GetBoolean(singlesignoff_ord);
                        bool sendmail = reader.GetBoolean(sendmail_ord);
                        bool displaydeclaration = reader.GetBoolean(displaydeclaration_ord);
                        DateTime createdon = reader.IsDBNull(createdon_ord) ? new DateTime(1900, 01, 01) : reader.GetDateTime(createdon_ord);
                        int createdby = reader.IsDBNull(createdby_ord) ? 0 : reader.GetInt32(createdby_ord);
                        DateTime modifiedon = reader.IsDBNull(modifiedon_ord) ? new DateTime(1900, 01, 01) : reader.GetDateTime(modifiedon_ord);
                        int modifiedby = reader.IsDBNull(modifiedby_ord) ? 0 : reader.GetInt32(modifiedby_ord);
                        bool approveHigherLevelsOnly = reader.GetBoolean(approveHigherLevelsOnly_ord);
                        bool nhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner = reader.GetBoolean(nhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner_ord);
                        bool approverJustificationRequired = reader.GetBoolean(approverJustificationRequired_ord);
                        
                        var reqstage = new cStage(
                            signoffid,
                            signofftype,
                            relid,
                            extraApprovalLevels,
                            include,
                            amount,
                            notify,
                            stage,
                            onholiday,
                            holidaytype,
                            holidayid,
                            includeid,
                            claimantmail,
                            singlesignoff,
                            sendmail,
                            displaydeclaration,
                            createdon,
                            createdby,
                            modifiedon,
                            modifiedby,
                            approveHigherLevelsOnly,
                            approverJustificationRequired,
                            nhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner,
                            reader.GetRequiredValue<bool>("AllocateForPayment"),
                            reader.GetRequiredValue<bool>("IsPostValidationCleanupStage"),
                                reader.GetNullable<int>("ValidationCorrectionThreshold"),
                                reader.GetNullable<decimal>("ClaimPercentageToValidate")); //TODO: Feature Flag

                        if (!allStages.ContainsKey(groupid))
                        {
                            allStages.Add(groupid, new SerializableDictionary<int, cStage>());
                        }

                        allStages[groupid].Add(signoffid, reqstage);
                    }
                    reader.Close();
                }
            }

            return allStages;
        }

        /// <summary>
        /// Get a sign off group stage by it's Id
        /// </summary>
        /// <param name="signOffId">The Id of the stage</param>
        /// <returns>An instance of a <see cref="cStage"/></returns>
        public cStage GetStageById(int signOffId)
        {
            // Check cache is populated before finding stage
            if (this.list == null)
            {
                this.InitialiseData();
            }

            return this.list.Values
                .OfType<cGroup>()
                .Where(g => g.stages.ContainsKey(signOffId))
                .Select(g => g.stages[signOffId])
                .FirstOrDefault();
        }

        public SortedList sortStages(cGroup grp)
        {
            SortedList sorted = new SortedList();

            foreach (cStage stage in grp.stages.Values)
            {
                sorted.Add(stage.stage, stage);
            }
            return sorted;
        }

        public System.Data.DataSet getStagesGrid(cGroup grp)
        {
            SortedList sorted = sortStages(grp);
            System.Data.DataSet ds = new System.Data.DataSet();
            System.Data.DataTable tbl = new System.Data.DataTable();
            cStage reqstage;
            object[] values;
            int i;

            tbl.Columns.Add("signoffid", System.Type.GetType("System.Int32"));
            tbl.Columns.Add("stage", System.Type.GetType("System.Byte"));
            tbl.Columns.Add("signofftype", System.Type.GetType("System.Byte"));
            tbl.Columns.Add("relid", System.Type.GetType("System.Int32"));
            tbl.Columns.Add("include", System.Type.GetType("System.Int32"));
            tbl.Columns.Add("notify", System.Type.GetType("System.Int32"));
            tbl.Columns.Add("amount", System.Type.GetType("System.Decimal"));
            tbl.Columns.Add("cleanupstage", System.Type.GetType("System.Boolean"));
            tbl.Columns.Add("allocateforpayment", System.Type.GetType("System.Boolean"));

            for (i = 0; i < sorted.Count; i++)
            {
                reqstage = (cStage)sorted.GetByIndex(i);
                values = new object[9];
                values[0] = reqstage.signoffid;
                values[1] = reqstage.stage;
                values[2] = (byte)reqstage.signofftype;
                values[3] = reqstage.relid;
                values[4] = reqstage.include;
                values[5] = reqstage.notify;
                values[6] = reqstage.amount;
                values[7] = reqstage.IsPostValidationCleanupStage;
                values[8] = reqstage.AllocateForPayment;
                tbl.Rows.Add(values);
            }
            ds.Tables.Add(tbl);
            return ds;

        }

        /// <summary>
        /// Validates the stages of a group.
        /// </summary>
        /// <param name="group">The group to check the stages for.</param>
        /// <returns></returns>
        public GroupStageValidationResult ValidateGroupStages(cGroup group)
        {
            var validation = new GroupStageValidationResult();

            if (group == null)
            {
                validation.Result = false;
                validation.Messages.Add(GroupStageValidationResult.NotFound);
                return validation;
            }

            // check for stages.
            if (group.stages.Count == 0)
            {
                validation.Result = false;
                validation.Messages.Add(GroupStageValidationResult.NoStages);
                return validation;
            }

            List<cStage> stages = group.stages.Values.OrderBy(s => s.stage).ToList();
            cStage stage;
            int selCount;
            var account = new cAccounts().GetAccountByID(this.AccountId);

            this.PayBeforeValidateValidation(stages, validation);

            // check for any Expedite signofftypes in this stage.
            List<SignoffType> stageSignoffTypes = stages.Select(s => s.signofftype).ToList();

            // if the account has Expedite:
            if (account.ReceiptServiceEnabled)
            {
                // create the list of types that are allowed to be the next stage.
                var allowedNextStageTypes = new List<SignoffType>
                {
                    SignoffType.AssignmentSignOffOwner,
                    SignoffType.CostCodeOwner,
                    SignoffType.LineManager
                };

                // get the number of scan and attach stages - there can only be one.
                selCount = stageSignoffTypes.Count(s => s == SignoffType.SELScanAttach);

                if (selCount > 1)
                {
                    validation.Result = false;
                    validation.Messages.Add(GroupStageValidationResult.OnlyOneScanAttachStagePermitted);
                }

                if (account.ValidationServiceEnabled)
                {
                    // get the number of validation stages - there can only be one.
                    selCount = stageSignoffTypes.Count(s => s == SignoffType.SELValidation);

                    if (selCount > 0)
                    {

                        if (selCount > 1)
                        {
                            validation.Result = false;
                            validation.Messages.Add(GroupStageValidationResult.OnlyOneValidationStagePermitted);
                        }

                        // the next stage must have user input or be allowed.
                        var index = stageSignoffTypes.IndexOf(SignoffType.SELValidation);
                        stage = stages.ElementAtOrDefault(index + 1);
                        if (stage == null || stage.notify != 2 || (stage.relid == 0 && !allowedNextStageTypes.Contains(stage.signofftype)))
                        {
                            validation.Result = false;
                            validation.Messages.Add(GroupStageValidationResult.ValidationMustBeFollowedByUserInteration);
                        }
                    }
                }
            }

            // validate last stage...
            stage = stages.Last();


            if (stage.signofftype == SignoffType.SELScanAttach || stage.signofftype == SignoffType.SELValidation)
            {
                validation.Result = false;
                validation.Messages.Add(GroupStageValidationResult.SELStageMustNotBeTheLastStage);
            }

            if (stage.include != StageInclusionType.Always)
            {
                validation.Result = false;
                validation.Messages.Add(GroupStageValidationResult.WhenToIncludeMustBeSetToAlways);
            }
            if (stage.notify != 2)
            {
                validation.Result = false;
                validation.Messages.Add(GroupStageValidationResult.InvolvementLastStageMustBeSetToUserCheck);
            }
            if (stage.onholiday == 2)
            {
                validation.Result = false;
                validation.Messages.Add(GroupStageValidationResult.LastStageCannotBeSkippedIfUserOnHoliday);
            }

            // set the message if the result is good.
            if (validation.Result)
            {
                validation.Messages = new List<string> { GroupStageValidationResult.Valid };
            }

            return validation;
        }

        private void PayBeforeValidateValidation(List<cStage> stages, GroupStageValidationResult validation)
        {
            var allocateForPaymentStages = stages.Where(s => s.AllocateForPayment).ToList();
            var postValidationCleanupStages = stages.Where(s => s.IsPostValidationCleanupStage).ToList();

            if (!allocateForPaymentStages.Any() && !postValidationCleanupStages.Any())
            {
                // Pay before Validation is not used
                return;
            }

            try
            {
                if (
                    (!allocateForPaymentStages.Any() && postValidationCleanupStages.Any()) ||
                    (!postValidationCleanupStages.Any() && allocateForPaymentStages.Any())
                )
                {
                    // One of the two required stages is missing
                    throw new ValidationException(GroupStageValidationResult.PostValidationCleanupStageMustBeAfterAllocateForPayment);
                }

                if (allocateForPaymentStages.Count > 1)
                {
                    // There are multiple Allocate for Payment stages
                    throw new ValidationException(GroupStageValidationResult.OnlyOneAllocateForpaymentStagePermitted);
                }

                if (postValidationCleanupStages.Count > 1)
                {
                    // There are multiple Post-Validation Cleanup stages
                    throw new ValidationException(GroupStageValidationResult.OnlyOnePostValidationCleanupStagePermitted);
                }

                // Only one of each stage exists in the group
                cStage allocateForPaymentStage = allocateForPaymentStages[0];
                cStage postValidationCleanupStage = postValidationCleanupStages[0];

                if (stages.Any(s => s.signofftype == SignoffType.SELValidation && s.stage < allocateForPaymentStage.stage))
                {
                    throw new ValidationException(GroupStageValidationResult.AllocateForPaymentCannotBeAfterValidation);
                }

                if (!stages.Any(s => s.signofftype == SignoffType.SELValidation))
                {
                    // The Validation stage has not been included
                    throw new ValidationException(GroupStageValidationResult.ValidationIsRequired);
                }

                if (stages.Last() != postValidationCleanupStage)
                {
                    // The Post-Validation Cleanup stage is not last
                    throw new ValidationException(GroupStageValidationResult.PostValidationCleanupStageMustBeLast);
                }

                if (postValidationCleanupStage.signofftype == SignoffType.None)
                {
                    // The Post-Validation Cleanup stage is not configured
                    throw new ValidationException(GroupStageValidationResult.PostValidationCleanupStageMustBeconfigured);
                }

                if (stages.Any(s => s.AllocateForPayment && s.onholiday == 2))
                {
                    // The Allocate for Payment stage is skipped if on holiday
                    throw new ValidationException(GroupStageValidationResult.CannotSkipAllocateForPayment);
                }

                if (stages.Any(s => s.IsPostValidationCleanupStage && s.onholiday == 2))
                {
                    // The Post-Validation Cleanup stage is skipped if on holiday
                    throw new ValidationException(GroupStageValidationResult.CannotSkipPostValidationCleanupStage);
                }

                // Remove the Post-Validation Cleanup stage as it may interfere with other sign-off validations
                stages.Remove(postValidationCleanupStage);

            }
            catch (ValidationException ex)
            {
                validation.Result = false;
                validation.Messages.Add(ex.Message);
            }
        }

        public int checkLastStage(cGroup grp)
        {
            SortedList sorted = sortStages(grp);
            cStage reqstage;

            if (sorted.Count == 0)
            {
                return 0;
            }
            reqstage = (cStage)sorted.GetByIndex(sorted.Count - 1);

            if (reqstage.include != StageInclusionType.Always)
            {
                return 1;
            }
            if (reqstage.notify != 2)
            {
                return 2;
            }
            if (reqstage.onholiday == 2)
            {
                return 3;
            }

            return 0;
        }

        public void deleteStages(int groupid)
        {
            string strsql;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.AccountId));
            expdata.sqlexecute.Parameters.AddWithValue("@groupid", groupid);
            strsql = "delete from signoffs where groupid = @groupid";
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
            this.InvalidateCache();
        }

        /// <summary>
        /// Update a <see cref="cStage"/>
        /// </summary>
        /// <param name="signofftype">The signoff type of the <see cref="cStage"/></param>
        /// <param name="relid">The relid of the <see cref="cStage"/></param>
        /// <param name="include">The include type of the <see cref="cStage"/></param>
        /// <param name="amount">The amount of the <see cref="cStage"/></param>
        /// <param name="notify">The notify type of the <see cref="cStage"/></param>
        /// <param name="onholiday">The onholiday state of the <see cref="cStage"/></param>
        /// <param name="holidaytype">The holiday type of the <see cref="cStage"/></param>
        /// <param name="holidayid">The holiday id of the <see cref="cStage"/></param>
        /// <param name="includeid">The include id of the <see cref="cStage"/></param>
        /// <param name="claimantmail">The claimant mail state of the <see cref="cStage"/></param>
        /// <param name="singlesignoff">The single signoff state of the <see cref="cStage"/></param>
        /// <param name="sendmail">The approver mail state of the <see cref="cStage"/></param>
        /// <param name="displaydeclaration">The display declaration state of the <see cref="cStage"/></param>
        /// <param name="userid">The user id of the <see cref="cStage"/></param>
        /// <param name="signoffid">The signoff id of the <see cref="cStage"/></param>
        /// <param name="extraApprovalLevels">The extra approver levels of the <see cref="cStage"/></param>
        /// <param name="approveHigherLevelsOnly">The approver higher levels state of the <see cref="cStage"/></param>
        /// <param name="approverJustificationsRequired">The approver justification state of the <see cref="cStage"/></param>
        /// <param name="nhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner">The no cost code owner state of the <see cref="cStage"/></param>
        /// <param name="allocateForPayment">The allocate for payment state of the <see cref="cStage"/></param>
        /// <param name="validationCorrectionThreshold">The validation threshold of the <see cref="cStage"/></param>
        /// <param name="claimPercentageToValidate">The percentage of item in a claim sent for validation of the <see cref="cStage"/></param>
        /// <returns>The new id of the stage</returns>
        public int updateStage(int signoffid, SignoffType signofftype, int relid, int include, decimal amount, int notify, int onholiday, SignoffType holidaytype, int holidayid, int includeid, bool claimantmail, bool singlesignoff, bool sendmail, bool displaydeclaration, int userid, int extraApprovalLevels, bool approveHigherLevelsOnly, bool approverJustificationsRequired, bool nhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner, bool allocateForPayment, int? validationCorrectionThreshold, decimal? claimPercentageToValidate) //TODO: Feature flag
        {
            var currentStage = this.GetStageById(signoffid);
            if (currentStage.AllocateForPayment && !allocateForPayment)
            {
                var group = currentStage.GetGroup(this.list);
                var postValidationCleanupStage = group.stages.Values.FirstOrDefault(s => s.IsPostValidationCleanupStage);
                if (postValidationCleanupStage != null)
                {
                    this.deleteStage(group, postValidationCleanupStage.signoffid, true);
                }
            }

            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
            {

                expdata.sqlexecute.Parameters.AddWithValue("@signoffid", signoffid);
                expdata.sqlexecute.Parameters.AddWithValue("@signofftype", (byte) signofftype);
                expdata.sqlexecute.Parameters.AddWithValue("@relid", relid);
                expdata.sqlexecute.Parameters.AddWithValue("@extraApprovalLevels", extraApprovalLevels);
                expdata.sqlexecute.Parameters.AddWithValue("@include", include);
                expdata.sqlexecute.Parameters.AddWithValue("@amount", amount);
                expdata.sqlexecute.Parameters.AddWithValue("@notify", notify);
                expdata.sqlexecute.Parameters.AddWithValue("@onholiday", onholiday);
                expdata.sqlexecute.Parameters.AddWithValue("@holidaytype", (byte) holidaytype);
                expdata.sqlexecute.Parameters.AddWithValue("@holidayid", holidayid);
                expdata.sqlexecute.Parameters.AddWithValue("@claimantmail", Convert.ToByte(claimantmail));
                expdata.sqlexecute.Parameters.AddWithValue("@singlesignoff", Convert.ToByte(singlesignoff));
                if (includeid == 0)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@includeid", DBNull.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@includeid", includeid);
                }

                expdata.sqlexecute.Parameters.AddWithValue("@sendmail", Convert.ToByte(sendmail));
                expdata.sqlexecute.Parameters.AddWithValue("@displaydeclaration", Convert.ToByte(displaydeclaration));
                expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());
                expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", userid);
                expdata.sqlexecute.Parameters.AddWithValue("@approveHigherLevelsOnly", approveHigherLevelsOnly);
                expdata.sqlexecute.Parameters.AddWithValue("@nhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner",
                    nhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner);
                expdata.sqlexecute.Parameters.AddWithValue("@AllocateForPayment", allocateForPayment);
                expdata.sqlexecute.Parameters.AddWithValue("@ValidationCorrectionThreshold",
                    (object) validationCorrectionThreshold ?? DBNull.Value);
                expdata.sqlexecute.Parameters.AddWithValue("@approverJustificationsRequired",
                    Convert.ToByte(approverJustificationsRequired));
                expdata.sqlexecute.Parameters.AddWithValue("@ClaimPercentageToValidate", (object) claimPercentageToValidate ?? DBNull.Value); //TODO: Feature flag
                
                expdata.ExecuteProc("SaveSignoffStage");
                expdata.sqlexecute.Parameters.Clear();
            }

            this.InvalidateCache();
            return 0;
        }

        /// <summary>
        /// Save a <see cref="cStage"/>
        /// </summary>
        /// <param name="groupid">The id of the <see cref="cGroup"/></param>
        /// <param name="signofftype">The signoff type of the <see cref="cStage"/></param>
        /// <param name="relid">The relid of the <see cref="cStage"/></param>
        /// <param name="include">The include type of the <see cref="cStage"/></param>
        /// <param name="amount">The amount of the <see cref="cStage"/></param>
        /// <param name="notify">The notify type of the <see cref="cStage"/></param>
        /// <param name="onholiday">The onholiday state of the <see cref="cStage"/></param>
        /// <param name="holidaytype">The holiday type of the <see cref="cStage"/></param>
        /// <param name="holidayid">The holiday id of the <see cref="cStage"/></param>
        /// <param name="includeid">The include id of the <see cref="cStage"/></param>
        /// <param name="claimantmail">The claimant mail state of the <see cref="cStage"/></param>
        /// <param name="singlesignoff">The single signoff state of the <see cref="cStage"/></param>
        /// <param name="sendmail">The approver mail state of the <see cref="cStage"/></param>
        /// <param name="displaydeclaration">The display declaration state of the <see cref="cStage"/></param>
        /// <param name="userid">The user id of the <see cref="cStage"/></param>
        /// <param name="signoffid">The signoff id of the <see cref="cStage"/></param>
        /// <param name="offline">Offline state</param>
        /// <param name="extraApprovalLevels">The extra approver levels of the <see cref="cStage"/></param>
        /// <param name="approveHigherLevelsOnly">The approver higher levels state of the <see cref="cStage"/></param>
        /// <param name="approverJustificationsRequired">The approver justification state of the <see cref="cStage"/></param>
        /// <param name="nhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner">The no cost code owner state of the <see cref="cStage"/></param>
        /// <param name="allocateForPayment">The allocate for payment state of the <see cref="cStage"/></param>
        /// <param name="isPostValidationCleanupStage">The cleanup stage state of the <see cref="cStage"/></param>
        /// <param name="validationCorrectionThreshold">The validation threshold of the <see cref="cStage"/></param>
        /// <returns>The new id of the stage</returns>
        public int addStage(int groupid, SignoffType signofftype, int relid, int include, decimal amount, int notify, int onholiday, SignoffType holidaytype, int holidayid, int includeid, bool claimantmail, bool singlesignoff, bool sendmail, bool displaydeclaration, int userid, int signoffid, bool offline, int extraApprovalLevels, bool approveHigherLevelsOnly, bool approverJustificationsRequired, bool nhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner, bool allocateForPayment, bool isPostValidationCleanupStage, int? validationCorrectionThreshold, decimal? claimPercentageToValidate) //tODO: Feature flag
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.AccountId));

            expdata.sqlexecute.Parameters.AddWithValue("@signofftype", (byte)signofftype);
            expdata.sqlexecute.Parameters.AddWithValue("@relid", relid);
            expdata.sqlexecute.Parameters.AddWithValue("@extraApprovalLevels", extraApprovalLevels);
            expdata.sqlexecute.Parameters.AddWithValue("@include", include);
            expdata.sqlexecute.Parameters.AddWithValue("@amount", amount);
            expdata.sqlexecute.Parameters.AddWithValue("@notify", notify);
            expdata.sqlexecute.Parameters.AddWithValue("@onholiday", onholiday);
            expdata.sqlexecute.Parameters.AddWithValue("@holidaytype", (byte)holidaytype);
            expdata.sqlexecute.Parameters.AddWithValue("@holidayid", holidayid);
            expdata.sqlexecute.Parameters.AddWithValue("@claimantmail", Convert.ToByte(claimantmail));
            if (includeid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@includeid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@includeid", includeid);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@sendmail", Convert.ToByte(sendmail));
            expdata.sqlexecute.Parameters.AddWithValue("@singlesignoff", Convert.ToByte(singlesignoff));
            expdata.sqlexecute.Parameters.AddWithValue("@groupid", groupid);
            expdata.sqlexecute.Parameters.AddWithValue("@displaydeclaration", displaydeclaration);
            expdata.sqlexecute.Parameters.AddWithValue("@createdon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@createdby", userid);
            expdata.sqlexecute.Parameters.AddWithValue("@approveHigherLevelsOnly", approveHigherLevelsOnly);
            expdata.sqlexecute.Parameters.AddWithValue("@nhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner", nhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner);
            expdata.sqlexecute.Parameters.AddWithValue("@AllocateForPayment", allocateForPayment);
            expdata.sqlexecute.Parameters.AddWithValue("@IsPostValidationCleanupStage", isPostValidationCleanupStage);
            expdata.sqlexecute.Parameters.AddWithValue("@ValidationCorrectionThreshold", (object)validationCorrectionThreshold ?? DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@approverJustificationsRequired", Convert.ToByte(approverJustificationsRequired));
            expdata.sqlexecute.Parameters.AddWithValue("@ClaimPercentageToValidate", (object)claimPercentageToValidate ?? DBNull.Value); //TODO: Feature flag

            expdata.sqlexecute.Parameters.AddWithValue("@stage", Byte.MaxValue);

            expdata.sqlexecute.Parameters.AddWithValue("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;

            expdata.ExecuteProc("SaveSignoffStage");
            var returnVal = (int)expdata.sqlexecute.Parameters["@identity"].Value;
            expdata.sqlexecute.Parameters.Clear();
            this.InvalidateCache();

            return returnVal;
        }

        public byte deleteStage(cGroup grp, int signoffId, bool systemAction = false)
        {
            if (!grp.stages.ContainsKey(signoffId))
            {
                return 0;
            }

            cStage stage = grp.stages[signoffId];

            if (stage.IsPostValidationCleanupStage && !systemAction)
            {
                throw new Exception("You cannot delete the post-validation verification stage.");
            }

            var stageNo = stage.stage;
            var isAllocateForPayment = stage.AllocateForPayment;

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
            {
                connection.AddWithValue("@signoffid", signoffId);
                connection.ExecuteProc("dbo.DeleteSignoffStage");
            }

            if (isAllocateForPayment)
            {
                return this.deleteStage(grp, grp.stages.Values.FirstOrDefault(s => s.IsPostValidationCleanupStage).signoffid, true);
            }

            this.InvalidateCache();

            return stageNo;

        }

        #endregion

        /// <summary>
        /// Check to see if a particular user would be in the signoff group as an employee signoff stage, a budgetholder or a holiday delegate
        /// </summary>
        /// <param name="groupID">The signoff group to check</param>
        /// <param name="checkEmployeeID">The employee to check the group stages for (this may be a delegated employee)</param>
        /// <returns>True or false</returns>
        public bool EmployeeInSignoffGroup(int groupID, int checkEmployeeID)
        {
            bool isPresentInGroup = false;
            cGroup group = GetGroupById(groupID);
            cBudgetholders clsBudgetHolders = new cBudgetholders(this.AccountId);
            cBudgetHolder oBudgetHolder = null;
            cEmployees clsEmployees = new cEmployees(this.AccountId);
            Employee oEmployee = null;

            bool employeeOnHoliday = clsEmployees.userIsOnHoliday(checkEmployeeID);

            if (group != null && group.stagecount > 0)
            {
                foreach (cStage oStage in group.stages.Values)
                {
                    #region check signoff for this stage
                    if (oStage.notify == 2) // notify 2 is to check claim
                    {
                        switch (oStage.signofftype)
                        {
                            case SignoffType.BudgetHolder:
                                // check budget holder
                                oBudgetHolder = clsBudgetHolders.getBudgetHolderById(oStage.relid);
                                if (oBudgetHolder != null && oBudgetHolder.employeeid == checkEmployeeID)
                                {
                                    return true;
                                }
                                break;

                            case SignoffType.Employee:
                                // check employee
                                if (checkEmployeeID == oStage.relid)
                                {
                                    return true;
                                }
                                break;

                            case SignoffType.Team:
                                // check team
                                // We are currently not checking team membership
                                break;

                            case SignoffType.LineManager:
                                // check line manager
                                oEmployee = clsEmployees.GetEmployeeById(checkEmployeeID);
                                if (oEmployee != null && oEmployee.LineManager == checkEmployeeID)
                                {
                                    return true;
                                }
                                break;

                            case SignoffType.ClaimantSelectsOwnChecker:
                            case SignoffType.DeterminedByClaimantFromApprovalMatrix:
                            case SignoffType.CostCodeOwner:
                                // choose signoff - can't do much to check here
                                break;
                        }
                    }
                    #endregion

                    #region check holiday behaviour

                    if (oStage.onholiday == 3 && oStage.notify == 2 && employeeOnHoliday) // 3 should be to send to a holiday approver and notify 2 is check claim
                    {
                        // check holiday signoff
                        switch (oStage.holidaytype)
                        {
                            case SignoffType.BudgetHolder:
                                // check budget holder
                                oBudgetHolder = clsBudgetHolders.getBudgetHolderById(oStage.holidayid);
                                if (oBudgetHolder != null && oBudgetHolder.employeeid == checkEmployeeID)
                                {
                                    return true;
                                }
                                break;

                            case SignoffType.Employee:
                                // check employee
                                if (checkEmployeeID == oStage.holidayid)
                                {
                                    return true;
                                }
                                break;

                            case SignoffType.Team:
                                // check team
                                // We are currently not checking team membership
                                break;

                            case SignoffType.LineManager:
                                // check line manager
                                oEmployee = clsEmployees.GetEmployeeById(checkEmployeeID);
                                if (oEmployee != null && oEmployee.LineManager == checkEmployeeID)
                                {
                                    return true;
                                }
                                break;
                            case SignoffType.ClaimantSelectsOwnChecker:
                            case SignoffType.DeterminedByClaimantFromApprovalMatrix:
                                // choose signoff - can't do much to check here
                                break;

                            default:
                                break;
                        }
                    }
                    #endregion
                }
            }
            return isPresentInGroup;
        }

        /// <summary>
        /// Check to see if a particular user would be in the signoff group as an employee signoff stage, a budgetholder or a holiday delegate
        /// </summary>
        /// <param name="groupID">The signoff group to check</param>
        /// <param name="checkEmployeeID">The employee to check the group stages for (this may be a delegated employee)</param>
        /// <returns>True or false</returns>
        public bool ClaimantSubmittingClaimInSignoffGroup(cGroup group, Employee oEmployee, cBudgetholders clsBudgetHolders, cEmployees clsEmployees)
        {
            bool isPresentInGroup = false;
            
            
            cBudgetHolder oBudgetHolder = null;
            
            bool checkerOnHoliday;
            if (group != null && group.stagecount > 0)
            {
                foreach (cStage oStage in group.stages.Values)
                {
                    checkerOnHoliday = false;
                    #region check signoff for this stage
                    if (oStage.notify == 2) // notify 2 is to check claim
                    {
                        switch (oStage.signofftype)
                        {
                            case SignoffType.BudgetHolder:
                                // check budget holder
                                oBudgetHolder = clsBudgetHolders.getBudgetHolderById(oStage.relid);
                                checkerOnHoliday = clsEmployees.userIsOnHoliday(oBudgetHolder.employeeid);
                                if (oBudgetHolder != null && oBudgetHolder.employeeid == oEmployee.EmployeeID)
                                {
                                    return true;
                                }
                                break;

                            case SignoffType.Employee:
                                // check employee
                                checkerOnHoliday = clsEmployees.userIsOnHoliday(oStage.relid);
                                if (oEmployee.EmployeeID == oStage.relid)
                                {
                                    return true;
                                }
                                break;

                            case SignoffType.Team:
                                // check team
                                // We are currently not checking team membership
                                break;

                            case SignoffType.LineManager:
                                // check line manager
                                
                                if (oEmployee.LineManager > 0)
                                {
                                    checkerOnHoliday = clsEmployees.userIsOnHoliday(oEmployee.LineManager);
                                }
                                if (oEmployee != null && oEmployee.LineManager == oEmployee.EmployeeID)
                                {
                                    return true;
                                }
                                break;

                            case SignoffType.ClaimantSelectsOwnChecker:
                            case SignoffType.DeterminedByClaimantFromApprovalMatrix:
                            case SignoffType.CostCodeOwner:
                            case SignoffType.AssignmentSignOffOwner:
                                // choose signoff - can't do much to check here
                                break;

                            default:
                                break;
                        }
                    }
                    #endregion

                    #region check holiday behaviour

                    if (oStage.onholiday == 3 && oStage.notify == 2 && checkerOnHoliday) // 3 should be to send to a holiday approver and notify 2 is check claim
                    {
                        // check holiday signoff
                        switch (oStage.holidaytype)
                        {
                            case SignoffType.BudgetHolder:
                                // check budget holder
                                oBudgetHolder = clsBudgetHolders.getBudgetHolderById(oStage.holidayid);
                                if (oBudgetHolder != null && oBudgetHolder.employeeid == oEmployee.EmployeeID)
                                {
                                    return true;
                                }
                                break;

                            case SignoffType.Employee:
                                // check employee
                                if (oEmployee.EmployeeID == oStage.holidayid)
                                {
                                    return true;
                                }
                                break;

                            case SignoffType.Team:
                                // check team
                                // We are currently not checking team membership
                                break;

                            case SignoffType.LineManager:
                                // check line manager
                                
                                if (oEmployee != null && oEmployee.LineManager == oEmployee.EmployeeID)
                                {
                                    return true;
                                }
                                break;

                            case SignoffType.ClaimantSelectsOwnChecker:
                            case SignoffType.DeterminedByClaimantFromApprovalMatrix:
                            case SignoffType.CostCodeOwner:
                            case SignoffType.AssignmentSignOffOwner:
                                // choose signoff - can't do much to check here
                                break;

                            default:
                                break;
                        }
                    }
                    #endregion
                }
            }
            return isPresentInGroup;
        }
    }


    /// <summary>
    /// Represents the result of validating the stages in a group.
    /// </summary>
    public class GroupStageValidationResult
    {
        /// <summary>
        /// Stages in the group are valid.
        /// </summary>
        public const string Valid = "The stages in this Signoff group are valid.";

        /// <summary>
        /// Group not found.
        /// </summary>
        public const string NotFound = "Group not found.";

        /// <summary>
        /// A group with this name already exists.
        /// </summary>
        public const string AlreadyExists = "A group with this name already exists.";

        /// <summary>
        /// There are no stages to validate in this group.
        /// </summary>
        public const string NoStages = "You must have at least one stage to save a Signoff Group.";

        /// <summary>
        /// There can only be one scan and attach stage permitted.
        /// </summary>
        public const string OnlyOneScanAttachStagePermitted = "There can only be one Scan & Attach stage in a Signoff Group.";

        /// <summary>
        /// There can only be one scan and attach stage permitted.
        /// </summary>
        public const string ScanAttachMustBeFollowedByUserInteration = "There must always be a stage following Scan & Attach that requires a user to check the claim.";

        /// <summary>
        /// There can only be one scan and attach stage permitted.
        /// </summary>
        public const string OnlyOneValidationStagePermitted = "There can only be one Validation stage in a Signoff Group.";

        /// <summary>
        /// There can only be one scan and attach stage permitted.
        /// </summary>
        public const string ValidationMustBeFollowedByUserInteration = "There must always be a stage following Validation that requires a user to check the claim.";

        /// <summary>
        /// Scan & Attach or validation must not be the last stage in the approval process.
        /// </summary>
        public const string SELStageMustNotBeTheLastStage = "Scan & Attach or Validation must not be the last stage in the Signoff Group.";

        /// <summary>
        /// When to include of last stage must be 'always'.
        /// </summary>
        public const string WhenToIncludeMustBeSetToAlways = "The last stage of a Signoff Group must always have When to Include set as Always.";

        /// <summary>
        /// Involvement of last stage must be 'user to check'.
        /// </summary>
        public const string InvolvementLastStageMustBeSetToUserCheck = "The last stage of a Signoff Group must always have Action set as User is to check claim.";

        /// <summary>
        /// Last stage cannot be skipped if the approver is on holiday.
        /// </summary>
        public const string LastStageCannotBeSkippedIfUserOnHoliday = "The last stage of a Signoff Group cannot be skipped if the approver is on holiday.";

        /// <summary>
        /// There can only be one Allocate for Payment stage in a Signoff group.
        /// </summary>
        public const string OnlyOneAllocateForpaymentStagePermitted = "There can only be one Allocate for Payment stage in a Signoff Group.";

        /// <summary>
        /// There can only be one Post-Validation Cleanup stage in a Signoff group.
        /// </summary>
        public const string OnlyOnePostValidationCleanupStagePermitted = "There can only be one post-validation cleanup stage in a signoff group.";

        /// <summary>
        /// The Post-Validation Cleanup stage must be the last stage in the approval process.
        /// </summary>
        public const string PostValidationCleanupStageMustBeLast = "The post-validation cleanup stage must be the last stage in the approval process.";

        /// <summary>
        /// The Post-Validation Cleanup stage must be after the Allocate for Payment stage.
        /// </summary>
        public const string PostValidationCleanupStageMustBeAfterAllocateForPayment = "The post-validation cleanup stage must be after the allocate for payment stage.";

        /// <summary>
        /// The 'allocate for payment' stage must be before the 'validation' stage.
        /// </summary>
        public const string AllocateForPaymentCannotBeAfterValidation = "The Allocate for Payment stage must be before the Validation stage.";

        /// <summary>
        /// The Post-Validation Cleanup stage has not been configured.
        /// </summary>
        public const string PostValidationCleanupStageMustBeconfigured = "The post-validation cleanup stage has not been configured.";

        /// <summary>
        /// The 'allocate for payment' stage cannot be skipped if user is on holiday.
        /// </summary>
        public const string CannotSkipAllocateForPayment = "The Allocate for Payment stage cannot be skipped if the approver is on holiday.";

        /// <summary>
        /// The 'post-validation cleaup' stage cannot be skipped if user is on holiday.
        /// </summary>
        public const string CannotSkipPostValidationCleanupStage = "The post-validation cleaup stage cannot be skipped if user is on holiday.";

        /// <summary>
        /// To use Pay before Validate a Validation stage must included.
        /// </summary>
        public const string ValidationIsRequired = "To use pay before validate a validation stage must included.";

        /// <summary>
        /// The result of the validation.
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// The message (public constants from this class).
        /// </summary>
        public List<string> Messages { get; set; }

        /// <summary>
        /// Creates a new GroupStageValidationResult with Result = true and an initialised list.
        /// </summary>
        public GroupStageValidationResult()
        {
            Result = true;
            Messages = new List<string>();
        }
    }


}
