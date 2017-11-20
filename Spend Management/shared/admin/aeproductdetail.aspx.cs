using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SpendManagementLibrary;
using AjaxControlToolkit;

namespace Spend_Management
{
	/// <summary>
	/// Product details page
	/// </summary>
	public partial class aeproductdetail : System.Web.UI.Page
	{
		/// <summary>
		/// Primary page load
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!this.IsPostBack)
			{
				CurrentUser curUser = cMisc.GetCurrentUser();
				Master.PageSubTitle = "Product Details";
				Master.enablenavigation = false;

				int productid = 0;
				if (Request.QueryString["pid"] != null)
				{
					productid = int.Parse(Request.QueryString["pid"]);
				}

				ViewState["productid"] = productid;
                cProducts products = new cProducts(curUser.AccountID);

                cBaseDefinitions clsBaseDefs = new cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.ProductCategories);

				if (productid > 0)
				{
                    curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Products, true, true);

                    cProduct product = products.GetProductById(productid);
					populateProductDetails(curUser, productid);

					TabPanel tpLic = new TabPanel();
					tpLic.ID = "tabPDLicences";
					tpLic.HeaderText = "Product Licences";

					cProductLicences lic = new cProductLicences(curUser.AccountID, curUser.CurrentSubAccountId, curUser.EmployeeID, productid, cAccounts.getConnectionString(curUser.AccountID));
					productTabContainer.Tabs.Add(tpLic);
                    Title = "Product Detail: " + product.ProductName;
				}
                else
                {
                    lstProductCategory.Items.AddRange(clsBaseDefs.CreateDropDown(true, 0));

                    curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Products, true, true);
                    Title = "Product Detail: New";
                }

                Master.title = Title;
				cUserdefinedFields ufields = new cUserdefinedFields(curUser.AccountID);
				cTables clsTables = new cTables(curUser.AccountID);
                List<cUserDefinedField> pdUFields = ufields.GetFieldsByTable(clsTables.GetTableByName("productDetails"));

				if (pdUFields.Count > 0)
				{
					TabPanel tpUF = new TabPanel();
					tpUF.ID = "pdufTab";
					tpUF.HeaderText = "Product User Fields";
					PlaceHolder phUF = new PlaceHolder();
					System.Text.StringBuilder jscript = new System.Text.StringBuilder();
                    ufields.createFieldPanel(ref phUF, clsTables.GetTableByID(new Guid(ReportTable.ProductDetails)), "ufproducts", out jscript);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "udfscript", jscript.ToString(), true);

					tpUF.Controls.Add(phUF);
					productTabContainer.Tabs.Add(tpUF);
				}
			}
		}

		/// <summary>
		/// Populate the product details onto the form
		/// </summary>
		/// <param name="curUser">Current user's credentials</param>
		/// <param name="productid">ID of the product to be populated</param>
		private void populateProductDetails(CurrentUser curUser, int productid)
		{
			cProducts products = new cProducts(curUser.AccountID, curUser.CurrentSubAccountId);
			cProduct curProduct = products.GetProductById(productid);
            cBaseDefinitions clsBaseDefs = new cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.ProductCategories);

			txtProductName.Text = curProduct.ProductName;
			txtProductCode.Text = curProduct.ProductCode;
			txtDescription.Text = curProduct.ProductDescription;
			int idx = 0;
            if (curProduct.ProductCategory != null)
            {
                lstProductCategory.Items.AddRange(clsBaseDefs.CreateDropDown(true, curProduct.ProductCategory.ID));
                idx = lstProductCategory.Items.IndexOf(lstProductCategory.Items.FindByValue(curProduct.ProductCategory.ID.ToString()));
            }
            else
            {
                lstProductCategory.Items.AddRange(clsBaseDefs.CreateDropDown(true, 0));
            }

			lstProductCategory.SelectedIndex = idx;

			return;
		}

		/// <summary>
		/// Save Product Details
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void cmdExecSave_Click(object sender, ImageClickEventArgs e)
		{
			CurrentUser curUser = cMisc.GetCurrentUser();
			DBConnection db = new DBConnection(cAccounts.getConnectionString(curUser.AccountID));

			int productid = (int)ViewState["productid"];
            cBaseDefinitions clsBaseDefs = new cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.ProductCategories);
            cProductCategory prodCategory = (cProductCategory)clsBaseDefs.GetDefinitionByID(Convert.ToInt32(lstProductCategory.SelectedValue));

            cProducts products = new cProducts(curUser.AccountID);
            cProduct product = new cProduct(productid, curUser.CurrentSubAccountId, txtProductCode.Text.Trim(), txtProductName.Text.Trim(), txtDescription.Text.Trim(), prodCategory, false, DateTime.Now, curUser.EmployeeID, DateTime.Now, curUser.EmployeeID);

            productid = products.UpdateProduct(product, curUser.EmployeeID);

            return;
        }
	}
}
