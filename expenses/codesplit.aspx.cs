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
using BusinessLogic;
using BusinessLogic.DataConnections;
using BusinessLogic.ProjectCodes;
using SpendManagementLibrary;
using Spend_Management;

namespace expenses
{
	/// <summary>
	/// Summary description for codesplit.
	/// </summary>
	public partial class codesplit : System.Web.UI.Page
	{
	    [Dependency]
	    public IDataFactoryCustom<IProjectCodeWithUserDefinedFields, int> ProjectCodesRepository { get; set; }

        protected void Page_Load(object sender, System.EventArgs e)
		{
			if (IsPostBack == false)
			{
				
				object[] values;
				DataSet ds = new DataSet();
				DataTable tbl = new DataTable();
                CurrentUser user = cMisc.GetCurrentUser();
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;



                cCostcodes clscostcodes = new cCostcodes(user.AccountID);
                cDepartments clsdepartments = new cDepartments(user.AccountID);

				cCostCode reqcostcode;
				cDepartment reqdepartment;
				int expenseid = 0;
				int claimid = 0;
				int field = 0;
				if (Request.QueryString["expenseid"] != null)
				{
					expenseid = int.Parse(Request.QueryString["expenseid"]);
				}

				if (Request.QueryString["field"] != null)
				{
					field = int.Parse(Request.QueryString["field"]);
				}
				if (Request.QueryString["claimid"] != null)
				{
					claimid = int.Parse(Request.QueryString["claimid"]);
				}
                cClaims clsclaims = new cClaims(user.AccountID);
				
				cExpenseItem item = clsclaims.getExpenseItemById(expenseid);

				tbl.Columns.Add("code",System.Type.GetType("System.String"));
				tbl.Columns.Add("percentused",System.Type.GetType("System.Int32"));
				tbl.Columns.Add("total",System.Type.GetType("System.Decimal"));
                cExpenseItems expenseItems = new cExpenseItems(user.AccountID);
                item.costcodebreakdown = expenseItems.getCostCodeBreakdown(expenseid);
				foreach (cDepCostItem costitem in item.costcodebreakdown)
				{
					values = new object[3];
					switch (field)
					{
						case 1: //cost codes
							reqcostcode = clscostcodes.GetCostcodeById(costitem.costcodeid);
							values[0] = reqcostcode.Costcode;
							break;
						case 2: //departments
							reqdepartment = clsdepartments.GetDepartmentById(costitem.departmentid);
							values[0] = reqdepartment.Department;
							break;
						case 3: //project codes
						    IProjectCodeWithUserDefinedFields projectCode = this.ProjectCodesRepository[costitem.projectcodeid];
							values[0] = projectCode.Name;
							break;
					}
					values[1] = costitem.percentused;
					values[2] = (item.grandtotal / 100) * costitem.percentused;
					tbl.Rows.Add(values);
				}

				ds.Tables.Add(tbl);

				cGrid clsgrid = new cGrid(ds,true,false);
				clsgrid.tblclass = "datatbl";
				clsgrid.getColumn("percentused").description = "Allocation";
				clsgrid.getColumn("total").description = "Total";
				clsgrid.getColumn("total").fieldtype = "C";
				switch (field)
				{
					case 1:
						clsgrid.getColumn("code").description = "Cost Code";
						break;
					case 2:
						clsgrid.getColumn("code").description = "Department";
						break;
					case 3:
						clsgrid.getColumn("code").description = "Project Code";
						break;
				}
				litgrid.Text = clsgrid.CreateGrid();

                cColours clscolours = new cColours(user.AccountID, user.CurrentSubAccountId, user.CurrentActiveModule);
				
				System.Text.StringBuilder output = new System.Text.StringBuilder();
				output.Append("<style type\"text/css\">\n");
				if (clscolours.sectionHeadingUnderlineColour != clscolours.defaultSectionHeadingUnderlineColour)
				{
					output.Append(".infobar\n");
					output.Append("{\n");
					output.Append("background-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
					output.Append("}\n");
					output.Append(".datatbl th\n");
					output.Append("{\n");
					output.Append("background-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
					output.Append("}\n");
					output.Append(".inputpaneltitle\n");
					output.Append("{\n");
					output.Append("background-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
					output.Append("border-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
					output.Append("}\n");
					output.Append(".paneltitle\n");
					output.Append("{\n");
					output.Append("background-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
					output.Append("border-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
					output.Append("}\n");
					output.Append(".homepaneltitle\n");
					output.Append("{\n");
					output.Append("background-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
					output.Append("border-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
					output.Append("}\n");
                }
                if (clscolours.rowBGColour != clscolours.defaultRowBGColour)
                {
                    output.Append(".datatbl .row1\n");
                    output.Append("{\n");
                    output.Append("background-color: " + clscolours.rowBGColour + ";\n");
                    output.Append("}\n");
                }
                if (clscolours.rowTxtColour != clscolours.defaultRowTxtColour)
                {
                    output.Append(".datatbl .row1\n");
                    output.Append("{\n");
                    output.Append("color: " + clscolours.rowTxtColour + ";\n");
                    output.Append("}\n");
                }
                if (clscolours.altRowBGColour != clscolours.defaultAltRowBGColour)
                {
                    output.Append(".datatbl .row2\n");
                    output.Append("{\n");
                    output.Append("background-color: " + clscolours.altRowBGColour + ";\n");
                    output.Append("}\n");
                }
                if (clscolours.altRowTxtColour != clscolours.defaultAltRowTxtColour)
                {
                    output.Append(".datatbl .row2\n");
                    output.Append("{\n");
                    output.Append("color: " + clscolours.altRowTxtColour + ";\n");
                    output.Append("}\n");
                }
				if (clscolours.fieldTxtColour != clscolours.defaultFieldTxt)
				{
					output.Append(".labeltd\n");
					output.Append("{\n");
					output.Append("color: " + clscolours.fieldTxtColour + ";\n");
					output.Append("}\n");
				}
				output.Append("</style>");
				litstyles.Text = output.ToString();
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
