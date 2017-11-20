//===========================================================================
// This file was modified as part of an ASP.NET 2.0 Web project conversion.
// The class name was changed and the class modified to inherit from the abstract base class 
// in file 'App_Code\Migrated\Stub_policy_aspx_cs.cs'.
// During runtime, this allows other classes in your web application to bind and access 
// the code-behind page using the abstract base class.
// The associated content page 'policy.aspx' was also modified to refer to the new class name.
// For more information on this code pattern, please refer to http://go.microsoft.com/fwlink/?LinkId=46995 
//===========================================================================
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using expenses.Old_App_Code;
using SpendManagementLibrary;
using Spend_Management;

namespace expenses
{
	/// <summary>
	/// Summary description for policy.
	/// </summary>
	public partial class policy : Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
            CurrentUser user = cMisc.GetCurrentUser();
            this.ViewState["accountid"] = user.AccountID;
            this.ViewState["employeeid"] = user.EmployeeID;

            var clsmisc = new cMisc(user.AccountID);
            var clsproperties = clsmisc.GetGlobalProperties(user.AccountID);
            cAccounts clsaccounts = new cAccounts();
            cAccount reqaccount = clsaccounts.GetAccountByID(user.AccountID);

            switch (clsproperties.policytype)
			{
				case 1:
                    this.litpolicy.Text = clsproperties.CompanyPolicy;
					break;
				case 2:
                    
					
					if (System.IO.File.Exists(this.Server.MapPath("policies/" + reqaccount.companyid + ".htm")) == true)
					{
						System.IO.FileStream fs = new System.IO.FileStream(this.Server.MapPath("policies/" + reqaccount.companyid + ".htm"),System.IO.FileMode.Open);
						System.IO.StreamReader sr = new System.IO.StreamReader(fs);
                        this.litpolicy.Text = sr.ReadToEnd();
						sr.Close();
						fs.Close();
					}
					else
					{
                        this.litpolicy.Text = @"<center>A policy does not currently exist</center>";
					}
					break;
                case 3:
                    this.Response.ContentType = "application/pdf";
			        this.Response.AddHeader("Content-Type", "application/pdf");
			        if (this.Request.Browser.Type.ToUpper().Contains("IE"))
			        {
			            if (this.Request.Browser.MajorVersion < 11)
			            {
			                this.Response.AppendHeader("content-disposition", "attachment; filename=Company Policy.pdf");
			            }
			        }

			        this.Response.WriteFile(this.Server.MapPath("policies/" + reqaccount.companyid + ".pdf"));
                    this.Response.End();
			        break;
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
