namespace SQLDataAccess.CustomEntities
{
    using System;
    using System.Collections.Generic;

    using BusinessLogic.Fields.Type;
    using BusinessLogic.Fields.Type.ValueList;

    /// <summary>
    /// The sql custom entity field list values factory.
    /// </summary>
    public class SqlCustomEntityFieldListValuesFactory : CustomEntityFieldListValuesRepository
    {
        /// <summary>
        /// Gets a List of <see cref="ListItemValues">ListItemValues</see> 
        /// </summary>
        /// <returns>
        /// A List of <see cref="ListItemValues"/>.
        /// </returns>
        public override List<ListItemValues> Get()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a <see cref="ListItemValues">ListItemValues</see> by its Id
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ListItemValues"/>.
        /// </returns>
        public override ListItemValues Get(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
