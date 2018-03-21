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
    using SpendManagementLibrary.Employees;

    /// <summary>
	/// Summary description for adminfloats.
	/// </summary>
	public partial class adminfloats : Page
	{
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
            //Stops user accessing directly via url if no access
            if (!(cMisc.GetCurrentUser().CheckAccessRole(AccessRoleType.View, SpendManagementElement.Advances, true) && cMisc.GetCurrentUser().Account.AdvancesEnabled))
		    {
		        Response.Redirect("http://" + Request.Url.Host + "/restricted.aspx");
		    }

            Response.Expires = 60;
			Response.ExpiresAbsolute = DateTime.Now.AddMinutes(-1);
			Response.AddHeader ("pragma","no-cache");
			Response.AddHeader ("cache-control","private");
			Response.CacheControl = "no-cache";

			
			Title = "Advances";
            Master.title = Title;
            Master.helpid = 1055;
			if (IsPostBack == false)
			{
			    CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Advances, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;
                ViewState["subAccountID"] = user.CurrentSubAccountId;

				cFloats clsfloats = new cFloats((int)ViewState["accountid"]);

			    if (Request.QueryString["action"] != null)
				{
					int action = int.Parse(Request.QueryString["action"]);
				    cFloat reqfloat;
				    if (action == 4) //approve advance
					{
						
						reqfloat = clsfloats.GetFloatById(int.Parse(Request.QueryString["floatid"]));
						clsfloats.SendClaimToNextStage(reqfloat, false, user.EmployeeID, reqfloat.employeeid);
					}
					else if (action == 5) //pay advance
					{
						reqfloat = clsfloats.GetFloatById(int.Parse(Request.QueryString["floatid"]));
						clsfloats.payAdvance((int)ViewState["accountid"], reqfloat.floatid);
					}
                    else if (action == 6) //settle advance
                    {
                        reqfloat = clsfloats.GetFloatById(int.Parse(Request.QueryString["floatid"]));
                        clsfloats.settleAdvance(reqfloat.floatid);
                    }
                    else if (action == 7)
                    {
                        reqfloat = clsfloats.GetFloatById(int.Parse(Request.QueryString["floatid"]));
                        clsfloats.returnRemainder(reqfloat.floatid);
                    }
				}
				litgrid.Text = getGrid(false);

				litactive.Text = getGrid(true);

			    clsfloats.AuditViewAdvances("Advances", user);
            }
		}

		private string getGrid(bool active)
		{
			cGridColumn newcol;
		    int i;

		    cFloats clsfloats = new cFloats((int)ViewState["accountid"]);
			cCurrencies clscurrencies = new cCurrencies((int)ViewState["accountid"], (int)ViewState["subAccountID"]);
			cEmployees clsemployees = new cEmployees((int)ViewState["accountid"]);
		    cGroups clsgroups = new cGroups((int)ViewState["accountid"]);
		    cGrid clsgrid = new cGrid(clsfloats.getGrid(true,(int)ViewState["employeeid"],active),true,false);

			clsgrid.getColumn("floatid").hidden = true;
            clsgrid.getColumn("basecurrency").hidden = true;
			clsgrid.getColumn("employeeid").listitems = clsemployees.CreateColumnList((int)ViewState["accountid"]);
			clsgrid.getColumn("employeeid").description = "Employee";
			clsgrid.getColumn("name").description = "Advance Name";
			clsgrid.getColumn("reason").description = "Reason for Advance";
			clsgrid.getColumn("originalcurrency").description = "Currency";
			clsgrid.getColumn("originalcurrency").listitems = clscurrencies.CreateColumnList();
			clsgrid.getColumn("floatamount").description = "Amount Required";
			clsgrid.getColumn("floatamount").fieldtype = "C";
			clsgrid.getColumn("floatused").fieldtype = "C";
			clsgrid.getColumn("floatavailable").fieldtype = "C";
			clsgrid.getColumn("floatused").description = "Amount Used";
			clsgrid.getColumn("floatavailable").description = "Amount Available";
			clsgrid.getColumn("exchangerate").description = "Exchange Rate";
			clsgrid.getColumn("Total Prior To Convert").description = "Foreign Amount";
            clsgrid.getColumn("settled").hidden = true;
			if (active == false)
			{
				clsgrid.getColumn("floatused").hidden = true;
				clsgrid.getColumn("floatavailable").hidden = true;
			}
			else
			{
				clsgrid.getColumn("floatamount").description = "Amount Issued";
				clsgrid.getColumn("requiredby").hidden = true;
				clsgrid.getColumn("stage").hidden = true;
				clsgrid.getColumn("rejected").hidden = true;
				clsgrid.getColumn("rejectreason").hidden = true;
				clsgrid.getColumn("disputed").hidden = true;
				clsgrid.getColumn("dispute").hidden = true;
				clsgrid.getColumn("paid").hidden = true;
			}
			clsgrid.getColumn("requiredby").description = "Required By";
			clsgrid.getColumn("requiredby").fieldtype = "D";
			clsgrid.getColumn("approver").hidden = true;
			clsgrid.getColumn("approved").hidden = true;

			clsgrid.getColumn("rejected").description = "Rejected";
			clsgrid.getColumn("rejectreason").description = "Reason for Rejection";
			clsgrid.getColumn("rejected").description = "Rejected";
			clsgrid.getColumn("disputed").description = "Corrected / Disputed";
			clsgrid.getColumn("dispute").hidden = true;
			clsgrid.getColumn("paid").description = "Paid";

			clsgrid.getColumn("stage").description = "Stage";
			clsgrid.getColumn("issuenum").description = "Issue Num";

			if (active == false)
			{
				newcol = new cGridColumn("reject","Reject","S","",false, true);
				clsgrid.gridcolumns.Insert(0,newcol);
				newcol = new cGridColumn("approve","Approve","S","",false, true);
				clsgrid.gridcolumns.Insert(0,newcol);
				newcol = new cGridColumn("changeamount","Change Amount","S","",false, true);
				clsgrid.gridcolumns.Insert(0,newcol);
			}
			else
			{
                newcol = new cGridColumn("settleadvance", "Settle Advance", "S", "", false, true);
                clsgrid.gridcolumns.Insert(0, newcol);
				newcol = new cGridColumn("topup","Top-Up","S","",false,true);
				clsgrid.gridcolumns.Insert(0,newcol);
                newcol = new cGridColumn("returnRemainder", "Return Remainder", "S", "", false, true);
                clsgrid.gridcolumns.Insert(0, newcol);
			}

			clsgrid.tblclass = "datatbl";
			clsgrid.getData();

			for (i = 0; i < clsgrid.gridrows.Count; i++)
			{
				cGridRow reqrow = (cGridRow)clsgrid.gridrows[i];

				bool approved = (bool)reqrow.getCellByName("approved").thevalue;
				bool rejected = (bool)reqrow.getCellByName("rejected").thevalue;
				bool disputed = (bool)reqrow.getCellByName("disputed").thevalue;
				bool paid = (bool)reqrow.getCellByName("paid").thevalue;

				Employee reqemp = clsemployees.GetEmployeeById((int)reqrow.getCellByName("employeeid").thevalue);
				cGroup reqgroup = clsgroups.GetGroupById(reqemp.AdvancesSignOffGroup);
				if (active == false)
				{
					if ((bool)reqrow.getCellByName("paid").thevalue == false && (bool)reqrow.getCellByName("approved").thevalue == true && (byte)reqrow.getCellByName("stage").thevalue == reqgroup.stagecount)
					{
						reqrow.getCellByName("approve").thevalue = "<a href=\"adminfloats.aspx?action=5&floatid=" + reqrow.getCellByName("floatid").thevalue + "\">Pay Advance</a>";
					}
					else
					{
						reqrow.getCellByName("approve").thevalue = "<a href=\"adminfloats.aspx?action=4&floatid=" + reqrow.getCellByName("floatid").thevalue + "\">Approve</a>";
					}
					reqrow.getCellByName("reject").thevalue = "<a href=\"rejectadvance.aspx?floatid=" + reqrow.getCellByName("floatid").thevalue + "\">Reject</a>";
					reqrow.getCellByName("changeamount").thevalue = "<a href=\"changeadvance.aspx?floatid=" + reqrow.getCellByName("floatid").thevalue + "\">Change Amount</a>";
                    
				}
				else
				{
					reqrow.getCellByName("topup").thevalue = "<a href=\"topupadvance.aspx?floatid=" + reqrow.getCellByName("floatid").thevalue + "\">Top-Up</a>";
                    reqrow.getCellByName("settleadvance").thevalue = "<a href=\"adminfloats.aspx?action=6&floatid=" + reqrow.getCellByName("floatid").thevalue + "\">Settle Advance</a>";
                    if ((bool)reqrow.getCellByName("approved").thevalue == true)
                    {
                        reqrow.getCellByName("returnRemainder").thevalue = "<a href=\"adminfloats.aspx?action=7&floatid=" + reqrow.getCellByName("floatid").thevalue + "\">Return Remainder</a>";
                    }
                    else
                    {
                        reqrow.getCellByName("returnRemainder").thevalue = "";
                    }

                }
				if ((DateTime)reqrow.getCellByName("requiredby").thevalue == new DateTime(1900,01,01))
				{
					reqrow.getCellByName("requiredby").thevalue = "";
				}
				reqrow.getCellByName("approved").thevalue = "<input type=checkbox disabled";
				if (approved == true)
				{
					reqrow.getCellByName("approved").thevalue += " checked";
				}
				reqrow.getCellByName("approved").thevalue += ">";
				reqrow.getCellByName("rejected").thevalue = "<input type=checkbox disabled";
				if (rejected == true)
				{
					reqrow.getCellByName("rejected").thevalue += " checked";
				}
				reqrow.getCellByName("rejected").thevalue += ">";
				reqrow.getCellByName("disputed").thevalue = "<input type=checkbox disabled";
				if (disputed == true)
				{
					reqrow.getCellByName("disputed").thevalue += " checked";
				}
				reqrow.getCellByName("disputed").thevalue += ">";
				reqrow.getCellByName("paid").thevalue = "<input type=checkbox disabled";
				if (paid == true)
				{
					reqrow.getCellByName("paid").thevalue += " checked";
				}
				reqrow.getCellByName("paid").thevalue += ">";

				reqrow.getCellByName("stage").thevalue += " of " + reqgroup.stagecount;
				if ((int)reqrow.getCellByName("issuenum").thevalue == 0)
				{
					reqrow.getCellByName("issuenum").thevalue = "";
				}
				if ((double)reqrow.getCellByName("exchangerate").thevalue == 0)
				{
					reqrow.getCellByName("exchangerate").thevalue = "";
				}

			}

			if (clsgrid.gridrows.Count == 0)
			{
				return "There are currently no advances in this section to show";
			}
			else
			{
				return clsgrid.CreateGrid();
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
