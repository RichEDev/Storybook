namespace Spend_Management.expenses.code
{
    using BusinessLogic;
    using SpendManagementLibrary;

    /// <summary>
    /// Validation message for a Signoff group and the Id of the group they relate to
    /// </summary>
    public class SignoffGroupValidation
    {
        /// <summary>
        /// Constructor for <see cref="SignoffGroupValidation"/>
        /// </summary>
        /// <param name="validationMessages">The validation message from <see cref="GroupStageValidationResult"/></param>
        /// <param name="groupId">The group id for the <see cref="cGroup"/></param>
        public SignoffGroupValidation(string validationMessages, int groupId)
        {
            Guard.ThrowIfNull(validationMessages, nameof(validationMessages));
            Guard.ThrowIfNull(groupId, nameof(groupId));

            this.ValidationMessages = validationMessages;
            this.GroupId = groupId;
        }

        /// <summary>
        /// Validation messages from <see cref="GroupStageValidationResult"/>
        /// </summary>
        public string ValidationMessages { get; set; }

        /// <summary>
        /// Id of the <see cref="cGroup"/>
        /// </summary>
        public int GroupId { get; set; }

    }
}