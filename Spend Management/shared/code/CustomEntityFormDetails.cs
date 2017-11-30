namespace Spend_Management
{
    /// <summary>
    /// An instance of custom entity form details
    /// </summary>
    public class CustomEntityFormDetails
    {
        /// <summary>
        /// Default constructor for CustomEntityFormDetails
        /// </summary>
        /// <param name="formId">Id of the form</param>
        /// <param name="textVal">String value from the dropdown</param>
        /// <param name="listVal">Value from the dropdown</param>
        public CustomEntityFormDetails(int formId, string textVal, int listVal)
        {
            this.FormId = formId;
            this.TextVal = textVal;
            this.ListVal = listVal;
        }

        /// <summary>
        /// Id of the form
        /// </summary>
        public int FormId { get; set; }

        /// <summary>
        /// String for the custom entity
        /// </summary>
        public string TextVal { get; set; }

        /// <summary>
        /// Value for the custom entity
        /// </summary>
        public int ListVal { get; set; }
    }
}
