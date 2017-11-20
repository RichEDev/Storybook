using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;

namespace SpendManagementUnitTests.Global_Objects
{
    class cUserDefinedFieldGroupingObject
    {
        public static cUserdefinedFieldGrouping BlankTemplate()
        {
            return new cUserdefinedFieldGrouping(0, "UDF Grouping " + DateTime.UtcNow.ToString() + DateTime.UtcNow.Ticks.ToString(), 0, null, new Dictionary<int, List<int>>(), DateTime.UtcNow, cGlobalVariables.EmployeeID, null, null);
        }

        public static cUserdefinedFieldGrouping ValidTemplate()
        {
            return new cUserdefinedFieldGrouping(0, "UDF Grouping " + DateTime.UtcNow.ToString() + DateTime.UtcNow.Ticks.ToString(), 0, GetUDFGroupingTable(), NoFilteredCategories(), DateTime.UtcNow, cGlobalVariables.EmployeeID, null, null);
        }

        public static cUserdefinedFieldGrouping InvalidTemplate()
        {
            return new cUserdefinedFieldGrouping(0, "UDF Grouping " + DateTime.UtcNow.ToString() + DateTime.UtcNow.Ticks.ToString(), 0, null, NoFilteredCategories(), DateTime.UtcNow, cGlobalVariables.EmployeeID, null, null);
        }

        public static int CreateID()
        {
            int newID;
            cUserdefinedFieldGroupings clsGroupings = new cUserdefinedFieldGroupings(cGlobalVariables.AccountID);
            newID = clsGroupings.SaveGrouping(ValidTemplate());

            return newID;
        }

        public static cUserdefinedFieldGrouping CreateObject()
        {
            int newID;
            cUserdefinedFieldGroupings clsGroupings = new cUserdefinedFieldGroupings(cGlobalVariables.AccountID);
            newID = clsGroupings.SaveGrouping(ValidTemplate());

            cUserdefinedFieldGrouping oGrouping = clsGroupings.GetGroupingByID(newID);

            return oGrouping;
        }

        public static cTable GetUDFGroupingTable()
        {
            cTables clsTables = new cTables(cGlobalVariables.AccountID);
            cTable oTable = clsTables.GetTableByID(new Guid("618db425-f430-4660-9525-ebab444ed754"));

            return oTable;
        }

        public static Dictionary<int, List<int>> NoFilteredCategories()
        {
            Dictionary<int, List<int>> filters = new Dictionary<int, List<int>>();
            filters.Add(cGlobalVariables.DefaultSubAccountID, new List<int>());

            return filters;
        }
    }
}
