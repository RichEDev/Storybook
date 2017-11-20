using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpendManagementLibrary;

namespace Spend_Management
{
	public class cUFInterim
	{
		public static SortedList<int, object> GetUFRecordValues(int accountid, int recordid, List<cUserDefinedField> ufields)
		{
			SortedList<int, object> retValues = null;

			if (ufields.Count > 0)
			{
				retValues = new SortedList<int, object>();

				string comma ="";
				StringBuilder sb = new StringBuilder();
				sb.Append("select ");

				foreach(cUserDefinedField uf in ufields)
				{
					sb.Append(comma);
					sb.Append(" udf" + uf.userdefineid.ToString());
					comma = ",";
				}
                sb.Append(" from " + ufields[0].table.TableName);
				sb.Append(" where " + ufields[0].table.GetPrimaryKey().FieldName + " = @id");
				
				DBConnection db = new DBConnection(cAccounts.getConnectionString(accountid));
				db.sqlexecute.Parameters.AddWithValue("@id", recordid);

			    using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(sb.ToString()))
			    {
			        while (reader.Read())
			        {
			            object ufVal = null;
			            foreach (cUserDefinedField uf in ufields)
			            {
			                switch (uf.fieldtype)
			                {
			                    case FieldType.AutoCompleteTextbox:
			                    case FieldType.Hyperlink:
			                    case FieldType.LargeText:
			                    case FieldType.Text:
			                    case FieldType.RelationshipTextbox:
			                        if (!reader.IsDBNull(reader.GetOrdinal("udf" + uf.userdefineid.ToString())))
			                        {
			                            ufVal = reader.GetString(reader.GetOrdinal("udf" + uf.userdefineid.ToString()));
			                        }
			                        else
			                        {
			                            ufVal = "";
			                        }
			                        break;
			                    case FieldType.Relationship:
			                    case FieldType.List:
			                    case FieldType.Integer:
			                        if (!reader.IsDBNull(reader.GetOrdinal("udf" + uf.userdefineid.ToString())))
			                        {
			                            ufVal = reader.GetInt32(reader.GetOrdinal("udf" + uf.userdefineid.ToString()));
			                        }
			                        else
			                        {
			                            ufVal = 0;
			                        }
			                        break;
			                    case FieldType.Number:
			                    case FieldType.Currency:
			                        if (!reader.IsDBNull(reader.GetOrdinal("udf" + uf.userdefineid.ToString())))
			                        {
			                            ufVal = reader.GetDecimal(reader.GetOrdinal("udf" + uf.userdefineid.ToString()));
			                        }
			                        else
			                        {
			                            ufVal = 0;
			                        }
			                        break;
			                    case FieldType.TickBox:
			                    case FieldType.RunWorkflow:
			                        if (!reader.IsDBNull(reader.GetOrdinal("udf" + uf.userdefineid.ToString())))
			                        {
			                            ufVal = reader.GetBoolean(reader.GetOrdinal("udf" + uf.userdefineid.ToString()));
			                        }
			                        else
			                        {
			                            ufVal = "";
			                        }
			                        break;
			                    case FieldType.DateTime:
			                        if (!reader.IsDBNull(reader.GetOrdinal("udf" + uf.userdefineid.ToString())))
			                        {
			                            ufVal = reader.GetDateTime(reader.GetOrdinal("udf" + uf.userdefineid.ToString()));
			                        }
			                        else
			                        {
			                            ufVal = DateTime.MinValue;
			                        }
			                        break;
			                    default:
			                        break;
			                }

			                retValues.Add(uf.userdefineid, ufVal);
			            }
			        }
			        reader.Close();
			    }

			    db.sqlexecute.Parameters.Clear();
			}
			return retValues;
		}
	}
}
