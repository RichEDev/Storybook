using System;
using System.Collections.Generic;
using System.Text;

namespace SpendManagementLibrary
{
    using SpendManagementLibrary.Employees;

    /// <summary>
    /// Base class for a workflow
    /// </summary>
    public class cWorkflow
    {
        private int nWorkflowid;
        private string sWorkflowName;
        private string sDescription;
        private DateTime dtCreatedOn;
        private int nCreatedBy;
        private DateTime? dtModifiedOn;
        private int? nModifiedBy;
        private WorkflowType wtWorkflowType;
        private bool bCanBeChildWorkflow;
        private bool bRunOnCreation;
        private bool bRunOnChange;
        private bool bRunOnDelete;
        private SortedList<int, cWorkflowStep> lstSteps;
        private cTable cBaseTable;

        /// <summary>
        /// Workflow construct
        /// </summary>
        /// <param name="workflowid">Unique ID</param>
        /// <param name="workflowname">The name of the workflow</param>
        /// <param name="description">Description of the workflow</param>
        /// <param name="createdon">Date created on</param>
        /// <param name="createdby">Created by, employeeID</param>
        /// <param name="modifiedon">Date last modified on</param>
        /// <param name="modifiedby">Last modified by, employeeID</param>
        /// <param name="workflowtype">WorkflowType Enumerator for the area</param>
        /// <param name="canbechildworkflow">Sub-Workflow</param>
        /// <param name="runoncreation">Can be run when a an entity is created</param>
        /// <param name="runonchange">Can be run when a property is changed</param>
        /// <param name="runondelete">Can be run when an entity is deleted</param>
        /// <param name="baseTable">The base table that this workflow works with</param>
        public cWorkflow(int workflowid, string workflowname, string description, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, WorkflowType workflowtype, bool canbechildworkflow, bool runoncreation, bool runonchange, bool runondelete, cTable baseTable)
        {
            nWorkflowid = workflowid;
            sWorkflowName = workflowname;
            sDescription = description;
            dtCreatedOn = createdon;
            nCreatedBy = createdby;
            dtModifiedOn = modifiedon;
            nModifiedBy = modifiedby;
            wtWorkflowType = workflowtype;
            bCanBeChildWorkflow = canbechildworkflow;
            bRunOnCreation = runoncreation;
            bRunOnChange = runonchange;
            bRunOnDelete = runondelete;
            cBaseTable = baseTable;
        }

        #region properties
        /// <summary>
        /// WorkflowID
        /// </summary>
        public int workflowid
        {
            get { return nWorkflowid; }
        }
        /// <summary>
        /// Name of the workflow
        /// </summary>
        public string workflowname
        {
            get { return sWorkflowName; }
        }
        /// <summary>
        /// Description of this workflow
        /// </summary>
        public string description
        {
            get { return sDescription; }
        }
        /// <summary>
        /// Date this workflow was created on
        /// </summary>
        public DateTime createdon
        {
            get { return dtCreatedOn; }
        }
        /// <summary>
        /// The employeeID that created this workflow
        /// </summary>
        public int createdby
        {
            get { return nCreatedBy; }
        }
        /// <summary>
        /// The date this workflow was modified on, or null if not modified
        /// </summary>
        public DateTime? modifiedon
        {
            get { return dtModifiedOn; }
        }
        /// <summary>
        /// The employeeID who modified this workflow or null if not modified
        /// </summary>
        public int? modifiedby
        {
            get { return nModifiedBy; }
        }
        /// <summary>
        /// The workflow type (Specific type or custom table)
        /// </summary>
        public WorkflowType workflowtype
        {
            get { return wtWorkflowType; }
        }
        /// <summary>
        /// If this workflow can be a child workflow
        /// </summary>
        public bool canbechildworkflow
        {
            get { return bCanBeChildWorkflow; }
        }
        /// <summary>
        /// If this workflow will run when a record is created in the base table
        /// </summary>
        public bool runoncreation
        {
            get { return bRunOnCreation; }
        }
        /// <summary>
        /// If this workflow will run when a record in the base table changes
        /// </summary>
        public bool runonchange
        {
            get { return bRunOnChange; }
        }
        /// <summary>
        /// If this workflow will run when a record is deleted
        /// </summary>
        public bool runondelete
        {
            get { return bRunOnDelete; }
        }
        /// <summary>
        /// The current workflow steps in this workflow
        /// </summary>
        public SortedList<int, cWorkflowStep> Steps
        {
            get { return lstSteps; }
            set { lstSteps = value; }
        }
        /// <summary>
        /// The base table this workflow works with
        /// </summary>
        public cTable BaseTable
        {
            get { return cBaseTable; }
        }
        #endregion

    }

    /// <summary>
    /// cWorkflowStep base class, inherited class
    /// </summary>
    public class cWorkflowStep 
    {
        /// <summary>
        /// WorkflowID
        /// </summary>
        protected int nWorkflowID;
        /// <summary>
        /// WorkflowStepID
        /// </summary>
        protected int nWorkflowStepID;
        /// <summary>
        /// Description
        /// </summary>
        protected string sDescription;
        /// <summary>
        /// What type of action this workflow step will perform
        /// </summary>
        protected WorkFlowStepAction eAction;
        /// <summary>
        /// Parent of this workflow step
        /// </summary>
        protected int nParentStepID;
        /// <summary>
        /// If this workflow step will show a question
        /// </summary>
        protected bool bShowQuestion;
        /// <summary>
        /// The question to show if ShowQuestion is enabled
        /// </summary>
        protected string sQuestion;
        /// <summary>
        /// The true/yes response if a question is asked
        /// </summary>
        protected string sTrueOption;
        /// <summary>
        /// The true/no response if a question is asked
        /// </summary>
        protected string sFalseOption;
        /// <summary>
        /// The formID if this workflow is used with custom entities
        /// </summary>
        protected int? nFormID;
        /// <summary>
        /// The step which this step is related to
        /// </summary>
        protected int? nRelatedStepID;

        public cWorkflowStep(int workflowid, int workflowstepid, string description, WorkFlowStepAction action, int parentStepID, bool showQuestionDialog, string question, string trueOption, string falseOption, int? formID, int? relatedStepID)
        {
            nWorkflowID = workflowid;
            nWorkflowStepID = workflowstepid;
            sDescription = description;
            eAction = action;
            nParentStepID = parentStepID;
            sQuestion = question;
            sTrueOption = trueOption;
            sFalseOption = falseOption;
            nFormID = formID;
            nRelatedStepID = relatedStepID;

        }
        #region properties

        /// <summary>
        /// Returns the related stepID if there is one, null otherwise
        /// </summary>
        public int? RelatedStepID
        {
            get { return nRelatedStepID; }
        }

        /// <summary>
        /// The workflowID this step belongs too
        /// </summary>
        public int WorkflowID
        {
            get { return nWorkflowID; }
        }

        /// <summary>
        /// The stepID/number this is not unique
        /// </summary>
        public int WorkflowStepID
        {
            get { return nWorkflowStepID; }
        }

        /// <summary>
        /// The step description
        /// </summary>
        public string Description
        {
            get { return sDescription; }
        }

        /// <summary>
        /// The type of step this is
        /// </summary>
        public WorkFlowStepAction Action
        {
            get { return eAction; }
        }

        /// <summary>
        /// The parent stepID of this step
        /// </summary>
        public int ParentStepID 
        {
            get { return nParentStepID;}
        }

        /// <summary>
        /// If to show the question or not
        /// </summary>
        public bool ShowQuestion
        {
            get { return bShowQuestion; }
        }

        /// <summary>
        /// The question shown
        /// </summary>
        public string Question
        {
            get { return sQuestion; }
        }

        /// <summary>
        ///  The link/button etc text that is shown for the true/yes option of a decision
        /// </summary>
        public string TrueOption
        {
            get { return sTrueOption; }
        }

        /// <summary>
        /// The link/button etc text that is shown for the false/no option
        /// </summary>
        public string FalseOption
        {
            get { return sFalseOption; }
        }

        /// <summary>
        /// Returns the form that should be used if this is applicable to a custom entity
        /// </summary>
        public int? FormID
        {
            get { return nFormID; }
        }
        #endregion
    }

    /// <summary>
    ///  Instance of a MoveToStep workflow step, Inherits cWorkflowStep
    /// </summary>
    public class cMovetoStepStep : cWorkflowStep
    {
        private int nStepID;

        public cMovetoStepStep(int workflowid, int workflowstepid, string descripion, WorkFlowStepAction action, int stepID, int parentStepID, int? formID)
            : base(workflowid, workflowstepid, descripion, action, parentStepID, false, "","","", formID, null)
        {
            nStepID = stepID;
        }
        #region properties
        /// <summary>
        /// The step to move to
        /// </summary>
        public int StepID
        {
            get { return nStepID; }
        }
        #endregion
    }

    /// <summary>
    /// Creates a string value based on the cFields and replace strings
    /// </summary>
    public class cCreateDynamicValue : cWorkflowStep
    {
        private cWorkflowDynamicValue cDynamicValue;
        private cField Field;

       public cCreateDynamicValue(int workflowid, int workflowstepid, string description, WorkFlowStepAction action, int parentStepID, int? formID, cWorkflowDynamicValue dynamicValue)
            : base(workflowid, workflowstepid, description, action, parentStepID, false, "", "", "", formID,  null)
        {
            cDynamicValue = dynamicValue;
        }

        #region properties
        /// <summary>
        /// DynamicValue
        /// </summary>
        public cWorkflowDynamicValue DynamicValue
        {
            get { return cDynamicValue; }
            set { cDynamicValue = value; }
        }
        #endregion properties
    }

    /// <summary>
    ///  Instance of a RunSubWorkflow workflow step, Inherits cWorkflowStep
    /// </summary>
    public class cRunSubworkflowStep : cWorkflowStep
    {
        private int nSubWorkflowID;

        public cRunSubworkflowStep(int workflowid, int workflowstepid, string descripion, WorkFlowStepAction action, int subworkflowid, int parentStepID, int? formID)
            : base(workflowid, workflowstepid, descripion, action, parentStepID, false, "","","", formID, null)
        {
            nSubWorkflowID = subworkflowid;
        }
        #region properties
        /// <summary>
        /// The subworkflow to run
        /// </summary>
        public int SubWorkflowID
        {
            get { return nSubWorkflowID; }
        }
        #endregion
    }

    public class cChangeFormStep : cWorkflowStep
    {
        public cChangeFormStep(int workflowID, int workflowStepID, string description, int parentStepID, int formID)
            : base(workflowID, workflowStepID, description, WorkFlowStepAction.ChangeCustomEntityForm, parentStepID, false, string.Empty, string.Empty, string.Empty, formID, null) 
        { 
        }
    }

    /// <summary>
    ///  Instance of a SendEmailStep workflow step, Inherits cWorkflowStep
    /// </summary>
    public class cSendEmailStep : cWorkflowStep
    {
        private int nEmailTemplateID;

        public cSendEmailStep(int workflowid, int workflowstepid, string descripion, WorkFlowStepAction action, int emailTemplateID, int parentStepID, int? formID)
            : base(workflowid, workflowstepid, descripion, action, parentStepID, false, "", "", "", formID, null)
        {
            nEmailTemplateID = emailTemplateID;
        }
        #region properties
        /// <summary>
        /// The email template to send
        /// </summary>
        public int EmailTemplateID
        {
            get { return nEmailTemplateID; }
        }
        #endregion
    }

    /// <summary>
    /// Instance of a DecisionStep workflow step, Inherits cWorkflowStep
    /// </summary>
    public class cDecisionStep : cWorkflowStep
    { // All of the data for this step type are stored in the base workflow step class
        public cDecisionStep(int workflowID, int workflowStepID, string description, WorkFlowStepAction action, string question, string trueAnswer, string falseAnswer, int parentStepID, int? formID, int? relatedStepID)
            : base(workflowID, workflowStepID, description, action, parentStepID, true, question, trueAnswer, falseAnswer, formID, relatedStepID)
        {

        }      
    }

    /// <summary>
    /// Instance of a CheckCondition workflow step, Inherits cWorkflowStep
    /// </summary>
    public class cCheckConditionStep : cWorkflowStep
    {
        private List<cWorkflowCriteria> lstCriteria;

        public cCheckConditionStep(int workflowID, int workflowStepID, string description, WorkFlowStepAction action, List<cWorkflowCriteria> criteria, int parentStepID, int? formID, int? relatedStepID)
            : base(workflowID, workflowStepID, description, action, parentStepID, false, "", "", "", formID, relatedStepID)
        {
            lstCriteria = criteria;
        }

        #region Properties
        /// <summary>
        /// The criteria
        /// </summary>
        public List<cWorkflowCriteria> Criteria
        {
            get { return lstCriteria; }
        }
        #endregion
    }

    /// <summary>
    /// Instance of a ChangeValue workflow step, Inherits cWorkflowStep
    /// </summary>
    public class cChangeValueStep : cWorkflowStep
    {
        private List<cWorkflowCriteria> lstCriteria;

        public cChangeValueStep(int workflowID, int workflowStepID, string description, WorkFlowStepAction action, List<cWorkflowCriteria> criteria, int parentStepID, int? formID)
            : base(workflowID, workflowStepID, description, action, parentStepID, false, "", "", "", formID, null)
        {
            lstCriteria = criteria;
        }

        #region Properties
        /// <summary>
        /// Returns a list of update criteria
        /// </summary>
        public List<cWorkflowCriteria> Criteria
        {
            get { return lstCriteria; }
        }
        #endregion
    }

    /// <summary>
    /// Instance of an Approval workflow step, Inherits cWorkflowStep
    /// </summary>
    public class cApprovalStep : cWorkflowStep
    {
        private WorkflowEntityType eApproverType;
        private int nApproverID;
        private bool bOneClickSignOff;
        private bool bFilterItems;
        private List<cWorkflowCriteria> lstCriteria;
        private bool bShowDeclaration;
        private int? nEmailTemplateID;
        private string sMessage;

        public cApprovalStep(int workflowID, int workflowStepID, string description, WorkFlowStepAction action, int parentStepID, WorkflowEntityType approverType, int approverID, bool oneClickSignOff, bool filterItems, List<cWorkflowCriteria> criteria, bool showDeclaration, int? formID, int? relatedStepID, string question, string trueResponse, string falseResponse, int? emailTemplateID, string message)
            : base (workflowID,workflowStepID,description,action,parentStepID, showDeclaration, question, trueResponse, falseResponse, formID, relatedStepID)
        {
            eApproverType = approverType;
            nApproverID = approverID;
            bOneClickSignOff = oneClickSignOff;
            bFilterItems = filterItems;
            lstCriteria = criteria;
            bShowDeclaration = showDeclaration;
            nEmailTemplateID = emailTemplateID;
            sMessage = message;
        }

        #region Properties
        public int? EmailTemplateID
        {
            get { return nEmailTemplateID; }
        }

        /// <summary>
        /// If to show a declaration
        /// </summary>
        public bool ShowDeclaration
        {
            get { return bShowDeclaration; }
        }

        /// <summary>
        /// The type of approver used on this step
        /// </summary>
        public WorkflowEntityType ApproverType
        {
            get { return eApproverType; }
        }

        /// <summary>
        /// The approverID
        /// </summary>
        public int ApproverID
        {
            get { return nApproverID; }
        }

        /// <summary>
        /// If to enable one click sign off or not
        /// </summary>
        public bool OneClickSignOff
        {
            get { return bOneClickSignOff; }
        }

        /// <summary>
        /// If to filter items or not
        /// </summary>
        public bool FilterItems
        {
            get { return bFilterItems; }
        }

        /// <summary>
        /// What items to actually filter for
        /// </summary>
        public List<cWorkflowCriteria> FilteredItems
        {
            get { return lstCriteria; }
        }

        /// <summary>
        /// Message for person submitting into approval step
        /// </summary>
        public string Message
        {
            get { return sMessage; }
        }

        #endregion

    }

    /// <summary>
    /// Show message step,  Inherits cWorkflowStep
    /// </summary>
    public class cShowMessageStep : cWorkflowStep
    {
        private string sMessage;

        public cShowMessageStep(int workflowID, int workflowStepID, string description, int parentStepID, string message)
            : base(workflowID, workflowStepID, description, WorkFlowStepAction.ShowMessage, parentStepID, false, string.Empty, string.Empty, string.Empty, null, null)
        {
            sMessage = message;
        }

        public string Message
        {
            get { return sMessage; }
            set { sMessage = value; }
        }
        
    }





    /// <summary>
    /// Holds information for a dynamic value step
    /// </summary>
    public class cWorkflowDynamicValue
    {
        private int nDynamicValueID;
        private string sValueFormula;
        private Guid gFieldID;


        public cWorkflowDynamicValue(int dynamicValueID, string valueFormula, Guid fieldID)
        {
            nDynamicValueID = dynamicValueID;
            sValueFormula = valueFormula;
            gFieldID = fieldID;
        }

        /// <summary>
        /// Gets or sets the dynamicValueID
        /// </summary>
        public int DynamicValueID
        {
            get { return nDynamicValueID; }
            set { nDynamicValueID = value; }
        }

        /// <summary>
        /// Gets or sets value formula
        /// </summary>
        public string ValueFormula
        {
            get { return sValueFormula; }
            set { sValueFormula = value; }
        }

        /// <summary>
        /// Gets or sets the fieldID to update with the value
        /// </summary>
        public Guid FieldID
        {
            get { return gFieldID; }
            set { gFieldID = value; }
        }
    }

    /// <summary>
    /// cWorkflowCriteria, holds a criteria for either update or an 'if' statement
    /// </summary>
    public class cWorkflowCriteria
    {
        private int nConditionID;
        private cField clsField;
        private ConditionType eConditionType;
        private string sValue;
        private bool bRuntime;
        private bool bUpdateCriteria;
        private bool bReplaceElements;
        private string sValueTwo;

        /// <summary>
        /// Add a runtime update field entry.
        /// </summary>
        /// <param name="conditionID"></param>
        /// <param name="field"></param>
        /// <param name="runtime"></param>
        public cWorkflowCriteria(int conditionID, cField field, bool runtime)
        {
            nConditionID = conditionID;
            clsField = field;
            bRuntime = runtime;
            bUpdateCriteria = true;
        }

        /// <summary>
        /// Used for holding new criteria with no criteriaID
        /// </summary>
        /// <param name="field"></param>
        /// <param name="condition"></param>
        /// <param name="criteriaMode"></param>
        /// <param name="runTime"></param>
        /// <param name="value"></param>
        public cWorkflowCriteria(cField field, ConditionType condition, CriteriaMode criteriaMode, bool runTime, string value, string value2)
        {
            clsField = field;

            if (criteriaMode == CriteriaMode.Select)
            {
                eConditionType = condition;
                sValue = value;
                sValueTwo = value2;
            }
            else
            {
                if (runTime == true)
                {
                    bRuntime = runTime;
                }
                else
                {
                    eConditionType = condition;
                    sValue = value;
                }
            }
        }

        /// <summary>
        /// Add a update field entry.
        /// </summary>
        /// <param name="conditionID"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public cWorkflowCriteria(int conditionID, cField field, string value, bool replaceElements)
        {
            nConditionID = conditionID;
            clsField = field;
            sValue = value;
            bUpdateCriteria = true;
            bRuntime = false;
            bReplaceElements = replaceElements;
        }

        /// <summary>
        /// Add a check condition entry.
        /// </summary>
        /// <param name="conditionID"></param>
        /// <param name="field"></param>
        /// <param name="condition"></param>
        /// <param name="value"></param>
        public cWorkflowCriteria(int conditionID, cField field, ConditionType condition, string value, string valueTwo)
        {
            nConditionID = conditionID;
            clsField = field;
            eConditionType = condition;
            sValue = value;
            sValueTwo = valueTwo;
            bUpdateCriteria = false;
        }

        #region properties
        /// <summary>
        /// If to replace elements in this criteria
        /// </summary>
        public bool ReplaceElements
        {
            get { return bReplaceElements; }
        }

        /// <summary>
        /// The identity field in the database for this condition
        /// </summary>
        public int ConditionID
        {
            get { return nConditionID; }
            set { nConditionID = value; }
        }

        /// <summary>
        /// The field that is being searched/updated
        /// </summary>
        public cField Field
        {
            get { return clsField; }
        }

        /// <summary>
        /// The value to use as our search parameter or update value for searches/non runtime updates
        /// </summary>
        public string Value
        {
            get { return sValue; }
            set { sValue = value; }
        }

        /// <summary>
        /// Gets the second value or null if one is not set
        /// </summary>
        public string ValueTwo
        {
            get { return sValueTwo; }
            set { sValueTwo = value; }
        }

        /// <summary>
        /// Check the type of condition is applied with the search
        /// </summary>
        public ConditionType Condition
        {
            get { return eConditionType; }
        }

        /// <summary>
        /// Check if this is a runtime update criteria
        /// </summary>
        public bool Runtime
        {
            get { return bRuntime; }
        }

        /// <summary>
        /// Check if this criteria is update or condition for search
        /// </summary>
        public bool IsUpdateCriteria
        {
            get { return bUpdateCriteria; }
        }

        #endregion
    }

    /// <summary>
    /// Holds the details for approval steps, these are passed to a cApprovalStep instance when cWorkflows is initialised
    /// </summary>
    public struct sWorkflowApprovalDetails
    {
        private WorkflowEntityType eApproverType;
        private int nApproverID;
        private bool bOneClickSignOff;
        private bool bFilterItems;
        private int? nEmailTemplateID;

        public sWorkflowApprovalDetails(WorkflowEntityType approverType, int approverID, bool oneClickSignOff, bool FilterItems, int? emailTemplateID)
        {
            eApproverType = approverType;
            nApproverID = approverID;
            bOneClickSignOff = oneClickSignOff;
            bFilterItems = FilterItems;
            nEmailTemplateID = emailTemplateID;
        }

        #region Properties

        public int? EmailTemplateID
        {
            get { return nEmailTemplateID; }
        }

        public WorkflowEntityType ApproverType
        {
            get { return eApproverType; }
        }

        public int ApproverID
        {
            get { return nApproverID; }
        }

        public bool OneClickSignOff
        {
            get { return bOneClickSignOff; }
        }

        public bool FilterItems
        {
            get { return bFilterItems; }
        }

        #endregion

    }

    /// <summary>
    /// Class that holds the next step and status of a workflow
    /// </summary>
    public class cWorkflowNextStep
    {
        private WorkflowStatus eStatus;
        private cWorkflowStep cStep;

        public cWorkflowNextStep(WorkflowStatus status, cWorkflowStep step)
        {
            eStatus = status;
            cStep = step;
        }

        /// <summary>
        /// The current status of the workflow
        /// </summary>
        public WorkflowStatus Status
        {
            get { return eStatus; }
        }

        /// <summary>
        /// The next step in the workflow if there is one
        /// </summary>
        public cWorkflowStep NextStep
        {
            get { return cStep; }
        }
    }

    /// <summary>
    /// Details on an entities current status
    /// </summary>
    public class cWorkflowEntityDetails
    {
        private int nWorkflowID;
        private int nEntityID;
        private int nStepNumber;
        private Employee clsOwner;
        private Employee clsApprover;

        public cWorkflowEntityDetails(int entityID, int workflowID, int stepNumber, Employee entityOwner, Employee assignedApprover)
        {
            nWorkflowID = workflowID;
            nEntityID = entityID;
            nStepNumber = stepNumber;
            clsOwner = entityOwner;
            clsApprover = assignedApprover;
        }

        /// <summary>
        /// The workflowID
        /// </summary>
        public int WorkflowID
        {
            get { return nWorkflowID; }
        }

        /// <summary>
        /// The entityID
        /// </summary>
        public int EntityID
        {
            get { return nEntityID; }
        }

        /// <summary>
        /// The current step number this entity is on in the workflow
        /// </summary>
        public int StepNumber
        {
            get { return nStepNumber; }
        }

        /// <summary>
        /// cEmployee of the owner of this entity
        /// </summary>
        public Employee EntityOwner
        {
            get { return clsOwner; }
        }


        /// <summary>
        /// The currently assigned cEmployee of the approver if one is assigned, null if no assigned approver
        /// </summary>
        public Employee EntityAssignedApprover
        {
            get { return clsApprover; }
        }

    }

    /// <summary>
    /// The current status of a workflow step
    /// </summary>
    /// <example>WorkflowStatus.RequireUserInput</example>
    public enum WorkflowStatus
    {
        /// <summary>
        /// If the next step requires user input
        /// </summary>
        RequireUserInput,
        /// <summary>
        /// If this workflow has finished
        /// </summary>
        Finished,
        /// <summary>
        /// If the required step is not found
        /// </summary>
        StepNotFound,
        /// <summary>
        /// If this step is an automated step
        /// </summary>
        AutomaticStep,
        /// <summary>
        /// If the entity is not currently in a workflow
        /// </summary>
        NotInWorkflow,
        /// <summary>
        /// If this workflow was not found
        /// </summary>
        WorkflowNotFound,
        /// <summary>
        /// If this workflow is currently in a state of error
        /// </summary>
        InError,
        /// <summary>
        /// If this workflow step will be actioned directly to the user but no response required
        /// </summary>
        FireAndForget
    }

    /// <summary>
    /// Enumerator for the workflow area. e.g. ClaimApproval, Self Reg, Custom Table
    /// </summary>
    public enum WorkflowType
    {
        /// <summary>
        /// Used specifically for claim approval
        /// </summary>
        ClaimApproval = 1,
        /// <summary>
        /// Used specifically for self registration
        /// </summary>
        SelfRegistration,
        /// <summary>
        /// Used specifically for approval of advances
        /// </summary>
        AdvanceApproval,
        /// <summary>
        /// Used specifically for car approval
        /// </summary>
        CarApproval,
        /// <summary>
        /// Used on a non custom table
        /// </summary>
        CustomTable
    }

    /// <summary>
    /// Holds the step actions available, approval, change val, check condition, decision, move to step etc
    /// </summary>
    public enum WorkFlowStepAction
    {
        /// <summary>
        /// An approval ste
        /// </summary>
        Approval = 1,
        /// <summary>
        /// A change value step
        /// </summary>
        ChangeValue = 2,
        /// <summary>
        /// A check condition step
        /// </summary>
        CheckCondition = 3,
        /// <summary>
        /// A decision step
        /// </summary>
        Decision = 4,
        /// <summary>
        /// Move to step step
        /// </summary>
        MoveToStep = 5,
        /// <summary>
        /// Run a subworkflow step
        /// </summary>
        RunSubWorkflow = 6,
        /// <summary>
        /// Send email step
        /// </summary>
        SendEmail = 7,
        /// <summary>
        /// Else condition, related to CheckConditon
        /// </summary>
        ElseCondition=8,
        /// <summary>
        /// Finish workflow step
        /// </summary>
        FinishWorkflow=9,
        /// <summary>
        /// Else otherwise condition, related to CheckCondition
        /// </summary>
        ElseOtherwise=10,
        /// <summary>
        /// Create a dynamic value
        /// </summary>
        CreateDynamicValue=11,
        /// <summary>
        /// DecisionFalse, related to Decision
        /// </summary>
        DecisionFalse=12,
        /// <summary>
        /// ShowMessage step
        /// </summary>
        ShowMessage=13,
        /// <summary>
        /// States a change in custom entity form is required
        /// </summary>
        ChangeCustomEntityForm=14,
        /// <summary>
        /// Used for specifically returning an error message to the user interface
        /// </summary>
        ErrorWarning=15
        

    }

    /// <summary>
    /// Holds the entity type for approval/send email to etc. e.g. (budget holder, employee, team etc), if none are applicable select none
    /// </summary>
    public enum WorkflowEntityType
    {
        /// <summary>
        /// Budget Holder
        /// </summary>
        BudgetHolder=1,
        /// <summary>
        /// Specific employee
        /// </summary>
        Employee=2,
        /// <summary>
        /// A team of employees
        /// </summary>
        Team=3,
        /// <summary>
        /// The employees default line manager
        /// </summary>
        LineManager=4,
        /// <summary>
        /// Determined by the claimant
        /// </summary>
        DeterminedByClaimant=5,
        /// <summary>
        /// Automatically selected
        /// </summary>
        Automatic,
        /// <summary>
        /// If not applicable set none
        /// </summary>
        None
    }
}