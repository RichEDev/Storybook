namespace SpendManagementLibrary.Definitions
{
    using System.Collections.Generic;

    /// <summary>
    /// The autocomplete field with its values
    /// </summary>
    public class AutoCompleteChildField
    {
        /// <summary>
        /// The id of the child field
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// A list of the child values
        /// </summary>
        public List<AutoCompleteChildFieldValues> ChildFieldValues { get; set; }
    }
}
