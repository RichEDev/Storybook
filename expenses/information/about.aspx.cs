namespace expenses.information
{
    using System;
    using System.Web.UI;

    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Modules;
    using BusinessLogic.ProductModules;

    using SpendManagementLibrary;

    using Spend_Management;

    /// <summary>
	/// Summary description for about.
	/// </summary>
	public partial class about : Page
	{
	    /// <summary>
	    /// An instance of <see cref="IDataFactory{TComplexType,TPrimaryKeyDataType}"/> to get a <see cref="IProductModule"/>
	    /// </summary>
	    [Dependency]
	    public IDataFactory<IProductModule, Modules> ProductModuleFactory { get; set; }

        protected void Page_Load(object sender, System.EventArgs e)
		{
			Title = "About";
            Master.title = Title;
			
			if (IsPostBack == false)
			{
               
				System.Diagnostics.FileVersionInfo ver = System.Diagnostics.FileVersionInfo.GetVersionInfo(Server.MapPath("../bin/expenses.dll"));
				string strversion;
                CurrentUser user = cMisc.GetCurrentUser();
			    this.ViewState["accountid"] = user.AccountID;
			    this.ViewState["employeeid"] = user.EmployeeID;
                cAccounts clsaccounts = new cAccounts();
                cAccount reqaccount = clsaccounts.GetAccountByID(user.AccountID);

			    var module = this.ProductModuleFactory[user.CurrentActiveModule];

			    this.lblname.Text = (module != null) ? module.BrandName : "Expenses";


			    this.lblexpiry.Text = reqaccount.expiry.ToShortDateString();
			    this.lblusers.Text = reqaccount.numusers.ToString();
				strversion = ver.FileVersion.ToString();
				
				lblversion.Text = strversion;
				
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
