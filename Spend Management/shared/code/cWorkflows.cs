using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Caching;
using System.Linq;
using SpendManagementLibrary;
using SpendManagementLibrary.Employees;
using Spend_Management;

using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebCalcManager;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Spend_Management
{
    using SpendManagementLibrary.Helpers;

    /// <summary>
    /// cWorkflows main class
    /// </summary>
    public class cWorkflows
    {
        #region Globals
        readonly Cache cache = HttpRuntime.Cache;
        private int nAccountid;
        SortedList<int, cWorkflow> lstCachedWorkflows;
        List<int> lstSavedStepIDs = new List<int>();
        private ICurrentUser oCurrentUser;

        /// <summary>
        /// The TableID for the parent workflow
        /// </summary>
        private cTable _workflowBaseTable;

        /// <summary>
        /// The current Entity state to address the workflow criteria step
        /// </summary>
        private int _currentEntityState;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="cWorkflows"/> class. 
        /// cWorkflows, constructor initialises the caching of all of the workflows in the database.
        /// </summary>
        /// <param name="currentUser">Any object that implements ICurrentUser</param>
        /// <param name="currentEntityState">The current entity state for the workflow</param>
        /// <param name="workflowBaseTable">The base table of the workflow</param>
        public cWorkflows(ICurrentUser currentUser, int currentEntityState = 0, cTable workflowBaseTable = null)
        {
            oCurrentUser = currentUser;
            nAccountid = currentUser.AccountID;
            this._currentEntityState = currentEntityState;
            this._workflowBaseTable = workflowBaseTable;
            InitialiseData();
        }

        #region properties
        /// <summary>
        /// Gets the accountID passed into the constructor
        /// </summary>
        public int accountid
        {
            get { return nAccountid; }
        }

        /// <summary>
        /// Gets the current list of cached workflows.
        /// </summary>
        public SortedList<int, cWorkflow> CachedWorkflows
        {
            get { return lstCachedWorkflows; }
        }
        #endregion

        /// <summary>
        /// Checks if there is an existing cache of workflows, if not it calls CacheList to create one
        /// </summary>
        private void InitialiseData()
        {
            lstCachedWorkflows = (SortedList<int, cWorkflow>)this.cache["workflows" + nAccountid];
            if (lstCachedWorkflows == null)
            {
                lstCachedWorkflows = CacheList();
            }
        }

        private void ResetCache()
        {
            lstCachedWorkflows = null;
            this.cache.Remove("workflows" + nAccountid);
            InitialiseData();
        }

        /// <summary>
        /// Caches all of the existing workflows in lstCachedWorkflows as instanced of cWorkflow
        /// </summary>
        /// <returns></returns>
        private SortedList<int, cWorkflow> CacheList()
        {
            SortedList<int, SortedList<int, cWorkflowStep>> lstWorkflowSteps = GetWorkflowSteps();

            #region - Retrieve cWorkflows
            SortedList<int, cWorkflow> list = new SortedList<int, cWorkflow>();
            cWorkflow workflow;
            int workflowid;
            string workflowname, description;
            WorkflowType workflowtype;
            bool canbechildworkflow, runoncreation, runonchange, runondelete;
            DateTime createdon;
            DateTime? modifiedon;
            int createdby;
            int? modifiedby;
            cTable baseTable;
            cTables clsTables = new cTables(nAccountid);
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountid));
            AggregateCacheDependency aggregateDependency = null;
            string mainSql = "SELECT workflowid, workflowName, description, canBeChildWorkflow, runOnCreation, runOnChange, runOnDelete, workflowType, createdon, createdby, modifiedon, modifiedby, baseTableID FROM dbo.workflows WHERE " + this.accountid + " = " + this.accountid;

            expdata.sqlexecute.CommandText = mainSql;

            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                aggregateDependency = new AggregateCacheDependency();

                string sqlFieldDependency = "SELECT fieldID, workflowID, listOrder FROM dbo.workflow_field_monitors WHERE " + this.accountid + " = " + this.accountid;
                SqlCacheDependency fieldmonitors = expdata.CreateSQLCacheDependency(sqlFieldDependency, new SortedList<string, object>());

                string sqlConditionsDependency = "SELECT conditionID, workflowID, workflowStepID, fieldID, operator, valueOne, valueTwo, runtime, updateCriteria, replaceElements FROM dbo.workflowConditions WHERE " + this.accountid + " = " + this.accountid;
                SqlCacheDependency conditionsdep = expdata.CreateSQLCacheDependency(sqlConditionsDependency, new SortedList<string, object>());

                string sqlApprovalsDependency = "SELECT workflowStepID, workflowID, approverType, approverID, oneClickSignOff, filterItems, emailTemplateID FROM dbo.workflowsApproval WHERE " + this.accountid + " = " + this.accountid;
                SqlCacheDependency approvalsDep = expdata.CreateSQLCacheDependency(sqlApprovalsDependency, new SortedList<string, object>());

                string sqlTrackerDependency = "SELECT workflowTrackerID, primaryWorkflowID, secondaryWorkflowID, entityID FROM dbo.workflowSubWorkflowTracker WHERE " + this.accountid + " = " + this.accountid;
                SqlCacheDependency trackerdep = expdata.CreateSQLCacheDependency(sqlTrackerDependency, new SortedList<string, object>());

                string sqlStepsDependency = "SELECT workflowStepID, workflowID, parentStepID, description, action, actionID, showQuestionDialog, question, trueOption, falseOption, formID, relatedStepID, message FROM dbo.workflowSteps WHERE " + this.accountid + " = " + this.accountid;
                SqlCacheDependency stepsDep = expdata.CreateSQLCacheDependency(sqlStepsDependency, new SortedList<string, object>());

                string sqlDynamicValuesDependency = "SELECT dynamicValueID, workflowStepID, dynamicValueFormula, fieldID FROM dbo.workflowDynamicValues WHERE " + this.accountid + " = " + this.accountid;
                SqlCacheDependency wfDvDep = expdata.CreateSQLCacheDependency(sqlDynamicValuesDependency, new SortedList<string, object>());

                SqlCacheDependency wfDep = expdata.CreateSQLCacheDependency(mainSql, new SortedList<string, object>());

                aggregateDependency.Add(new CacheDependency[] { fieldmonitors, conditionsdep, trackerdep, stepsDep, approvalsDep, wfDep, wfDvDep });

                var dep = new SqlCacheDependency(expdata.sqlexecute);

            }

            using (SqlDataReader reader = expdata.GetReader(mainSql))
            {
                while (reader.Read())
                {
                    workflowid = reader.GetInt32(reader.GetOrdinal("workflowid"));
                    workflowname = reader.GetString(reader.GetOrdinal("workflowName"));
                    if (reader.IsDBNull(reader.GetOrdinal("description")))
                    {
                        description = "";
                    }
                    else
                    {
                        description = reader.GetString(reader.GetOrdinal("description"));
                    }
                    canbechildworkflow = reader.GetBoolean(reader.GetOrdinal("canbechildworkflow"));
                    runoncreation = reader.GetBoolean(reader.GetOrdinal("runoncreation"));
                    runonchange = reader.GetBoolean(reader.GetOrdinal("runonchange"));
                    runondelete = reader.GetBoolean(reader.GetOrdinal("runondelete"));
                    workflowtype = (WorkflowType)reader.GetInt32(reader.GetOrdinal("workflowType"));
                    createdon = reader.GetDateTime(reader.GetOrdinal("createdon"));
                    createdby = reader.GetInt32(reader.GetOrdinal("createdby"));
                    if (reader.IsDBNull(reader.GetOrdinal("modifiedon")))
                    {
                        modifiedon = null;
                    }
                    else
                    {
                        modifiedon = reader.GetDateTime(reader.GetOrdinal("modifiedon"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("modifiedby")))
                    {
                        modifiedby = null;
                    }
                    else
                    {
                        modifiedby = reader.GetInt32(reader.GetOrdinal("modifiedby"));
                    }

                    baseTable = clsTables.GetTableByID(reader.GetGuid(reader.GetOrdinal("baseTableID")));

                    workflow = new cWorkflow(workflowid, workflowname, description, createdon, createdby, modifiedon, modifiedby, workflowtype, canbechildworkflow, runoncreation, runonchange, runondelete, baseTable);

                    if (lstWorkflowSteps.ContainsKey(workflowid))
                    {
                        workflow.Steps = lstWorkflowSteps[workflowid];
                    }

                    list.Add(workflowid, workflow);
                }
                reader.Close();
            }
            expdata.sqlexecute.Parameters.Clear();
            #endregion

            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                this.cache.Insert("workflows" + accountid, list, aggregateDependency, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.Medium), CacheItemPriority.Default, null);
            }
            return list;
        }

        /// <summary>
        /// Returns a list of all of the approval details for the approval steps
        /// </summary>
        /// <returns></returns>
        private SortedList<int, SortedList<int, sWorkflowApprovalDetails>> GetWorkflowApprovalDetails()
        {
            SortedList<int, SortedList<int, sWorkflowApprovalDetails>> lstWorkflowApprovalDetails = new SortedList<int, SortedList<int, sWorkflowApprovalDetails>>();
            SortedList<int, sWorkflowApprovalDetails> lstApprovalDetails;
            sWorkflowApprovalDetails tmpApprovalDetails;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountid));
            System.Data.SqlClient.SqlDataReader reader;
            string strSQL = "SELECT workflowStepID, workflowID, approverType, approverID, oneClickSignOff, filterItems, emailTemplateID FROM dbo.workflowsApproval";
            int nWorkflowStepID;
            int nWorkflowID;
            WorkflowEntityType eApproverType;
            int nApproverID;
            bool bOneClickSignOff;
            bool bFilterItems;
            int? emailTemplateID;

            using (reader = expdata.GetReader(strSQL))
            {
                while (reader.Read())
                {
                    nWorkflowStepID = reader.GetInt32(reader.GetOrdinal("workflowStepID"));
                    nWorkflowID = reader.GetInt32(reader.GetOrdinal("workflowID"));
                    eApproverType = (WorkflowEntityType)reader.GetByte(reader.GetOrdinal("approverType"));
                    if (reader.IsDBNull(reader.GetOrdinal("approverID")))
                    {
                        nApproverID = -1;
                    }
                    else
                    {
                        nApproverID = reader.GetInt32(reader.GetOrdinal("approverID"));
                    }

                    bOneClickSignOff = reader.GetBoolean(reader.GetOrdinal("oneClickSignOff"));
                    bFilterItems = reader.GetBoolean(reader.GetOrdinal("filterItems"));

                    if (reader.IsDBNull(reader.GetOrdinal("emailTemplateID")) == false)
                    {
                        emailTemplateID = reader.GetInt32(reader.GetOrdinal("emailTemplateID"));
                    }
                    else
                    {
                        emailTemplateID = null;
                    }

                    tmpApprovalDetails = new sWorkflowApprovalDetails(eApproverType, nApproverID, bOneClickSignOff, bFilterItems, emailTemplateID);

                    if (lstWorkflowApprovalDetails.ContainsKey(nWorkflowID))
                    {
                        lstWorkflowApprovalDetails[nWorkflowID].Add(nWorkflowStepID, tmpApprovalDetails);
                    }
                    else
                    {
                        lstApprovalDetails = new SortedList<int, sWorkflowApprovalDetails>();
                        lstApprovalDetails.Add(nWorkflowStepID, tmpApprovalDetails);
                        lstWorkflowApprovalDetails.Add(nWorkflowID, lstApprovalDetails);
                    }

                }

                reader.Close();
            }
            return lstWorkflowApprovalDetails;
        }

        /// <summary>
        /// Returns all dynamic
        /// </summary>
        /// <returns></returns>
        private SortedList<int, cWorkflowDynamicValue> GetDynamicValues()
        {
            DBConnection smdata = new DBConnection(cAccounts.getConnectionString(nAccountid));
            string strSQL = "SELECT dynamicValueID, workflowStepID, fieldID, dynamicValueFormula FROM dbo.workflowDynamicValues";

            int dynamicValueID;
            int workflowStepID;
            string valueFormula;
            Guid gFieldID;

            SortedList<int, cWorkflowDynamicValue> lstAllDynamicValues = new SortedList<int, cWorkflowDynamicValue>();
            cWorkflowDynamicValue tmpDynamicValue;
            using (System.Data.SqlClient.SqlDataReader reader = smdata.GetReader(strSQL))
            {
                while (reader.Read())
                {
                    dynamicValueID = reader.GetInt32(reader.GetOrdinal("dynamicValueID"));
                    workflowStepID = reader.GetInt32(reader.GetOrdinal("workflowStepID"));

                    if (reader.IsDBNull(reader.GetOrdinal("dynamicValueFormula")) == true)
                    {
                        valueFormula = "";
                    }
                    else
                    {
                        valueFormula = reader.GetString(reader.GetOrdinal("dynamicValueFormula"));
                    }

                    gFieldID = reader.GetGuid(reader.GetOrdinal("fieldID"));

                    tmpDynamicValue = new cWorkflowDynamicValue(dynamicValueID, valueFormula, gFieldID);

                    lstAllDynamicValues.Add(workflowStepID, tmpDynamicValue);
                }

                reader.Close();
            }
            return lstAllDynamicValues;
        }

        /// <summary>
        /// Get all workflow criteria data
        /// </summary>
        /// <returns></returns>
        private SortedList<int, SortedList<int, List<cWorkflowCriteria>>> GetWorkflowCriteria()
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountid));
            System.Data.SqlClient.SqlDataReader reader;
            string strSQL = "SELECT conditionID, workflowID, workflowStepID, fieldID, operator, valueOne, valueTwo, runtime, updateCriteria,replaceElements FROM workflowConditions";
            cWorkflowCriteria tmpWorkflowCriteria;

            cFields clsFields = new cFields(nAccountid);

            int nConditionID;
            int nWorkflowID;
            int nWorkflowStepID;
            Guid nFieldID;
            ConditionType eConditionType;
            string sValue;
            string sValueTwo;
            bool bRunTime;
            bool bUpdateCriteria;
            bool bReplaceElements;
            cField reqField;

            SortedList<int, SortedList<int, List<cWorkflowCriteria>>> lstCriteriaAllWorkflows = new SortedList<int, SortedList<int, List<cWorkflowCriteria>>>();
            SortedList<int, List<cWorkflowCriteria>> lstStepsCriteria;
            List<cWorkflowCriteria> lstCriteria;
            using (reader = expdata.GetReader(strSQL))
            {
                while (reader.Read())
                {
                    nConditionID = reader.GetInt32(reader.GetOrdinal("conditionID"));
                    nWorkflowID = reader.GetInt32(reader.GetOrdinal("workflowID"));
                    nWorkflowStepID = reader.GetInt32(reader.GetOrdinal("workflowStepID"));
                    nFieldID = reader.GetGuid(reader.GetOrdinal("fieldID"));
                    eConditionType = (ConditionType)reader.GetByte(reader.GetOrdinal("operator"));

                    if (reader.IsDBNull(reader.GetOrdinal("valueOne")) == false)
                    {
                        Guid attributeFieldId;
                        sValue = reader.GetString(reader.GetOrdinal("valueOne"));
                        if (Guid.TryParse(sValue, out attributeFieldId) && this._currentEntityState != 0 && this._workflowBaseTable != null)
                        {
                            var baseWorkflow = this.GetWorkflowById(nWorkflowID);
                            if (baseWorkflow.BaseTable.TableID == this._workflowBaseTable.TableID)
                            {
                                cCalculationField clsCalculationField = new cCalculationField(this.nAccountid);
                                sValue = clsCalculationField.CalculatedColumn(this.GenerateFormulaForWorkflows(attributeFieldId), this._workflowBaseTable, this._currentEntityState, new cFields(nAccountid), cAccounts.getConnectionString(nAccountid), new cTables(nAccountid));
                            }
                        }
                    }
                    else
                    {
                        sValue = "";
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("valueTwo")) == false)
                    {
                        sValueTwo = reader.GetString(reader.GetOrdinal("valueTwo"));
                    }
                    else
                    {
                        sValueTwo = "";
                    }

                    bReplaceElements = reader.GetBoolean(reader.GetOrdinal("replaceElements"));

                    if (reader.IsDBNull(reader.GetOrdinal("runtime")))
                    {
                        bRunTime = false;
                    }
                    else
                    {
                        bRunTime = reader.GetBoolean(reader.GetOrdinal("runtime"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("updateCriteria")))
                    {
                        bUpdateCriteria = false;
                    }
                    else
                    {
                        bUpdateCriteria = reader.GetBoolean(reader.GetOrdinal("updateCriteria"));
                    }

                    reqField = clsFields.GetFieldByID(nFieldID);

                    if (bUpdateCriteria == true)
                    {
                        if (bRunTime == true)
                        {
                            tmpWorkflowCriteria = new cWorkflowCriteria(nConditionID, reqField, true);
                        }
                        else
                        {
                            tmpWorkflowCriteria = new cWorkflowCriteria(nConditionID, reqField, sValue, bReplaceElements);
                        }
                    }
                    else
                    {
                        tmpWorkflowCriteria = new cWorkflowCriteria(nConditionID, reqField, eConditionType, sValue, sValueTwo);
                    }

                    if (lstCriteriaAllWorkflows.ContainsKey(nWorkflowID) == false)
                    {
                        lstStepsCriteria = new SortedList<int, List<cWorkflowCriteria>>();
                        lstCriteriaAllWorkflows.Add(nWorkflowID, lstStepsCriteria);
                    }

                    if (lstCriteriaAllWorkflows[nWorkflowID].ContainsKey(nWorkflowStepID) == false)
                    {
                        lstCriteria = new List<cWorkflowCriteria>();
                        lstCriteriaAllWorkflows[nWorkflowID].Add(nWorkflowStepID, lstCriteria);
                    }

                    lstCriteriaAllWorkflows[nWorkflowID][nWorkflowStepID].Add(tmpWorkflowCriteria);
                }

                reader.Close();
            }
            return lstCriteriaAllWorkflows;
        }

        /// <summary>
        /// Gets a list of all workflow steps
        /// </summary>
        private SortedList<int, SortedList<int, cWorkflowStep>> GetWorkflowSteps()
        {
            SortedList<int, cWorkflowDynamicValue> lstAllDynamicValues = GetDynamicValues();

            SortedList<int, SortedList<int, List<cWorkflowCriteria>>> lstAllWorkflowCriteria = GetWorkflowCriteria();
            SortedList<int, List<cWorkflowCriteria>> lstAllStepsCriteria; ;
            List<cWorkflowCriteria> lstStepCriteria;

            SortedList<int, SortedList<int, sWorkflowApprovalDetails>> lstWorkflowApprovalDetails = GetWorkflowApprovalDetails();
            SortedList<int, sWorkflowApprovalDetails> lstApprovalDetails;

            SortedList<int, SortedList<int, cWorkflowStep>> lstWorkflowSteps = new SortedList<int, SortedList<int, cWorkflowStep>>();

            SortedList<int, cWorkflowStep> lstSteps;
            cWorkflowStep tmpWorkflowStep;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountid));
            System.Data.SqlClient.SqlDataReader reader;

            int workflowID;
            int workflowStepID;
            int parentStepID;
            string description;
            WorkFlowStepAction workflowAction;
            int actionID;
            bool showQuestionDialog;
            string question;
            string trueOption;
            string falseOption;
            int? formID;
            int? relatedStepID;
            string message;


            string strSQL = "SELECT workflowStepID, workflowID, parentStepID, description, action, actionID, showQuestionDialog, question, trueOption, falseOption, formID, relatedStepID, message FROM dbo.workflowSteps";

            using (reader = expdata.GetReader(strSQL))
            {
                expdata.sqlexecute.Parameters.Clear();

                while (reader.Read())
                {

                    lstStepCriteria = null;

                    workflowID = reader.GetInt32(reader.GetOrdinal("workflowID"));
                    workflowStepID = reader.GetInt32(reader.GetOrdinal("workflowStepID"));

                    if (reader.IsDBNull(reader.GetOrdinal("parentStepID")))
                    {
                        parentStepID = -1;
                    }
                    else
                    {
                        parentStepID = reader.GetInt32(reader.GetOrdinal("parentStepID"));
                    }

                    description = reader.GetString(reader.GetOrdinal("description"));
                    workflowAction = (WorkFlowStepAction)reader.GetInt32(reader.GetOrdinal("action"));

                    if (reader.IsDBNull(reader.GetOrdinal("actionID")))
                    {
                        actionID = -1;
                    }
                    else
                    {
                        actionID = reader.GetInt32(reader.GetOrdinal("actionID"));
                    }

                    showQuestionDialog = reader.GetBoolean(reader.GetOrdinal("showQuestionDialog"));

                    if (reader.IsDBNull(reader.GetOrdinal("question")) == true)
                    {
                        question = "";
                    }
                    else
                    {
                        question = reader.GetString(reader.GetOrdinal("question"));
                    }


                    if (reader.IsDBNull(reader.GetOrdinal("trueOption")) == true)
                    {
                        trueOption = "";
                    }
                    else
                    {
                        trueOption = reader.GetString(reader.GetOrdinal("trueOption"));
                    }


                    if (reader.IsDBNull(reader.GetOrdinal("falseOption")) == true)
                    {
                        falseOption = "";
                    }
                    else
                    {
                        falseOption = reader.GetString(reader.GetOrdinal("falseOption"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("formID")) == true)
                    {
                        formID = null;
                    }
                    else
                    {
                        formID = reader.GetInt32(reader.GetOrdinal("formID"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("relatedStepID")) == true)
                    {
                        relatedStepID = null;
                    }
                    else
                    {
                        relatedStepID = reader.GetInt32(reader.GetOrdinal("relatedStepID"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("message")) == false)
                    {
                        message = reader.GetString(reader.GetOrdinal("message"));
                    }
                    else
                    {
                        message = string.Empty;
                    }

                    if (lstWorkflowSteps.ContainsKey(workflowID) == false)
                    {
                        lstSteps = new SortedList<int, cWorkflowStep>();
                        lstWorkflowSteps.Add(workflowID, lstSteps);
                    }

                    lstAllWorkflowCriteria.TryGetValue(workflowID, out lstAllStepsCriteria);

                    switch (workflowAction)
                    {
                        case WorkFlowStepAction.Approval:
                            lstAllWorkflowCriteria.TryGetValue(workflowID, out lstAllStepsCriteria);
                            if (lstAllStepsCriteria == null)
                            {
                                lstStepCriteria = new List<cWorkflowCriteria>();
                            }
                            else
                            {
                                lstAllStepsCriteria.TryGetValue(workflowStepID, out lstStepCriteria);
                            }
                            lstWorkflowApprovalDetails.TryGetValue(workflowID, out lstApprovalDetails);
                            tmpWorkflowStep = new cApprovalStep(workflowID, workflowStepID, description, workflowAction, parentStepID, lstApprovalDetails[workflowStepID].ApproverType, lstApprovalDetails[workflowStepID].ApproverID, lstApprovalDetails[workflowStepID].OneClickSignOff, lstApprovalDetails[workflowStepID].FilterItems, lstStepCriteria, showQuestionDialog, formID, relatedStepID, question, trueOption, falseOption, lstApprovalDetails[workflowStepID].EmailTemplateID, message);
                            lstWorkflowSteps[workflowID].Add(workflowStepID, tmpWorkflowStep);
                            break;
                        case WorkFlowStepAction.ChangeValue:

                            if (lstAllStepsCriteria == null)
                            {
                                lstStepCriteria = new List<cWorkflowCriteria>();
                            }
                            else
                            {
                                lstAllStepsCriteria.TryGetValue(workflowStepID, out lstStepCriteria);
                            }
                            tmpWorkflowStep = new cChangeValueStep(workflowID, workflowStepID, description, workflowAction, lstStepCriteria, parentStepID, formID);
                            lstWorkflowSteps[workflowID].Add(workflowStepID, tmpWorkflowStep);
                            break;
                        case WorkFlowStepAction.ElseCondition:
                        case WorkFlowStepAction.ElseOtherwise:
                        case WorkFlowStepAction.CheckCondition:

                            if (lstAllStepsCriteria == null)
                            {
                                lstStepCriteria = new List<cWorkflowCriteria>();
                            }
                            else
                            {
                                lstAllStepsCriteria.TryGetValue(workflowStepID, out lstStepCriteria);
                            }
                            tmpWorkflowStep = new cCheckConditionStep(workflowID, workflowStepID, description, workflowAction, lstStepCriteria, parentStepID, formID, relatedStepID);
                            lstWorkflowSteps[workflowID].Add(workflowStepID, tmpWorkflowStep);
                            break;
                        case WorkFlowStepAction.CreateDynamicValue:

                            cWorkflowDynamicValue tmpDynamicValue;
                            if (lstAllDynamicValues == null || lstAllDynamicValues.ContainsKey(workflowStepID) == false)
                            {
                                tmpDynamicValue = new cWorkflowDynamicValue(0, "", new Guid());
                            }
                            else
                            {
                                tmpDynamicValue = lstAllDynamicValues[workflowStepID];
                            }

                            tmpWorkflowStep = new cCreateDynamicValue(workflowID, workflowStepID, description, workflowAction, parentStepID, formID, tmpDynamicValue);
                            lstWorkflowSteps[workflowID].Add(workflowStepID, tmpWorkflowStep);
                            break;
                        case WorkFlowStepAction.DecisionFalse:
                        case WorkFlowStepAction.Decision:
                            tmpWorkflowStep = new cDecisionStep(workflowID, workflowStepID, description, workflowAction, question, trueOption, falseOption, parentStepID, formID, relatedStepID);
                            lstWorkflowSteps[workflowID].Add(workflowStepID, tmpWorkflowStep);
                            break;
                        case WorkFlowStepAction.MoveToStep:
                            tmpWorkflowStep = new cMovetoStepStep(workflowID, workflowStepID, description, workflowAction, actionID, parentStepID, formID);
                            lstWorkflowSteps[workflowID].Add(workflowStepID, tmpWorkflowStep);
                            break;
                        case WorkFlowStepAction.RunSubWorkflow:
                            tmpWorkflowStep = new cRunSubworkflowStep(workflowID, workflowStepID, description, workflowAction, actionID, parentStepID, formID);
                            lstWorkflowSteps[workflowID].Add(workflowStepID, tmpWorkflowStep);
                            break;
                        case WorkFlowStepAction.SendEmail:
                            tmpWorkflowStep = new cSendEmailStep(workflowID, workflowStepID, description, workflowAction, actionID, parentStepID, formID);
                            lstWorkflowSteps[workflowID].Add(workflowStepID, tmpWorkflowStep);
                            break;
                        case WorkFlowStepAction.FinishWorkflow:
                            tmpWorkflowStep = new cWorkflowStep(workflowID, workflowStepID, description, workflowAction, parentStepID, false, "", "", "", formID, null);
                            lstWorkflowSteps[workflowID].Add(workflowStepID, tmpWorkflowStep);
                            break;
                        case WorkFlowStepAction.ShowMessage:
                            tmpWorkflowStep = new cShowMessageStep(workflowID, workflowStepID, description, parentStepID, message);
                            lstWorkflowSteps[workflowID].Add(workflowStepID, tmpWorkflowStep);
                            break;
                        case WorkFlowStepAction.ChangeCustomEntityForm:
                            tmpWorkflowStep = new cChangeFormStep(workflowID, workflowStepID, description, parentStepID, formID.Value);
                            lstWorkflowSteps[workflowID].Add(workflowStepID, tmpWorkflowStep);
                            break;
                        default:
                            throw new Exception("Invalid workflow step action!");
                    }
                }

                reader.Close();
            }
            return lstWorkflowSteps;
        }

        /// <summary>
        /// A list containing all of the instances of cWorkflow that can be run as child-workflows/sub-workflows.
        /// </summary>
        /// <returns></returns>
        public List<cWorkflow> GetSelectableSubWorkflows(Guid baseTableID, int workflowID)
        {
            List<cWorkflow> lstSubWorkflows = new List<cWorkflow>();

            cWorkflow tmpWorkflow;
            for (int i = 0; i < lstCachedWorkflows.Count; i++)
            {
                tmpWorkflow = lstCachedWorkflows.Values[i];
                if (tmpWorkflow.BaseTable != null && tmpWorkflow.canbechildworkflow == true && tmpWorkflow.BaseTable.TableID == baseTableID && tmpWorkflow.workflowid != workflowID) // check if this workflow is the same base table id as the one being created and that it is allowed to be a sub workflow
                {
                    lstSubWorkflows.Add(tmpWorkflow);
                }
            }
            return lstSubWorkflows;
        }

        /// <summary>
        /// Add a workflow to the database, checks if the workflow already exists (compares the workflow name).
        /// </summary>
        /// <param name="workflowID">WorkflowID if updating, 0 if saving a new workflow.</param>
        /// <param name="name">Workflow name</param>
        /// <param name="description">Workflow description</param>
        /// <param name="workflowType">Type of workflow</param>
        /// <param name="canBeChild">Can this workflow be selected to run as a child/sub workflow</param>
        /// <param name="runOnCreation">Does this workflow run when a record is created</param>
        /// <param name="runOnChange">Does this workflow run when a record is changed/updated</param>
        /// <param name="runOnDelete">Does this workflow run when a record is deleted</param>
        /// <param name="baseTableID">The base table this workflow is for</param>
        /// <param name="employeeID">employeeID creating the workflow</param>
        /// <param name="steps">steps array</param>
        /// <returns>Positive INT for the new workflowID or -1 if it already exists.</returns>
        public int SaveWorkFlow(int workflowID, WorkflowType workflowType, string name, string description, Guid baseTableID, bool canBeChild, bool runOnCreation, bool runOnChange, bool runOnDelete, int employeeID, object[,] steps)
        {
            if (WorkflowAlreadyExists(workflowID, name) == true)
            {
                return -1; // Workflow name is already in use so return.
            }

            try
            {
                using (System.Transactions.TransactionScope transaction = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, new TimeSpan(1, 0, 0)))
                {
                    List<cWorkflowCriteria> lstCriteria;


                    WorkFlowStepAction stepAction;
                    string stepDescription;
                    string question;
                    string trueAnswer;
                    string falseAnswer;
                    WorkflowEntityType approver;
                    int actionID;
                    bool oneClickSignOff;
                    bool showDeclaration;
                    int? parentStepID;
                    int? relatedStepID;
                    int? approvalEmailTemplate;
                    string message;
                    int? formID;

                    cFields clsFields = new cFields(nAccountid);
                    cField tmpField;
                    Guid fieldID;
                    ConditionType condition; // eq, not eq etc
                    CriteriaMode criteriaMode; // update or check con
                    bool runtime; // if update is it runtime at runtime?
                    string value;
                    string value2;

                    cWorkflowCriteria tmpWorkflowCriteria;

                    SortedList<int, int> tmpMoveToStepRefs = new SortedList<int, int>();

                    DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountid));
                    CurrentUser currentUser = cMisc.GetCurrentUser();

                    #region insert/update the workflow
                    using (System.Transactions.TransactionScope transaction2 = new System.Transactions.TransactionScope())
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@workflowid", workflowID);
                        expdata.sqlexecute.Parameters.AddWithValue("@workflowname", name);
                        expdata.sqlexecute.Parameters.AddWithValue("@description", description);
                        expdata.sqlexecute.Parameters.AddWithValue("@canbechildworkflow", canBeChild);
                        expdata.sqlexecute.Parameters.AddWithValue("@runoncreation", runOnCreation);
                        expdata.sqlexecute.Parameters.AddWithValue("@runonchange", runOnChange);
                        expdata.sqlexecute.Parameters.AddWithValue("@runondelete", runOnDelete);
                        expdata.sqlexecute.Parameters.AddWithValue("@workflowtype", (int)workflowType);
                        expdata.sqlexecute.Parameters.AddWithValue("@activeUser", employeeID);
                        expdata.sqlexecute.Parameters.AddWithValue("@baseTableID", baseTableID);
                        expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                        if (currentUser.isDelegate == true)
                        {
                            expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                        }
                        else
                        {
                            expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                        }
                        expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                        expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
                        expdata.ExecuteProc("dbo.saveWorkflow");
                        workflowID = (int)expdata.sqlexecute.Parameters["@identity"].Value;
                        expdata.sqlexecute.Parameters.Clear();

                        transaction2.Complete();
                    }
                    #endregion insert/update the workflow

                    using (System.Transactions.TransactionScope transaction3 = new System.Transactions.TransactionScope())
                    {
                        #region delete all existing steps
                        expdata.sqlexecute.Parameters.AddWithValue("@workflowid", workflowID);
                        expdata.sqlexecute.Parameters.AddWithValue("@employeeID", employeeID);
                        expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                        if (currentUser.isDelegate == true)
                        {
                            expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                        }
                        else
                        {
                            expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                        }
                        expdata.ExecuteProc("dbo.deleteWorkflowSteps");
                        expdata.sqlexecute.Parameters.Clear();
                        #endregion delete all existing steps

                        if (steps != null)
                        {
                            for (int i = 0; i < steps.GetLongLength(0); i++)
                            {
                                if (steps[i, 0] != null)
                                {
                                    lstCriteria = new List<cWorkflowCriteria>();
                                    stepAction = (WorkFlowStepAction)Convert.ToInt32(steps[i, 0]);
                                    stepDescription = (string)steps[i, 1];
                                    question = (string)steps[i, 2];
                                    trueAnswer = (string)steps[i, 3];
                                    falseAnswer = (string)steps[i, 4];

                                    if (steps[i, 5] != null)
                                    {
                                        for (int j = 0; j < ((object[])steps[i, 5]).Length; j++)
                                        {
                                            fieldID = new Guid((string)((object[])((object[])steps[i, 5])[j])[0]);
                                            tmpField = clsFields.GetFieldByID(fieldID);
                                            condition = (ConditionType)Convert.ToInt16((string)((object[])((object[])steps[i, 5])[j])[1]);
                                            if (stepAction == WorkFlowStepAction.ChangeValue)
                                            {
                                                criteriaMode = CriteriaMode.Update;
                                            }
                                            else
                                            {
                                                criteriaMode = CriteriaMode.Select;
                                            }

                                            if (((object[])((object[])steps[i, 5])[j])[3] == null || string.IsNullOrEmpty(((object[])((object[])steps[i, 5])[j])[3].ToString().Trim()) == true)
                                            {
                                                runtime = false;
                                            }
                                            else
                                            {
                                                runtime = Convert.ToBoolean(((object[])((object[])steps[i, 5])[j])[3]);
                                            }

                                            value = Convert.ToString(((object[])((object[])steps[i, 5])[j])[4]);

                                            if (((object[])((object[])steps[i, 5])[j])[5] == null || string.IsNullOrEmpty(((object[])((object[])steps[i, 5])[j])[5].ToString().Trim()) == true)
                                            {
                                                value2 = "";
                                            }
                                            else
                                            {
                                                value2 = Convert.ToString(((object[])((object[])steps[i, 5])[j])[5]);
                                            }

                                            tmpWorkflowCriteria = new cWorkflowCriteria(tmpField, condition, criteriaMode, runtime, value, value2);
                                            lstCriteria.Add(tmpWorkflowCriteria);
                                        }
                                    }
                                    approver = (WorkflowEntityType)Convert.ToInt32(steps[i, 6]);
                                    if (stepAction == WorkFlowStepAction.MoveToStep)
                                    {
                                        actionID = lstSavedStepIDs[Convert.ToInt32(steps[i, 7])];
                                    }
                                    else
                                    {
                                        actionID = Convert.ToInt32(steps[i, 7]);
                                    }
                                    oneClickSignOff = Convert.ToBoolean(steps[i, 8]);
                                    showDeclaration = Convert.ToBoolean(steps[i, 9]);

                                    if (steps[i, 10] == null)
                                    {
                                        parentStepID = null;
                                    }
                                    else
                                    {
                                        parentStepID = Convert.ToInt32(steps[i, 10]);
                                    }


                                    if (steps[i, 11] == null)
                                    {
                                        relatedStepID = (int?)steps[i, 11];
                                    }
                                    else
                                    {
                                        relatedStepID = Convert.ToInt32(steps[i, 11]);
                                    }

                                    if (steps[i, 13] == null)
                                    {
                                        approvalEmailTemplate = null;
                                    }
                                    else
                                    {
                                        approvalEmailTemplate = Convert.ToInt32(steps[i, 13]);
                                    }

                                    if (steps[i, 14] == null)
                                    {
                                        message = string.Empty;
                                    }
                                    else
                                    {
                                        message = Convert.ToString(steps[i, 14]);
                                    }

                                    if (steps[i, 15] == null || steps[i, 15].ToString() == "" || string.IsNullOrEmpty(steps[i, 15].ToString()))
                                    {
                                        formID = null;
                                    }
                                    else
                                    {
                                        formID = Convert.ToInt32(steps[i, 15]);
                                    }

                                    SaveWorkflowStep(workflowID, i, (int)stepAction, stepDescription, question, trueAnswer, falseAnswer, lstCriteria, (int)approver, actionID, oneClickSignOff, showDeclaration, parentStepID, relatedStepID, approvalEmailTemplate, message, formID);

                                    if (stepAction == WorkFlowStepAction.MoveToStep) // check to see if the step that has just been saved has been referenced previously by a movetostep step
                                    {
                                        tmpMoveToStepRefs.Add(lstSavedStepIDs[i], actionID);
                                    }
                                }
                            }
                        }
                        transaction3.Complete();
                    }

                    foreach (KeyValuePair<int, int> kvp in tmpMoveToStepRefs)
                    {

                        UpdateMoveToStepActionID(workflowID, kvp.Key, lstSavedStepIDs[kvp.Value]);
                    }

                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                return -2; // Error saving something to the database so roll back
            }
            return workflowID;
        }

        /// <summary>
        /// Check if a workflow with the specified name already exists.
        /// </summary>
        /// <param name="name">The workflow name to check for existance.</param>
        /// <param name="workflowID">WorkflowID</param>
        /// <returns>True if one already exists, false if the name does not match any existing.</returns>
        public bool WorkflowAlreadyExists(int workflowID, string name)
        {
            cWorkflow tmpWorkflow;
            for (int j = 0; j < lstCachedWorkflows.Count; j++)
            {
                tmpWorkflow = lstCachedWorkflows.Values[j];
                if (tmpWorkflow.workflowname == name && tmpWorkflow.workflowid != workflowID)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Delete all workflow steps from a workflow.
        /// </summary>
        /// <param name="workflowID">The workflowID that you want to delete all the steps for</param>
        public void DeleteWorkflowSteps(int workflowID)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountid));
            expdata.sqlexecute.Parameters.AddWithValue("@workflowid", workflowID);
            CurrentUser currentUser = cMisc.GetCurrentUser();
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            expdata.ExecuteProc("dbo.deleteWorkflowSteps");
            expdata.sqlexecute.Parameters.Clear();
        }
        /// <summary>
        /// Used to save the workflow steps
        /// </summary>
        /// <returns></returns>
        private bool SaveWorkflowStep(int workflowID, int workflowStepID, int action, string description, string question, string trueAnswer, string falseAnswer, List<cWorkflowCriteria> conditions, int approverType, int actionID, bool oneclicksignoff, bool showdeclaration, int? parentStepID, int? relatedStepID, int? approvalEmailTemplateID, string message, int? formID)
        {
            WorkFlowStepAction eAction = (WorkFlowStepAction)action;

            if (relatedStepID.HasValue == true)
            {
                relatedStepID = lstSavedStepIDs[relatedStepID.Value];
            }

            bool stepSuccess = false;
            switch (eAction)
            {
                case WorkFlowStepAction.Approval: // cant do due to criteria builder
                    #region
                    stepSuccess = SaveWorkflowApprovalStep(workflowID, parentStepID, description, approverType, actionID, conditions, oneclicksignoff, showdeclaration, question, trueAnswer, falseAnswer, approvalEmailTemplateID, message, relatedStepID);
                    break;
                #endregion
                case WorkFlowStepAction.ChangeValue: // cant do due to sql builder
                    stepSuccess = SaveWorkflowChangeValueStep(workflowID, parentStepID, description, conditions);
                    break;
                case WorkFlowStepAction.ElseCondition: // cant do due to sql builder
                case WorkFlowStepAction.CheckCondition: // cant do due to sql builder
                case WorkFlowStepAction.ElseOtherwise:
                    stepSuccess = SaveWorkflowCheckConditionStep(workflowID, parentStepID, description, eAction, conditions, relatedStepID);
                    break;
                case WorkFlowStepAction.Decision:
                    #region
                    stepSuccess = SaveWorkflowDecisionStep(workflowID, parentStepID, description, question, trueAnswer, falseAnswer);
                    break;
                #endregion
                case WorkFlowStepAction.DecisionFalse:
                    #region
                    stepSuccess = SaveWorkflowDecisionFalseStep(workflowID, parentStepID, description, question, trueAnswer, falseAnswer, Convert.ToInt32(relatedStepID));
                    break;
                #endregion
                case WorkFlowStepAction.MoveToStep:
                    #region
                    stepSuccess = SaveWorkflowMoveToStepStep(workflowID, parentStepID, description, actionID);
                    break;
                #endregion
                case WorkFlowStepAction.RunSubWorkflow:
                    #region
                    stepSuccess = SaveWorkflowRunSubWorkflowStep(workflowID, parentStepID, description, actionID);
                    break;
                #endregion
                case WorkFlowStepAction.SendEmail: // cant do due to email template system
                    stepSuccess = SaveWorkflowSendEmailStep(workflowID, parentStepID, description, actionID);
                    break;
                case WorkFlowStepAction.FinishWorkflow:
                    #region
                    stepSuccess = SaveWorkflowFinishWorkflowStep(workflowID, parentStepID);
                    break;
                #endregion
                case WorkFlowStepAction.CreateDynamicValue:
                    stepSuccess = SaveWorkflowCreateDynamicValueStep(workflowID, parentStepID, description, conditions);
                    break;
                case WorkFlowStepAction.ShowMessage:
                    stepSuccess = SaveWorkflowShowMessageStep(workflowID, parentStepID, description, message);
                    break;
                case WorkFlowStepAction.ChangeCustomEntityForm:
                    stepSuccess = SaveWorkflowChangeFormStep(workflowID, parentStepID, description, formID.Value);
                    break;
                default:
                    #region default behaviour is to throw an exception
                    throw new Exception("Unknown WorkflowStepAction: " + eAction.ToString());
                    #endregion
            }
            return stepSuccess;
        }

        private bool SaveWorkflowShowMessageStep(int workflowID, int? parentStepID, string description, string message)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountid));
            expdata.sqlexecute.Parameters.AddWithValue("@workflowID", workflowID);

            if (parentStepID.HasValue == false)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@parentStepID", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@parentStepID", getSavedStepID(parentStepID.Value));
            }

            expdata.sqlexecute.Parameters.AddWithValue("@description", description);
            expdata.sqlexecute.Parameters.AddWithValue("@message", message);
            CurrentUser currentUser = cMisc.GetCurrentUser();
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("dbo.saveWorkflowShowMessageStep");

            int workflowStepID = (int)expdata.sqlexecute.Parameters["@identity"].Value;

            lstSavedStepIDs.Add(workflowStepID);
            expdata.sqlexecute.Parameters.Clear();

            return true;
        }

        private bool SaveWorkflowChangeFormStep(int workflowID, int? parentStepID, string description, int formID)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountid));
            expdata.sqlexecute.Parameters.AddWithValue("@workflowID", workflowID);

            if (parentStepID.HasValue == false)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@parentStepID", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@parentStepID", getSavedStepID(parentStepID.Value));
            }

            expdata.sqlexecute.Parameters.AddWithValue("@description", description);
            expdata.sqlexecute.Parameters.AddWithValue("@formID", formID);
            CurrentUser currentUser = cMisc.GetCurrentUser();
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("dbo.saveWorkflowChangeFormStep");

            int workflowStepID = (int)expdata.sqlexecute.Parameters["@identity"].Value;

            lstSavedStepIDs.Add(workflowStepID);
            expdata.sqlexecute.Parameters.Clear();

            return true;
        }

        /// <summary>
        /// Saves a workflow create dynamic value step to the database
        /// </summary>
        /// <param name="workflowID"></param>
        /// <param name="parentStepID"></param>
        /// <param name="description"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        private bool SaveWorkflowCreateDynamicValueStep(int workflowID, int? parentStepID, string description, List<cWorkflowCriteria> conditions)
        {
            bool saveSuccessful = true;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountid));
            expdata.sqlexecute.Parameters.AddWithValue("@workflowID", workflowID);

            if (parentStepID.HasValue == false)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@parentStepID", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@parentStepID", getSavedStepID(parentStepID.Value));
            }

            expdata.sqlexecute.Parameters.AddWithValue("@description", description);
            CurrentUser currentUser = cMisc.GetCurrentUser();
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("dbo.saveWorkflowCreateDynamicValueStep");

            int workflowStepID = (int)expdata.sqlexecute.Parameters["@identity"].Value;

            lstSavedStepIDs.Add(workflowStepID);
            expdata.sqlexecute.Parameters.Clear();

            Guid fieldID = (conditions.Count > 0 && conditions[0].Field != null && conditions[0].Field.FieldID != null) ? conditions[0].Field.FieldID : Guid.Empty;
            string formula = (conditions.Count > 0 && conditions[0].Value != null) ? conditions[0].Value.Replace("&quot;", "\"") : string.Empty;

            expdata.sqlexecute.Parameters.AddWithValue("@workflowStepID", workflowStepID);
            expdata.sqlexecute.Parameters.AddWithValue("@dynamicValueFormula", formula);
            expdata.sqlexecute.Parameters.AddWithValue("@fieldID", fieldID);
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            expdata.ExecuteProc("dbo.saveWorkflowStepCreateDynamicValue");
            expdata.sqlexecute.Parameters.Clear();
            return saveSuccessful;
        }

        /// <summary>
        /// Saving a check condition to the database
        /// </summary>
        /// <returns></returns>
        private bool SaveWorkflowCheckConditionStep(int workflowID, int? parentStepID, string description, WorkFlowStepAction action, List<cWorkflowCriteria> conditions, int? relatedStepID)
        {
            bool saveSuccessful = true;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountid));
            expdata.sqlexecute.Parameters.AddWithValue("@workflowID", workflowID);
            expdata.sqlexecute.Parameters.AddWithValue("@stepAction", (int)action);
            if (parentStepID.HasValue == false)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@parentStepID", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@parentStepID", getSavedStepID(parentStepID.Value));
            }

            if (relatedStepID.HasValue == true)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@relatedStepID", relatedStepID.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@relatedStepID", DBNull.Value);
            }

            expdata.sqlexecute.Parameters.AddWithValue("@description", description);
            CurrentUser currentUser = cMisc.GetCurrentUser();
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("dbo.saveWorkflowStepCheckCondition");
            int workflowStepID = (int)expdata.sqlexecute.Parameters["@identity"].Value;

            lstSavedStepIDs.Add(workflowStepID);
            expdata.sqlexecute.Parameters.Clear();

            SaveWorkflowStepCriteria(workflowID, workflowStepID, conditions);

            return saveSuccessful;
        }

        /// <summary>
        /// Saving a change value step to the database
        /// </summary>
        /// <param name="workflowID"></param>
        /// <param name="parentStepID"></param>
        /// <param name="description"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        private bool SaveWorkflowChangeValueStep(int workflowID, int? parentStepID, string description, List<cWorkflowCriteria> conditions)
        {
            bool saveSuccessful = true;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountid));
            expdata.sqlexecute.Parameters.AddWithValue("@workflowid", workflowID);

            if (parentStepID.HasValue == false)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@parentStepID", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@parentStepID", getSavedStepID(parentStepID.Value));
            }

            expdata.sqlexecute.Parameters.AddWithValue("@description", description);
            CurrentUser currentUser = cMisc.GetCurrentUser();
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("dbo.saveWorkflowStepChangeValue");
            int workflowStepID = (int)expdata.sqlexecute.Parameters["@identity"].Value;

            lstSavedStepIDs.Add(workflowStepID);
            expdata.sqlexecute.Parameters.Clear();

            SaveWorkflowStepCriteria(workflowID, workflowStepID, conditions);

            return saveSuccessful;
        }

        /// <summary>
        /// Saving a send email step to the database
        /// </summary>
        /// <param name="workflowID"></param>
        /// <param name="parentStepID"></param>
        /// <param name="description"></param>
        /// <param name="emailTemplateID">emailTemplateID</param>
        /// <returns></returns>
        private bool SaveWorkflowSendEmailStep(int workflowID, int? parentStepID, string description, int emailTemplateID)
        {
            bool saveSuccessful = true;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountid));
            expdata.sqlexecute.Parameters.AddWithValue("@workflowID", workflowID);

            if (parentStepID.HasValue == false)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@parentStepID", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@parentStepID", getSavedStepID(parentStepID.Value));
            }

            expdata.sqlexecute.Parameters.AddWithValue("@description", description);
            expdata.sqlexecute.Parameters.AddWithValue("@emailTemplateID", emailTemplateID);
            CurrentUser currentUser = cMisc.GetCurrentUser();
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }

            expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;

            expdata.ExecuteProc("dbo.saveWorkflowStepEmailTemplate");

            int workflowStepID = (int)expdata.sqlexecute.Parameters["@identity"].Value;

            lstSavedStepIDs.Add(workflowStepID);

            expdata.sqlexecute.Parameters.Clear();

            return saveSuccessful;
        }

        /// <summary>
        /// Saves a finish workflow step to the database
        /// </summary>
        /// <param name="workflowID"></param>
        /// <param name="parentStepID"></param>
        /// <returns></returns>
        private bool SaveWorkflowFinishWorkflowStep(int workflowID, int? parentStepID)
        {
            bool saveSuccessful = true;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountid));
            expdata.sqlexecute.Parameters.AddWithValue("@workflowID", workflowID);

            if (parentStepID.HasValue == false)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@parentStepID", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@parentStepID", getSavedStepID(parentStepID.Value));
            }
            CurrentUser currentUser = cMisc.GetCurrentUser();
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;

            expdata.ExecuteProc("dbo.saveWorkflowStepFinishWorkflow");

            int workflowStepID = (int)expdata.sqlexecute.Parameters["@identity"].Value;

            lstSavedStepIDs.Add(workflowStepID);

            expdata.sqlexecute.Parameters.Clear();
            return saveSuccessful;
        }

        private int getSavedStepID(int parentStepID)
        {
            return lstSavedStepIDs[parentStepID];
        }

        /// <summary>
        /// Saving a decision step to the database
        /// </summary>
        /// <param name="workflowID">workflowID this step belongs too.</param>
        /// <param name="parentStepID">Parent stepID or null if no parent</param>
        /// <param name="description">Description of the step</param>
        /// <param name="question">The question to ask the user</param>
        /// <param name="trueAnswer">The value of the true/yes button.</param>
        /// <param name="falseAnswer">The value of the false/no button.</param>
        /// <returns></returns>
        private bool SaveWorkflowDecisionStep(int workflowID, int? parentStepID, string description, string question, string trueAnswer, string falseAnswer)
        {
            bool saveSuccessful = true;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountid));
            expdata.sqlexecute.Parameters.AddWithValue("@workflowid", workflowID);

            if (parentStepID.HasValue == false)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@parentStepID", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@parentStepID", getSavedStepID(parentStepID.Value));
            }

            expdata.sqlexecute.Parameters.AddWithValue("@description", description);
            expdata.sqlexecute.Parameters.AddWithValue("@question", question);
            expdata.sqlexecute.Parameters.AddWithValue("@trueOption", trueAnswer);
            expdata.sqlexecute.Parameters.AddWithValue("@falseOption", falseAnswer);

            CurrentUser currentUser = cMisc.GetCurrentUser();
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }

            expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;

            expdata.ExecuteProc("dbo.saveWorkflowStepDecision");

            int workflowStepID = (int)expdata.sqlexecute.Parameters["@identity"].Value;

            lstSavedStepIDs.Add(workflowStepID);

            expdata.sqlexecute.Parameters.Clear();

            return saveSuccessful;
        }

        /// <summary>
        /// Saving a decision steps false step to the database
        /// </summary>
        /// <param name="workflowID"></param>
        /// <param name="parentStepID"></param>
        /// <param name="description"></param>
        /// <param name="question"></param>
        /// <param name="trueAnswer"></param>
        /// <param name="falseAnswer"></param>
        /// <param name="relatedStep"></param>
        /// <returns></returns>
        private bool SaveWorkflowDecisionFalseStep(int workflowID, int? parentStepID, string description, string question, string trueAnswer, string falseAnswer, int relatedStep)
        {
            bool saveSuccessful = true;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountid));

            expdata.sqlexecute.Parameters.AddWithValue("@workflowID", workflowID);

            if (parentStepID.HasValue == false)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@parentStepID", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@parentStepID", getSavedStepID(parentStepID.Value));
            }

            expdata.sqlexecute.Parameters.AddWithValue("@description", description);
            expdata.sqlexecute.Parameters.AddWithValue("@question", question);
            expdata.sqlexecute.Parameters.AddWithValue("@trueOption", trueAnswer);
            expdata.sqlexecute.Parameters.AddWithValue("@falseOption", falseAnswer);
            expdata.sqlexecute.Parameters.AddWithValue("@relatedStep", relatedStep);

            CurrentUser currentUser = cMisc.GetCurrentUser();
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }

            expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;

            expdata.ExecuteProc("dbo.SaveWorkflowStepDecisionFalse");

            int workflowStepID = (int)expdata.sqlexecute.Parameters["@identity"].Value;

            lstSavedStepIDs.Add(workflowStepID);

            expdata.sqlexecute.Parameters.Clear();
            return saveSuccessful;
        }

        /// <summary>
        /// Saving an approval step to the database.
        /// </summary>
        /// <param name="workflowID">workflowID this step belongs too.</param>
        /// <param name="parentStepID">Parent stepID or null if no parent</param>
        /// <param name="description">Description of the step</param>
        /// <param name="approverType">WorkflowEntityType enum passed as an int from JavaScript</param>
        /// <param name="approverID">approverID, related to approverType, if its for a specific team, employee etc</param>
        /// <param name="involvement"></param>
        /// <param name="useOneClickSignoff">If to use one click signoff</param>
        /// <param name="showDeclaration">If to show the declaration upon approving</param>
        /// <param name="approvalEmailTemplateID">EmailTemplateID for this approval step</param>
        /// <param name="falseAnswer">False response for dialog</param>
        /// <param name="trueAnswer">True response for dialog</param>
        /// <param name="question">Question for dialog</param>
        /// <returns></returns>
        private bool SaveWorkflowApprovalStep(int workflowID, int? parentStepID, string description, int approverType, int? approverID, List<cWorkflowCriteria> involvement, bool useOneClickSignoff, bool showDeclaration, string question, string trueAnswer, string falseAnswer, int? approvalEmailTemplateID, string message, int? relatedStepID)
        {
            bool saveSuccessful = true;

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountid));

            expdata.sqlexecute.Parameters.AddWithValue("@workflowid", workflowID);
            if (parentStepID.HasValue == false) { expdata.sqlexecute.Parameters.AddWithValue("@parentStepID", DBNull.Value); } else { expdata.sqlexecute.Parameters.AddWithValue("@parentStepID", getSavedStepID(parentStepID.Value)); }
            if (relatedStepID.HasValue == false) { expdata.sqlexecute.Parameters.AddWithValue("@relatedStepID", DBNull.Value); } else { expdata.sqlexecute.Parameters.AddWithValue("@relatedStepID", relatedStepID.Value); }
            if (string.IsNullOrEmpty(question) == true) { expdata.sqlexecute.Parameters.AddWithValue("@question", DBNull.Value); } else { expdata.sqlexecute.Parameters.AddWithValue("@question", question); }
            if (string.IsNullOrEmpty(trueAnswer) == true) { expdata.sqlexecute.Parameters.AddWithValue("@trueResponse", DBNull.Value); } else { expdata.sqlexecute.Parameters.AddWithValue("@trueResponse", trueAnswer); }
            if (string.IsNullOrEmpty(falseAnswer) == true) { expdata.sqlexecute.Parameters.AddWithValue("@falseResponse", DBNull.Value); } else { expdata.sqlexecute.Parameters.AddWithValue("@falseResponse", falseAnswer); }
            if (approvalEmailTemplateID.HasValue) { expdata.sqlexecute.Parameters.AddWithValue("@emailTemplateID", approvalEmailTemplateID.Value); } else { expdata.sqlexecute.Parameters.AddWithValue("@emailTemplateID", DBNull.Value); }
            if (approverID.HasValue == true) { expdata.sqlexecute.Parameters.AddWithValue("@approverID", approverID); } else { expdata.sqlexecute.Parameters.AddWithValue("@approverID", DBNull.Value); }
            if (involvement.Count > 0) { expdata.sqlexecute.Parameters.AddWithValue("@filterItems", true); } else { expdata.sqlexecute.Parameters.AddWithValue("@filterItems", false); }
            expdata.sqlexecute.Parameters.AddWithValue("@description", description);
            expdata.sqlexecute.Parameters.AddWithValue("@approverType", approverType);
            expdata.sqlexecute.Parameters.AddWithValue("@useOneClickSignoff", useOneClickSignoff);
            expdata.sqlexecute.Parameters.AddWithValue("@showQuestionDialog", showDeclaration);

            if (string.IsNullOrEmpty(message) == true) { expdata.sqlexecute.Parameters.AddWithValue("@message", DBNull.Value); } else { expdata.sqlexecute.Parameters.AddWithValue("@message", message); }

            CurrentUser currentUser = cMisc.GetCurrentUser();
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }

            expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("dbo.saveWorkflowStepApproval");
            int workflowStepID = (int)expdata.sqlexecute.Parameters["@identity"].Value;

            lstSavedStepIDs.Add(workflowStepID);
            expdata.sqlexecute.Parameters.Clear();
            SaveWorkflowStepCriteria(workflowID, workflowStepID, involvement);
            return saveSuccessful;
        }

        /// <summary>
        /// Save a move to step step to the database.
        /// </summary>
        /// <param name="workflowID">workflowID this step belongs too.</param>
        /// <param name="parentStepID">Parent stepID or null if no parent</param>
        /// <param name="description">Description of the step</param>
        /// <param name="moveToStepID">The moveToStepID to move to in the workflow</param>
        /// <returns></returns>
        private bool SaveWorkflowMoveToStepStep(int workflowID, int? parentStepID, string description, int moveToStepID)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountid));

            expdata.sqlexecute.Parameters.AddWithValue("@workflowid", workflowID);

            if (parentStepID.HasValue == false)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@parentStepID", DBNull.Value);
            }
            else
            {
                //expdata.sqlexecute.Parameters.AddWithValue("@parentStepID", parentStepID);
                expdata.sqlexecute.Parameters.AddWithValue("@parentStepID", getSavedStepID(parentStepID.Value));
            }

            expdata.sqlexecute.Parameters.AddWithValue("@actionID", moveToStepID);
            expdata.sqlexecute.Parameters.AddWithValue("@description", description);


            CurrentUser currentUser = cMisc.GetCurrentUser();
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }


            expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("dbo.saveWorkflowStepMoveToStep");
            int workflowStepID = (int)expdata.sqlexecute.Parameters["@identity"].Value;
            lstSavedStepIDs.Add(workflowStepID);
            expdata.sqlexecute.Parameters.Clear();
            return true;
        }

        /// <summary>
        /// Save a run sub workflow step to the database
        /// </summary>
        /// <param name="workflowID">workflowID this step belongs too.</param>
        /// <param name="parentStepID">Parent stepID or null if no parent</param>
        /// <param name="description">Description of the step</param>
        /// <param name="subWorkflowID">The subworkflow to run</param>
        /// <returns></returns>
        private bool SaveWorkflowRunSubWorkflowStep(int workflowID, int? parentStepID, string description, int subWorkflowID)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountid));

            expdata.sqlexecute.Parameters.AddWithValue("@workflowid", workflowID);

            if (parentStepID.HasValue == false)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@parentStepID", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@parentStepID", getSavedStepID(parentStepID.Value));
            }

            expdata.sqlexecute.Parameters.AddWithValue("@description", description);
            expdata.sqlexecute.Parameters.AddWithValue("@actionID", subWorkflowID);

            CurrentUser currentUser = cMisc.GetCurrentUser();
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }


            expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;

            expdata.ExecuteProc("dbo.saveWorkflowStepRunSubWorkflow");

            int workflowStepID = (int)expdata.sqlexecute.Parameters["@identity"].Value;

            lstSavedStepIDs.Add(workflowStepID);

            expdata.sqlexecute.Parameters.Clear();

            return true;
        }

        /// <summary>
        /// Saves a workflow step criteria to the database
        /// </summary>
        /// <param name="workflowID"></param>
        /// <param name="workflowStepID"></param>
        /// <param name="fieldID"></param>
        /// <param name="oper"></param>
        /// <param name="value"></param>
        /// <param name="value2"></param>
        /// <param name="runtime"></param>
        /// <param name="update"></param>
        /// <param name="replaceElements"></param>
        /// <returns></returns>
        public bool SaveWorkflowStepCriteriaExec(int workflowID, int workflowStepID, Guid fieldID, Int16 oper, string value, string value2, bool runtime, bool update, bool replaceElements)
        {
            if ((oper != 8 && value != null) || (oper == 8 && value != null && value2 != null))
            {
                DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountid));
                expdata.sqlexecute.Parameters.AddWithValue("@workflowID", workflowID);
                expdata.sqlexecute.Parameters.AddWithValue("@workflowStepID", workflowStepID);
                expdata.sqlexecute.Parameters.AddWithValue("@fieldID", fieldID);
                expdata.sqlexecute.Parameters.AddWithValue("@operator", oper);
                expdata.sqlexecute.Parameters.AddWithValue("@valueOne", value);

                if (oper == 8 && string.IsNullOrEmpty(value2) != false)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@valueTwo", value2);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@valueTwo", DBNull.Value);
                }
                expdata.sqlexecute.Parameters.AddWithValue("@runtime", runtime);
                expdata.sqlexecute.Parameters.AddWithValue("@updateCriteria", update);
                expdata.sqlexecute.Parameters.AddWithValue("@replaceElements", replaceElements);

                CurrentUser currentUser = cMisc.GetCurrentUser();
                expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }

                expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
                expdata.ExecuteProc("dbo.saveWorkflowStepCriteria");
                int workflowCriteriaID = (int)expdata.sqlexecute.Parameters["@identity"].Value;
                expdata.sqlexecute.Parameters.Clear();
            }
            return true;
        }

        /// <summary>
        /// Loops through a list of cWorkflowCriteria and adds them to the database
        /// </summary>
        /// <param name="workflowID"></param>
        /// <param name="workflowStepID"></param>
        /// <param name="lstCriteria"></param>
        private void SaveWorkflowStepCriteria(int workflowID, int workflowStepID, List<cWorkflowCriteria> lstCriteria)
        {
            foreach (cWorkflowCriteria criteria in lstCriteria)
            {
                SaveWorkflowStepCriteriaExec(workflowID, workflowStepID, criteria.Field.FieldID, (Int16)criteria.Condition, criteria.Value, criteria.ValueTwo, criteria.Runtime, criteria.IsUpdateCriteria, criteria.ReplaceElements);
            }
        }

        /// <summary>
        /// Get a cWorkflow from cache
        /// </summary>
        /// <param name="workflowID">The workflowID of the workflow you want to return</param>
        /// <returns>An instance of the requested cWorkflow</returns>
        public cWorkflow GetWorkflowByID(int workflowID)
        {
            cWorkflow reqWorkflow = null;

            lstCachedWorkflows.TryGetValue(workflowID, out reqWorkflow);

            return reqWorkflow;
        }

        /// <summary>
        /// Delete a workflow from the database
        /// </summary>
        /// <param name="workflowID">The workflowID of the workflow you want to delete</param>
        public bool DeleteWorkflowByID(int workflowID)
        {
            if (WorkflowInUse(workflowID) == true)
            {
                return false;
            }
            else
            {
                DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountid));
                expdata.sqlexecute.Parameters.AddWithValue("@workflowID", workflowID);

                CurrentUser currentUser = cMisc.GetCurrentUser();
                expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }

                expdata.ExecuteProc("dbo.deleteWorkflow");
                ResetCache();
                return true;
            }
        }

        /// <summary>
        /// Returns true if a workflow is in use, i.e. an entity is currently using it
        /// </summary>
        /// <param name="workflowID"></param>
        /// <returns></returns>
        public bool WorkflowInUse(int workflowID)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountid));
            expdata.sqlexecute.Parameters.AddWithValue("@workflowID", workflowID);

            expdata.sqlexecute.Parameters.Add("@entityCount", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@entityCount"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("dbo.entitiesInWorkflow");
            int entityCount = (int)expdata.sqlexecute.Parameters["@entityCount"].Value;

            if (entityCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Get the current status of an entity in the workflow, returns an instance of cWorkflowEntityDetails with the stepID, ownerID and approverID if set
        /// </summary>
        /// <param name="entityID"></param>
        /// <param name="workflowID"></param>
        /// <returns></returns>
        public cWorkflowEntityDetails GetCurrentEntityStatus(int entityID, int workflowID)
        {
            cWorkflowEntityDetails reqDetails = null;

            cEmployees clsEmployees = new cEmployees(nAccountid);

            string strSQL = "SELECT stepID, ownerID, approverID FROM workflowEntityState WHERE entityID=@entityID AND workflowID=@workflowID";

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountid));

            expdata.sqlexecute.Parameters.AddWithValue("@entityID", entityID);
            expdata.sqlexecute.Parameters.AddWithValue("@workflowID", workflowID);

            using (System.Data.SqlClient.SqlDataReader reader = expdata.GetReader(strSQL))
            {
                while (reader.Read())
                {
                    int stepID = reader.GetInt32(0);
                    int ownerID = reader.GetInt32(1);
                    Employee reqOwner = clsEmployees.GetEmployeeById(ownerID);

                    Employee reqApprover;
                    int approverID;
                    if (reader.IsDBNull(2))
                    {
                        approverID = -1;
                        reqApprover = null;
                    }
                    else
                    {
                        approverID = reader.GetInt32(2);
                        reqApprover = clsEmployees.GetEmployeeById(approverID);
                    }

                    reqDetails = new cWorkflowEntityDetails(entityID, workflowID, stepID, reqOwner, reqApprover);
                }
                reader.Close();
            }
            expdata.sqlexecute.Parameters.Clear();
            return reqDetails;
        }

        /// <summary>
        /// Get the current step an entity is on in a workflow, returns the step number or -1 if not found in a workflow
        /// </summary>
        /// <param name="entityID">The entityID that you want</param>
        /// <param name="workflowID">The workflowID that the entity is in</param>
        /// <returns>step number or -1 if not found in a workflow</returns>
        public int GetCurrentWorkflowStepNumber(int entityID, int workflowID)
        {
            int stepID = -1;

            string strSQL = "SELECT stepID FROM workflowEntityState WHERE workflowID=@workflowID AND entityID=@entityID";

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountid));

            expdata.sqlexecute.Parameters.AddWithValue("@workflowID", workflowID);
            expdata.sqlexecute.Parameters.AddWithValue("@entityID", entityID);

            using (System.Data.SqlClient.SqlDataReader reader = expdata.GetReader(strSQL))
            {
                while (reader.Read())
                {
                    stepID = reader.GetInt32(0);
                }

                reader.Close();
            }
            expdata.sqlexecute.Parameters.Clear();

            return stepID;
        }

        /// <summary>
        /// Increments an entities stepNumber
        /// </summary>
        /// <param name="entityID">The entityID that you want to update</param>
        /// <param name="workflowID">The workflowID that the entity is in</param>
        public int IncrementEntityStepNumber(int entityID, int workflowID)
        {
            cWorkflow reqWorkflow = GetWorkflowByID(workflowID);

            if (reqWorkflow == null || EntityInWorkflow(entityID, workflowID) == false)
            {
                return -1;
            }

            cWorkflowEntityDetails reqWorkflowStepEntityDetails = GetCurrentEntityStatus(entityID, workflowID);
            int nextStepID = reqWorkflow.Steps.IndexOfKey(reqWorkflowStepEntityDetails.StepNumber);
            nextStepID++;

            nextStepID = AdvanceToNextStep(entityID, workflowID, null, null);
            nextStepID = reqWorkflow.Steps.IndexOfKey(nextStepID);

            if (reqWorkflow.Steps.Count > nextStepID)
            {
                return UpdateEntityStepNumber(entityID, workflowID, reqWorkflow.Steps.Values[nextStepID].WorkflowStepID);
            }
            else
            {
                RemoveFromWorkflow(workflowID, entityID);
            }

            return -1;
        }

        /// <summary>
        /// Updates an entities stepNumber with the specified data, -1 if workflow is not found, -2 if the step is not found, 1 if the sql is executed
        /// </summary>
        /// <param name="entityID">The entityID that you want to update</param>
        /// <param name="workflowID">The workflowID that the entity is in</param>
        /// <param name="stepNumber">The step number you want to move this entity too</param>
        /// <returns>-1 if workflow is not found, -2 if the step is not found, 1 if the sql is executed</returns>
        private int UpdateEntityStepNumber(int entityID, int workflowID, int stepNumber)
        {
            cWorkflow reqWorkflow = reqWorkflow = GetWorkflowByID(workflowID);

            if (reqWorkflow != null)
            {
                if (reqWorkflow.Steps.ContainsKey(stepNumber))
                {
                    //for (int i = stepNumber; i < reqWorkflow.Steps.Count; i++)
                    //{
                    //    if (reqWorkflow.Steps[i].Action != WorkFlowStepAction.DecisionFalse && reqWorkflow.Steps[i].Action != WorkFlowStepAction.ElseCondition && reqWorkflow.Steps[i].Action != WorkFlowStepAction.ElseOtherwise)
                    //    {
                    //        stepNumber = i;
                    //        break;
                    //    }
                    //}
                    DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountid));
                    expdata.sqlexecute.Parameters.AddWithValue("@stepID", stepNumber);
                    expdata.sqlexecute.Parameters.AddWithValue("@workflowID", workflowID);
                    expdata.sqlexecute.Parameters.AddWithValue("@entityID", entityID);
                    string strSQL = "UPDATE workflowEntityState SET stepID=@stepID WHERE workflowID=@workflowID AND entityID=@entityID";
                    expdata.ExecuteSQL(strSQL);
                    expdata.sqlexecute.Parameters.Clear();
                    System.Diagnostics.EventLog.WriteEntry("Application", "Workflow moved step\nWorkflowID: " + workflowID + "\nEntityID: " + entityID + "\nStepID:" + stepNumber, System.Diagnostics.EventLogEntryType.Information, 2);
                    return 1;
                }
                else
                {
                    return -2;
                }
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Updates an entities workflowID with a new one
        /// </summary>
        /// <param name="entityID">The entityID that you want to update</param>
        /// <param name="workflowID">The workflowID that the entity is in</param>
        /// <param name="newWorkflowID">The workflowID that you want to move this entity onto</param>
        /// <returns>-1 if workflow is not found, -2 if the new workflow is not found, 1 if the sql is executed</returns>
        private int UpdateEntityWorkflowID(int entityID, int workflowID, int newWorkflowID)
        {
            cWorkflow reqCurrentWorkflow = GetWorkflowByID(workflowID);
            cWorkflow reqNewWorkflow = GetWorkflowByID(newWorkflowID);

            if (reqCurrentWorkflow != null)
            {
                if (reqNewWorkflow != null)
                {
                    DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountid));
                    expdata.sqlexecute.Parameters.AddWithValue("@newWorkflowID", newWorkflowID);
                    expdata.sqlexecute.Parameters.AddWithValue("@workflowID", workflowID);
                    expdata.sqlexecute.Parameters.AddWithValue("@entityID", entityID);
                    string strSQL = "UPDATE workflowEntityState SET workflowID=@newWorkflowID, workflowStepID=1 WHERE workflowID=@workflowID AND entityID=@entityID";
                    expdata.ExecuteSQL(strSQL);
                    expdata.sqlexecute.Parameters.Clear();
                    return 1;
                }
                else
                {
                    return -2;
                }
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Gets the next next in the workflow for an entity, returns a status and the next step if there is one, or a status with a null step if there is not one. If it encounters a step that is done automatically, i.e. no user input it will complete this step and move onto the next step
        /// </summary>
        /// <param name="entityID">The entityID you want to find the next step for</param>
        /// <param name="workflowID">The workflowID the entity is in</param>
        /// <returns></returns>
        public cWorkflowNextStep GetNextWorkflowStep(int entityID, int workflowID)
        {
            cWorkflowNextStep reqWorkflowStep = null;

            while (reqWorkflowStep == null || reqWorkflowStep.Status == WorkflowStatus.AutomaticStep)
            {
                reqWorkflowStep = GetNextStep(entityID, workflowID);
            }

            if (reqWorkflowStep.Status == WorkflowStatus.Finished)
            {
                RemoveFromWorkflow(workflowID, entityID);
            }

            return reqWorkflowStep;
        }

        /// <summary>
        /// Checks to see if any queries are in the value, if matches are found a query is generated to lookup the value for that guid, inner joining the entityID onto the query to limit it. If no guids are found the valueString remains untouched
        /// </summary>
        /// <param name="valueString"></param>
        /// <returns></returns>
        private string ParseValueStringForGuids(string valueString, cTable baseTable, int entityID)
        {
            Regex regex = new Regex("[a-z0-9]{8}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{12}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(valueString);

            if (matches.Count > 0)
            {
                string newValue = valueString;
                cFields clsFields = new cFields(nAccountid);
                cQueryBuilder clsQueryBuilder = new cQueryBuilder(nAccountid, cAccounts.getConnectionString(nAccountid), ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, baseTable, new cTables(nAccountid), clsFields);
                cField tmpField;
                List<cField> lstMatchedFields = new List<cField>();
                foreach (Match match in matches)
                {
                    tmpField = clsFields.GetFieldByID(new Guid(match.ToString()));
                    if (tmpField != null)
                    {
                        clsQueryBuilder.addColumn(tmpField);
                        lstMatchedFields.Add(tmpField);
                    }
                }

                clsQueryBuilder.addFilter(baseTable.GetPrimaryKey(), ConditionType.Equals, new object[] { entityID }, null, ConditionJoiner.And, null); // null as pk on bt? !!!!!!!!

                DataSet ds = clsQueryBuilder.getDataset();

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Columns.Count > 0)
                {
                    int matchedFieldCounter = 0;
                    for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                    {
                        if (ds.Tables[0].Rows[0][i].ToString().EndsWith("_text") == false)
                        {
                            matchedFieldCounter++;
                            newValue = newValue.Replace("[" + lstMatchedFields[matchedFieldCounter].FieldID.ToString() + "]", Convert.ToString(ds.Tables[0].Rows[0][i]));
                        }
                    }
                }

                return newValue;
            }
            else
            {
                return valueString;
            }
        }

        /// <summary>
        /// Processes a conditional step
        /// </summary>
        /// <param name="entityID"></param>
        /// <param name="workflowID"></param>
        /// <param name="workflow"></param>
        /// <param name="reqEntDetails"></param>
        /// <returns></returns>
        private cWorkflowNextStep ProcessConditionStep(int entityID, int workflowID, cWorkflow workflow, cWorkflowEntityDetails reqEntDetails)
        {
            cWorkflowEntityDetails reqEntityDetails = GetCurrentEntityStatus(entityID, workflowID);
            cWorkflowStep reqWorkflowStep = workflow.Steps[reqEntDetails.StepNumber];
            cWorkflowNextStep reqNextStep = new cWorkflowNextStep(WorkflowStatus.AutomaticStep, reqWorkflowStep);
            cCheckConditionStep reqCheckConditionStep = (cCheckConditionStep)reqWorkflowStep;

            if (reqCheckConditionStep.Action == WorkFlowStepAction.CheckCondition || reqCheckConditionStep.Action == WorkFlowStepAction.ElseCondition)
            {
                if (reqCheckConditionStep.Criteria != null)
                {
                    cQueryBuilder clsQueryBuilder = new cQueryBuilder(nAccountid, cAccounts.getConnectionString(nAccountid), ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, workflow.BaseTable, new cTables(nAccountid), new cFields(nAccountid));
                    object[] values;
                    object[] values2;



                    foreach (cWorkflowCriteria criteria in reqCheckConditionStep.Criteria)
                    {

                        // if we find guids in field 





                        values = new object[] { this.ParseValueStringForGuids(criteria.Value, workflow.BaseTable, entityID) };
                        values2 = new object[] { this.ParseValueStringForGuids(criteria.ValueTwo, workflow.BaseTable, entityID) };
                        ////format date values before Query Builder is executed for comparing operation
                        DateTime dynamicDateAttribute;
                        if (DateTime.TryParse(values[0].ToString(), out dynamicDateAttribute))
                        {
                            values[0] = dynamicDateAttribute.ToString("yyyy-MM-dd H:mm:ss", CultureInfo.InvariantCulture);
                        }

                        if (criteria.Field != null)
                        {
                            clsQueryBuilder.addColumn(criteria.Field, SpendManagementLibrary.SelectType.Fields);
                            if (criteria.Field.FieldName.Contains("dbo.") == false)
                            {
                                clsQueryBuilder.addFilter(criteria.Field, criteria.Condition, values, values2, ConditionJoiner.And, null);  // null as no vias in workflow yet but may need to be changed?   !!!!!!
                            }
                        }
                        else
                        {
                            throw new Exception("Unable to find the field when trying to build update query. (workflowid: " + workflowID + ")");
                        }
                    }

                    clsQueryBuilder.addFilter(workflow.BaseTable.GetPrimaryKey(), ConditionType.Equals, new object[] { entityID }, null, ConditionJoiner.And, null); // null as pk on bt? !!!!!!!

                    clsQueryBuilder.selectType = SpendManagementLibrary.SelectType.Count;

                    bool checkConditionsMet = false;
                    using (System.Data.SqlClient.SqlDataReader reader = clsQueryBuilder.getReader())
                    {
                        checkConditionsMet = reader.HasRows;

                        if (reader.HasRows)
                        {
                            reader.Read();
                            if (reader.IsDBNull(0) == true)
                            {
                                checkConditionsMet = false;
                            }
                        }
                        reader.Close();
                    }
                    //if (checkConditionsMet == true)
                    //{
                    //    IncrementEntityStepNumber(entityID, workflowID);
                    //}
                    //else // conditions have not been met
                    //{
                    //int nextValidStep = GetNextNonSubStep(entityID, workflowID, reqEntDetails.StepNumber);
                    int nextValidStep = AdvanceToNextStep(entityID, workflowID, checkConditionsMet, null);
                    if (nextValidStep > 0)
                    {
                        int updateStepResult = UpdateEntityStepNumber(entityID, workflowID, nextValidStep);
                        if (updateStepResult == -2)
                        {
                            reqNextStep = new cWorkflowNextStep(WorkflowStatus.StepNotFound, null);
                        }
                        else
                        {
                            reqNextStep = new cWorkflowNextStep(WorkflowStatus.AutomaticStep, workflow.Steps[nextValidStep]);
                        }
                    }
                    else
                    {
                        reqNextStep = new cWorkflowNextStep(WorkflowStatus.Finished, null);
                        //throw new Exception("An error occured trying to get the next valid substep: AdvanceToNextStep(" + entityID + ", " + workflowID + ", " + reqEntDetails.StepNumber + ");");
                    }
                    //}
                }
            }
            else
            {
                int nextValidStep = AdvanceToNextStep(entityID, workflowID, true, null);
                if (nextValidStep > 0)
                {
                    int updateStepResult = UpdateEntityStepNumber(entityID, workflowID, nextValidStep);
                    if (updateStepResult == -2)
                    {
                        reqNextStep = new cWorkflowNextStep(WorkflowStatus.StepNotFound, null);
                    }
                    else
                    {
                        if (workflow.Steps[nextValidStep].Action == WorkFlowStepAction.Approval || workflow.Steps[nextValidStep].Action == WorkFlowStepAction.Decision || workflow.Steps[nextValidStep].Action == WorkFlowStepAction.ChangeValue)
                        {

                            WorkflowStatus workflowStatus = WorkflowStatus.RequireUserInput;

                            if (workflow.Steps[nextValidStep].Action == WorkFlowStepAction.ChangeValue)
                            {
                                int runtimeCriteria = 0;
                                int autoCriteria = 0;
                                foreach (cWorkflowCriteria criteria in ((cChangeValueStep)workflow.Steps[nextValidStep]).Criteria)
                                {
                                    if (criteria.Runtime == true)
                                    {
                                        runtimeCriteria++;
                                    }
                                    else
                                    {
                                        autoCriteria++;
                                    }
                                }

                                if (runtimeCriteria == 0)
                                {
                                    workflowStatus = WorkflowStatus.AutomaticStep;
                                }
                            }

                            reqNextStep = new cWorkflowNextStep(workflowStatus, workflow.Steps[nextValidStep]);
                        }
                        else
                        {
                            reqNextStep = new cWorkflowNextStep(WorkflowStatus.AutomaticStep, workflow.Steps[nextValidStep]);
                        }
                    }
                }
                else
                {
                    throw new Exception("An error occured trying to get the next valid substep: AdvanceToNextStep(" + entityID + ", " + workflowID + ", " + reqEntDetails.StepNumber + ");");
                }
            }

            return reqNextStep;
        }


        private cWorkflowNextStep ProcessChangeValueStep(cChangeValueStep reqChangeValueStep, cWorkflow reqWorkflow, int entityID)
        {
            cWorkflowNextStep reqNextStep = null;

            #region check if any are runtime changes

            bool anyRuntimeCriteria = false;
            List<cWorkflowCriteria> lstAutoUpdates = new List<cWorkflowCriteria>();

            if (reqChangeValueStep.Criteria != null)
            {
                foreach (cWorkflowCriteria criteria in reqChangeValueStep.Criteria)
                {
                    if (criteria.Runtime == true)
                    {
                        anyRuntimeCriteria = true;
                    }
                    else
                    {
                        lstAutoUpdates.Add(criteria);
                    }
                }
            }

            if (anyRuntimeCriteria == true)
            {
                reqNextStep = new cWorkflowNextStep(WorkflowStatus.RequireUserInput, reqChangeValueStep);
            }
            else
            {
                reqNextStep = new cWorkflowNextStep(WorkflowStatus.AutomaticStep, reqChangeValueStep);
            }
            #endregion
            cUpdateQuery clsUpdateQuery = new cUpdateQuery(nAccountid, cAccounts.getConnectionString(nAccountid), ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, reqWorkflow.BaseTable, new cTables(nAccountid), new cFields(nAccountid));

            #region Do autoupdates

            if (lstAutoUpdates.Count > 0)
            {
                foreach (cWorkflowCriteria criteria in lstAutoUpdates)
                {
                    if (criteria.Field != null)
                    {
                        if (criteria.ReplaceElements == true)
                        {
                            if (Regex.IsMatch(criteria.Value, "\\[(.*)\\]", RegexOptions.Compiled) == true)
                            {

                            }
                        }
                        else
                        {
                            clsUpdateQuery.addColumn(criteria.Field, criteria.Value);
                        }
                    }
                    else
                    {
                        throw new Exception("Unable to find the field when trying to build update query. (workflowid: " + reqWorkflow.workflowid + ")");
                    }
                }

                clsUpdateQuery.addFilter(reqWorkflow.BaseTable.GetPrimaryKey(), ConditionType.Equals, new object[] { entityID }, null, ConditionJoiner.And, null); // null as bt? !!!!!!

                try
                {
                    clsUpdateQuery.executeUpdateStatement();

                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to perform update for workflow update criteria step", ex);
                }

            }

            #endregion Do autoupdates

            if (anyRuntimeCriteria == false)
            {
                IncrementEntityStepNumber(entityID, reqWorkflow.workflowid);
            }

            return reqNextStep;
        }

        /// <summary>
        /// Performs the next steps action if no user input required or returns the step
        /// </summary>
        /// <param name="entityID"></param>
        /// <param name="workflowID"></param>
        /// <returns></returns>
        private cWorkflowNextStep GetNextStep(int entityID, int workflowID)
        {
            cWorkflowNextStep reqNextStep = null;
            cWorkflowStep reqWorkflowStep = null;
            cWorkflow reqWorkflow = GetWorkflowByID(workflowID);
            cRunSubworkflowStep reqRunSubWorkflowStep;
            cMovetoStepStep reqMoveToStepStep;
            cChangeValueStep reqChangeValueStep;
            cSendEmailStep reqEmailStep;
            //cDecisionStep reqDecisionStep;
            cApprovalStep reqApprovalStep;
            cCreateDynamicValue reqCreateValue;
            cChangeFormStep reqChangeFormstep;

            if (reqWorkflow != null)
            {
                cWorkflowEntityDetails reqEntDetails = null;
                reqEntDetails = GetCurrentEntityStatus(entityID, workflowID);

                if (reqEntDetails == null)
                {
                    return new cWorkflowNextStep(WorkflowStatus.Finished, null);
                }

                reqWorkflow.Steps.TryGetValue(reqEntDetails.StepNumber, out reqWorkflowStep);

                if (reqWorkflowStep != null)
                {
                    try
                    {
                        switch (reqWorkflowStep.Action)
                        {
                            case WorkFlowStepAction.ElseOtherwise:
                            case WorkFlowStepAction.ElseCondition:
                            case WorkFlowStepAction.CheckCondition:
                                reqNextStep = ProcessConditionStep(entityID, workflowID, reqWorkflow, reqEntDetails);
                                break;
                            case WorkFlowStepAction.MoveToStep:
                                #region MoveToStep Logic - /
                                reqNextStep = new cWorkflowNextStep(WorkflowStatus.AutomaticStep, reqWorkflowStep);
                                reqMoveToStepStep = (cMovetoStepStep)reqWorkflowStep;
                                UpdateEntityStepNumber(entityID, workflowID, reqMoveToStepStep.StepID);
                                break;
                            #endregion
                            case WorkFlowStepAction.RunSubWorkflow:
                                #region RunSubWorkflow Logic - /
                                reqNextStep = new cWorkflowNextStep(WorkflowStatus.AutomaticStep, reqWorkflowStep);
                                reqRunSubWorkflowStep = (cRunSubworkflowStep)reqWorkflowStep;

                                InsertIntoWorkflow(entityID, reqRunSubWorkflowStep.SubWorkflowID, reqEntDetails.EntityOwner.EmployeeID);
                                GetNextWorkflowStep(entityID, reqRunSubWorkflowStep.SubWorkflowID);

                                UpdateEntityWorkflowID(entityID, workflowID, reqRunSubWorkflowStep.SubWorkflowID);
                                break;
                            #endregion
                            case WorkFlowStepAction.SendEmail:
                                #region SendEmail Logic - /
                                reqNextStep = new cWorkflowNextStep(WorkflowStatus.AutomaticStep, reqWorkflowStep);
                                reqEmailStep = (cSendEmailStep)reqWorkflowStep;
                                if (reqEmailStep.EmailTemplateID > 0)
                                {
                                    sendEmailTemplate(reqEmailStep.EmailTemplateID, reqWorkflow.BaseTable.GetPrimaryKey(), entityID, reqEntDetails.EntityOwner.EmployeeID);
                                }
                                else
                                {
                                    System.Diagnostics.EventLog.WriteEntry("Application", "Workflow - Invalid emailID specified in SendEmail step. AccountID " + nAccountid + ", WorkflowID: " + workflowID + ", StepID: " + reqEmailStep.WorkflowStepID);
                                }

                                int nextStepID = AdvanceToNextStep(entityID, workflowID, null, null);
                                if (nextStepID > 0)
                                {
                                    int updateStepResult = UpdateEntityStepNumber(entityID, workflowID, nextStepID);
                                    if (updateStepResult == -2)
                                    {
                                        reqNextStep = new cWorkflowNextStep(WorkflowStatus.StepNotFound, null);
                                    }
                                }

                                //IncrementEntityStepNumber(entityID, workflowID);
                                break;
                            #endregion
                            case WorkFlowStepAction.ChangeValue:
                                #region ChangeValue Logic
                                reqChangeValueStep = (cChangeValueStep)reqWorkflowStep;
                                reqNextStep = ProcessChangeValueStep(reqChangeValueStep, reqWorkflow, entityID);
                                break;
                            #endregion
                            case WorkFlowStepAction.Approval:
                                #region approval logic
                                reqApprovalStep = (cApprovalStep)reqWorkflowStep;
                                bool moveToNextStep = false;
                                switch (reqApprovalStep.ApproverType)
                                {
                                    case WorkflowEntityType.Employee:
                                    case WorkflowEntityType.Team:
                                    case WorkflowEntityType.BudgetHolder:
                                        if (reqApprovalStep.ApproverID > 0)
                                        {
                                            UpdateEntityState(workflowID, entityID, reqApprovalStep.ApproverID);
                                            moveToNextStep = true;
                                        }
                                        else
                                        {
                                            System.Diagnostics.EventLog.WriteEntry("Application", "Missing approvalID for approval step (workflowID: " + reqWorkflow.workflowid + " stepID: " + reqWorkflowStep.WorkflowStepID + ")");
                                            throw new Exception("Invalid approval step in workflow.");
                                        }
                                        break;
                                    case WorkflowEntityType.LineManager:
                                        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountid));
                                        expdata.sqlexecute.Parameters.AddWithValue("@employeeID", reqEntDetails.EntityOwner.EmployeeID);
                                        string strSQL = "SELECT linemanager FROM employees WHERE employeeID=@employeeID";

                                        int? approverID = null;
                                        using (SqlDataReader reader = expdata.GetReader(strSQL))
                                        {
                                            if (reader.HasRows)
                                            {

                                                while (reader.Read())
                                                {
                                                    if (reader.IsDBNull(0) == false)
                                                    {
                                                        approverID = reader.GetInt32(0);
                                                    }
                                                }
                                            }
                                            reader.Close();
                                        }

                                        expdata.sqlexecute.Parameters.Clear();
                                        if (approverID.HasValue)
                                        {
                                            UpdateEntityState(workflowID, entityID, approverID);
                                            moveToNextStep = true;
                                        }
                                        else
                                        {
                                            System.Diagnostics.EventLog.WriteEntry("Application", "Missing line manager for approval step (workflowID: " + reqWorkflow.workflowid + " stepID: " + reqWorkflowStep.WorkflowStepID + ")");
                                            throw new Exception("You currently have no line manager set.");
                                        }
                                        break;
                                    case WorkflowEntityType.DeterminedByClaimant:
                                    default:
                                        throw new Exception("Invalid approval step");
                                }

                                if (reqApprovalStep.EmailTemplateID.HasValue && moveToNextStep == true)
                                {
                                    sendEmailTemplate(reqApprovalStep.EmailTemplateID.Value, reqWorkflow.BaseTable.GetPrimaryKey(), entityID, reqEntDetails.EntityOwner.EmployeeID);
                                }

                                if (moveToNextStep)
                                {
                                    if (reqApprovalStep.RelatedStepID.HasValue)
                                    {
                                        reqNextStep = new cWorkflowNextStep(WorkflowStatus.AutomaticStep, reqWorkflowStep);
                                    }
                                    else
                                    {
                                        reqNextStep = new cWorkflowNextStep(WorkflowStatus.RequireUserInput, reqWorkflowStep);
                                    }
                                }
                                break;
                            #endregion approval logic
                            case WorkFlowStepAction.DecisionFalse:
                                #region Decision False
                                reqNextStep = new cWorkflowNextStep(WorkflowStatus.AutomaticStep, reqWorkflowStep);
                                IncrementEntityStepNumber(entityID, workflowID);
                                break;
                            #endregion Decision False
                            case WorkFlowStepAction.Decision:
                                #region Decision
                                reqNextStep = new cWorkflowNextStep(WorkflowStatus.RequireUserInput, reqWorkflowStep);
                                break;
                            #endregion Decision
                            case WorkFlowStepAction.FinishWorkflow:
                                #region Finish Workflow
                                reqNextStep = new cWorkflowNextStep(WorkflowStatus.Finished, null);
                                CompleteEntityWorkflowEntry(entityID, workflowID);
                                break;
                            #endregion Finish Workflow
                            case WorkFlowStepAction.CreateDynamicValue:
                                #region CreateDynamicValue
                                reqNextStep = new cWorkflowNextStep(WorkflowStatus.AutomaticStep, reqWorkflowStep);
                                reqCreateValue = (cCreateDynamicValue)reqWorkflowStep;

                                cFields clsFields = new cFields(nAccountid);
                                cField reqField = clsFields.GetFieldByID(reqCreateValue.DynamicValue.FieldID);

                                if (string.IsNullOrEmpty(reqCreateValue.DynamicValue.ValueFormula) == false && reqField != null)
                                {
                                    string dynamicValue = CreateDynamicValue(reqCreateValue.DynamicValue, reqWorkflow, entityID);
                                    ////format date values before Inserting
                                    DateTime dynamicDateAttribute;
                                    decimal checkForDecimalValue;
                                    if (!decimal.TryParse(dynamicValue, out checkForDecimalValue))
                                    {
                                        if (DateTime.TryParse(dynamicValue, out dynamicDateAttribute))
                                        {
                                           Guid validateFormulaForGuid;
                                           if ((reqField.FieldType == "DT" || reqField.FieldType == "D") && (reqCreateValue.DynamicValue.ValueFormula.IndexOf("[") > 0 && reqCreateValue.DynamicValue.ValueFormula.IndexOf("]") > 0)  && Guid.TryParse(reqCreateValue.DynamicValue.ValueFormula.Substring(reqCreateValue.DynamicValue.ValueFormula.IndexOf("[")+1, reqCreateValue.DynamicValue.ValueFormula.IndexOf("]") - reqCreateValue.DynamicValue.ValueFormula.IndexOf("[")-1), out validateFormulaForGuid))
                                            {
                                                dynamicValue = dynamicDateAttribute.ToString("yyyy-MM-dd H:mm:ss", CultureInfo.InvariantCulture);
                                            }
                                          
                                        }
                                    }

                                    cUpdateQuery clsUpdateQuery = new cUpdateQuery(nAccountid, cAccounts.getConnectionString(nAccountid), ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, reqWorkflow.BaseTable, new cTables(nAccountid), new cFields(nAccountid));
                                    clsUpdateQuery.addColumn(reqField, dynamicValue);
                                    clsUpdateQuery.addFilter(reqWorkflow.BaseTable.GetPrimaryKey(), ConditionType.Equals, new object[] { entityID }, null, ConditionJoiner.None, null); // null as pk on bt ? !!!!!!!
                                    clsUpdateQuery.executeUpdateStatement();
                                }

                                IncrementEntityStepNumber(entityID, workflowID);

                                break;
                            #endregion CreateDynamicValue
                            case WorkFlowStepAction.ShowMessage:
                                #region ShowMessage
                                reqNextStep = new cWorkflowNextStep(WorkflowStatus.RequireUserInput, reqWorkflowStep);
                                IncrementEntityStepNumber(entityID, workflowID);
                                break;
                            #endregion ShowMessage
                            case WorkFlowStepAction.ChangeCustomEntityForm:
                                reqChangeFormstep = (cChangeFormStep)reqWorkflowStep;
                                //reqNextStep = new cWorkflowNextStep(WorkflowStatus.FireAndForget, reqChangeFormstep);
                                reqNextStep = new cWorkflowNextStep(WorkflowStatus.RequireUserInput, reqChangeFormstep);
                                IncrementEntityStepNumber(entityID, workflowID);
                                break;
                            default:
                                reqNextStep = new cWorkflowNextStep(WorkflowStatus.InError, reqWorkflowStep);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        return new cWorkflowNextStep(WorkflowStatus.InError, new cWorkflowStep(0, 0, ex.Message, WorkFlowStepAction.ErrorWarning, 0, false, string.Empty, string.Empty, string.Empty, null, null));
                    }
                }
                else
                {
                    reqNextStep = new cWorkflowNextStep(WorkflowStatus.Finished, null);
                }
            }
            else
            {
                reqNextStep = new cWorkflowNextStep(WorkflowStatus.WorkflowNotFound, null);
            }


            return reqNextStep;
        }

        private string CreateDynamicValue(cWorkflowDynamicValue DynamicValue, cWorkflow workflow, int entityID)
        {
            cCalculationField clsCalculationField = new cCalculationField(nAccountid);
            return clsCalculationField.CalculatedColumn(DynamicValue.ValueFormula, workflow.BaseTable, entityID, new cFields(nAccountid), cAccounts.getConnectionString(nAccountid), new cTables(nAccountid));
        }


        /// <summary>
        /// Sends an email template from workflows (used in send email and approval steps)
        /// </summary>
        /// <param name="emailTemplateID">The email template ID the workflow should have sent</param>
        /// <param name="primaryKeyField">The primarykey of the table this workflow is based on</param>
        /// <param name="entityID">The entityID currently in the workflow</param>
        /// <param name="senderID">The owner of the entity, e.g. if a user created a an expense claim it will be his employeeID used here</param>
        public void sendEmailTemplate(int emailTemplateID, cField primaryKeyField, int entityID, int senderID)
        {
            cEmailTemplates clsEmailTemplates = new cEmailTemplates(this.oCurrentUser);
            clsEmailTemplates.SendMessage(emailTemplateID, primaryKeyField, entityID, senderID, "");
        }

        private void UpdateEntityState(int workflowID, int entityID, int? approverID)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountid));
            expdata.sqlexecute.Parameters.AddWithValue("@workflowID", workflowID);
            expdata.sqlexecute.Parameters.AddWithValue("@entityID", entityID);

            if (approverID.HasValue == true)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@approverID", @approverID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@approverID", DBNull.Value);
            }

            string strSQL = "UPDATE workflowEntityState SET approverID=@approverID  WHERE workflowID=@workflowID AND entityID=@entityID";
            expdata.ExecuteSQL(strSQL);
            expdata.sqlexecute.Parameters.Clear();

        }

        /// <summary>
        /// Ends an entities entry in workflowEntityState and thus removing it from workflows
        /// </summary>
        /// <param name="entityID">The entityID you want to delete from being in an active workflow</param>
        /// <param name="workflowID">The workflow you want to remove the entityID from</param>
        public void CompleteEntityWorkflowEntry(int entityID, int workflowID)
        {
            RemoveFromWorkflow(workflowID, entityID);
        }

        /// <summary>
        /// Check to see if entityID is already active in workflowID, returns true if it is or false if not
        /// </summary>
        /// <param name="entityID"></param>
        /// <param name="workflowID"></param>
        /// <returns></returns>
        public bool EntityInWorkflow(int entityID, int workflowID)
        {
            cWorkflowEntityDetails reqEntDetails = null;
            reqEntDetails = GetCurrentEntityStatus(entityID, workflowID);

            if (reqEntDetails != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a list of the workflows for use in a drop down list
        /// </summary>
        /// <param name="tableid"></param>
        /// <returns></returns>
        public List<ListItem> CreateDropDown(Guid tableid)
        {
            //List<ListItem> lst = new List<ListItem>();
            //SortedList<string, cWorkflow> workflows = sortList();
            //foreach (cWorkflow workflow in workflows.Values)
            //{

            //    if (workflow.BaseTable != null && workflow.BaseTable.TableID == tableid)
            //    {
            //        lst.Add(new ListItem(workflow.workflowname, workflow.workflowid.ToString()));
            //    }
            //}
            List<ListItem> lst = (from x in lstCachedWorkflows.Values
                                  where x.BaseTable != null && x.BaseTable.TableID == tableid
                                  orderby x.workflowname
                                  select new ListItem(x.workflowname, x.workflowid.ToString())).ToList();
            return lst;
        }

        /// <summary>
        /// Sorts the list of workflows in ascending order by workflow name
        /// </summary>
        /// <returns></returns>
        private SortedList<string, cWorkflow> sortList()
        {
            SortedList<string, cWorkflow> sorted = new SortedList<string, cWorkflow>();
            foreach (cWorkflow workflow in lstCachedWorkflows.Values)
            {
                sorted.Add(workflow.workflowname, workflow);
            }
            return sorted;
        }

        /// <summary>
        /// Insert an entity into a workflow at step 1, returns -1 if workflow not found, -2 if step not found, -3 if already in the workflow or 1 if added
        /// </summary>
        /// <param name="entityID">The entity you want to enter into a workflow</param>
        /// <param name="workflowID">The workflowID you want the entity to follow</param>
        /// <param name="ownerID">The owner of the entityID normally the person that created it</param>
        /// <returns>-1 if workflow not found, -2 if step not found or 1 if added</returns>
        public int InsertIntoWorkflow(int entityID, int workflowID, int ownerID)
        {
            return InsertIntoWorkflow(entityID, workflowID, null, ownerID);
        }

        /// <summary>
        /// Insert an entity into a workflow at the specified step, returns -1 if workflow not found, -2 if step not found, -3 if already in the workflow or 1 if added
        /// </summary>
        /// <param name="entityID">The entity you want to enter into a workflow</param>
        /// <param name="workflowID">The workflowID you want the entity to follow</param>
        /// <param name="ownerID">The owner of the entityID normally the person that created it</param>
        /// <param name="startStepID">Step to start the workflow on</param>
        /// <returns>-1 if workflow not found, -2 if step not found or 1 if added</returns>
        public int InsertIntoWorkflow(int entityID, int workflowID, int? startStepID, int ownerID)
        {
            if (EntityInWorkflow(entityID, workflowID) == false)
            {

                if (lstCachedWorkflows.ContainsKey(workflowID))
                {
                    cWorkflow reqWorkflow = GetWorkflowByID(workflowID);

                    if (startStepID.HasValue == false)
                    {
                        if (reqWorkflow.Steps.Count > 0)
                        {
                            startStepID = reqWorkflow.Steps.Values[0].WorkflowStepID;
                        }
                        else
                        {
                            return -2;
                        }
                    }


                    if (reqWorkflow.Steps.ContainsKey(startStepID.Value))
                    {
                        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountid));
                        expdata.sqlexecute.Parameters.AddWithValue("@workflowID", workflowID);
                        expdata.sqlexecute.Parameters.AddWithValue("@entityID", entityID);
                        expdata.sqlexecute.Parameters.AddWithValue("@stepID", startStepID.Value);
                        expdata.sqlexecute.Parameters.AddWithValue("@ownerID", ownerID);

                        string strSQL = "INSERT INTO workflowEntityState (workflowID, entityID, stepID, ownerID) VALUES (@workflowID, @entityID, @stepID, @ownerID);";
                        expdata.ExecuteSQL(strSQL);
                        expdata.sqlexecute.Parameters.Clear();
                        return 1;
                    }
                    else
                    {
                        return -2; // workflow step not found
                    }
                }
                else
                {
                    return -1; // workflow not found
                }
            }
            else
            {
                return -3; // entitiy already in a workflow
            }
        }

        /// <summary>
        /// Remove an entity from its current workflow (unsubmit an item etc)
        /// </summary>
        /// <param name="workflowID"></param>
        /// <param name="entityID"></param>
        public void RemoveFromWorkflow(int workflowID, int entityID)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountid));
            expdata.sqlexecute.Parameters.AddWithValue("@workflowID", workflowID);
            expdata.sqlexecute.Parameters.AddWithValue("@entityID", entityID);
            CurrentUser currentUser = cMisc.GetCurrentUser();
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            expdata.ExecuteProc("dbo.deleteEntityFromWorkflow");
            expdata.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// Returns a workflow for a specific base table
        /// </summary>
        /// <param name="tableID"></param>
        /// <returns></returns>
        public cWorkflow GetWorkflowByTableID(Guid tableID)
        {
            //cWorkflow reqWorkflow = null;
            //foreach (cWorkflow workflow in lstCachedWorkflows.Values)
            //{
            //    if (workflow.BaseTable != null && workflow.BaseTable.TableID == tableID)
            //    {
            //        reqWorkflow = workflow;
            //        break;
            //    }
            //}

            cWorkflow reqWorkflow = (from x in lstCachedWorkflows.Values
                                     where x.BaseTable != null && x.BaseTable.TableID == tableID
                                     select x).FirstOrDefault();

            return reqWorkflow;
        }

        /// <summary>
        /// Gets the workflowID of an entity based on the basetable and entityID, returns the workflowID or -1 if its not in a workflow
        /// </summary>
        /// <param name="baseTable"></param>
        /// <param name="entityID"></param>
        /// <returns></returns>
        public int GetWorkflowIDForEntity(cTable baseTable, int entityID)
        {
            int workflowID = -1;

            if (baseTable != null)
            {
                string strSQL = "SELECT workflows.workflowID FROM workflowEntityState INNER JOIN workflows ON workflows.workflowid = workflowEntityState.workflowid AND workflowEntityState.entityID=@entityID AND workflows.baseTableID=@baseTable";

                DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountid));
                expdata.sqlexecute.Parameters.AddWithValue("@entityID", entityID);
                expdata.sqlexecute.Parameters.AddWithValue("@baseTable", baseTable.TableID);
                using (System.Data.SqlClient.SqlDataReader reader = expdata.GetReader(strSQL))
                {
                    while (reader.Read())
                    {
                        workflowID = reader.GetInt32(0);
                    }

                    reader.Close();
                }
                expdata.sqlexecute.Parameters.Clear();
            }
            return workflowID;
        }

        /// <summary>
        /// Gets a list of all parent steps for a specific step up to its top node
        /// </summary>
        /// <param name="reqWorkflow"></param>
        /// <param name="reqWorkflowStep"></param>
        /// <returns></returns>
        private List<int> GetParentSteps(cWorkflow reqWorkflow, cWorkflowStep reqWorkflowStep)
        {
            List<int> lstParentSteps = new List<int>();

            bool foundTopLevel = false;
            int totalIterations = 0;

            int stepCounter = reqWorkflow.Steps.IndexOfKey(reqWorkflowStep.WorkflowStepID);

            while (foundTopLevel == false)
            {
                if (reqWorkflow.Steps.Values[stepCounter].ParentStepID == -1 || stepCounter < 1)
                {
                    if (lstParentSteps.Contains(reqWorkflow.Steps.Values[stepCounter].WorkflowStepID) == false)
                    {
                        lstParentSteps.Add(reqWorkflow.Steps.Values[stepCounter].WorkflowStepID);
                    }
                    foundTopLevel = true;
                    break;
                }
                else
                {
                    if (lstParentSteps.Contains(reqWorkflow.Steps.Values[stepCounter].ParentStepID) == false)
                    {
                        lstParentSteps.Add(reqWorkflow.Steps.Values[stepCounter].ParentStepID);
                    }
                }

                totalIterations++;
                stepCounter--;

                if (totalIterations > reqWorkflow.Steps.Count)
                {
                    throw new Exception("GetParentSteps in endless loop");
                }
            }

            return lstParentSteps;
        }

        /// <summary>
        /// AdvanceToNextStep
        /// </summary>
        /// <param name="entityID"></param>
        /// <param name="workflowID"></param>
        /// <param name="hasMatched">CheckCondition and ElseConditions need to pass if they have passed </param>
        /// <param name="hasDecided">Decision steps need to pass if the user has responded with a true or false response</param>
        /// <returns></returns>
        public int AdvanceToNextStep(int entityID, int workflowID, bool? hasMatched, bool? hasDecided)
        {
            cWorkflow reqWorkflow = GetWorkflowByID(workflowID);

            if (reqWorkflow != null)
            {
                if (EntityInWorkflow(entityID, workflowID) == true)
                {
                    cWorkflowEntityDetails reqEntityDetails = GetCurrentEntityStatus(entityID, workflowID);
                    cWorkflowStep reqCurrentWorkflowStep = reqWorkflow.Steps[reqEntityDetails.StepNumber];
                    int reqCurrentWorkflowStepIndexID = reqWorkflow.Steps.IndexOfKey(reqEntityDetails.StepNumber);
                    cWorkflowStep step;
                    List<cWorkflowStep> lstPreviousSteps = new List<cWorkflowStep>();

                    System.Diagnostics.Debug.WriteLine(reqCurrentWorkflowStepIndexID);
                    System.Diagnostics.Debug.WriteLine(reqCurrentWorkflowStep.WorkflowStepID);
                    System.Diagnostics.Debug.WriteLine(reqCurrentWorkflowStep.Action);
                    System.Diagnostics.Debug.WriteLine(reqCurrentWorkflowStep.Description);

                    for (int i = 0; i <= reqCurrentWorkflowStepIndexID; i++)
                    {
                        lstPreviousSteps.Add(reqWorkflow.Steps.Values[i]);
                    }

                    int nextStepKey = -1;

                    if (reqCurrentWorkflowStep.Action == WorkFlowStepAction.Decision)
                    {
                        #region handle decision steps
                        int decisionFalseStepID = -1;
                        int decisionFalseStepIndex = 0;
                        if (hasDecided.HasValue)// && hasDecided.Value == true)
                        {
                            for (int i = reqCurrentWorkflowStepIndexID + 1; i < reqWorkflow.Steps.Count; i++)
                            {
                                step = reqWorkflow.Steps.Values[i];

                                if (step.RelatedStepID == reqCurrentWorkflowStep.WorkflowStepID)
                                {
                                    decisionFalseStepID = step.WorkflowStepID;
                                    decisionFalseStepIndex = i;
                                    break;
                                }
                            }
                        }

                        for (int i = reqCurrentWorkflowStepIndexID + 1; i < reqWorkflow.Steps.Count; i++)
                        {
                            step = reqWorkflow.Steps.Values[i];
                            int parentStepsRelatedStepID;
                            if (reqWorkflow.Steps[step.ParentStepID].RelatedStepID.HasValue)
                            {
                                parentStepsRelatedStepID = reqWorkflow.Steps[step.ParentStepID].RelatedStepID.Value;
                            }



                            if (hasDecided.HasValue == true && (hasDecided == true && step.RelatedStepID != reqCurrentWorkflowStep.WorkflowStepID && GetParentSteps(reqWorkflow, step).Contains(decisionFalseStepID) == false) || (hasDecided == false && (step.ParentStepID == decisionFalseStepID || i > decisionFalseStepIndex + 1)))// && step.RelatedStepID == reqCurrentWorkflowStep.WorkflowStepID && GetParentSteps(reqWorkflow, step).Contains(reqCurrentWorkflowStep.WorkflowStepID) == false))
                            {
                                nextStepKey = step.WorkflowStepID;
                                break;

                            }
                        }
                        #endregion handle decision steps
                    }
                    else if (reqCurrentWorkflowStep.Action == WorkFlowStepAction.CheckCondition || reqCurrentWorkflowStep.Action == WorkFlowStepAction.ElseCondition)
                    {
                        #region handle check condition steps
                        bool validStep = false;
                        for (int i = reqCurrentWorkflowStepIndexID + 1; i < reqWorkflow.Steps.Count; i++)
                        {
                            validStep = false;
                            step = reqWorkflow.Steps.Values[i];
                            if (hasMatched.HasValue == true && (hasMatched.Value == false && (step.ParentStepID <= reqCurrentWorkflowStep.WorkflowStepID && ((step.RelatedStepID != null && step.RelatedStepID == reqCurrentWorkflowStep.RelatedStepID) || step.RelatedStepID == reqCurrentWorkflowStep.WorkflowStepID))) || (hasMatched.Value == true && (step.RelatedStepID != reqCurrentWorkflowStep.WorkflowStepID || step.ParentStepID == reqCurrentWorkflowStep.WorkflowStepID || (reqCurrentWorkflowStep.RelatedStepID != null && step.RelatedStepID != reqCurrentWorkflowStep.RelatedStepID)))) //(hasMatched.Value == true && ((step.RelatedStepID != null &&  step.RelatedStepID != reqCurrentWorkflowStep.RelatedStepID) && step.RelatedStepID != reqCurrentWorkflowStep.WorkflowStepID)))
                            {
                                if (step.Action != WorkFlowStepAction.DecisionFalse)
                                {
                                    //foreach (cWorkflowStep tmpStep in lstPreviousSteps)
                                    //{
                                    //if (tmpStep.RelatedStepID != null && tmpStep.RelatedStepID != step.RelatedStepID && tmpStep.WorkflowStepID != step.RelatedStepID)
                                    //{
                                    validStep = true;
                                    //}
                                    //}

                                    if (validStep == true)
                                    {
                                        nextStepKey = step.WorkflowStepID;
                                        break;
                                    }
                                }
                            }
                        }


                        if (nextStepKey == -1)
                        {
                            for (int i = reqCurrentWorkflowStepIndexID + 1; i < reqWorkflow.Steps.Count; i++)
                            {
                                step = reqWorkflow.Steps.Values[i];
                                if (step.ParentStepID <= reqCurrentWorkflowStep.ParentStepID && step.Action != WorkFlowStepAction.DecisionFalse)
                                {
                                    nextStepKey = step.WorkflowStepID;
                                    break;
                                }
                            }
                        }
                        #endregion handle check condition steps
                    }
                    else if (reqCurrentWorkflowStep.Action == WorkFlowStepAction.ElseOtherwise)
                    {
                        #region else otherwise
                        for (int i = (reqCurrentWorkflowStepIndexID + 1); i < reqWorkflow.Steps.Count; i++)
                        {
                            step = reqWorkflow.Steps.Values[i];
                            nextStepKey = step.WorkflowStepID;
                            break;
                        }
                        #endregion else otherwise
                    }
                    else if (reqCurrentWorkflowStep.Action == WorkFlowStepAction.Approval)
                    {
                        int reqApprovalFalseStep = 0;
                        for (int i = reqCurrentWorkflowStepIndexID + 1; i < reqWorkflow.Steps.Count; i++)
                        {
                            if (reqWorkflow.Steps.Values[i].RelatedStepID == reqCurrentWorkflowStep.WorkflowStepID)
                            {
                                reqApprovalFalseStep = i;
                            }
                        }




















                        if (hasDecided.HasValue)
                        {
                            if (hasDecided.Value == true)
                            {


                                if ((reqApprovalFalseStep - reqCurrentWorkflowStepIndexID) > 1)
                                {
                                    /// move to the imediate next step
                                    nextStepKey = reqWorkflow.Steps.Values[reqCurrentWorkflowStepIndexID + 1].WorkflowStepID;
                                }
                                else
                                {
                                    /// move to the next none approval false/decision false/else condition/otherwise
                                    for (int i = reqCurrentWorkflowStepIndexID + 1; i < reqWorkflow.Steps.Count; i++)
                                    {
                                        if (
                                            reqWorkflow.Steps.Values[i].Action != WorkFlowStepAction.ElseCondition && reqWorkflow.Steps.Values[i].Action != WorkFlowStepAction.ElseOtherwise &&
                                            reqWorkflow.Steps.Values[i].Action != WorkFlowStepAction.DecisionFalse &&
                                            (reqWorkflow.Steps.Values[i].Action != WorkFlowStepAction.Decision && reqWorkflow.Steps.Values[i].RelatedStepID.HasValue == true) &&
                                            (reqWorkflow.Steps.Values[i].Action != WorkFlowStepAction.Approval && reqWorkflow.Steps.Values[i].RelatedStepID.HasValue == true) &&
                                            /// some check to make sure its not under the decision false
                                            GetParentSteps(reqWorkflow, reqWorkflow.Steps.Values[i]).Contains(reqWorkflow.Steps.Values[reqApprovalFalseStep].WorkflowStepID) == false
                                        )
                                        {
                                            nextStepKey = reqWorkflow.Steps.Values[i].WorkflowStepID;
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                ///// move to the next none approval false/decision false/else condition/otherwise
                                //for (int i = reqApprovalFalseStep + 1; i < reqWorkflow.Steps.Count; i++)
                                //{
                                //    if (
                                //        reqWorkflow.Steps.Values[i].Action != WorkFlowStepAction.ElseCondition && reqWorkflow.Steps.Values[i].Action != WorkFlowStepAction.ElseOtherwise &&
                                //        reqWorkflow.Steps.Values[i].Action != WorkFlowStepAction.DecisionFalse &&
                                //        (reqWorkflow.Steps.Values[i].Action != WorkFlowStepAction.Decision && reqWorkflow.Steps.Values[i].RelatedStepID.HasValue == true) &&
                                //        (reqWorkflow.Steps.Values[i].Action != WorkFlowStepAction.Approval && reqWorkflow.Steps.Values[i].RelatedStepID.HasValue == true)
                                //    )
                                //    {
                                //        nextStepKey = reqWorkflow.Steps.Values[i].WorkflowStepID;
                                //        break;
                                //    }                                    
                                //}
                                if ((reqApprovalFalseStep + 1) < reqWorkflow.Steps.Count)
                                {
                                    nextStepKey = reqWorkflow.Steps.Values[reqApprovalFalseStep + 1].WorkflowStepID;
                                }
                            }
                        }
                        else
                        {
                            nextStepKey = reqWorkflow.Steps.Values[reqApprovalFalseStep].WorkflowStepID;
                        }














                    }
                    else
                    {
                        #region handle other steps
                        for (int i = reqCurrentWorkflowStepIndexID + 1; i < reqWorkflow.Steps.Count; i++)
                        {
                            step = reqWorkflow.Steps.Values[i];
                            if (step.ParentStepID <= reqCurrentWorkflowStep.ParentStepID && step.Action != WorkFlowStepAction.DecisionFalse)
                            {
                                nextStepKey = step.WorkflowStepID;
                                break;
                            }
                        }
                        #endregion handle other steps
                    }

                    return nextStepKey;
                }
                else
                {
                    // entity is not in this workflow
                    return -2;
                }
            }
            else
            {
                // workflow is not found
                return -1;
            }

        }

        public WorkflowStatus GetWorkflowStatusByStepType(WorkFlowStepAction stepAction)
        {
            if (stepAction == WorkFlowStepAction.Approval || stepAction == WorkFlowStepAction.Decision)
            {
                return WorkflowStatus.RequireUserInput;
            }
            else if (stepAction == WorkFlowStepAction.ChangeCustomEntityForm || stepAction == WorkFlowStepAction.ShowMessage)
            {
                return WorkflowStatus.FireAndForget;
            }
            else
            {
                return WorkflowStatus.AutomaticStep;
            }
        }

        /// <summary>
        /// Used to complete a decision step
        /// </summary>
        /// <param name="entityid"></param>
        /// <param name="workflowid"></param>
        /// <param name="response"></param>
        public cWorkflowNextStep UpdateDecisionStep(int entityID, int workflowID, bool response)
        {
            cWorkflowNextStep reqNextStep;
            int nextValidStep = AdvanceToNextStep(entityID, workflowID, null, response);
            if (nextValidStep > 0)
            {
                int updateStepResult = UpdateEntityStepNumber(entityID, workflowID, nextValidStep);
                reqNextStep = GetNextWorkflowStep(entityID, workflowID);
                //cWorkflow reqWorkflow = GetWorkflowByID(workflowID);
                //reqNextStep = new cWorkflowNextStep(GetWorkflowStatusByStepType(reqWorkflow.Steps[nextValidStep].Action), reqWorkflow.Steps[nextValidStep]);
            }
            else
            {
                reqNextStep = new cWorkflowNextStep(WorkflowStatus.Finished, null);
            }

            return reqNextStep;
        }

        public cWorkflowNextStep UpdateApprovalStep(int entityID, int workflowID, bool response, string message)
        {
            cWorkflowNextStep reqNextStep;
            this.UpdateEntityApprovalHistory(workflowID, entityID, response, message);

            int nextValidStep = this.AdvanceToNextStep(entityID, workflowID, null, response);
            if (nextValidStep > 0)
            {
                this.UpdateEntityStepNumber(entityID, workflowID, nextValidStep);
                this.UpdateEntityState(workflowID, entityID, null);
                reqNextStep = this.GetNextWorkflowStep(entityID, workflowID);
                //cWorkflow reqWorkflow = GetWorkflowByID(workflowID);
                //reqNextStep = new cWorkflowNextStep(GetWorkflowStatusByStepType(reqWorkflow.Steps[nextValidStep].Action), reqWorkflow.Steps[nextValidStep]);
            }
            else
            {
                reqNextStep = new cWorkflowNextStep(WorkflowStatus.Finished, null);
            }


            return reqNextStep;
        }

        /// <summary>
        /// Used to update the step references in move to step steps after the workflow steps have all been saved
        /// </summary>
        /// <param name="workflowID"></param>
        /// <param name="moveToStepID"></param>
        /// <param name="actionID"></param>
        /// <returns>Success boolean</returns>
        public bool UpdateMoveToStepActionID(int workflowID, int moveToStepID, int actionID)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountid));

            expdata.sqlexecute.Parameters.AddWithValue("@workflowID", workflowID);
            expdata.sqlexecute.Parameters.AddWithValue("@stepID", moveToStepID);
            expdata.sqlexecute.Parameters.AddWithValue("@actionID", actionID);

            expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("dbo.updateWorkflowStepMoveToStepActionID");
            int workflowStepsAffected = (int)expdata.sqlexecute.Parameters["@identity"].Value;
            expdata.sqlexecute.Parameters.Clear();

            if (workflowStepsAffected != 1)
            {
                throw new Exception("Updating MoveToStep failed");
            }

            return true;
        }


        private void UpdateEntityApprovalHistory(int workflowID, int entityID, bool approved, string reason)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            DBConnection data = new DBConnection(cAccounts.getConnectionString(currentUser.AccountID));

            data.sqlexecute.Parameters.AddWithValue("@workflowID", workflowID);
            data.sqlexecute.Parameters.AddWithValue("@entityID", entityID);
            data.sqlexecute.Parameters.AddWithValue("@approverID", currentUser.EmployeeID);
            data.sqlexecute.Parameters.AddWithValue("@approved", approved);
            data.sqlexecute.Parameters.AddWithValue("@reason", reason);

            data.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }

            data.sqlexecute.Parameters.Add("@returnValue", SqlDbType.Int);
            data.sqlexecute.Parameters["@returnValue"].Direction = ParameterDirection.ReturnValue;

            data.ExecuteProc("dbo.saveWorkflowApprovalHistory");

            int status = (int)data.sqlexecute.Parameters["@returnValue"].Value;

            data.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// Used to generate formula for calculations to happen in <see cref="cCalculationField"/> class.
        /// </summary>
        /// <param name="fieldId">FieldId of the custom entity attribute</param> 
        /// <returns>FieldId formula</returns>
        private string GenerateFormulaForWorkflows(Guid fieldId)
        {
            return "(\"[" + fieldId + "]\")";
        }

        /// <summary>
        /// Gets workflow object by workflowId <see cref="cWorkflow"/> class.
        /// </summary>
        /// <param name="workflowId">WorkflowId of base workflow</param> 
        /// <returns>Workflow object</returns>
        private cWorkflow GetWorkflowById(int workflowId)
        {
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.nAccountid)))
            {
                connection.sqlexecute.Parameters.AddWithValue("@WorkflowID", workflowId);
                using (var reader = connection.GetReader("GetWorkflowById", CommandType.StoredProcedure))
                {
                    cWorkflow workflow = null;
                    while (reader.Read())
                    {
                        var workflowName = reader.GetString(reader.GetOrdinal("workflowName"));
                        var clsTables = new cTables(this.nAccountid);
                        var description = !reader.IsDBNull(reader.GetOrdinal("description")) ? reader.GetString(reader.GetOrdinal("description")) : null;
                        var canBeChildWorkflow = reader.GetBoolean(reader.GetOrdinal("canbechildworkflow"));
                        var runOnCreation = reader.GetBoolean(reader.GetOrdinal("runoncreation"));
                        var runOnChange = reader.GetBoolean(reader.GetOrdinal("runonchange"));
                        var runOnDelete = reader.GetBoolean(reader.GetOrdinal("runondelete"));
                        var workFlowType = (WorkflowType)reader.GetInt32(reader.GetOrdinal("workflowType"));
                        var createdOn = reader.GetDateTime(reader.GetOrdinal("createdOn"));
                        var createdBy = reader.GetInt32(reader.GetOrdinal("createdBy"));
                        var baseTable = clsTables.GetTableByID(reader.GetGuid(reader.GetOrdinal("baseTableID")));
                        workflow = new cWorkflow(workflowId, workflowName, description, createdOn, createdBy, null, 0, workFlowType, canBeChildWorkflow, runOnCreation, runOnChange, runOnDelete, baseTable);
                    }

                    return workflow;
                }
            }
        }
    }
}
