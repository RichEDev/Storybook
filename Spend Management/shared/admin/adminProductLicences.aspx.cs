using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SpendManagementLibrary;
using System.Text;

namespace Spend_Management
{
	public partial class adminProductLicences : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            if (!this.IsPostBack)
            {
                CurrentUser curUser = cMisc.GetCurrentUser();
                curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ProductLicences, true, true);
                
                if(!curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.ProductLicences, true, false))
                {
                    lnkAddLicence.Style.Add(HtmlTextWriterStyle.Display, "None");
                }

                if (Request.QueryString["pid"] != null)
                {
                    int productid = int.Parse(Request.QueryString["pid"]);

                    cProductLicences lics = new cProductLicences(curUser.AccountID, curUser.CurrentSubAccountId, curUser.EmployeeID, productid, cAccounts.getConnectionString(curUser.AccountID));

                    string[] gridData = lics.getLicencesGrid(productid);
                    litLicenceGrid.Text = gridData[2];

                    // set the sel.grid javascript variables
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "LicencesGridVars", cGridNew.generateJS_init("LicencesGridVars", new List<string>() { gridData[1] }, curUser.CurrentActiveModule), true);
                }
            }
		}

        private void populateRecord(int productId, int licenceId)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();

            cProductLicences lics = new cProductLicences(curUser.AccountID, curUser.CurrentSubAccountId, curUser.EmployeeID, productId, cAccounts.getConnectionString(curUser.AccountID));
            cProductLicence curLicence = lics.GetLicenceById(licenceId);
            //Needs implementing as a dropdown
            cBaseDefinitions clsBaseDefs = new cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.ProductLicenceTypes); 

            if (curLicence != null)
            {
                txtInstalledVersion.Text = curLicence.InstalledVersion;
                txtAvailableVersion.Text = curLicence.AvailableVersion;
                txtDateInstalled.Text = curLicence.DateInstalled.HasValue ? curLicence.DateInstalled.Value.ToShortDateString() : "";
                txtUserCode.Text = curLicence.UserCode;
            }
        }
	}
}
