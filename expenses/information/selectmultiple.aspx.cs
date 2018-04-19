namespace expenses.information
{
    using System;
    using System.Data;
    using System.Web.UI;
    using System.Collections.Generic;

    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;

    using Spend_Management;

	/// <summary>
	/// Summary description for selectmultiple.
	/// </summary>
	public partial class selectmultiple : Page
	{
		protected System.Web.UI.WebControls.ImageButton ImageButton3;

	    /// <summary>
	    /// An instance of <see cref="IDataFactory{IGeneralOptions,Int32}"/> to get a <see cref="IGeneralOptions"/>
	    /// </summary>
        [Dependency]
	    public IDataFactory<IGeneralOptions, int> GeneralOptionsFactory { get; set; }

        protected void Page_Load(object sender, System.EventArgs e)
		{
			
			Title = "Claimable Items";
            Master.title = Title;
            Master.helpid = 1031;
			if (IsPostBack == false)
			{

                CurrentUser user = cMisc.GetCurrentUser();
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;
                createGrid(user.Employee);
		}
	}

		public void createGrid(Employee employee)
		{
            cCategories clscategories = new cCategories((int)ViewState["accountid"]);
            cMisc clsmisc = new cMisc((int)ViewState["accountid"]);
		    var generalOptions = this.GeneralOptionsFactory[cMisc.GetCurrentUser().CurrentSubAccountId].WithCurrency();
            int i;
            DataSet ds = new DataSet();
            DataTable tbl = new DataTable();
            tbl.Columns.Add("category", typeof(System.String));
            tbl.Columns.Add("subcat", typeof(System.String));
            tbl.Columns.Add("description", typeof(System.String));
            tbl.Columns.Add("receiptmaximum", typeof(System.Decimal));
            tbl.Columns.Add("maximum", typeof(System.Decimal));
            tbl.Columns.Add("isadditem", typeof(System.Boolean));
            tbl.Columns.Add("subcatid", typeof(System.Int32));
            tbl.Columns.Add("basecurrency", typeof(System.Int32));

            var subcats = new cSubcats((int)ViewState["accountid"]);
            List<SubcatItemRoleBasic> roleitems = subcats.GetSubCatsByEmployeeItemRoles(employee.EmployeeID);
            foreach (SubcatItemRoleBasic rolesub in roleitems)
            {
                cCategory category = clscategories.FindById(rolesub.CategoryId);
                object[] values = new object[8];
                values[0] = category.category;
                values[1] = rolesub.Subcat;
                values[2] = rolesub.Description;
                values[3] = rolesub.ReceiptMaximum;
                values[4] = rolesub.Maximum;
                values[5] = employee.GetSubCategories().Contains(rolesub.SubcatId);
                values[6] = rolesub.SubcatId;
                values[7] = generalOptions.Currency.BaseCurrency;
                tbl.Rows.Add(values);
            }

            ds.Tables.Add(tbl);
            cGrid clsgrid = new cGrid(ds, true, false);
            cGridColumn newcol = new cGridColumn("appear", "Tick to<br>appear", "S", "", false, true);
            clsgrid.tblclass = "datatbl";
            clsgrid.gridcolumns.Insert(0, newcol);
            clsgrid.getColumn("category").description = "Expense Category";

            clsgrid.getColumn("basecurrency").hidden = true;
            clsgrid.getColumn("subcat").description = "Expense Item";
            clsgrid.getColumn("isadditem").hidden = true;
            clsgrid.getColumn("subcatid").hidden = true;
            clsgrid.getColumn("description").description = "Description";
            clsgrid.getColumn("receiptmaximum").description = "Maximum Limit<br>without receipt";
            clsgrid.getColumn("receiptmaximum").fieldtype = "C";
            clsgrid.getColumn("maximum").description = "Maximum Limit";
            clsgrid.getColumn("maximum").fieldtype = "C";
            clsgrid.getData();
            for (i = 0; i < clsgrid.gridrows.Count; i++)
            {
                cGridRow reqrow = (cGridRow)clsgrid.gridrows[i];
                bool isadditem = (bool)reqrow.getCellByName("isadditem").thevalue;
                reqrow.getCellByName("appear").thevalue = "<input title=\"The items ticked here will appear on your add multiple screen.\" type=checkbox name=\"isadditem\" value=\"" + reqrow.getCellByName("subcatid").thevalue + "\"";
                if (isadditem)
                {
                    reqrow.getCellByName("appear").thevalue += " checked";
                }
                reqrow.getCellByName("appear").thevalue += " />";

            }
            litgrid.Text = clsgrid.CreateGrid();
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
			this.cmdok.Click += new System.Web.UI.ImageClickEventHandler(this.cmdok_Click);
			this.cmdcancel.Click += new System.Web.UI.ImageClickEventHandler(this.cmdcancel_Click);

		}
		#endregion



		private void cmdok_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			
		
			int accountid = (int)ViewState["accountid"];
			string isadditem = Request.Form["isadditem"];
			string[] arrisadditem = new string[0];
			if (isadditem != null)
			{
				if (isadditem != "")
				{
					arrisadditem = isadditem.Split(',');
				}
			}

            List<int> checkeditems = new List<int>();
			for (int i = 0; i < arrisadditem.Length; i++)
			{
				checkeditems.Add(int.Parse(arrisadditem[i].Trim()));
			}

            
            cEmployees clsemployees = new cEmployees(accountid);
            Employee reqemp = clsemployees.GetEmployeeById((int)ViewState["employeeid"]);
            
            
		    reqemp.GetSubCategories().AddMultiple(checkeditems.ToArray());

            createGrid(reqemp);
			lblmsg.Text = "Your item selection has been updated successfully.";
			lblmsg.Visible = true;

			if (Request.QueryString["change"] != null)
			{
				Response.Redirect("../aeexpense.aspx?additems=2",true);
			}
		}



		private void cmdcancel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
            Response.Redirect("~/home.aspx", true);
		}

		
	}
}
