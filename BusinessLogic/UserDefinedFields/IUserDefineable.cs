namespace BusinessLogic.UserDefinedFields
{
    /// <summary>
    /// When implemented,the class will implement User defined field values.
    /// </summary>
    public interface IUserDefineable
    {
        /// <summary>
        /// Get the <see cref="UserDefinedFieldValueCollection"/> for the given class.
        /// </summary>
        UserDefinedFieldValueCollection UserDefinedFieldValues { get; }
    }
}
