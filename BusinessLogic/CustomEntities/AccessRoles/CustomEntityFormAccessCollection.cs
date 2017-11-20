namespace BusinessLogic.CustomEntities.AccessRoles
{
    using System.Collections.Generic;

    /// <summary>
    /// <see cref="CustomEntityFormAccessCollection">CustomEntityFormAccess</see> defines the AccessCollection for a Custom Entity Form
    /// </summary>
    public class CustomEntityFormAccessCollection : CustomEntityAccessWrapper<CustomEntityFormAccess>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomEntityFormAccessCollection"/> class. 
        /// </summary>
        public CustomEntityFormAccessCollection() : base(new Dictionary<int, CustomEntityFormAccess>())
        {
        }

        /// <summary>
        /// Add a customerEntityFormId and <see cref="CustomEntityFormAccess">CustomEntityFormAccess</see> to the base backing collection
        /// </summary>
        /// <param name="customEntityFormId">The customEntityFormId</param>
        /// <param name="formAccess">The <see cref="CustomEntityFormAccess">CustomEntityFormAccess</see></param>
        public override void Add(int customEntityFormId, CustomEntityFormAccess formAccess)
        {
            base.Add(customEntityFormId, formAccess);
        }
    }
}
