namespace SpendManagementLibrary.Definitions.JoinVia
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Used to specify the way to join to a field on a grid or on a report, not to be confused with cJoin/cJoinStep
    /// </summary>
    [Serializable]
    public class JoinVia
    {
        #region fields
        private int _joinViaID;
        private string _joinViaDescription;
        private Guid _joinViaAS;
        private SortedList<int, JoinViaPart> _joinViaList;
        #endregion fields

        /// <summary>
        /// Initialises a new instance of the <see cref="JoinVia"/> class. 
        /// Constructor
        /// </summary>
        /// <param name="id">
        /// the id
        /// </param>
        /// <param name="description">
        /// the description
        /// </param>
        /// <param name="viaAS">
        /// ID and unique join AS alias
        /// </param>
        /// <param name="viaList">
        /// List of via fields or tables with their 0 indexed order as the key, set to an empty list if not passed
        /// </param>
        public JoinVia(int id, string description, Guid viaAS, SortedList<int, JoinViaPart> viaList = null)
        {
            this._joinViaID = id;
            this._joinViaDescription = description;
            this._joinViaAS = viaAS;
            this._joinViaList = viaList ?? new SortedList<int, JoinViaPart>();
        }

        #region methods
        /// <summary>
        /// Adds a via to the end of the order
        /// </summary>
        /// <param name="viaPart">The JoinViaPart to add</param>
        /// <returns>Returns the order int added at</returns>
        public int AddVia(JoinViaPart viaPart)
        {
            int order = this._joinViaList.Keys.Max() + 1;
            this._joinViaList.Add(order, viaPart);

            return order;
        }
        #endregion methods

        #region properties
        /// <summary>
        /// The ID for the JoinVia
        /// </summary>
        public int JoinViaID
        {
            get { return this._joinViaID; }
        }
        /// <summary>
        /// The ID for the JoinVia, also used as the unique id for the join AS alias
        /// </summary>
        public Guid JoinViaAS
        {
            get { return this._joinViaAS; }
        }
        /// <summary>
        /// The list of via fields or tables
        /// </summary>
        public SortedList<int, JoinViaPart> JoinViaList
        {
            get { return this._joinViaList; }
        }
        /// <summary>
        /// The text used to describe where the join came from, notably on Grid column headings
        /// </summary>
        public string Description
        {
            get { return this._joinViaDescription; }
        }

        /// <summary>
        /// Returns a description including the full path to the view/view field. This description is built dynamically rather than using the Description property which 
        /// will not get updated when an attribute is renamed.
        /// </summary>
        /// <param name="accountId">Account ID</param>
        /// <returns>Description inclusive of paths.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public string RuntimeDescription(Int32 accountId)
        {
            string description = String.Empty;
            if (this.JoinViaList.Count > 0)
            {
                description += "(";
                foreach (var joinViaPart in this.JoinViaList.Values)
                {
                    Guid viaId = joinViaPart.ViaID;
                    JoinViaPart.IDType viaType = joinViaPart.ViaIDType;
                    switch (viaType)
                    {
                        case JoinViaPart.IDType.Field:
                            cFields fields = new cFields(accountId);
                            cField field = fields.GetFieldByID(viaId);
                            description += field.Description;
                            break;
                        case JoinViaPart.IDType.Table:
                            cTables tables = new cTables(accountId);
                            cTable table = tables.GetTableByID(viaId);
                            description += String.Format(" :  {0}", table.TableName);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                description += ")";
            }
            return description == string.Empty ? "N/A" : description;
        }

        /// <summary>
        /// Gets the table alias used for this JoinVia.
        /// </summary>
        public string TableAlias
        {
            get
            {
                var joinViaPart = this.JoinViaList.LastOrDefault();
                 return $"{joinViaPart.Value.ViaID}_{joinViaPart.Key}";
             } 
        }
        #endregion properties
    }
}
