using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SpendManagementLibrary;
using System.Web.Services;
using System.Web.Script.Services;
using System.Text;

namespace Spend_Management
{
    using BusinessLogic.Modules;

    public partial class aeuserdefinedgrouping : System.Web.UI.Page
    {
        public int nCurrentGroupingId;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                cmdsave.Attributes.Add("style", "cursor: hand;");
                cmdsave.Attributes.Add("onclick", "if (validateform(null) == false) { return; } else { saveGrouping(); }");
                ddlstassociatedtable.Attributes.Add("onchange", "displayAssociationTable();");

                Master.enablenavigation = false;
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.UserdefinedGroupings, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;
                switch (user.CurrentActiveModule)
                {
                    case Modules.Contracts:
                        Master.helpid = 1147;
                        break;
                    default:
                        Master.helpid = 1021;
                        break;
                }
                int userdefinedgroupid = 0;
                if (Request.QueryString["userdefinedgroupid"] != null)
                {
                    userdefinedgroupid = Convert.ToInt32(Request.QueryString["userdefinedgroupid"]);
                }
                ViewState["userdefinedgroupid"] = userdefinedgroupid;

                cTables clstables = new cTables(user.AccountID);
                ddlstassociatedtable.Items.AddRange(clstables.CreateUserDefinedDropDown(user.CurrentActiveModule).ToArray());

                // remove non-groupable udf tables
                ddlstassociatedtable.Items.Remove(ddlstassociatedtable.Items.FindByValue("0efa50b5-da7b-49c7-a9aa-1017d5f741d0")); // Claims
                ddlstassociatedtable.Items.Remove(ddlstassociatedtable.Items.FindByValue("d70d9e5f-37e2-4025-9492-3bcf6aa746a8")); // Expense Items

                if (userdefinedgroupid > 0)
                {

                    // Disable the associated grouping if in edit mode
                    ddlstassociatedtable.Enabled = false;
                    user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.UserdefinedGroupings, true, true);

                    cUserdefinedFieldGroupings groupings = new cUserdefinedFieldGroupings(user.AccountID);
                    cUserdefinedFieldGrouping grouping = groupings.GetGroupingByID(userdefinedgroupid);
                    if (grouping == null)
                    {
                        Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                    }

                    txtgroupname.Text = grouping.GroupName;
                    //txtorder.Text = grouping.Order.ToString();
                    if (ddlstassociatedtable.Items.FindByValue(grouping.AssociatedTable.TableID.ToString()) != null)
                    {
                        ddlstassociatedtable.Items.FindByValue(grouping.AssociatedTable.TableID.ToString()).Selected = true;
                    }

                    Master.title = "Userdefined Grouping: " + grouping.GroupName;
                }
                else
                {
                    user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.UserdefinedGroupings, true, true);
                    Master.title = "Userdefined Grouping: New";
                }

                nCurrentGroupingId = userdefinedgroupid;
                Master.PageSubTitle = "Userdefined Grouping Details";

                svcUserFieldGroupings svc = new svcUserFieldGroupings();
                string[] gridData = svc.getAssociationGrid((string)ddlstassociatedtable.Items[ddlstassociatedtable.SelectedIndex].Value, userdefinedgroupid);
                if (gridData.Length > 0)
                {
                    litAssociations.Text = gridData[2];

                    // set the sel.grid javascript variables
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "aeUDFGrpGridVars", cGridNew.generateJS_init("aeUDFGrpGridVars", new List<string>() { gridData[1] }, user.CurrentActiveModule), true);
                }
            }
        }

        [WebMethod(EnableSession=true)]
        [ScriptMethod]
        public static string saveGrouping(int groupingid, string groupname, string basetableid, int order, int[] selectedItems)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cUserdefinedFieldGroupings groupings = new cUserdefinedFieldGroupings(user.AccountID);
            cTables clstables = new cTables(user.AccountID);
            int userdefinedgroupid = groupingid;
            //string groupname = txtgroupname.Text;
            Guid associatedtableid = new Guid(basetableid); //ddlstassociatedtable.SelectedValue);
            cTable associatedtable = clstables.GetTableByID(associatedtableid);
            //int order = 0;
            //if (txtorder.Text.Trim() != "")
            //{
            //    order = Convert.ToInt32(txtorder.Text);
            //}

            DateTime createdon;
            int createdby;
            int? modifiedby;
            DateTime? modifiedon;
            if (userdefinedgroupid > 0)
            {
                cUserdefinedFieldGrouping oldcode = groupings.GetGroupingByID(userdefinedgroupid);
                createdon = oldcode.CreatedOn;
                createdby = oldcode.CreatedBy;
                modifiedby = user.EmployeeID; // (int)ViewState["employeeid"];
                modifiedon = DateTime.Now;
            }
            else
            {
                createdon = DateTime.Now;
                createdby = user.EmployeeID; //(int)ViewState["employeeid"];
                modifiedby = null;
                modifiedon = null;
            }
            List<int> filterCategories = new List<int>(selectedItems.ToArray<int>());
            //string[] items = selectedItems.Split(',');
            //for (int x = 0; x < items.Length; x++)
            //{
            //    filterCategories.Add(int.Parse(items[x]));
            //}
            Dictionary<int, List<int>> subAccountCats = new Dictionary<int, List<int>>();
            subAccountCats.Add(user.CurrentSubAccountId, filterCategories);
            cUserdefinedFieldGrouping grouping = new cUserdefinedFieldGrouping(userdefinedgroupid, groupname, order, associatedtable, subAccountCats, createdon, createdby, modifiedon, modifiedby);

            userdefinedgroupid = groupings.SaveGrouping(grouping);

            if (userdefinedgroupid == -1)
            {
                //Page.ClientScript.RegisterStartupScript(Page.GetType(), "alert", "alert('The group name you have entered already exists');\n", true);

                return "The group name you have entered already exists";
            }

            return "";
            //Response.Redirect("userdefinedFieldGroupings.aspx", true);
        }

        protected void cmbcancel_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("userdefinedFieldGroupings.aspx", true);
        }
    }
}
