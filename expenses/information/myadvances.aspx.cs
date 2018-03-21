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

namespace expenses
{
	/// <summary>
	/// Summary description for myadvances.
	/// </summary>
	public partial class myadvances : Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
		    //Stops user accessing directly via url if no access
            if (!(cMisc.GetCurrentUser().Employee.AdvancesSignOffGroup != 0 && cMisc.GetCurrentUser().Account.AdvancesEnabled))
		    {
		        Response.Redirect("http://" + Request.Url.Host + "/restricted.aspx");
            }

			Title = "My Advances";
            Master.title = Title;
            Master.helpid = 1162;
			if (IsPostBack == false)
			{
			    int i;
                CurrentUser user = cMisc.GetCurrentUser();
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                var clsemployees = new cEmployees(user.AccountID);
				int accountid = user.AccountID;
				ViewState["accountid"] = accountid;
				var clsfloats = new cFloats(accountid);
				var clscurrencies = new cCurrencies(accountid, user.CurrentSubAccountId);

                var clsgroups = new cGroups(user.AccountID);
				cGroup reqgroup = clsgroups.GetGroupById(user.Employee.AdvancesSignOffGroup);

				if (Request.QueryString["action"] != null)
				{
				    int action = int.Parse(Request.QueryString["action"]);
				    if (action == 3)
					{
						clsfloats.deleteFloat(int.Parse(Request.QueryString["advanceid"]));
					}
				}

			    var clsgrid = new cGrid(clsfloats.getGrid(false, (int)ViewState["employeeid"], false), true, false);
				var newcol = new cGridColumn("Delete","<img title=\"Delete\" src=\"../icons/delete2_blue.gif\">","S","",false, true);
				clsgrid.gridcolumns.Insert(0,newcol);
				newcol = new cGridColumn("Edit","<img title=\"Edit\" src=\"../icons/edit_blue.gif\">","S","", false, true);
				clsgrid.gridcolumns.Insert(0,newcol);
				newcol = new cGridColumn("Dispute","<img title=\"Dispute\" src=\"../icons/redo_blue.gif\">","S","",false,true);
				clsgrid.gridcolumns.Insert(0,newcol);
				clsgrid.tblclass = "datatbl";
				clsgrid.getColumn("floatid").hidden = true;
				clsgrid.getColumn("name").description = "Name";
				clsgrid.getColumn("reason").description = "Reason";
				clsgrid.getColumn("floatamount").description = "Amount";
				clsgrid.getColumn("floatamount").fieldtype = "C";
				clsgrid.getColumn("originalcurrency").description = "Requested Currency";
				clsgrid.getColumn("originalcurrency").listitems = clscurrencies.CreateColumnList();
				clsgrid.getColumn("requiredby").description = "Required By";
				clsgrid.getColumn("requiredby").fieldtype = "D";
				clsgrid.getColumn("approved").description = "Approved";
				clsgrid.getColumn("approver").description = "Approver";
				clsgrid.getColumn("approver").listitems = clsemployees.CreateColumnList(user.AccountID);
				clsgrid.getColumn("floatused").description = "Amount<br>Used";
				clsgrid.getColumn("floatused").fieldtype = "C";
				clsgrid.getColumn("floatavailable").description = "Amount<br>Available";
				clsgrid.getColumn("floatavailable").fieldtype = "C";
				clsgrid.getColumn("employeeid").hidden = true;
				clsgrid.getColumn("rejected").description = "Rejected";
				clsgrid.getColumn("rejectreason").description = "Reason for Rejection";
				clsgrid.getColumn("rejected").description = "Rejected";
				clsgrid.getColumn("disputed").description = "Corrected / Disputed";
				clsgrid.getColumn("dispute").hidden = true;
				clsgrid.getColumn("paid").description = "Paid";
				clsgrid.getColumn("stage").description = "Stage";
				clsgrid.getColumn("issuenum").description = "Issue Num";
				clsgrid.getColumn("exchangerate").description = "Exchange Rate";
                clsgrid.getColumn("basecurrency").hidden = true;
				clsgrid.getColumn("Total Prior To Convert").description = "Foreign Amount";
                clsgrid.getColumn("settled").description = "Settled";
			    clsgrid.emptytext = "There are currently no advances to display.";
                clsgrid.getData();
				
				for (i = 0; i < clsgrid.gridrows.Count; i++)
				{
					var reqrow = (cGridRow)clsgrid.gridrows[i];
					var approved = (bool)reqrow.getCellByName("approved").thevalue;
					var rejected = (bool)reqrow.getCellByName("rejected").thevalue;
					var disputed = (bool)reqrow.getCellByName("disputed").thevalue;
					var paid = (bool)reqrow.getCellByName("paid").thevalue;

					if ((DateTime)reqrow.getCellByName("requiredby").thevalue == new DateTime(1900,01,01))
					{
						reqrow.getCellByName("requiredby").thevalue = "";
					}
					reqrow.getCellByName("approved").thevalue = "<input type=checkbox disabled";
					if (approved)
					{
						reqrow.getCellByName("approved").thevalue += " checked";
					}
					reqrow.getCellByName("approved").thevalue += ">";
					reqrow.getCellByName("rejected").thevalue = "<input type=checkbox disabled";
					if (rejected)
					{
						reqrow.getCellByName("rejected").thevalue += " checked";
					}
					reqrow.getCellByName("rejected").thevalue += ">";
					reqrow.getCellByName("disputed").thevalue = "<input type=checkbox disabled";
					if (disputed)
					{
						reqrow.getCellByName("disputed").thevalue += " checked";
					}
					reqrow.getCellByName("disputed").thevalue += ">";
					reqrow.getCellByName("paid").thevalue = "<input type=checkbox disabled";
					if (paid)
					{
						reqrow.getCellByName("paid").thevalue += " checked";
					}
					reqrow.getCellByName("paid").thevalue += ">";

					
					if (reqgroup != null)
					{
						reqrow.getCellByName("stage").thevalue += " of " + reqgroup.stagecount;
					}

					if (rejected && disputed == false)
					{
						reqrow.getCellByName("Edit").thevalue = "<a href=\"aeadvancereq.aspx?action=2&floatid=" + reqrow.getCellByName("floatid").thevalue + "\"><img title=\"Edit\" src=\"../icons/edit.gif\"></a>";
						reqrow.getCellByName("Delete").thevalue = "<a href=\"javascript:deleteAdvance(" + reqrow.getCellByName("floatid").thevalue + ");\"><img title=\"Delete\" src=\"../icons/delete2.gif\"></a>";
						reqrow.getCellByName("Dispute").thevalue = "<a href=\"disputeadvance.aspx?floatid=" + reqrow.getCellByName("floatid").thevalue + "\"><img title=\"Dispute\" src=\"../icons/redo.gif\"></a>";
					}
					else
					{
						reqrow.getCellByName("Edit").thevalue = "";
						reqrow.getCellByName("Delete").thevalue = "";
						reqrow.getCellByName("Dispute").thevalue = "";
					}
					if ((int)reqrow.getCellByName("issuenum").thevalue == 0)
					{
						reqrow.getCellByName("issuenum").thevalue = "";
					}
					if ((double)reqrow.getCellByName("exchangerate").thevalue == 0)
					{
						reqrow.getCellByName("exchangerate").thevalue = "";
					}

                    if ((bool)reqrow.getCellByName("settled").thevalue == false)
                    {
                        reqrow.getCellByName("settled").thevalue = "<input type=\"checkbox\" disabled=\"disabled\" />";
                    }
                    else
                    {
                        reqrow.getCellByName("settled").thevalue = "<input type=\"checkbox\" disabled=\"disabled\" checked=\"checked\" />";
                    }
				}

                litgrid.Text = clsgrid.CreateGrid();      
                
                clsfloats.AuditViewAdvances($"Advances for {user.Employee.FullNameUsername}", user);
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



        /// <summary>
        /// Close button event function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdClose_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string sPreviousURL = (SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url;

            Response.Redirect(sPreviousURL, true);
        }
	}
}
