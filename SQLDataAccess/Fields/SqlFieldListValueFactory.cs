namespace SQLDataAccess.Fields
{
    using System;
    using System.Collections.Generic;
    
    using BusinessLogic.Fields.Type;
    using BusinessLogic.Fields.Type.Base;
    using BusinessLogic.Fields.Type.ValueList;

    /// <summary>
    /// The sql field list value factory.
    /// </summary>
    public class SqlFieldListValueFactory : FieldListValuesRepository
    {
        /// <summary>
        /// Get the complete list of <see cref="ListItemValues"/>
        /// </summary>
        /// <returns>A list of <see cref="ListItemValues"/></returns>
        public override List<ListItemValues> Get()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the <see cref="ListItemValues"/> for a specific <see cref="IField"/> based on the <seealso cref="Guid"/> ID.
        /// </summary>
        /// <param name="id">The ID of the <see cref="IField"/> to get list items values for</param>
        /// <returns>A specific <see cref="ListItemValues"/></returns>
        public override ListItemValues Get(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
