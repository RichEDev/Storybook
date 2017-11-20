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
using System.IO;

using expenses.Old_App_Code;
using SpendManagementLibrary;
using Spend_Management;

namespace expenses.admin
{
	/// <summary>
	/// Summary description for companylogo.
	/// </summary>
	public partial class companylogo : Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Title = "Company Logo";
            Master.title = Title;
            Master.helpid = 1039;
			if (IsPostBack == false)
			{
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CompanyLogo, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                if (File.Exists(Server.MapPath("../logos/" + user.AccountID + ".jpg")) == true)
				{
                    litlogo.Text = "<img border=0 src=\"../logos/" + user.AccountID + ".jpg\">";
				}
                else if (File.Exists(Server.MapPath("../logos/" + user.AccountID + ".gif")) == true)
				{
                    litlogo.Text = "<img border=0 src=\"../logos/" + user.AccountID + ".gif\">";
				}
				else
				{
					litlogo.Text = "You do not currently have a company logo displayed.";
				}
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
			lblmsg.Visible = false;
			if (filelogo.Value == "")
			{
				lblmsg.Text = "Please select a logo to upload";
				lblmsg.Visible = true;
				return;
			}

			string ext;
			string filename = filelogo.PostedFile.FileName;
			if (filename.Substring(filename.Length-3,3).ToLower() != "jpg" && filename.Substring(filename.Length-3,3).ToLower() != "gif")
			{
				lblmsg.Text = "The file you have selected is not a valid file format. To upload your logo it must be a JPG or GIF file.";
				lblmsg.Visible = true;
				return;
			}
			else
			{
				ext = filename.Substring(filename.Length-3,3).ToLower();
			}


            cAccounts clsAccounts = new cAccounts();
            Databases clsDatabases = new Databases();
            cDatabase reqDatabase = clsDatabases.GetDatabaseByID(clsAccounts.GetAccountByID((int)ViewState["accountid"]).dbserverid);

            string uploadpath = System.IO.Path.Combine(reqDatabase.LogoPath, ViewState["accountid"] + "." + ext);
            if (!System.IO.Directory.Exists(reqDatabase.LogoPath))
            {
                lblmsg.Text = "The upload path specified in the configuration is invalid. Please contact Selenity for this to be resolved.";
                lblmsg.Visible = true;
                return;
            }
            else
            {
                filelogo.PostedFile.SaveAs(uploadpath);
            }
			litlogo.Text = "<img border=0 src=\"../logos/" + ViewState["accountid"] + "." + ext + "\">";
			
		}

		protected void cmdremove_Click(object sender, System.EventArgs e)
		{
			File.Delete(Server.MapPath("../logos/" + ViewState["accountid"] + ".jpg"));
			File.Delete(Server.MapPath("../logos/" + ViewState["accountid"] + ".gif"));
			litlogo.Text = "You do not currently have a company logo displayed";
		}

        protected void cmdCancel_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/tailoringmenu.aspx", true);
        }
	}
}
