namespace SpendManagementLibrary.Definitions.JoinVia
{
    using System;

    /// <summary>
    /// Describes one leg of a JoinVia
    /// </summary>
    [Serializable]
    public class JoinViaPart
    {
        #region fields
        private Guid _viaID;
        private IDType _type;
        private JoinType _join;
        #endregion fields

        #region enums
        /// <summary>
        /// Whether the ID is from a foreign key field join or from a jointables join 
        /// </summary>
        public enum IDType
        {
            Field = 0, // IsForeignKey / Many To One / Genlist
            Table = 1, // JoinTable link / One To Many
            RelatedTable = 2 //Use related table back to current tables primary key.
        }

        /// <summary>
        /// The SQL join type to use, LEFT unless otherwise stated
        /// </summary>
        public enum JoinType
        {
            LEFT = 0,
            INNER = 1
        }
        #endregion enums

        public JoinViaPart(Guid viaID, IDType idType, JoinType joinType = JoinType.LEFT)
        {
            this._viaID = viaID;
            this._type = idType;
            this._join = joinType;
        }

        #region properties
        /// <summary>
        /// The field or table id
        /// </summary>
        public Guid ViaID { get { return this._viaID; } }

        /// <summary>
        /// The id type
        /// </summary>
        public IDType ViaIDType { get { return this._type; } }

        /// <summary>
        /// The join type
        /// </summary>
        public JoinType TypeOfJoin { get { return this._join; } }
        #endregion properties
    }
}