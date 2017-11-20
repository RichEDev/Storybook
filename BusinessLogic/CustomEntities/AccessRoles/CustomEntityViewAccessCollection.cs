
namespace BusinessLogic.CustomEntities.AccessRoles
{
    using System.Collections.Generic;

    /// <summary>
    /// <see cref="CustomEntityViewAccessCollection">CustomEntityFormAccess</see> defines the AccessCollection for a Custom Entity View
    /// </summary>
    public class CustomEntityViewAccessCollection : CustomEntityAccessWrapper<CustomEntityViewAccess>
    {
        /// <summary>
        /// Initalises an instance of <see cref="CustomEntityViewAccessCollection">CustomEntityViewAccessCollection</see>
        /// </summary>
        public CustomEntityViewAccessCollection() : base(new Dictionary<int, CustomEntityViewAccess>())
        {
        }

        /// <summary>
        /// Adds a customerEntityViewId and the <see cref="CustomEntityViewAccess">CustomEntityViewAccess </see>to the base backing collection
        /// </summary>
        /// <param name="customEntityFormId">The customEntityViewId</param>
        /// <param name="viewAccess"> The <see cref="CustomEntityViewAccess">CustomEntityViewAccess </see></param>
        public override void Add(int customEntityFormId, CustomEntityViewAccess viewAccess)
        {
            base.Add(customEntityFormId, viewAccess);
        }
    }
}
