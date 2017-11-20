namespace BusinessLogic.CustomEntities.AccessRoles
{
    using System.Collections.Generic;
    using BusinessLogic.AccessRoles;

    /// <summary>
    /// A wrapper class for the Custom Entity Access
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CustomEntityAccessWrapper<T> where T : ElementAccessLevel
    {
        private readonly IDictionary<int, T> _customEntityAccessCollection;

        /// <summary>
        /// Initialises an instance of <see>CustomEntityAccessWrapper
        ///         <cref>CustomEntityAccessWrapper</cref>
        ///     </see>
        /// </summary>
        /// <param name="accessLevels">The list of Access Levels</param>
        protected CustomEntityAccessWrapper(IDictionary<int, T> accessLevels)
        {
            this._customEntityAccessCollection = accessLevels;
        }

        /// <summary>
        /// Gets the Access Level from its Id
        /// </summary>
        /// <param name="id">The accessLevelId</param>
        /// <returns></returns>
        public T this[int id] => this._customEntityAccessCollection[id];

        /// <summary>
        /// Adds an accessLevelId and AcccessLevel to the backing collection
        /// </summary>
        /// <param name="customEntityFormId"></param>
        /// <param name="value"></param>
        public virtual void Add(int customEntityFormId, T value)
        {
            this._customEntityAccessCollection.Add(customEntityFormId, value);
        }
    }
}
