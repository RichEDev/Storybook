//===========================================================================
// This file was modified as part of an ASP.NET 2.0 Web project conversion.
// The class name was changed and the class modified to inherit from the abstract base class 
// in file 'App_Code\Migrated\Stub_dispute_aspx_cs.cs'.
// During runtime, this allows other classes in your web application to bind and access 
// the code-behind page using the abstract base class.
// The associated content page 'dispute.aspx' was also modified to refer to the new class name.
// For more information on this code pattern, please refer to http://go.microsoft.com/fwlink/?LinkId=46995 
//===========================================================================
namespace expenses
{
    using System;
    using System.Web.UI;
    using SpendManagementLibrary;
    using Spend_Management;

	/// <summary>
	/// Summary description for dispute.
	/// </summary>
	public partial class dispute : Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Title = "Dispute Returned Expense";
            Master.title = Title;
            Master.helpid = 1165;
			
			if (IsPostBack == false)
			{
                CurrentUser user = cMisc.GetCurrentUser();
                ViewState["accountid"] = user.AccountID;
                
				string expenseid;
				string claimid;
				expenseid = Request.QueryString["expenseid"];
				claimid = Request.QueryString["claimid"];
				txtexpenseid.Text = expenseid;
				txtclaimid.Text = claimid;

				
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
			this.cmdok.Click += new System.Web.UI.ImageClickEventHandler(this.cmdok_Click);
			this.cmdcancel.Click += new System.Web.UI.ImageClickEventHandler(this.cmdcancel_Click);

		}
		#endregion

		private void cmdok_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			
			cClaims clsclaims = new cClaims((int)ViewState["accountid"]);
			cClaim reqclaim;
			int expenseid = 0;
			string dispute;
			int claimid;
			dispute = txtdispute.Text;
			claimid = int.Parse(txtclaimid.Text);
			expenseid = int.Parse(txtexpenseid.Text);

			reqclaim = clsclaims.getClaimById(claimid);
			
            clsclaims.DisputeExpense(reqclaim, clsclaims.getExpenseItemById(expenseid), dispute);
            
            Response.Redirect("expenses/claimViewer.aspx?returned=1&claimid=" + claimid, true);
		}

		private void cmdcancel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			int claimid;
			claimid = int.Parse(txtclaimid.Text);
            Response.Redirect("expenses/claimViewer.aspx?claimid=" + claimid, true);
		}
	}
}
