using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Auto_Tests.Tools;
using System.Data.SqlClient;

namespace Auto_Tests
{
    public class UserDefinedFieldsSQLStatements
    {
        public static string GET_ALL_UDFS = "SELECT userdefineid AS userdefineid," +
            " display_name AS  display_name," +
            " description AS description," +
            " userdefined.fieldtype AS fieldtype," +
            " userdefined.mandatory AS mandatory," +
            " userdefinedInformation.appliesTo AS appliesTo" +
            " FROM userdefined" +
            " inner join userdefinedinformation on userdefinedinformation.userdefined_table = userdefined.tableid" +
            " ORDER BY {0} {1}";
    }

    public class UserDefinedFieldsDAO
    {
        private cDatabaseConnection db;
        public UserDefinedFieldsDAO(cDatabaseConnection db)
        {
            this.db = db;
        }

        public List<UserDefinedFieldsDTO> GetAll(SortByColumn sortby, EnumHelper.TableSortOrder order)
        {
            List<UserDefinedFieldsDTO> userDefinedFields = new List<UserDefinedFieldsDTO>();
            SqlDataReader reader = db.GetReader(String.Format(UserDefinedFieldsSQLStatements.GET_ALL_UDFS, EnumHelper.GetEnumDescription(sortby), EnumHelper.GetEnumDescription(order)));
            while (reader.Read())
            {
                userDefinedFields.Add(new UserDefinedFieldsDTO(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), Convert.ToInt32(reader.GetByte(3)), reader.GetBoolean(4), reader.GetString(5)));
            }
            reader.Close();
            return userDefinedFields;
        }
    }
}
