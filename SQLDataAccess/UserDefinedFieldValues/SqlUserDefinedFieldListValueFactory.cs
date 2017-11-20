namespace SQLDataAccess.UserDefinedFieldValues
{
    using System;
    using System.Collections.Generic;
    
    using BusinessLogic.Fields.Type;
    using BusinessLogic.Fields.Type.ValueList;

    /// <summary>
    /// The sql user defined field list value factory.
    /// </summary>
    public class SqlUserDefinedFieldListValueFactory : UserDefinedFieldListValuesRepository
    {
        /// <summary>
        /// The get.
        /// </summary>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public override List<ListItemValues> Get()
        {
            return new List<ListItemValues>();

            // TODO: Actually get this data from the db.
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ListItemValues"/>.
        /// </returns>
        public override ListItemValues Get(Guid id)
        {
            return new ListItemValues(new Dictionary<int, object>());

            // TODO: Actually get this data from the db.
        }
    }
}