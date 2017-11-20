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

using SpendManagementLibrary;
using Spend_Management;

namespace expenses.admin
{
	/// <summary>
	/// Summary description for aefaq.
	/// </summary>
	public partial class aefaq : Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Title = "Add / Edit FAQ";
            Master.title = Title;
			Master.showdummymenu = true;
            Master.helpid = 1028;

			

			if (IsPostBack == false)
			{
                Master.enablenavigation = false;
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.FAQS, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

				cFaqCategories clsfaqcats = new cFaqCategories(user.AccountID);
				int action = 0;
				clsfaqcats.CreateDropDown(ref cmbcategory, false);
				if (Request.QueryString["action"] != null)
				{
					action = int.Parse(Request.QueryString["action"]);
					ViewState["action"] = action;
				}

				if (action == 2) //update
				{
					int faqid = int.Parse(Request.QueryString["faqid"]);
					ViewState["faqid"] = faqid;
                    cFaqs clsfaqs = new cFaqs(user.AccountID);
					cFaq clsfaq = clsfaqs.getFaqById(faqid);
					txtquestion.Text = clsfaq.question;
					txtanswer.Text = clsfaq.answer;
					txttip.Text = clsfaq.tip;
					if (cmbcategory.Items.FindByValue(clsfaq.faqcategoryid.ToString()) != null)
					{
						cmbcategory.Items.FindByValue(clsfaq.faqcategoryid.ToString()).Selected = true;
					}
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
			this.cmdcancel.Click += new System.Web.UI.ImageClickEventHandler(this.cmdcancel_Click);

		}
		#endregion

		private void cmdcancel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			Response.Redirect("adminfaqs.aspx",true);
		}

		private void cmdok_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			string question, answer, tip;
			int action = 0;
			int faqcategoryid;
			cFaqs clsfaqs = new cFaqs((int)ViewState["accountid"]);

			question = txtquestion.Text;


            if(question.Length > 3999) 
            {
                question = question.Substring(0, 3999);
            }

            answer = txtanswer.Text;
            if (answer.Length > 3999)
            {
                answer = answer.Substring(0, 3999);
            }


			tip = txttip.Text;

			faqcategoryid = int.Parse(cmbcategory.SelectedValue);
			if (ViewState["action"] != null)
			{
				action = (int)ViewState["action"];
			}

            int returnvalue;
			if (action == 2) //update
			{
				returnvalue = clsfaqs.updateFaq((int)ViewState["faqid"],question,answer,tip,faqcategoryid);
			}
			else
			{
				returnvalue = clsfaqs.addFaq(question,answer,tip, faqcategoryid);
			}
            if (returnvalue == -1)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "alert", "alert('The question you have entered already exists')", true);
            }
            else
            {
                Response.Redirect("adminfaqs.aspx", true);
            }

		}
	}
}
