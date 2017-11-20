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
using SpendManagementLibrary;
using System.Text;
using System.Collections.Generic;



namespace Spend_Management
{
	/// <summary>
	/// Summary description for admindepartments.
	/// </summary>
	public partial class adminIPfilters : Page
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{       
            Title = "IP Address Filtering";
            Master.title = Title;
            //Master.helpid = 1021;

            CurrentUser user = cMisc.GetCurrentUser();
            user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.IPAdressFiltering, true, true);

            if (user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.IPAdressFiltering, true) == false)
            {
                pnlAddNewIPFilter.Visible = false;
            }
           
			if (IsPostBack == false)
			{
                litHelpGuide.Text = @"Please take caution when activating the IP Address Filtering.
                                    <p>The IP Address Filter is installed by default, but is not enabled.</p>                                 
                                    <p>The IP Address Filter lets you control what IP address traffic is allowed into the product, thus allowing you to deny access to unwanted IP addresses. By enabling this feature you will be restricting access to the product from the specified locations.</p>
                                    Once you have added an IP address entry, the feature is not enabled until the Active option has been selected.";            

                string[] gridData = CreateGrid();
                litgrid.Text = gridData[1];

                // set the sel.grid javascript variables
                Page.ClientScript.RegisterStartupScript(this.GetType(), "IPFiltersGridVars", cGridNew.generateJS_init("IPFiltersGridVars", new List<string>() { gridData[0] }, user.CurrentActiveModule), true);
			}

            StringBuilder sbJS = new StringBuilder();
            sbJS.Append("IPFiltersVars = function () {\n");
            sbJS.Append("SEL.IPFilters.TxtIPAddressDomID = \"" + txtipaddress.ClientID + "\";\n");
            sbJS.Append("SEL.IPFilters.ChkActiveDomID = \"" + chkactive.ClientID + "\";\n");
            sbJS.Append("SEL.IPFilters.TxtDescriptionDomID = \"" + txtdescription.ClientID + "\";\n");
            sbJS.Append("SEL.IPFilters.MdlWindowDomID = \"" + modIPFilter.ClientID + "\";\n");
            sbJS.Append("};\n");
            sbJS.Append("IPFiltersVars.registerClass(\"IPFiltersVars\", Sys.Component);\n");
            sbJS.Append("Sys.Application.add_init(\n");
            sbJS.Append("\tfunction(){\n");
            sbJS.Append("\t\t$create(IPFiltersVars, {\"id\":\"IPFiltersVariables\"}, null, null, null)\n");
            sbJS.Append("\t}\n");
            sbJS.Append(");\n");

            Page.ClientScript.RegisterStartupScript(this.GetType(), "IPFiltersVars", sbJS.ToString(), true);
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


        /// <summary>
        /// Create grid and output to screen
        /// </summary>
        /// <param name="modalID"></param>
        /// <returns></returns>
        public string[] CreateGrid()
        {
            CurrentUser user = cMisc.GetCurrentUser();            
            cGridNew gridIPFilters = new cGridNew(user.AccountID, user.EmployeeID, "gridIPFilters", "SELECT ipFilterID, ipAddress, description, active FROM ipFilters");

            gridIPFilters.KeyField = "ipFilterID";
            gridIPFilters.EmptyText = "There are no IP Filters defined.";
            gridIPFilters.enablearchiving = false;
            if (user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.IPAdressFiltering, true))
            {
                gridIPFilters.editlink = "javascript:SEL.IPFilters.EditIPFilter({ipFilterID});";
                gridIPFilters.enableupdating = true;
            }
            else
            {
                gridIPFilters.enableupdating = false;
            }
            if (user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.IPAdressFiltering, true))
            {
                gridIPFilters.deletelink = "javascript:SEL.IPFilters.DeleteIPFilter({ipFilterID});";
                gridIPFilters.enabledeleting = true;
            }
            else
            {
                gridIPFilters.enabledeleting = false;
            }            
            
            if (user.Employee.GetNewGridSortOrders().GetBy("gridIPFilters") == null)
            {
                gridIPFilters.SortedColumn = gridIPFilters.getColumnByName("ipAddress");
                gridIPFilters.SortDirection = SpendManagementLibrary.SortDirection.Ascending;
            }

            gridIPFilters.getColumnByName("ipFilterID").hidden = true;
            return gridIPFilters.generateGrid();          
        }


        /// <summary>
        /// Close button event function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdClose_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Response.Redirect((SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url, true);
        }
	}
}
