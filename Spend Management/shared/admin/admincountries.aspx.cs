using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Services;
using Infragistics.WebUI.UltraWebGrid;
using SpendManagementLibrary;
using Spend_Management;
using System.Text;

namespace Spend_Management
{
    using BusinessLogic.Modules;

    /// <summary>
	/// Summary description for admincountries.
	/// </summary>
	public partial class admincountries : Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.Expires = 60;
			Response.ExpiresAbsolute = DateTime.Now.AddMinutes(-1);
			Response.AddHeader ("pragma","no-cache");
			Response.AddHeader ("cache-control","private");
			Response.CacheControl = "no-cache";
			
			Title = "Countries";
            Master.title = Title;
            CurrentUser user = cMisc.GetCurrentUser();
            switch (user.CurrentActiveModule)
            {
                case Modules.Contracts:
                    Master.helpid = 0;
                    break;
                default:
                    Master.helpid = 1019;
                    break;
            }

			if (IsPostBack == false)
			{
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Countries, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                string[] gridData = CreateGrid(cmbfilter.SelectedValue);
                litgrid.Text = gridData[1];

                // set the sel.grid javascript variables
                Page.ClientScript.RegisterStartupScript(this.GetType(), "CountriesGridVars", cGridNew.generateJS_init("CountriesGridVars", new System.Collections.Generic.List<string>() { gridData[0] }, user.CurrentActiveModule), true);

                if (user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Countries, true) == false)
                {
                    string tmpstr = "if(document.getElementById('lnkAddCountry') != null) { document.getElementById('lnkAddCountry').style.display = 'none'; }\n";
                    ClientScript.RegisterStartupScript(this.GetType(), "hideAddLink", tmpstr, true);
                }
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion

        [WebMethod(EnableSession = true)]
        public static string[] CreateGrid(string FilterVal)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cCountries clsCountries = new cCountries(user.AccountID, user.CurrentSubAccountId);
            cGridNew newgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridCountries", clsCountries.GetGrid());

            newgrid.addFilter(((cFieldColumn)newgrid.getColumnByName("subAccountId")).field, ConditionType.Equals, new object[] { user.CurrentSubAccountId }, null, ConditionJoiner.None);
            
            if (FilterVal != "2")
            {
                int val = int.Parse(FilterVal);
                newgrid.addFilter(((cFieldColumn)newgrid.getColumnByName("archived")).field, ConditionType.Equals, new object[] { val }, null, ConditionJoiner.And);
            }

            newgrid.enablearchiving = user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Countries, true);
            newgrid.enabledeleting = user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.Countries, true);
            newgrid.enableupdating = user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Countries, true);
            newgrid.EmptyText = "There are no countries to display.";
            newgrid.editlink = "aecountry.aspx?countryid={countryid}";
            newgrid.deletelink = "javascript:deleteCountry({countryid});";
            newgrid.archivelink = "javascript:changeArchiveStatus({countryid});";
            newgrid.ArchiveField = "archived";
            newgrid.getColumnByName("countryid").hidden = true;
            newgrid.getColumnByName("archived").hidden = true;
            newgrid.getColumnByName("subAccountId").hidden = true;
            newgrid.KeyField = "countryid";
            return newgrid.generateGrid();
        }



        [WebMethod(EnableSession = true)]
        public static int deleteCountry(int countryid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cCountries clscountries = new cCountries(user.AccountID, user.CurrentSubAccountId);
            return clscountries.deleteCountry(countryid);
        }

        [WebMethod(EnableSession = true)]
        public static int changeStatus(int countryid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cCountries clsCountries = new cCountries(user.AccountID, user.CurrentSubAccountId);
            cCountry reqCountry = clsCountries.getCountryById(countryid);

            return clsCountries.changeStatus(countryid, !reqCountry.Archived);
        }

        /// <summary>
        /// Close button event function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdClose_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            switch (user.CurrentActiveModule)
            {
                case Modules.SmartDiligence:
                case Modules.SpendManagement:
                case Modules.Contracts:
                    Response.Redirect("~/MenuMain.aspx?menusection=baseinfo", true);
                    break;
                default:
                    Response.Redirect("~/categorymenu.aspx", true);
                    break;
            }
        }
	}
}
