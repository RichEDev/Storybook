namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;

    /// <summary>
	/// Summary description for print.
	/// </summary>
	public partial class print : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{			
			if (this.IsPostBack == false)
			{
                CurrentUser user = cMisc.GetCurrentUser();                
                var clsemployees = new cEmployees(user.AccountID);
                var clsclaims = new cClaims(user.AccountID);

			    this.litstyles.Text = cColours.customiseStyles(false);			
				this.lbldate.Text = DateTime.Today.ToLongDateString();               
				int viewid = int.Parse(this.Request.QueryString["viewid"]);
				int claimid = int.Parse(this.Request.QueryString["claimid"]);
				cClaim reqclaim = clsclaims.getClaimById(claimid);
				Employee claimemp = clsemployees.GetEmployeeById(reqclaim.employeeid);
				string empname = string.Format("{0} {1} {2}", claimemp.Title, claimemp.Forename, claimemp.Surname);
				this.lblemployee.Text = empname;			
                var viewtype = UserView.CurrentPrint;
                switch (viewid)
                {
                    case 1:
                        viewtype = UserView.CurrentPrint;
                        break;
                    case 2:
                        viewtype = UserView.SubmittedPrint;
                        break;
                    case 3:
                        viewtype = UserView.PreviousPrint;
                        break;
                    case 4:
                        viewtype = UserView.CheckAndPayPrint;
                        break;
                }

                var clsglobalcurrencies = new cGlobalCurrencies();
                var clscurrencies = new cCurrencies(user.AccountID, user.CurrentSubAccountId);
                cCurrency reqcurrency = clscurrencies.getCurrencyById(reqclaim.currencyid);
                var misc = new cMisc(user.AccountID);
                cGlobalProperties properties = misc.GetGlobalProperties(user.AccountID);

			    string symbol = reqcurrency != null ? clsglobalcurrencies.getGlobalCurrencyById(reqcurrency.globalcurrencyid).symbol : "£";
                var expenseItems = new cExpenseItems(user.AccountID);
                string[] gridData = expenseItems.generateClaimGrid(user.EmployeeID, reqclaim, "gridExpenses", viewtype, Filter.None, true, false, properties.allowmultipledestinations, false, symbol);
                this.litExpensesGrid.Text = gridData[1];
                Page.ClientScript.RegisterStartupScript(this.GetType(), "gridHistoryVars", cGridNew.generateJS_init("gridHistoryVars", new List<string> { gridData[0] }, user.CurrentActiveModule), true);         
				misc.GeneratePrintOutFields(ref this.fields, claimid, properties.allowmultipledestinations);
                int numreceipts = reqclaim.NumberOfReceipts;
				this.lblnumreceipts.Text = numreceipts.ToString(CultureInfo.InvariantCulture);
                
                this.litJavascript.Text = user.CurrentUserInfoJavascriptVariable; 
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
	}	
}
