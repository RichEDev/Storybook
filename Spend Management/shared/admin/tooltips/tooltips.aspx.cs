namespace Spend_Management
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web.Script.Serialization;
    using System.Web.Services;
    using System.Web.UI;

    #endregion

    /// <summary>
    /// The tooltips.
    /// </summary>
    public partial class tooltips : Page
    {
        #region Methods

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = "Tooltips";
            this.Master.title = this.Title;

            CurrentUser currentUser = cMisc.GetCurrentUser();

            currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Tooltips, true, true);

            switch (currentUser.CurrentActiveModule)
            {
                case Modules.contracts:
                    this.Master.helpid = 1182;
                    break;
                default:
                    this.Master.helpid = 0;
                    break;
            }

            if (this.IsPostBack == false)
            {
                this.ViewState["accountid"] = currentUser.AccountID;
                this.ViewState["employeeid"] = currentUser.EmployeeID;
                
                this.Master.scriptman.Scripts.Add(new ScriptReference("SyncFusion", string.Empty));
            }

            switch (currentUser.CurrentActiveModule)
            {
                case Modules.SmartDiligence:
                case Modules.SpendManagement:
                case Modules.contracts:
                    this.hlClose.NavigateUrl = "~/MenuMain.aspx?menusection=tailoring";
                    break;
                default:
                    this.hlClose.NavigateUrl = "~/tailoringmenu.aspx";
                    break;
            }
        }


        [WebMethod]
        public static string GetGriDataSet()
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            var clshelp = new cHelp(currentUser.AccountID);
            
            var x = clshelp.getGrid((int)currentUser.CurrentActiveModule);

            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            foreach (DataRow row in x.Rows)
            {
                childRow = x.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => row[col]);
                childRow.Add("edit", "<a href=\"edittooltip.aspx?tooltipID=" + row["tooltipID"] + "\"><img src=\"/shared/images/icons/edit.png\" /></a>");
                parentRow.Add(childRow);
            }

            return jsSerializer.Serialize(parentRow);
        }

        #endregion
    }
}