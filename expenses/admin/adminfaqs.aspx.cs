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
	/// Summary description for adminfaqs.
	/// </summary>
	public partial class adminfaqs : Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.Expires = 60;
			Response.ExpiresAbsolute = DateTime.Now.AddMinutes(-1);
			Response.AddHeader ("pragma","no-cache");
			Response.AddHeader ("cache-control","private");
			Response.CacheControl = "no-cache";

			
			Title = "FAQs";
            Master.title = Title;
            Master.helpid = 1097;
			if (IsPostBack == false)
			{
				int action = 0;
				cGridColumn newcol;
				cGridRow reqrow;
				int i;
				System.Data.DataSet rcdstcats = new DataSet();
				System.Data.DataSet rcdstfaqs = new DataSet();
				cGrid clsgridcats;
				cGrid clsgridfaqs;
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.FAQS, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                cFaqs clsfaqs = new cFaqs(user.AccountID);
                cFaqCategories clsfaqcats = new cFaqCategories(user.AccountID);

				if (Request.QueryString["action"] != null)
				{
					action = int.Parse(Request.QueryString["action"]);
					if (action == 3) //delete faq
					{
						clsfaqs.deleteFaq(int.Parse(Request.Form["faqid"]));
					}
					else if (action == 4) //delete faq category
					{
						clsfaqcats.deleteCategory(int.Parse(Request.Form["faqcategoryid"]));
					}
				}
				rcdstfaqs.Tables.Add(clsfaqs.getGrid(false));
				
				clsgridfaqs = new cGrid(user.AccountID, rcdstfaqs,true,false,Grid.FAQS);
				clsgridfaqs.tbodyid = "faqs";
				newcol = new cGridColumn("Delete","<img alt=\"Delete\" src=\"../icons/delete2_blue.gif\">","S","",false, true);
				clsgridfaqs.gridcolumns.Insert(0,newcol);
				newcol = new cGridColumn("Edit","<img alt=\"Edit\" src=\"../icons/edit_blue.gif\">","S","", false, true);
				clsgridfaqs.gridcolumns.Insert(0,newcol);
				clsgridfaqs.getColumn("question").description = "Question";
				clsgridfaqs.getColumn("datecreated").description = "Date Created";
				clsgridfaqs.getColumn("faqid").hidden = true;
				clsgridfaqs.tblclass = "datatbl";
				clsgridfaqs.tableid = "faqs";
				clsgridfaqs.idcolumn = clsgridfaqs.getColumn("faqid");
				clsgridfaqs.getData();
				for (i = 0; i < clsgridfaqs.gridrows.Count; i++)
				{
					reqrow = (cGridRow)clsgridfaqs.gridrows[i];
					reqrow.getCellByName("Edit").thevalue = "<a href=\"aefaq.aspx?action=2&faqid=" + reqrow.getCellByName("faqid").thevalue + "\"><img alt=\"Edit\" src=\"../icons/edit.gif\"></a>";
					reqrow.getCellByName("Delete").thevalue = "<a href=\"javascript:deleteFAQ(" + reqrow.getCellByName("faqid").thevalue + ");\"><img alt=\"Delete\" src=\"../icons/delete2.gif\"></a>";
				}
				litfaqs.Text = clsgridfaqs.CreateGrid();


				
				rcdstcats.Tables.Add(clsfaqcats.getGrid(false));
				clsgridcats = new cGrid(user.AccountID, rcdstcats,true,false,Grid.FAQCategories);
				clsgridcats.tbodyid = "faqcats";
				newcol = new cGridColumn("Delete","<img alt=\"Delete\" src=\"../icons/delete2_blue.gif\">","S","",false, true);
				clsgridcats.gridcolumns.Insert(0,newcol);
				newcol = new cGridColumn("Edit","<img alt=\"Edit\" src=\"../icons/edit_blue.gif\">","S","", false, true);
				clsgridcats.gridcolumns.Insert(0,newcol);
				clsgridcats.getColumn("faqcategoryid").hidden = true;
				clsgridcats.getColumn("category").description = "Category";
				clsgridcats.tblclass = "datatbl";
				clsgridcats.tableid = "faqcategories";
				clsgridcats.idcolumn = clsgridcats.getColumn("faqcategoryid");
				clsgridcats.getData();
				for (i = 0; i < clsgridcats.gridrows.Count; i++)
				{
					reqrow = (cGridRow)clsgridcats.gridrows[i];
					reqrow.getCellByName("Edit").thevalue = "<a href=\"aefaqcategory.aspx?action=2&faqcategoryid=" + reqrow.getCellByName("faqcategoryid").thevalue + "\"><img alt=\"Edit\" src=\"../icons/edit.gif\"></a>";
					reqrow.getCellByName("Delete").thevalue = "<a href=\"javascript:deleteFAQCategory(" + reqrow.getCellByName("faqcategoryid").thevalue + ");\"><img alt=\"Delete\" src=\"../icons/delete2.gif\"></a>";
				}
				litcategories.Text = clsgridcats.CreateGrid();
				
				
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

		protected void cmdaddfaq_Click(object sender, System.EventArgs e)
		{
			cFaqCategories clsfaqcats = new cFaqCategories((int)ViewState["accountid"]);

			if (clsfaqcats.itemCount == 0)
			{
				lblmsg.Text = "Please add a FAQ Category before adding a FAQ";
				lblmsg.Visible = true;
				return;
			}
			else
			{
				Response.Redirect("aefaq.aspx",true);
			}
		}












		
	}
}
