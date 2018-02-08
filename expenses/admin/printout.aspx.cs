namespace expenses.admin
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Services;
    using System.Web.UI;

    using SpendManagementLibrary;

    using Spend_Management;

    /// <summary>
    /// Summary description for printout.
    /// </summary>
    public partial class printout : Page
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            this.Title = "Print-Out";
            this.Master.title = this.Title;
            this.Master.helpid = 1107;
            if (this.IsPostBack == false)
            {
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.DefaultPrintView, true, true);
                this.ViewState["accountid"] = user.AccountID;
                this.ViewState["employeeid"] = user.EmployeeID;

                string[] gridData = this.CreateGrid(user.AccountID, user.EmployeeID);
                this.litgrid.Text = gridData[1];

                // set the sel.grid javascript variables
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "PrintoutGridVars", cGridNew.generateJS_init("PrintoutGridVars", new List<string>() { gridData[0] }, user.CurrentActiveModule), true);
            }
        }

        /// <summary>
        /// Creates print out grid
        /// </summary>
        /// <param name="accountid">The account id of logged in user</param>
        /// <param name="employeeid">The employee id of logged in user</param>
        /// <returns>The html and data of the grid</returns>
        private string[] CreateGrid(int accountid, int employeeid)
        {
            cTables clstables = new cTables(accountid);
            cFields clsfields = new cFields(accountid);
            List<cNewGridColumn> columns = new List<cNewGridColumn>();
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("7B41BAEF-A51D-4C54-81E5-0F1CB594195D")))); // fieldid
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("327D7EE7-C11A-4B16-8C05-BABF7EFE1F71")))); // description
            cGridNew clsgrid = new cGridNew(accountid, employeeid, "gridPrintout", clstables.GetTableByID(new Guid("5B32610E-35DB-492A-B6D1-5F392CA4C040")), columns); // fields
            clsgrid.getColumnByName("fieldid").hidden = true;
            clsgrid.KeyField = "fieldid";
            clsgrid.EnableSelect = true;
            clsgrid.EnableSorting = false;
            var printoutField = clsfields.GetFieldByID(Guid.Parse("01638BC5-5FA4-4D61-A612-B64D026B402C")); // printout
            var idfield = clsfields.GetFieldByID(Guid.Parse("D4FA8348-526C-442F-AC2B-B749CFA5D500")); // idfield
            clsgrid.addFilter(printoutField, ConditionType.Equals, new object[] { true }, null, ConditionJoiner.None);
            clsgrid.addFilter(idfield, ConditionType.Equals, new object[] { false }, null, ConditionJoiner.And);
            return clsgrid.generateGrid();
        }

        /// <summary>
        /// Gets all selected print out fields
        /// </summary>
        /// <param name="accountid">The account id of the logged in user</param>
        /// <returns>Returns the list of fieldids</returns>
        [WebMethod(EnableSession = true)]
        public static List<Guid> GetSelectedFields(int accountid)
        {
            cMisc clsMisc = new cMisc(accountid);
            return clsMisc.GetPrintoutFields();
        }

        /// <summary>
        /// Saves selected printout fields
        /// </summary>
        /// <param name="accountid">The account id of the logged in user</param>
        /// <param name="fields">The list of selected fields</param>
        [WebMethod(EnableSession = true)]
        public static void SaveSelectedFields(int accountid, List<string> fields)
        {
            cMisc clsMisc = new cMisc(accountid);
            Guid[] fieldIds = new Guid[fields.Count];
            for (var i = 0; i < fields.Count; i++)
            {
                Guid fieldid;
                if (Guid.TryParse(fields[i], out fieldid))
                {
                    fieldIds[i] = fieldid;
                }
            }

            clsMisc.UpdatePrintOut(fieldIds);
        }

        /// <summary>
        /// Click event if the cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ImageButton2_Click(object sender, ImageClickEventArgs e)
        {
            this.Response.Redirect(cMisc.Path + "/tailoringmenu.aspx", true);
        }
    }
}