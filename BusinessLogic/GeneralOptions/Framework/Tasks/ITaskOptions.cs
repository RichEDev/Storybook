namespace BusinessLogic.GeneralOptions.Framework.Tasks
{
    /// <summary>
    /// Defines a <see cref="ITaskOptions"/> and it's members
    /// </summary>
    public interface ITaskOptions
    {
        /// <summary>
        /// Gets or set task escalation request
        /// </summary>
        int TaskEscalationRepeat { get; set; }

        /// <summary>
        /// Gets or sets the task start date mandatory
        /// </summary>
        bool TaskStartDateMandatory { get; set; }

        /// <summary>
        /// Gets or sets the task end date mandatory
        /// </summary>
        bool TaskEndDateMandatory { get; set; }

        /// <summary>
        /// Gets or sets the task due date mandatory
        /// </summary>
        bool TaskDueDateMandatory { get; set; }
    }
}
