namespace SpendManagementLibrary.UserDefinedFields
{
    /// <summary>
    /// The structure for passing the value for a user defined field into the system.
    /// </summary>
    public class UserDefinedFieldValue
    {
        /// <summary>
        /// Constructs an instance of the <see cref="UserDefinedFieldValue">UserDefinedField</see> object.
        /// </summary>
        /// <param name="id">The numeric identifier of the user defined field held in the system.</param>
        /// <param name="value">The value supplied by the user for the user defined field</param>
        public UserDefinedFieldValue(int id, object value)
        {
            this.Id = id;
            this.Value = value;
        }

        /// <summary>
        /// The id number for the user defined field.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// The value supplied for the user defined field.
        /// </summary>
        public object Value { get; set; }

        
    }
}
