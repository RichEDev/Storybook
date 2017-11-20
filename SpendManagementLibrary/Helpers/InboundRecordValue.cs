namespace SpendManagementLibrary.Helpers
{
    /// <summary>
    /// An instance of Element field name and value.
    /// </summary>
    public class InboundRecordValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InboundRecordValue"/> class. 
        /// </summary>
        /// <param name="elementFieldName">
        /// The name of the element field.
        /// </param>
        /// <param name="value">
        /// The value of the element field.
        /// </param>
        public InboundRecordValue(string elementFieldName, string value)
        {
            this.ElementFieldName = elementFieldName;
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string ElementFieldName { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.</summary>
        /// <returns>A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.</returns>
        public override string ToString()
        {
            return string.Format("{0},{1}", this.ElementFieldName, this.Value);
        }
    }
}