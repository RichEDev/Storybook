namespace SpendManagementLibrary.Cards
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Web.UI.WebControls;

    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    using Utilities.DistributedCaching;

    #endregion

    /// <summary>
    /// The corporate cards class.
    /// </summary>
    public class CorporateCards
    {
        #region Constants

        /// <summary>
        /// The cache area.
        /// </summary>
        public const string CacheArea = "corporatecards";

        #endregion

        #region Fields

        /// <summary>
        /// The n accountid.
        /// </summary>
        private readonly int _accountid;

        /// <summary>
        /// The list.
        /// </summary>
        private Dictionary<int, cCorporateCard> list;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the CorporateCards class.
        /// </summary>
        /// <param name="accountid">
        /// The accountid.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        public CorporateCards(int accountid, IDBConnection connection = null)
        {
            this._accountid = accountid;
            this.InitialiseData(connection);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the accountid.
        /// </summary>
        public int Accountid
        {
            get
            {
                return this._accountid;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Creates a drop down list of card providers.
        /// </summary>
        /// <returns>
        /// A List Item array - Web Controls.
        /// </returns>
        public ListItem[] CreateDropDown()
        {
            SortedList<string, cCorporateCard> sorted = this.SortList();

            return sorted.Values.Select(card => new ListItem(card.cardprovider.cardprovider, card.cardprovider.cardproviderid.ToString(CultureInfo.InvariantCulture))).ToArray();
        }

        /// <summary>
        /// Adds a corporate card.
        /// </summary>
        /// <param name="card">
        /// The card.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        public void AddCorporateCard(cCorporateCard card, ICurrentUserBase user, IDBConnection connection = null)
        {
            IDBConnection expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.Accountid));
            expdata.sqlexecute.Parameters.AddWithValue("@cardproviderid", card.cardprovider.cardproviderid);
            expdata.sqlexecute.Parameters.AddWithValue("@claimantsettlesbill", Convert.ToByte(card.claimantsettlesbill));
            expdata.sqlexecute.Parameters.AddWithValue("@date", card.createdon);
            expdata.sqlexecute.Parameters.AddWithValue("@userid", card.createdby);
            if (card.allocateditem == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@allocateditem", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@allocateditem", card.allocateditem);
            }

            expdata.sqlexecute.Parameters.AddWithValue("@blockcash", Convert.ToByte(card.blockcash));
            expdata.sqlexecute.Parameters.AddWithValue("@reconciledbyadministrator", Convert.ToByte(card.reconciledbyadministrator));
            expdata.sqlexecute.Parameters.AddWithValue("@singleclaim", Convert.ToByte(card.singleclaim));
            expdata.sqlexecute.Parameters.AddWithValue("@fileIdentifier", card.FileIdentifier);
            if (user.isDelegate)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@delegateID", user.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            }

            expdata.ExecuteProc("saveCorporateCard");
            expdata.sqlexecute.Parameters.Clear();

            this.ResetCache();
        }

        /// <summary>
        /// Deletes a corporate card.
        /// </summary>
        /// <param name="cardproviderid">
        /// The cardproviderid.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <returns>
        /// Returns the success of the method call.
        /// </returns>
        public bool DeleteCorporateCard(int cardproviderid, ICurrentUserBase user)
        {
            var expdata = new DBConnection(cAccounts.getConnectionString(this.Accountid));
            expdata.sqlexecute.Parameters.AddWithValue("@cardproviderid", cardproviderid);
            expdata.sqlexecute.Parameters.AddWithValue("@employeeID", user.EmployeeID);
            if (user.isDelegate)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@delegateID", user.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            }

            expdata.sqlexecute.Parameters.Add("@returnvalue", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@returnvalue"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("deleteCorporateCard");
            var returnvalue = (int)expdata.sqlexecute.Parameters["@returnvalue"].Value;
            expdata.sqlexecute.Parameters.Clear();

            this.ResetCache();

            return returnvalue == 0;
        }

        /// <summary>
        /// The get corporate card by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="cCorporateCard"/>.
        /// </returns>
        public cCorporateCard GetCorporateCardById(int id)
        {
            cCorporateCard card;
            this.list.TryGetValue(id, out card);
            return card;
        }

        /// <summary>
        /// The get grid.
        /// </summary>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public DataTable GetGrid()
        {
            var tbl = new DataTable();

            tbl.Columns.Add("cardproviderid", typeof(int));
            tbl.Columns.Add("provider", typeof(string));
            tbl.Columns.Add("claimantssettlebill", typeof(bool));

            foreach (cCorporateCard card in this.list.Values)
            {
                var values = new object[3];
                values[0] = card.cardprovider.cardproviderid;
                values[1] = card.cardprovider.cardprovider;
                values[2] = card.claimantsettlesbill;
                tbl.Rows.Add(values);
            }

            return tbl;
        }

        /// <summary>
        /// Returns a list of modified corporate cards.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// A dictionary of corporate cards with the id as the key.
        /// </returns>
        public Dictionary<int, cCorporateCard> GetModifiedCorporateCards(DateTime date)
        {
            return
                this.list.Values.Where(c => c.createdon > date || c.modifiedon > date)
                    .ToDictionary(c => c.cardprovider.cardproviderid, c => c);
        }

        /// <summary>
        /// Sorts the list of corporate cards.
        /// </summary>
        /// <returns>
        /// A sorted list of corporate cars with the car provider as key.
        /// </returns>
        public SortedList<string, cCorporateCard> SortList()
        {
            var lst = new SortedList<string, cCorporateCard>();
            foreach (cCorporateCard card in this.list.Values)
            {
                lst.Add(card.cardprovider.cardprovider, card);
            }

            return lst;
        }

        /// <summary>
        /// updates a corporate card.
        /// </summary>
        /// <param name="card">
        /// The card.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        public void UpdateCorporateCard(cCorporateCard card, ICurrentUserBase user)
        {
            var expdata = new DBConnection(cAccounts.getConnectionString(this.Accountid));
            expdata.sqlexecute.Parameters.AddWithValue("@cardproviderid", card.cardprovider.cardproviderid);
            expdata.sqlexecute.Parameters.AddWithValue("@claimantsettlesbill", Convert.ToByte(card.claimantsettlesbill));
            expdata.sqlexecute.Parameters.AddWithValue("@date", card.modifiedon);
            expdata.sqlexecute.Parameters.AddWithValue("@userid", card.modifiedby);
            if (card.allocateditem == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@allocateditem", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@allocateditem", card.allocateditem);
            }

            expdata.sqlexecute.Parameters.AddWithValue("@blockcash", Convert.ToByte(card.blockcash));
            expdata.sqlexecute.Parameters.AddWithValue("@reconciledbyadministrator", Convert.ToByte(card.reconciledbyadministrator));
            expdata.sqlexecute.Parameters.AddWithValue("@singleclaim", Convert.ToByte(card.singleclaim));
            expdata.sqlexecute.Parameters.AddWithValue("@fileIdentifier", card.FileIdentifier);
            if (user.isDelegate)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@delegateID", user.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            }

            expdata.ExecuteProc("saveCorporateCard");
            expdata.sqlexecute.Parameters.Clear();
            if (this.list.ContainsKey(card.cardprovider.cardproviderid))
            {
                this.list[card.cardprovider.cardproviderid] = card;
            }
            else
            {
                this.list.Add(card.cardprovider.cardproviderid, card);
            }

            this.ResetCache();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Caches the list of corporate cards.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// A dictionary of corporate cars with the id as key.
        /// </returns>
        private Dictionary<int, cCorporateCard> CacheList(IDBConnection connection = null)
        {
            var returnValue = new Dictionary<int, cCorporateCard>();
            var clsproviders = new CardProviders();
            IDBConnection expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.Accountid));

            const string Sql =
                "select cardproviderid, claimants_settle_bill, createdon, createdby, modifiedon, modifiedby, allocateditem, blockcash, reconciled_by_admin, singleclaim, FileIdentifier from dbo.corporate_cards";

            expdata.sqlexecute.CommandText = Sql;

            using (var reader = expdata.GetReader(Sql))
            {
                var fileIdentifierOrd = reader.GetOrdinal("FileIdentifier");
                while (reader.Read())
                {
                    cCardProvider provider = clsproviders.getProviderByID((int)reader["cardproviderid"]);
                    var claimantsettlesbill = (bool)reader["claimants_settle_bill"];
                    var createdon = (DateTime)reader["createdon"];
                    var createdby = (int)reader["createdby"];
                    DateTime? modifiedon = reader.GetNullable<DateTime>("modifiedon");
                    int? modifiedby = reader.GetNullable<int>("modifiedby");
                    int? allocateditem = reader.GetNullable<int>("allocateditem");
                    var blockcash = (bool)reader["blockcash"];
                    var reconciledbyadministrator = (bool)reader["reconciled_by_admin"];
                    var singleclaim = (bool)reader["singleclaim"];
                    var fileIdentifier = reader.IsDBNull(fileIdentifierOrd)
                        ? string.Empty
                        : reader.GetString(fileIdentifierOrd);
                    var card = new cCorporateCard(
                        provider, 
                        claimantsettlesbill, 
                        createdon, 
                        createdby, 
                        modifiedon, 
                        modifiedby, 
                        allocateditem, 
                        blockcash, 
                        reconciledbyadministrator, 
                        singleclaim, 
                        fileIdentifier);
                    returnValue.Add(provider.cardproviderid, card);
                }

                reader.Close();
            }

            return returnValue;
        }

        /// <summary>
        /// Initialises the list of corporate cards.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        private void InitialiseData(IDBConnection connection = null)
        {
            var cache = new Cache();
            this.list = cache.Get(this.Accountid, string.Empty, CacheArea) as Dictionary<int, cCorporateCard>;
            if (this.list == null)
            {
                this.list = this.CacheList(connection);
                cache.Add(this.Accountid, string.Empty, CacheArea, this.list);
            }
        }

        /// <summary>
        /// Force an update of the cache
        /// </summary>
        private void ResetCache()
        {
            var cache = new Cache();
            cache.Delete(this.Accountid, string.Empty, CacheArea);
            this.InitialiseData();
        }

        #endregion
    }
}