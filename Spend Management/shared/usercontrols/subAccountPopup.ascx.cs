using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SpendManagementLibrary;
using System.Text;

namespace Spend_Management.shared.usercontrols
{
    public partial class subAccountPopup : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                cAccountSubAccounts clsSubAccs = new cAccountSubAccounts(currentUser.AccountID);

                ListItem[] tmpLstSA = clsSubAccs.CreateFilteredDropDown(currentUser.Employee, currentUser.CurrentSubAccountId);
                if (tmpLstSA.Length > 1 || (tmpLstSA.Length == 1 && tmpLstSA[0].Value != currentUser.CurrentSubAccountId.ToString()))
                {
                    int rowCount = 0;
                    string[] gridData = clsSubAccs.generateSubaccountGrid(ref rowCount);
                    litSubAccounts.Text = gridData[2];

                    // set the sel.grid javascript variables
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "subAccPopupGridVars", cGridNew.generateJS_init("subAccPopupGridVars", new List<string>() { gridData[1] }, currentUser.CurrentActiveModule), true);

                    if (rowCount > 10)
                    {
                        divSubAccounts.Style.Add("overflow", "auto");
                        divSubAccounts.Style.Add("height", "350px");
                    }
                }
            }
        }  
    }
}