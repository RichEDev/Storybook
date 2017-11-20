namespace SpendManagementLibrary
{
    using System;

    /// <summary>
    /// JavaScript and WebService compatible Trigger Field object
    /// </summary>
    public class JsAutoCompleteTriggerField
    {
        /// <summary>
        /// Gets or sets the control id
        /// </summary>
        public string ControlId { get; set; }

        /// <summary>
        /// Gets or sets the field id of the cField that the trigger field gets its displayed value from
        /// </summary>
        public string DisplayFieldId { get; set; }

        /// <summary>
        /// Gets or sets the JoinVia id of the JoinVia that is associated with the cField (if any) 
        /// if there is no JoinVia then this should be 0
        /// </summary>
        public int JoinViaId { get; set; }

        /// <summary>
        /// Gets or sets the value to be displayed
        /// </summary>
        public string DisplayValue { get; set; }

        /// <summary>
        /// Gets or sets the value of the parent control
        /// </summary>
        public string ParentControlId { get; set; }

        /// <summary>
        /// Gets or sets the value of the child control
        /// </summary>
        public string ChildControlId { get; set; }

        /// <summary>
        /// Gets or sets the value of Document guid value. 
        /// </summary>
        public string DocumentGuid { get; set; }

        /// <summary>
        /// Convert the object to the native form
        /// </summary>
        /// <returns>Normal version of the object</returns>
        public AutoCompleteTriggerField ToC()
        {
            Guid displayFieldGuid;
            AutoCompleteTriggerField autoCompleteTriggerField = new AutoCompleteTriggerField();
            if (Guid.TryParseExact(this.DisplayFieldId, "D", out displayFieldGuid))
            {
                autoCompleteTriggerField.ControlId = this.ControlId;
                autoCompleteTriggerField.DisplayFieldId = displayFieldGuid;
                autoCompleteTriggerField.JoinViaId = this.JoinViaId;
                autoCompleteTriggerField.DisplayValue = this.DisplayValue;
            }

            return autoCompleteTriggerField;
        }
    }

    /// <summary>
    /// JavaScript and WebService compatible Trigger Field object
    /// </summary>
    public class AutoCompleteTriggerField
    {
        /// <summary>
        /// Gets or sets the control id
        /// </summary>
        public string ControlId { get; set; }

        /// <summary>
        /// Gets or sets the field id of the cField that the trigger field gets its displayed value from
        /// </summary>
        public Guid DisplayFieldId { get; set; }

        /// <summary>
        /// Gets or sets the JoinVia id of the JoinVia that is associated with the cField (if any) 
        /// if there is no JoinVia then this should be 0
        /// </summary>
        public int JoinViaId { get; set; }

        /// <summary>
        /// Gets or sets the value to be displayed
        /// </summary>
        public string DisplayValue { get; set; }
    }
}
