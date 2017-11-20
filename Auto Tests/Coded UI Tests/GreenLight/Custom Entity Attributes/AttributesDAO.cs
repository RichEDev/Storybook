using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Auto_Tests.Tools;
using System.Data.SqlClient;

namespace Auto_Tests
{
    class AttributesSQLStatements
    {
        public static string sortingOrderForAttributes = "SELECT display_name, description, fieldtype, is_audit_identity FROM customEntityAttributes WHERE entityid = @entityid ORDER BY {0} {1}";
    }

    class AttributesDAO
    {
        private cDatabaseConnection db;
        public AttributesDAO(cDatabaseConnection db)
        {
            this.db = db;
        }

        /// <summary>
        /// Used to return the correct sorting order from the database 
        ///</summary>
        public List<CustomEntitiesUtilities.CustomEntityAttribute> GetCorrectSortingOrderFromDB(SortAttributesByColumn sortby, EnumHelper.TableSortOrder sortingOrder, int entityid)
        {
            List<CustomEntitiesUtilities.CustomEntityAttribute> attributes = new List<CustomEntitiesUtilities.CustomEntityAttribute>();
            db.sqlexecute.Parameters.AddWithValue("@entityid", entityid);
            SqlDataReader reader = db.GetReader(string.Format(AttributesSQLStatements.sortingOrderForAttributes, EnumHelper.GetEnumDescription(sortby), EnumHelper.GetEnumDescription(sortingOrder)));
            while (reader.Read())
            {
                attributes.Add(new CustomEntitiesUtilities.CustomEntityAttribute(reader.GetString(0), reader.GetString(1), (FieldType)reader.GetByte(2), reader.GetBoolean(3)));
            }
            reader.Close();
            db.sqlexecute.Parameters.Clear();
            return attributes;
        }
    }
}
