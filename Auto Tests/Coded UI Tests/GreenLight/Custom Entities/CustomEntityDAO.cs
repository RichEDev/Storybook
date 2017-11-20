using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Auto_Tests.Tools;
using System.Data.SqlClient;
using Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entities;

namespace Auto_Tests
{
    class CustomEntitySQLStatements
    {
        public static string sortingOrderForCustomEntities = "SELECT TOP 20 entity_name, description FROM customEntities ORDER BY {0} {1}";
    }

    class CustomEntityDAO
    {
         
        private cDatabaseConnection db;
        public CustomEntityDAO(cDatabaseConnection db)
        {
            this.db = db;
        }

        /// <summary>
        /// Used to return the correct sorting order from the database 
        ///</summary>
        public List<CustomEntity> GetCorrectSortingOrderFromDB(SortCustomEntitiesByColumn sortby, EnumHelper.TableSortOrder sortingOrder)
        {
            List<CustomEntity> customEntities = new List<CustomEntity>();
            SqlDataReader reader = db.GetReader(string.Format(CustomEntitySQLStatements.sortingOrderForCustomEntities, EnumHelper.GetEnumDescription(sortby), EnumHelper.GetEnumDescription(sortingOrder)));
            while (reader.Read())
            {
                string entityName = reader.GetString(0);
                string entityDescription = " ";
                if (!reader.IsDBNull(1))
                {
                    entityDescription = reader.GetString(1);
                }
                customEntities.Add(new CustomEntity(entityName, entityDescription));
                //customEntities.Add(new CustomEntity(reader.GetString(0), reader.GetString(1)));
            }
            reader.Close();
            return customEntities;
        }
    }
}
