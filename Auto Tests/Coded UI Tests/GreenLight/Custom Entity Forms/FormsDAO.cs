using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Auto_Tests.Tools;
using System.Data.SqlClient;

namespace Auto_Tests
{
    class FormsSQLStatements
    {
        public static string sortingOrderForForms = "SELECT form_name, description FROM customEntityForms WHERE entityid = @entityid ORDER BY {0} {1}";
    }

    class FormsDAO
    {
        private cDatabaseConnection db;
        public FormsDAO(cDatabaseConnection db)
        {
            this.db = db;
        }

        /// <summary>
        /// Used to return the correct sorting order from the database 
        ///</summary>
        public List<CustomEntitiesUtilities.CustomEntityForm> GetCorrectSortingOrderFromDB(SortFormsByColumn sortby, EnumHelper.TableSortOrder sortingOrder, int entityid)
        {
            List<CustomEntitiesUtilities.CustomEntityForm> forms = new List<CustomEntitiesUtilities.CustomEntityForm>();
            db.sqlexecute.Parameters.AddWithValue("@entityid", entityid);
            SqlDataReader reader = db.GetReader(string.Format(FormsSQLStatements.sortingOrderForForms, EnumHelper.GetEnumDescription(sortby), EnumHelper.GetEnumDescription(sortingOrder)));
            while (reader.Read())
            {
                forms.Add(new CustomEntitiesUtilities.CustomEntityForm(reader.GetString(0), reader.GetString(1)));
            }
            reader.Close();
            return forms;
        }
    }
}
