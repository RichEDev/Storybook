namespace expenses.admin
{
    using System;
    using System.Web.UI;

    using SpendManagementLibrary;
    using Spend_Management;

	/// <summary>
	/// Summary description for changeadvance.
	/// </summary>
	public partial class changeadvance : Page
	{
	
		protected void Page_Load(object sender, EventArgs e)
		{
			Title = "Change Advance Amount";
            Master.title = Title;
			Master.showdummymenu = true;

			if (IsPostBack == false)
			{
				
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Advances, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                cFloats clsfloats = new cFloats(user.AccountID);
				int floatid = int.Parse(Request.QueryString["floatid"]);
				ViewState["floatid"] = floatid;

				cFloat reqfloat = clsfloats.GetFloatById(floatid);
				txtamount.Text = reqfloat.foreignAmount.ToString("########0.00");
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

		}
		#endregion

		private void cmdok_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
		    decimal amount = decimal.Parse(this.txtamount.Text);
			var clsfloats = new cFloats((int)ViewState["accountid"]);
			clsfloats.changeAmount((int)ViewState["floatid"],amount);
			Response.Redirect("adminfloats.aspx",true);
		}
	}
}
