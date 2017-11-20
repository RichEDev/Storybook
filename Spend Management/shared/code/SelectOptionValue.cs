namespace Spend_Management.shared.webServices
{
    /// <summary>
    /// A data item for a web service call to return to populate a select with options
    /// </summary>
    public class SelectOptionValue
    {
        /// <summary>
        /// The ID of the object, to provide the option's value
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// The name of the object, to provide the option's text
        /// </summary>
        public string name { get; set; }
    }
}