namespace SpendManagementLibrary.Definitions.JoinVia
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    [Serializable]
    public class JoinSteps
    {
        private readonly int accountId;

        /// <summary>
        /// The join table id.
        /// </summary>
        private readonly Guid joinTableId;

        /// <summary>
        /// The _backing collection.
        /// </summary>
        private readonly IDictionary<int, cJoinStep> _backingCollection;

        /// <summary>
        /// Initialises a new instance of the <see cref="JoinSteps"/> class.
        /// </summary>
        /// <param name="accountId">
        /// The account Id.
        /// </param>
        /// <param name="joinTableId">
        /// The join table id.
        /// </param>
        /// <param name="currentUser">
        /// The current user.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        public JoinSteps(int accountId, Guid joinTableId, IDBConnection connection = null)
        {
            this.accountId = accountId;
            this.joinTableId = joinTableId;
            this._backingCollection = this.Get(connection);
        }

        /// <summary>
        /// The remove.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        public void Remove(int key)
        {
            this._backingCollection.Remove(key);
        }

        /// <summary>
        /// Gets the steps.
        /// </summary>
        public IDictionary<int, cJoinStep> Steps
        {
            get
            {
                return this._backingCollection;
            }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count
        {
            get
            {
                return this._backingCollection.Count;
            }
        }

        /// <summary>
        /// The this.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="cJoinStep"/>.
        /// </returns>
        public cJoinStep this[int key]
        {
            get
            {
                return this._backingCollection[key];
            }
        }

        /// <summary>
        /// The get by.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <returns>
        /// The <see cref="cJoinStep"/>.
        /// </returns>
        public cJoinStep GetBy(int order)
        {
            cJoinStep result;
            this._backingCollection.TryGetValue(order, out result);
            return result;
        }

        /// <summary>
        /// Get Join Steps from Database.
        /// </summary>
        /// <returns>
        /// The <see cref="IDictionary"/>.
        /// </returns>
        public IDictionary<int, cJoinStep> Get(IDBConnection connection = null)
        {
            Dictionary<int, cJoinStep> lstJoinSteps;
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.accountId)))
            {
                lstJoinSteps = new Dictionary<int, cJoinStep>();
                databaseConnection.sqlexecute.Parameters.Clear();
                string strSQL = "SELECT [joinbreakdownid], [jointableid], [order], [tableid], [sourcetable], [joinkey],  destinationkey, amendedon FROM [dbo].";

                strSQL += "[joinbreakdown] WHERE [jointableid] = @jointableid";
                databaseConnection.sqlexecute.Parameters.AddWithValue("jointableid", this.joinTableId);

                cJoinStep reqStep = null;

                Guid joinbreakdownid, jointableid, joinkey, destinationkey;
                Guid sourcetableid, destinationtableid;
                byte order;
                DateTime amendedon;

                using (IDataReader reader = databaseConnection.GetReader(strSQL))
                {
                    while (reader.Read())
                    {
                        joinbreakdownid = reader.GetGuid(reader.GetOrdinal("joinbreakdownid"));
                        jointableid = reader.GetGuid(reader.GetOrdinal("jointableid"));
                        sourcetableid = reader.GetGuid(reader.GetOrdinal("sourcetable"));
                        destinationtableid = reader.GetGuid(reader.GetOrdinal("tableid"));
                        joinkey = reader.GetGuid(reader.GetOrdinal("joinkey"));
                        order = reader.GetByte(reader.GetOrdinal("order"));
                        if (reader.IsDBNull(reader.GetOrdinal("amendedon")) == true)
                        {
                            amendedon = new DateTime(1900, 01, 01);
                        }
                        else
                        {
                            amendedon = reader.GetDateTime(reader.GetOrdinal("amendedon"));
                        }
                        if (reader.IsDBNull(reader.GetOrdinal("destinationkey")) == true)
                        {
                            destinationkey = Guid.Empty;
                        }
                        else
                        {
                            destinationkey = reader.GetGuid(reader.GetOrdinal("destinationkey"));
                        }

                        reqStep = new cJoinStep(joinbreakdownid, jointableid, order, destinationtableid, sourcetableid, joinkey, amendedon, destinationkey);
                        lstJoinSteps.Add(order, reqStep);
                    }

                    databaseConnection.sqlexecute.Parameters.Clear();
                    reader.Close();
                }
            }

            return lstJoinSteps;
        }

        public void Save(cJoinStep step)
        {

        }

        public void Delete(Guid joinStep)
        {

        }

        public IEnumerator<KeyValuePair<int, cJoinStep>> GetEnumerator()
        {
            return this._backingCollection.GetEnumerator();
        }
    }
}
