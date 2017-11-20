using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    #region Task Enumerations
    /// <summary>
    /// Enumeration of the TaskStatus
    /// </summary>
    public enum TaskStatus
    {
        NotStarted = 0,
        InProgress,
        Postponed,
        Cancelled,
        Completed
    }

    /// <summary>
    /// TaskCommand enumeration
    /// </summary>
    public enum TaskCommand
    {
        Standard = 0,
        ESR_RecordArchiveOn,
        ESR_RecordArchiveManual,
        ESR_RecordActivateOn,
        ESR_RecordActivateManual,
        ESR_RecordAssignment, 
        ESR_HomeLocationPostcodeInvalid,
        ESR_WorkLocationPostcodeInvalid,
        ESR_CarAutoActivate
    }
    #endregion

    #region cTask
    /// <summary>
    /// cTask class
    /// </summary>
    [Serializable()]
    public class cTask
    {
        /// <summary>
        /// Task Id
        /// </summary>
        private int nTaskId;
        /// <summary>
        /// Gets or set the Task Id
        /// </summary>
        public int TaskId
        {
            get { return nTaskId; }
            set { nTaskId = value; }
        }
        /// <summary>
        /// Sub AccountID
        /// </summary>
        private int? nSubAccountId;
        /// <summary>
        /// Gets or set the subaccount id
        /// </summary>
        public int? SubAccountID
        {
            get { return nSubAccountId; }
            set { nSubAccountId = value; }
        }
        /// <summary>
        /// Task Command Type
        /// </summary>
        private TaskCommand eTaskCommandType;
        /// <summary>
        /// Gets the tasks command type
        /// </summary>
        public TaskCommand TaskCommandType
        {
            get { return eTaskCommandType; }
        }
        /// <summary>
        /// Employee who created the task
        /// </summary>
        private int nTaskCreator;
        /// <summary>
        /// Gets or set the creator of the task
        /// </summary>
        public int TaskCreator
        {
            get { return nTaskCreator; }
            set { nTaskCreator = value; }
        }
        /// <summary>
        /// Date task created
        /// </summary>
        private DateTime dTaskCreated;
        /// <summary>
        /// Gets the creation date
        /// </summary>
        public DateTime TaskCreatedDate
        {
            get { return dTaskCreated; }
        }
        /// <summary>
        /// Record ID task is associated to
        /// </summary>
        private int nRegardingId;
        /// <summary>
        /// Gets or set the record ID the task is associated to
        /// </summary>
        public int RegardingId
        {
            get { return nRegardingId; }
            set { nRegardingId = value; }
        }
        /// <summary>
        /// Area of application the task is relating to
        /// </summary>
        private AppliesTo nRegardingArea;
        /// <summary>
        /// Gets or sets the area of application the task is relating to
        /// </summary>
        public AppliesTo RegardingArea
        {
            get { return nRegardingArea; }
            set { nRegardingArea = value; }
        }
        /// <summary>
        /// Task Subject
        /// </summary>
        private string sSubject;
        /// <summary>
        /// Gets or sets the Task Subject
        /// </summary>
        public string Subject
        {
            get { return sSubject; }
            set { sSubject = value; }
        }
        /// <summary>
        /// Task description
        /// </summary>
        private string sDescription;
        /// <summary>
        /// Gets or sets the task description
        /// </summary>
        public string Description
        {
            get { return sDescription; }
            set { sDescription = value; }
        }
        /// <summary>
        /// Nullable Task Start Date
        /// </summary>
        private DateTime? dtStartDate;
        /// <summary>
        /// Gets or sets the Task Start Date (Nullable DateTime)
        /// </summary>
        public DateTime? StartDate
        {
            get { return dtStartDate; }
            set { dtStartDate = value; }
        }
        /// <summary>
        /// Nullable Task Due Date
        /// </summary>
        private DateTime? dtDueDate;
        /// <summary>
        /// Gets or set the Task Due Date (Nullable DateTime)
        /// </summary>
        public DateTime? DueDate
        {
            get { return dtDueDate; }
            set { dtDueDate = value; }
        }
        /// <summary>
        /// Nullable Task End Date
        /// </summary>
        private DateTime? dtEndDate;
        /// <summary>
        /// Gets or set the Task End (Completion) Date (Nullable DateTime)
        /// </summary>
        public DateTime? EndDate
        {
            get { return dtEndDate; }
            set { dtEndDate = value; }
        }
        /// <summary>
        /// The status of the task
        /// </summary>
        private TaskStatus nStatusId;
        /// <summary>
        /// Gets or sets the Task Status
        /// </summary>
        public TaskStatus StatusId
        {
            get { return nStatusId; }
            set { nStatusId = value; }
        }

        /// <summary>
        /// Gets the Task Status Description
        /// </summary>
        public string GetStatusDescription
        {
            get
            {
                string retStr = "";
                switch (nStatusId)
                {
                    case TaskStatus.NotStarted:
                        retStr = "Not Started";
                        break;
                    case TaskStatus.InProgress:
                        retStr = "In Progress";
                        break;
                    case TaskStatus.Postponed:
                        retStr = "Postponed";
                        break;
                    case TaskStatus.Cancelled:
                        retStr = "Cancelled";
                        break;
                    case TaskStatus.Completed:
                        retStr = "Completed";
                        break;
                    default:
                        retStr = "Unknown Status";
                        break;
                }

                return retStr;
            }
        }

        /// <summary>
        /// Identified the task ownership
        /// </summary>
        private cTaskOwner TaskOwnership;
        /// <summary>
        /// Gets or sets the Task Ownership
        /// </summary>
        public cTaskOwner TaskOwner
        {
            get { return TaskOwnership; }
            set { TaskOwnership = value; }
        }
        /// <summary>
        /// Has the task been escalated
        /// </summary>
        private bool bTaskEscalated;
        /// <summary>
        /// Gets or sets whether the task has been escalated
        /// </summary>
        public bool TaskEscalated
        {
            get { return bTaskEscalated; }
            set { bTaskEscalated = value; }
        }
        /// <summary>
        /// Date task escalated
        /// </summary>
        private DateTime? dTaskEscalated;
        /// <summary>
        /// Gets or sets the date the task was escalated (Nullable DateTime)
        /// </summary>
        public DateTime? TaskEscalatedDate
        {
            get { return dTaskEscalated; }
            set { dTaskEscalated = value; }
        }
        /// <summary>
        /// Type of task
        /// </summary>
        private cTaskType clTaskType;
        /// <summary>
        /// Gets or sets the Task Type
        /// </summary>
        public cTaskType TaskType
        {
            get { return clTaskType; }
            set { clTaskType = value; }
        }

        /// <summary>
        /// Is a user the creator of the current task
        /// </summary>
        /// <param name="employeeId">Employee ID to check</param>
        /// <returns>TRUE if the user is the creator the current task</returns>
        public bool isTaskCreator(int employeeId)
        {
            if (TaskCreator == employeeId)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Is a user the owner of the current task
        /// </summary>
        /// <param name="employeeId">ID of the employee to check</param>
        /// <returns></returns>
        public bool isTaskOwner(int employeeId)
        {
            if (TaskOwner.OwnerType == sendType.team)
            {
                cTeam team = TaskOwner.TaskTeam;
                for (int x = 0; x < team.teammembers.Count; x++)
                {
                    if (team.teammembers[x] == employeeId)
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (TaskOwner.OwnerId == employeeId)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// cTask constructor
        /// </summary>
        /// <param name="taskid">Task ID</param>
        /// <param name="subaccountid">Sub-account iD</param>
        /// <param name="taskcommandtype">Task Command Type</param>
        /// <param name="taskcreator">Task Creator</param>
        /// <param name="taskcreateddate">Creation Date</param>
        /// <param name="tasktype">Task Type</param>
        /// <param name="regardingid">Associated record ID</param>
        /// <param name="regardingarea">Application area task is associated to</param>
        /// <param name="subject">Task Subject</param>
        /// <param name="description">Task Description</param>
        /// <param name="startdate">Task Start Date</param>
        /// <param name="duedate">Task Due Date</param>
        /// <param name="enddate">Task End Date</param>
        /// <param name="status">Task Status</param>
        /// <param name="ownership">Task Ownership</param>
        /// <param name="taskescalated">Has Task been escalated</param>
        /// <param name="escalateddate">Date of task escalation</param>
        public cTask(int taskid, int? subaccountid, TaskCommand taskcommandtype, int taskcreator, DateTime taskcreateddate, cTaskType tasktype, int regardingid, AppliesTo regardingarea, string subject, string description, DateTime? startdate, DateTime? duedate, DateTime? enddate, TaskStatus status, cTaskOwner ownership, bool taskescalated, DateTime? escalateddate)
        {
            nTaskId = taskid;
            nSubAccountId = subaccountid;
            eTaskCommandType = taskcommandtype;
            nTaskCreator = taskcreator;
            dTaskCreated = taskcreateddate;
            clTaskType = tasktype;
            nRegardingId = regardingid;
            nRegardingArea = regardingarea;
            sSubject = subject;
            sDescription = description;
            dtStartDate = startdate;
            dtDueDate = duedate;
            dtEndDate = enddate;
            nStatusId = status;
            TaskOwnership = ownership;
            bTaskEscalated = taskescalated;
            dTaskEscalated = escalateddate;
        }
    }
    #endregion

    #region cTaskOwner
    /// <summary>
    /// cTaskOwner class
    /// </summary>
    [Serializable()]
    public class cTaskOwner
    {
        /// <summary>
        /// Task Owner ID
        /// </summary>
        private int nOwnerId;
        /// <summary>
        /// Gets or sets the task owner ID
        /// </summary>
        public int OwnerId
        {
            get { return nOwnerId; }
            set { nOwnerId = value; }
        }
        /// <summary>
        /// Task ownership type
        /// </summary>
        private sendType nOwnerType;
        /// <summary>
        /// Gets or sets Task ownership type (Individual, Line Mgr, Budget Holder, Team etc.)
        /// </summary>
        public sendType OwnerType
        {
            get { return nOwnerType; }
            set { nOwnerType = value; }
        }
        /// <summary>
        /// Team definition
        /// </summary>
        private cTeam ttTaskTeam;
        /// <summary>
        /// Gets or sets ownership team definition (if indicated by OwnerType)
        /// </summary>
        public cTeam TaskTeam
        {
            get { return ttTaskTeam; }
            set { ttTaskTeam = value; }
        }
        /// <summary>
        /// cTaskOwner constructor
        /// </summary>
        /// <param name="ownerid">ID of task owner</param>
        /// <param name="type">Task ownership type</param>
        /// <param name="taskTeam">Team record</param>
        public cTaskOwner(int ownerid, sendType type, cTeam taskTeam)
        {
            nOwnerId = ownerid;
            nOwnerType = type;
            ttTaskTeam = taskTeam;
        }

    }
    #endregion
}
