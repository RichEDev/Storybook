namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The online userdefined info struct.
    /// </summary>
    [Serializable()]
    public struct sOnlineUserdefinedInfo
    {
        /// <summary>
        /// The <see cref="Dictionary{TKey,TValue}"/> of user defined field ID and <seealso cref="cUserDefinedField"/> .
        /// </summary>
        public Dictionary<int, cUserDefinedField> lstUserdefined;

        /// <summary>
        /// A <see cref="List{T}"/> of user defined field ID's.
        /// </summary>
        public List<int> lstUserdefinedids;

        /// <summary>
        /// A <see cref="Dictionary{TKey,TValue}"/> of List Item ID's and <seealso cref="cListItem"/>.
        /// </summary>
        public Dictionary<int, cListItem> lstListitems;

        /// <summary>
        /// A <see cref="List{T}"/> if List Item ID's.
        /// </summary>
        public List<int> lstListitemids;
    }
}
