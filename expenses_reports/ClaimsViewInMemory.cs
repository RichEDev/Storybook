namespace Expenses_Reports
{
    using System;
    using System.Collections.Generic;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Logic_Classes.Fields;
    /// <summary>
    /// Class to add the claims view as an in memory table
    /// </summary>
    public class ClaimsViewInMemory
    {
        /// <summary>
        /// Modifies the sql string that is passed to include the claims view as an in memory table
        /// </summary>
        /// <param name="strsql">The sql string to modify</param>
        /// <param name="claimsFields">The list of cFields that will form the columns of the claims in memory table</param>
        internal void ModifySqlWithClaimsViewInMemoryTable(ref string strsql, List<cField> claimsFields)
        {
            if (claimsFields.Count > 0)
            {
                string prependSql = $"{this.DeclareClaimsInMemoryTable(claimsFields)} {this.BuildInsertToMemoryTable(claimsFields)}";
                strsql = $"{prependSql} {strsql}";
                strsql = strsql.Replace("join [claims]", "join @claims claims");
            }
        }

        /// <summary>
        /// Creates the list of fields that are used in the report and belong to the claims view
        /// </summary>
        /// <param name="claimsFields">The list of claims fields</param>
        /// <param name="fieldFound">The specific field that will be checked to see if it's in the claims view</param>
        /// <param name="fields">The account fields</param>
        internal void BuildClaimsFieldList(ref List<cField> claimsFields, cField fieldFound, IFields fields)
        {
            cTable table = fieldFound.GetParentTable();

            if (table.TableID == new Guid("0EFA50B5-DA7B-49C7-A9AA-1017D5F741D0"))
            {
                this.AddField(ref claimsFields, fieldFound);
                this.AddKeyFields(ref claimsFields, fields, table);
            }
        }

        private void AddKeyFields(ref List<cField> claimsFields, IFields fields, cTable table)
        {
            var foreignKeys = fields.GetFieldsByTableID(table.TableID).FindAll(f => f.IsForeignKey);
            var primaryKey = table.GetPrimaryKey();

            this.AddField(ref claimsFields, primaryKey);

            foreach (var field in foreignKeys)
            {
                this.AddField(ref claimsFields, field);
            }
        }

        private void AddField(ref List<cField> claimsFields, cField field)
        {
            if (claimsFields.Find(cf => cf.FieldID == field.FieldID) == null)
            {
                claimsFields.Add(field);
            }
        }

        private string BuildInsertToMemoryTable(List<cField> claimsFields)
        {
            string sql = @"INSERT INTO @claims SELECT ";
            string fieldSql = string.Empty;
            foreach (var field in claimsFields)
            {
                if (string.IsNullOrEmpty(fieldSql))
                {
                    fieldSql += field.FieldName;
                }
                else
                {
                    fieldSql += string.Format(", {0}", field.FieldName);
                }
            }
            sql += fieldSql + " FROM dbo.claims ";
            return sql;
        }

        private string DeclareClaimsInMemoryTable(List<cField> claimsFields)
        {
            var sql = @"DECLARE @claims TABLE ( ";
            var fieldSql = string.Empty;

            foreach (var field in claimsFields)
            {
                if (string.IsNullOrEmpty(fieldSql))
                {
                    fieldSql += string.Format("{0} {1}", field.FieldName, cFields.TranslateFieldTypeToSqlType(field.FieldType));
                }
                else
                {
                    fieldSql += string.Format(",{0} {1}", field.FieldName, cFields.TranslateFieldTypeToSqlType(field.FieldType));
                }
            }
            sql += fieldSql + ")";
            return sql;
        }
    }
}