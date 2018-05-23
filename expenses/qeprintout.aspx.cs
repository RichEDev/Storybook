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
using Spend_Management;

namespace expenses
{
    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Modules;
    using BusinessLogic.ProductModules;

    using SpendManagementLibrary;

	/// <summary>
	/// Summary description for qeprintout.
	/// </summary>
	public partial class qeprintout : System.Web.UI.Page
	{
	    /// <summary>
	    /// An instance of <see cref="IDataFactory{TComplexType,TPrimaryKeyDataType}"/> to get a <see cref="IProductModule"/>
	    /// </summary>
	    [Dependency]
	    public IDataFactory<IProductModule, Modules> ProductModuleFactory { get; set; }

        protected void Page_Load(object sender, System.EventArgs e)
		{
			if (IsPostBack == false)
			{
			    CurrentUser user = cMisc.GetCurrentUser();
                var mpm = new cMasterPageMethods(user, HostManager.GetTheme(HttpContext.Current.Request.Url.Host), this.ProductModuleFactory);
                this.litJavascript.Text = user.CurrentUserInfoJavascriptVariable;
                mpm.SetupJQueryReferences(ref jQueryCss,ref scriptman);
                mpm.SetupSessionTimeoutReferences(ref scriptman, this.Page);

				System.Data.DataSet ds;
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;
                ViewState["subAccountID"] = user.CurrentSubAccountId;

				cClaims clsclaims = new cClaims((int)ViewState["accountid"]);
				int quickentryid = int.Parse(Request.QueryString["quickentryid"]);
				ViewState["quickentryid"] = quickentryid;

                cQeForms clsforms = new cQeForms(user.AccountID);
				cQeForm reqform = clsforms.getFormById(quickentryid);
                ds = clsforms.generatePrintOutFields(quickentryid, user.EmployeeID);
				System.Text.StringBuilder output = new System.Text.StringBuilder();

				output.Append(generateHeader(reqform));
				output.Append(generateBody(reqform));
				output.Append("</table>");
				litform.Text = output.ToString();
                
				generateLabels(reqform,ds);
			}
		}

		private void generateLabels(cQeForm reqform, System.Data.DataSet ds)
		{
			string toadd;
			int i;
			cQePrintoutField curfield;
			for (i = 0; i < reqform.printout.Length; i++)
			{
				curfield = (cQePrintoutField)reqform.printout[i];
				if (curfield.field == null)
				{
					toadd = "<tr><td colspan=2>" + curfield.freetext + "</td></tr>";
				}
				else
				{
					toadd = "<tr>" +
						"<td class=labeltd>" + curfield.field.Description + "</td>" +
						"<td class=inputtd>" + ds.Tables[0].Rows[0][curfield.field.Description] + "</td>" +
                        //"<td class=inputtd>SOMETHING</td>" +
						"</tr>";
				}
				switch (curfield.pos)
				{
					case Position.Top_Left:
						littopleft.Text += toadd;
						break;
					case Position.Top_Centre:
						littopcenter.Text += toadd;
						break;
					case Position.Top_Right:
						littopright.Text += toadd;
						break;
					case Position.Bottom_Left:
						litbottomleft.Text += toadd;
						break;
					case Position.Bottom_Centre:
						litbottomcenter.Text += toadd;
						break;
					case Position.Bottom_Right:
						litbottomright.Text += toadd;
						break;
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

		private string generateHeader(cQeForm reqform)
		{
			cQeFieldColumn fieldcol;
			cQeSubcatColumn subcatcol;
			int i;
			System.Text.StringBuilder output = new System.Text.StringBuilder();
			output.Append("<table class=datatbl>");
			output.Append("<tr>");
			for (i = 0; i < reqform.columns.Length; i++)
			{
				output.Append("<th>");
				if (reqform.columns[i].GetType().ToString() == "expenses.cQeFieldColumn")
				{
					fieldcol = (cQeFieldColumn)reqform.columns[i];
					output.Append(fieldcol.field.Description);
				}
				else
				{
					subcatcol = (cQeSubcatColumn)reqform.columns[i];
					output.Append(subcatcol.subcat.subcat);
				}			
				output.Append("</th>");
			}
			output.Append("</tr>");
			
			return output.ToString();
		}

		
		private string generateBody(cQeForm reqform)
		{
			cQeFieldColumn fieldcol;
			cQeSubcatColumn subcatcol;
			string rowclass = "row1";
			System.Text.StringBuilder output = new System.Text.StringBuilder();
			int i, x;
			int numrows;
			int month = 0;
			DateTime date;
			if (reqform.genmonth == true)
			{
				
				if (Request.QueryString["month"] != null)
				{
					month = int.Parse(Request.QueryString["month"]);
				}
				else
				{
					month = DateTime.Today.Month;
				}
				numrows = getDaysInMonth(month);
			}
			else
			{
				numrows = reqform.numrows;
			}

			for (i = 0; i < numrows; i++)
			{
				output.Append("<tr>");
				for (x = 0; x < reqform.columns.Length; x++)
				{
					output.Append("<td class=\"" + rowclass + "\"");
					if (reqform.columns[x].GetType().ToString() == "expenses.cQeFieldColumn")
					{
						fieldcol = (cQeFieldColumn)reqform.columns[x];
						switch (fieldcol.field.FieldID.ToString())
						{
							case "a52b4423-c766-47bb-8bf3-489400946b4c":
								output.Append(" style=\"width:50px;\">");
								if (reqform.genmonth == true)
								{
									date = new DateTime(DateTime.Today.Year,month,(i+1));
									output.Append(date.ToShortDateString());
								}
								
								break;
							case "ec527561-dfee-48c7-a126-0910f8e031b0": //dd
								output.Append(" style=\"width:125px;\">");
								break;
							case "1ee53ae2-2cdf-41b4-9081-1789adf03459": //dd
								output.Append(" style=\"width:125px;\">");
								break;
							case "3d8c699e-9e0e-4484-b821-b49b5cb4c098":
								output.Append(" style=\"width:125px;\">");
								break;
							case "7cf61909-8d25-4230-84a9-f5701268f94b":
								output.Append(" style=\"width:200px;\">");
								break;
							case "af839fe7-8a52-4bd1-962c-8a87f22d4a10": //dd
								output.Append(" style=\"width:125px;\">");
								break;
						}
					}
					else
					{
						subcatcol = (cQeSubcatColumn)reqform.columns[x];
						output.Append("<input type=text style=\"width:50px;\" id=\"" + i + "subcat" + subcatcol.subcat.subcatid + "\">");
					}			
					output.Append("</td>");
				}
				output.Append("</tr>");
				if (rowclass == "row1")
				{
					rowclass = "row2";
				}
				else
				{
					rowclass = "row1";
				}
			}
			return output.ToString();
		}

		private int getDaysInMonth(int month)
		{
			DateTime date = new DateTime(DateTime.Today.Year,month,01);
			date = date.AddMonths(1);
			date = date.AddDays(-1);
			return date.Day;
		}
	}
}
