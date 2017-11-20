using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    using SpendManagementLibrary.Logic_Classes.Fields;

    [Serializable()]
    public class cTable : IRelabel
    {
        private int accountID;

        #region Properties

        /// <summary>
        /// Gets or sets the account id.
        /// </summary>
        public int AccountID
        {
            get
            {
                return this.accountID;
            }
            set
            {
                this.accountID = value;
            }
        }

        public string TableName { get; set; }
        public byte JoinType { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }
        public bool AllowReportOn { get; set; }
        public bool AllowImport { get; set; }
        public bool AllowWorkflow { get; set; }
        public bool AllowEntityRelationship { get; set; }
        public bool HasUserdefinedFields { get { return (UserDefinedTableID != Guid.Empty) ? true : false; } }
        public Guid TableID { get; set; }
        public Guid PrimaryKeyID { get; set; }
        public Guid KeyFieldID { get; set; }
        public Guid UserDefinedTableID { get; set; }
        public Guid SubAccountIDFieldID { get; set; }
        public cTable.TableSourceType TableSource { get; set; }
        public int? ElementID { get; set; }
        public Guid StringKeyFieldID { get; set; }
        public bool IsSystemView { get; set; }

        /// <summary>
        /// Gets or sets the relabel item to use (if any) used in <see cref="IRelabler{T}"/>
        /// </summary>
        public string RelabelParam { get; set; }

        public bool LinkingTable { get; set; }

        #region Table/Field properties

        public cField GetPrimaryKey()
        {
            return this.GetField(this.PrimaryKeyID);
        }

        public cField GetKeyField()
        {
            return this.GetField(this.KeyFieldID);
        }

        public cTable GetUserdefinedTable()
        {
            return this.GetTable(this.UserDefinedTableID);
        }

        public cField GetSubAccountIDField()
        {
            return this.GetField(this.SubAccountIDFieldID);
        }

        #endregion Table/Field properties
        #endregion Properties

        public enum TableSourceType
        {
            Metabase = 0,
            CustomEntites = 1,
            CustomTables = 2
        }

        private cTable GetTable(Guid tableID)
        {
            cTables clsTables = null;
            if (this.AccountID == 0)
            {
                clsTables = new cTables();
            }
            else
            {
                clsTables = new cTables(this.AccountID);
            }

            return clsTables.GetTableByID(tableID);
        }

        private cField GetField(Guid fieldID)
        {
            cFields clsFields = null;
            if (this.AccountID == 0) // metabase field
            {
                clsFields = new cFields();
            }
            else
            {
                clsFields = new cFields(this.AccountID);
            }

            return clsFields.GetFieldByID(fieldID);
        }

        public cTable()
        {

        }

        public cTable(int accountID, string tableName, byte joinType, string description, bool allowReportOn, bool allowImport, bool allowWorkflow, bool allowEntityRelationship, Guid tableID, Guid primaryKeyID, Guid keyFieldID, Guid userdefinedTableID, Guid subAccountIDFieldID, TableSourceType tableSource, int? elementID, Guid subAccountIDField, bool isSysView, string relabelParam, bool linkingTable)
        {
            this.AccountID = accountID;
            this.TableName = tableName;
            this.JoinType = joinType;
            this.Description = description;
            this.AllowReportOn = allowReportOn;
            this.AllowImport = allowImport;
            this.AllowWorkflow = allowWorkflow;
            this.AllowEntityRelationship = allowEntityRelationship;
            this.TableID = tableID;
            this.PrimaryKeyID = primaryKeyID;
            this.KeyFieldID = keyFieldID;
            this.UserDefinedTableID = userdefinedTableID;
            this.SubAccountIDFieldID = subAccountIDFieldID;
            this.TableSource = tableSource;
            this.ElementID = elementID;
            this.IsSystemView = isSysView;
            this.RelabelParam = relabelParam;
            this.LinkingTable = linkingTable;
        }

        /// <summary>
        /// instatiate a new version of <see cref="cField"/> based on an existing object
        /// </summary>
        /// <param name="table">The <see cref="cField"/>to copy</param>
        public cTable(cTable table)
        {
            if (table == null)
            {
                return;
            }

            this.AccountID = table.AccountID;
            this.TableName = table.TableName;
            this.JoinType = table.JoinType;
            this.Description = table.Description;
            this.AllowReportOn = table.AllowReportOn;
            this.AllowImport = table.AllowImport;
            this.AllowWorkflow = table.AllowWorkflow;
            this.AllowEntityRelationship = table.AllowEntityRelationship;
            this.TableID = table.TableID;
            this.PrimaryKeyID = table.PrimaryKeyID;
            this.KeyFieldID = table.KeyFieldID;
            this.UserDefinedTableID = table.UserDefinedTableID;
            this.SubAccountIDFieldID = table.SubAccountIDFieldID;
            this.TableSource = table.TableSource;
            this.ElementID = table.ElementID;
            this.IsSystemView = table.IsSystemView;
            this.RelabelParam = table.RelabelParam;
            this.LinkingTable = table.LinkingTable;
        }
    }

    [Serializable()]
    public struct sTableBasics
    {
        private Guid nTableID;
        private string sDescription;

        public sTableBasics(Guid tableID, string description)
        {
            nTableID = tableID;
            sDescription = description;
        }

        public Guid TableID
        {
            get { return nTableID; }
        }

        public string Description
        {
            get { return sDescription; }
        }
    }
}
