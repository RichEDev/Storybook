namespace BusinessLogic.Fields.Type.ValueList
{
    using System;
    using System.Collections.Generic;
    using BusinessLogic.Fields.Type.Base;

    public interface IFieldListValuesRepository
    {
        /// <summary>
        /// Get the complete list of <see cref="ListItemValues"/>
        /// </summary>
        /// <returns></returns>
        List<ListItemValues> Get();

        /// <summary>
        /// Get the <see cref="ListItemValues"/> for a specific <see cref="IField"/> based on the <seealso cref="Guid"/> ID.
        /// </summary>
        /// <param name="id">The ID of the <see cref="IField"/> to get list items values for</param>
        /// <returns></returns>
        ListItemValues Get(Guid id);
    }
}