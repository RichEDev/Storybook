namespace BusinessLogic.Fields.Type.ValueList
{
    using System;
    using System.Collections.Generic;
    using BusinessLogic.Fields.Type.Base;

    /// <summary>
    /// An implementaion of <see cref="IFieldListValuesRepository"/> for storing and accessing <see cref="ListItemValues"/>
    /// </summary>
    public abstract class CustomEntityFieldListValuesRepository : IFieldListValuesRepository
    {
        /// <summary>
        /// Get the complete list of <see cref="ListItemValues"/>
        /// </summary>
        /// <returns></returns>
        public abstract List<ListItemValues> Get();

        /// <summary>
        /// Get the <see cref="ListItemValues"/> for a specific <see cref="IField"/> based on the <seealso cref="Guid"/> ID.
        /// </summary>
        /// <param name="id">The ID of the <see cref="IField"/> to get list items values for</param>
        /// <returns></returns>
        public abstract ListItemValues Get(Guid id);
        
    }
}