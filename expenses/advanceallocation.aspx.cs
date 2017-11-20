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

using Infragistics.WebUI.UltraWebGrid;
using expenses.Old_App_Code;
using SpendManagementLibrary;
using Spend_Management;

namespace expenses
{
    using SpendManagementLibrary.Employees;

    /// <summary>
	/// Summary description for advanceallocation.
	/// </summary>
	public partial class advanceallocation : System.Web.UI.Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (IsPostBack == false)
			{
                cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
				int floatid = int.Parse(Request.QueryString["floatid"]);
				int viewid = int.Parse(Request.QueryString["viewid"]);
				ViewState["floatid"] = floatid;
                ViewState["viewid"] = viewid;

                CurrentUser user = cMisc.GetCurrentUser();
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;



                cFloats clsfloats = new cFloats(user.AccountID);
                cFloat reqfloat = clsfloats.GetFloatById(floatid);

                cCurrencies clscurrencies = new cCurrencies(user.AccountID, user.CurrentSubAccountId);
                cCurrency reqcur = clscurrencies.getCurrencyById(reqfloat.currencyid);

                lblname.Text = reqfloat.name;
                lbltotal.Text = reqfloat.floatamount.ToString(clsglobalcurrencies.getGlobalCurrencyById(reqcur.globalcurrencyid).symbol + "###,###,##0.00");
                lblallocated.Text = reqfloat.floatused.ToString(clsglobalcurrencies.getGlobalCurrencyById(reqcur.globalcurrencyid).symbol + "###,###,##0.00");
                lblavailable.Text = reqfloat.floatavailable.ToString(clsglobalcurrencies.getGlobalCurrencyById(reqcur.globalcurrencyid).symbol + "###,###,##0.00");
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

        protected override void OnInitComplete(EventArgs e)
        {
            base.OnInitComplete(e);
            gridexpenses.InitializeDataSource += new Infragistics.WebUI.UltraWebGrid.InitializeDataSourceEventHandler(gridexpenses_InitializeDataSource);
        }

        void gridexpenses_InitializeDataSource(object sender, Infragistics.WebUI.UltraWebGrid.UltraGridEventArgs e)
        {
            cViews clsview = new cViews((int)ViewState["accountid"],(int)ViewState["employeeid"]);

            gridexpenses.DataSource = clsview.getFloatAllocationView((int)ViewState["viewid"], (int)ViewState["floatid"]);
        }
        protected void gridexpenses_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns.FromKey("expenseid").Hidden = true;
            e.Layout.Bands[0].Columns.FromKey("basecurrency").Hidden = true;
            e.Layout.Bands[0].Columns.FromKey("returned").Hidden = true;
            e.Layout.Bands[0].Columns.FromKey("originalcurrency").Hidden = true;
            e.Layout.Bands[0].Columns.FromKey("globalbasecurrency").Hidden = true;
            cMisc clsmisc = new cMisc((int)ViewState["accountid"]);
            cEmployees clsemployees = new cEmployees((int)ViewState["accountid"]);
            Employee reqemp = clsemployees.GetEmployeeById((int)ViewState["employeeid"]);
            UserView viewtype = (UserView)ViewState["viewid"];
            cUserView view = clsemployees.getUserView(reqemp.EmployeeID,viewtype,false);
            foreach (cField field in view.fields.Values)
            {
                if (e.Layout.Bands[0].Columns.FromKey(field.FieldID.ToString()) != null)
                {
                    e.Layout.Bands[0].Columns.FromKey(field.FieldID.ToString()).Header.Caption = field.Description;
                }
                switch (field.FieldType)
                {
                    case "D":
                        e.Layout.Bands[0].Columns.FromKey(field.FieldID.ToString()).Format = "dd/MM/yyyy";
                        break;
                }
                switch (field.FieldType)
                {
                    case "C":
                    case "M":
                    case "FD":
                    case "N":
                        e.Layout.Bands[0].Columns.FromKey(field.FieldID.ToString()).CellStyle.HorizontalAlign = HorizontalAlign.Right;
                        break;
                }
                cFieldToDisplay clsfield;
                switch (field.FieldID.ToString())
                {
                    case "4D0F2409-0705-4F0F-9824-42057B25AEBE":
                        clsfield = clsmisc.GetGeneralFieldByCode("organisation");
                        e.Layout.Bands[0].Columns.FromKey(field.FieldID.ToString()).Header.Caption = clsfield.description;
                        break;
                    case "3d8c699e-9e0e-4484-b821-b49b5cb4c098":
                        clsfield = clsmisc.GetGeneralFieldByCode("from");
                        e.Layout.Bands[0].Columns.FromKey(field.FieldID.ToString()).Header.Caption = clsfield.description;
                        break;
                    case "1ee53ae2-2cdf-41b4-9081-1789adf03459":
                        clsfield = clsmisc.GetGeneralFieldByCode("currency");
                        e.Layout.Bands[0].Columns.FromKey(field.FieldID.ToString()).Header.Caption = clsfield.description;
                        break;
                    case "ec527561-dfee-48c7-a126-0910f8e031b0":
                        clsfield = clsmisc.GetGeneralFieldByCode("country");
                        e.Layout.Bands[0].Columns.FromKey(field.FieldID.ToString()).Header.Caption = clsfield.description;
                        break;
                    case "af839fe7-8a52-4bd1-962c-8a87f22d4a10":
                        clsfield = clsmisc.GetGeneralFieldByCode("reason");
                        e.Layout.Bands[0].Columns.FromKey(field.FieldID.ToString()).Header.Caption = clsfield.description;
                        break;
                    case "7cf61909-8d25-4230-84a9-f5701268f94b":
                        clsfield = clsmisc.GetGeneralFieldByCode("otherdetails");
                        e.Layout.Bands[0].Columns.FromKey(field.FieldID.ToString()).Header.Caption = clsfield.description;
                        break;
                    case "359dfac9-74e6-4be5-949f-3fb224b1cbfc":
                        clsfield = clsmisc.GetGeneralFieldByCode("costcode");
                        e.Layout.Bands[0].Columns.FromKey(field.FieldID.ToString()).Header.Caption = clsfield.description;
                        break;
                    case "9617a83e-6621-4b73-b787-193110511c17":
                        clsfield = clsmisc.GetGeneralFieldByCode("department");
                        e.Layout.Bands[0].Columns.FromKey(field.FieldID.ToString()).Header.Caption = clsfield.description;
                        break;
                    case "6d06b15e-a157-4f56-9ff2-e488d7647219":
                        clsfield = clsmisc.GetGeneralFieldByCode("projectcode");
                        e.Layout.Bands[0].Columns.FromKey(field.FieldID.ToString()).Header.Caption = clsfield.description;
                        break;
                }
            }

            if (e.Layout.Bands[0].Columns.FromKey("flagcount") == null)
            {
                e.Layout.Bands[0].Columns.Insert(0, new Infragistics.WebUI.UltraWebGrid.UltraGridColumn("splitcount", "", Infragistics.WebUI.UltraWebGrid.ColumnType.NotSet, ""));
                e.Layout.Bands[0].Columns.Insert(0, new Infragistics.WebUI.UltraWebGrid.UltraGridColumn("flagcount", "<img src=\"icons/about_blue.gif\">", Infragistics.WebUI.UltraWebGrid.ColumnType.HyperLink, ""));
                e.Layout.Bands[0].Columns.FromKey("flagcount").Width = Unit.Pixel(15);

            }
        }

        protected void gridexpenses_InitializeRow(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {



        }
    }
}
