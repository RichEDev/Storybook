namespace Spend_Management
{
    using System;
    using System.Web.UI;

    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Modules;
    using BusinessLogic.ProductModules;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Enumerators;

    /// <summary>
	/// Summary description for changeofdetails.
	/// </summary>
	public partial class changeofdetails : Page
    {
        /// <summary>
        /// An instance of <see cref="IDataFactory{TComplexType,TPrimaryKeyDataType}"/> to get a <see cref="IProductModule"/>
        /// </summary>
        [Dependency]
        public IDataFactory<IProductModule, Modules> ProductModuleFactory { get; set; }

        protected void Page_Load(object sender, System.EventArgs e)
		{
            CurrentUser user = cMisc.GetCurrentUser();
            if (user.isDelegate)
            {
                Response.Redirect("~/restricted.aspx", true);
            }
            Title = "Change of Details";
            Master.PageSubTitle = Title;
            Master.title = "My Details";
            Master.enablenavigation = false;

			if (IsPostBack == false)
			{
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                cmdok.Attributes.Add("onclick", "javascript:if(validateform('vgMain') == false) { return false;}");
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
            var employees = new cEmployees((int)ViewState["accountid"]);
            var currentUser = cMisc.GetCurrentUser();
            var subAccounts = new cAccountSubAccounts(currentUser.AccountID);
            cAccountProperties reqProperties = subAccounts.getFirstSubAccount().SubAccountProperties;
            var emailSender = new EmailSender(reqProperties.EmailServerAddress);

            employees.NotifyAdminOfChanges(currentUser, this.txtchanges.Text, this.ProductModuleFactory, reqProperties, emailSender);

		    Response.Redirect("mydetails.aspx", true);		        
		}
	}
}
