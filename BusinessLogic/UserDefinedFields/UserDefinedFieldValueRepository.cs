namespace BusinessLogic.UserDefinedFields
{
    using BusinessLogic.Accounts.Elements;

    /// <summary>
    /// Create an maintain a list of User Defined Field values.
    /// </summary>
    public abstract class UserDefinedFieldValueRepository
    {
        /// <summary>
        /// Get a <see cref="IUserDefinedFieldValues"/> for the given module.
        /// </summary>
        /// <param name="moduleElement"></param>
        /// <returns></returns>
        protected abstract IUserDefinedFieldValues Get(ModuleElements moduleElement);


        /// <summary>
        /// Save the given values to the database
        /// </summary>
        /// <param name="id">The record identifier</param>
        /// <param name="userDefinedFieldValues">The <see cref="UserDefinedFieldValueCollection"/> values to be saved</param>
        /// <param name="moduleElement">The module to save this for</param>
        public abstract void Save(int id, UserDefinedFieldValueCollection userDefinedFieldValues, ModuleElements moduleElement);
    }
}
