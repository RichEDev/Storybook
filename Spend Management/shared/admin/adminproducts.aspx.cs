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
	/// <summary>
	/// Product definition screen
	/// </summary>
	public partial class adminproducts : System.Web.UI.Page
	{
		/// <summary>
		/// ClientID of modal panel for use in js
		/// </summary>
		public string sModalPanel;

		/// <summary>
		/// Main page load function
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
			sModalPanel = pnlProductDetail.ClientID;

			if (!this.IsPostBack)
			{
				Master.title = "Product Details";
				Title = Master.title;

				CurrentUser curUser = cMisc.GetCurrentUser();

                curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Products, false, true);             

                cProducts products = new cProducts(curUser.AccountID);

				lnkAdd.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Products, false, false);

                string[] gridData = products.getProductsGrid();
                litProductGrid.Text = gridData[2];

                // set the sel.grid javascript variables
                Page.ClientScript.RegisterStartupScript(this.GetType(), "productsGridVars", cGridNew.generateJS_init("productsGridVars", new List<string>() { gridData[1] }, curUser.CurrentActiveModule), true);
			}			
		}
	}
}
