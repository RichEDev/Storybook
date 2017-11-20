using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using SpendManagementLibrary;
using System.Web.Script.Services;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for svcUserFieldGroupings
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class svcUserFieldGroupings : System.Web.Services.WebService
    {
        [WebMethod(EnableSession=true)]
        [ScriptMethod]
        public string[] getAssociationGrid(string basetableID, int groupingId)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            DBConnection db = new DBConnection(cAccounts.getConnectionString(curUser.AccountID));
            cTables tables = new cTables(curUser.AccountID);
            cFields fields = new cFields(curUser.AccountID);
            System.Data.DataSet ds;
            List<cNewGridColumn> columns = new List<cNewGridColumn>();
            cTable basetable = tables.GetTableByID(new Guid(basetableID));
            cTable catTable = null;
            cUserdefinedFieldGroupings groupings = new cUserdefinedFieldGroupings(curUser.AccountID);
            cUserdefinedFieldGrouping curGrouping = null;

            if (groupingId > 0)
            {
                curGrouping = groupings.GetGroupingByID(groupingId);
            }

            string sql = string.Empty;

            switch (basetable.TableName)
            {
                case "contract_details":
                case "contract_productdetails":
                    sql = "select categoryId, categoryDescription from codes_contractcategory where archived = 0 and subAccountId = @subAccId";
                    catTable = tables.GetTableByName("codes_contractcategory");
                    columns.Add(new cFieldColumn(fields.GetBy(catTable.TableID, "categoryId")));
                    columns.Add(new cFieldColumn(fields.GetBy(catTable.TableID, "categoryDescription")));
                    break;
                case "supplier_details":
                    sql = "select categoryid, Description from supplier_categories where archived = 0 and subAccountId = @subAccId";
                    catTable = tables.GetTableByName("supplier_categories");
                    columns.Add(new cFieldColumn(fields.GetBy(catTable.TableID, "categoryId")));
                    columns.Add(new cFieldColumn(fields.GetBy(catTable.TableID, "description")));
                    break;
                default:
                    sql ="";
                    break;
            }
            
            List<string> retVals = new List<string>();

            if (sql != "")
            {
                db.sqlexecute.Parameters.AddWithValue("@subAccId", curUser.CurrentSubAccountId);
                ds = db.GetDataSet(sql);

                cGridNew grid = new cGridNew(curUser.AccountID, curUser.EmployeeID, "assocGrid_" + basetable.TableID.ToString().Replace("-", "_"), basetable, columns, ds);
                grid.enablepaging = false;
                grid.EnableSelect = true;
                grid.enableupdating = false;
                grid.enabledeleting = false;
                grid.enablearchiving = false;
                grid.EnableSorting = false;
                grid.EmptyText = "No categories defined";
                grid.CssClass = "datatbl";
                grid.KeyField = "categoryId";
                grid.getColumnByName("categoryId").hidden = true;

                if (curGrouping != null && curGrouping.FilterCategories.ContainsKey(curUser.CurrentSubAccountId) != false)
                {
                    grid.SelectedItems = curGrouping.FilterCategories[curUser.CurrentSubAccountId].ConvertAll<object>(delegate(int i) { return (object)i; });
                }
                retVals.Add(grid.GridID);
                retVals.AddRange(grid.generateGrid());
            }

            return retVals.ToArray();
        }
    }
}
