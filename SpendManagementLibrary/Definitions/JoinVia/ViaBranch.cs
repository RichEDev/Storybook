namespace SpendManagementLibrary.Definitions.JoinVia
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Used only in the join builder to track joins that have been made for a query to de-duplicate
    /// </summary>
    public class ViaBranch
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ViaBranch"/> class.
        /// </summary>
        public ViaBranch()
        {
            this.TableName = string.Empty;
            this.UnderBranches = new Dictionary<Guid, ViaBranch>();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="ViaBranch"/> class.
        /// </summary>
        /// <param name="tableName">
        /// The base table name or an alias for a joined table
        /// </param>
        public ViaBranch(string tableName)
        {
            this.TableName = tableName;
            this.UnderBranches = new Dictionary<Guid, ViaBranch>();
        }

        /// <summary>
        /// Gets or sets the table name, base table will be a proper name; 
        /// otherwise a GUID AS substitute made up of the JoinViaId, and an incremented counter if not the last step in a join
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Gets the ViaBranches collection for joins beyond this branch, any JoinVia will add a new ViaBranch recursively
        /// </summary>
        public Dictionary<Guid, ViaBranch> UnderBranches { get; private set; }
    }
}