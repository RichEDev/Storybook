namespace BusinessLogic.GeneralOptions.Framework.Tasks
{
    /// <summary>
    /// Defines a <see cref="TaskOptions"/> and it's members
    /// </summary>
    public class TaskOptions : ITaskOptions
    {
        /// <summary>
        /// Gets or set task escalation request
        /// </summary>
        public int TaskEscalationRepeat { get; set; }

        /// <summary>
        /// Gets or sets the task start date mandatory
        /// </summary>
        public bool TaskStartDateMandatory { get; set; }

        /// <summary>
        /// Gets or sets the task end date mandatory
        /// </summary>
        public bool TaskEndDateMandatory { get; set; }

        /// <summary>
        /// Gets or sets the task due date mandatory
        /// </summary>
        public bool TaskDueDateMandatory { get; set; }
    }
}
