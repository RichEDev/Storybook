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
using expenses.admin;
using Spend_Management;


namespace expenses.information
{
	/// <summary>
	/// Summary description for faqs.
	/// </summary>
	public partial class faqs : Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
            Title = "Frequently Asked Questions";
            Master.title = Title;
			if (IsPostBack == false)
			{
                CurrentUser user = cMisc.GetCurrentUser();
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;
                cFaqCategories clsFaqCats = new cFaqCategories(user.AccountID);
                cFaqs clsFaqs = new cFaqs(user.AccountID);
                System.Collections.Generic.SortedList<string, cFaqCategory> lstFaqCats = clsFaqCats.getList();
                System.Collections.Generic.SortedList<string, cFaq> lstFaqs = clsFaqs.getList();

                cFaqCategory tmpCat;
                cFaq tmpFaq;

                System.Text.StringBuilder sbFaq = new System.Text.StringBuilder();

                AjaxControlToolkit.AccordionPane newPane;
                Literal litFaqContents;
                Literal litFaqCate;
                for (int j = 0; j < lstFaqCats.Count; j++)
                {
                    tmpCat = (cFaqCategory)lstFaqCats.Values[j];
                    newPane = new AjaxControlToolkit.AccordionPane();
                    newPane.ID = "faqPane" + j.ToString();
                    litFaqCate = new Literal();
                    litFaqCate.ID = tmpCat.faqcategoryid.ToString();
                    litFaqCate.Text = tmpCat.category;
                    newPane.HeaderContainer.Controls.Add(litFaqCate);

                    for (int i = 0; i < lstFaqs.Count; i++)
                    {
                        tmpFaq = (cFaq)lstFaqs.Values[i];
                        if (tmpFaq.CategoryName == tmpCat.category)
                        {
                            sbFaq = new System.Text.StringBuilder();
                            sbFaq.Append("<div class=\"faqQuestion\">"+tmpFaq.question+"</div>");
                            sbFaq.Append("<div class=\"faqAnswer\">"+tmpFaq.answer+"</div>");
                            if (tmpFaq.tip != "")
                            {
                                sbFaq.Append("<div class=\"faqTip\">" + tmpFaq.tip + "</div>");
                            }

                            litFaqContents = new Literal();
                            litFaqContents.Text = sbFaq.ToString();
                            litFaqContents.ID = "litFaqContents" + i.ToString();

                            newPane.ContentContainer.Controls.Add(litFaqContents);
                        }

                    }

                    if (newPane.ContentContainer.Controls.Count > 0)
                    {
                        accordionFaqs.Controls.Add(newPane);
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

		}
		#endregion
	}
}
