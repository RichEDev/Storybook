using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

using System.Collections.Generic;
using System.Web.Services;
using SpendManagementLibrary;
using SpendManagementLibrary.FinancialYears;
using System.Text;


namespace Spend_Management
{
    using SpendManagementLibrary.Enumerators;

    public partial class aeCars : System.Web.UI.UserControl
    {
        private int nAccountID;
        private int nEmployeeID;
        private int nCarID;
        private aeCarPageAction nAction;
        private string sReturnURL;
		private Modules currentModule = Modules.expenses;
        bool bAdministrator = false;
        bool bIsPoolCar = false;
        bool bSendEmailForNewCar = false;
        bool bIsAeExpenses = false;
        bool bInModalPopup = false;
        bool bHideButtons = false;
        bool bShowStartDateOnly = false;
        bool bShowDutyOfCare = false;

        protected void Page_Load(object sender, EventArgs e)
        {

            #region set props

            if (ViewState["hideButtons"] != null)
            {
                bHideButtons = (bool)ViewState["hideButtons"];
            }

            if (ViewState["accountid"] != null)
            {
                nAccountID = (int)ViewState["accountid"];
            }

            if (ViewState["employeeid"] != null)
            {
                nEmployeeID = (int)ViewState["employeeid"];
            }

            if (ViewState["bAdministrator"] != null)
            {
                bAdministrator = (bool)ViewState["bAdministrator"];
            }

            if (ViewState["carid"] != null)
            {
                nCarID = (int)ViewState["carid"];
            }

            if (ViewState["isPoolCar"] != null)
            {
                bIsPoolCar = (bool)ViewState["isPoolCar"];
            }

            if (ViewState["bSendEmailForNewCar"] != null)
            {
                bSendEmailForNewCar = (bool)ViewState["bSendEmailForNewCar"];
            }

            if (ViewState["ReturnURL"] != null)
            {
                sReturnURL = (string)ViewState["ReturnURL"];
            }

            if (ViewState["aeAction"] != null)
            {
                nAction = (aeCarPageAction)ViewState["aeAction"];
            }

            if (ViewState["aeExpenses"] != null)
            {
                bIsAeExpenses = (bool)ViewState["aeExpenses"];
            }

            if (ViewState["inModalPopup"] != null)
            {
                bInModalPopup = (bool)ViewState["inModalPopup"];
            }

            if (ViewState["bShowStartDateOnly"] != null)
            {
                bShowStartDateOnly = (bool)ViewState["bShowStartDateOnly"];
            }

            if (ViewState["bShowDutyOfCare"] != null)
            {
                bShowDutyOfCare = (bool)ViewState["bShowDutyOfCare"];
            }

		    this.currentModule = HostManager.GetModule(this.Request.Url.Host);				

            #endregion

            if (this.bIsAeExpenses)
            {
                ServiceReference svcRefAutoComplete = new ServiceReference(cMisc.Path + "/shared/webServices/svcAutoComplete.asmx");
                ScriptReference scrRefAutoComplete = new ScriptReference(cMisc.Path + "/shared/javascript/minify/sel.autoComplete.js?date=20180219");

                ScriptManagerProxy1.Services.Add(svcRefAutoComplete);
                ScriptManagerProxy1.Scripts.Add(scrRefAutoComplete);
            }

            cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(this.AccountID);
            cAccountProperties reqProperties = clsSubAccounts.getFirstSubAccount().SubAccountProperties.Clone();

            if (reqProperties.DisableCarOutsideOfStartEndDate)
            {
                chkactive.Enabled = false;
            }
            if (bAdministrator == false)
            {
                
                tabOdomter.Visible = false;
                pnlActive.Visible = false;
                pnlDates.Visible = false;

                if (reqProperties.ActivateCarOnUserAdd)
                {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "variables", "var approved = true;", true);
                }
                else
                {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "variables", "var approved = false;", true);
                }

                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Admin", "var isAdmin = false;", true);
            }
            else
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "variables", "var approved = true;", true);
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Admin", "var isAdmin = true;", true);

                if (cMisc.GetCurrentUser().Account.IsNHSCustomer)
                {
                    this.tabEsr.Visible = true;
                }
            }

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "editOnly", "var editOnly = false;", true);

            if (bShowStartDateOnly)
            {
                phCarEndDate.Visible = false;
                pnlDates.Visible = true;
                lblStartEndDateTitle.Text = "Start Date of Vehicle Usage";

                if (reqProperties.EmpToSpecifyCarStartDateOnAddMandatory)
                {
                    lblstart.Text = lblstart.Text + "*";
                    lblstart.CssClass = "mandatory";
                    reqcarstartdate.Enabled = true;
                }
            }

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ReturnUrl", "var returnURL = '" + sReturnURL + "';", true);

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "AEExpense", "var isAeExpense = " + (bIsAeExpenses ? "true" : "false") + ";", true);

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "IsPoolCar", "var isPoolCar = " + (bIsPoolCar ? "true" : "false") + ";", true);

            this.divLookupContainer.Visible = reqProperties.VehicleLookup;

            if ((this.currentModule == Modules.Greenlight || this.currentModule == Modules.GreenlightWorkforce) || (reqProperties.ShowMileageCatsForUsers == false && this.bAdministrator == false))
            {
                this.pnlMileageCats.Visible = false;				
            }
			if (this.currentModule == Modules.Greenlight || this.currentModule == Modules.GreenlightWorkforce)
			{
				this.exemptFromHomeToOffice.Style.Add(HtmlTextWriterStyle.Display, "none");
			}

            cCarsBase clsCars = null;
            if (bIsPoolCar)
            {
                clsCars = new cPoolCars(AccountID);
            }
            else
            {
                clsCars = new cEmployeeCars(AccountID, EmployeeID);
            }


            var clsTables = new cTables(nAccountID);
            var clsfields = new cFields(nAccountID);
            int vehicleEngineTypeId = 0;


            if (nCarID > 0) // && isPoolCar == true)
            {
                cCar reqCar = clsCars.GetCarByID(nCarID);
                cmbUom.SelectedValue = ((int)reqCar.defaultuom).ToString();
                vehicleEngineTypeId = Convert.ToInt32(reqCar.VehicleEngineTypeId);
            }
            CurrentUser currentUser = cMisc.GetCurrentUser();
            var financialYearID = FinancialYear.GetPrimary(currentUser).FinancialYearID;       
            var cars = new svcCars();
            this.ddlFinancialYear.Items.Clear();
            this.ddlFinancialYear.Items.AddRange(cars.GetFinancialYears(this.EmployeeID));
            int.TryParse(this.ddlFinancialYear.SelectedValue, out financialYearID);        
           
            string[] gridData = cars.createMileageGrid(cmbUom.SelectedValue, financialYearID, vehicleEngineTypeId);
            litmileagegrid.InnerHtml = gridData[2];
            
            string[] odoGrid = cars.createOdoGrid(0);
            litOdo.Text = odoGrid[2];

            // set the sel.grid javascript variables
            List<string> jsGridObjects = new List<string>();
            jsGridObjects.Add(gridData[1]);
            jsGridObjects.Add(odoGrid[1]);

            Page.ClientScript.RegisterStartupScript(this.GetType(), "carsGridVars", cGridNew.generateJS_init("carsGridVars", jsGridObjects, currentUser.CurrentActiveModule), true);

            cUserdefinedFields clsuserdefined = new cUserdefinedFields((int)ViewState["accountid"]);
            cTables clstables = new cTables((int)ViewState["accountid"]);
            cTable tbl = clstables.GetTableByID(new Guid("a184192f-74b6-42f7-8fdb-6dcf04723cef"));
            StringBuilder udfscript;
            clsuserdefined.createFieldPanel(ref holderUserdefined, clstables.GetTableByID(tbl.UserDefinedTableID), "ValidationSummaryAeCar", out udfscript, excludeAdminFields: !bAdministrator);
            if (udfscript.Length > 0)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "carudfs", udfscript.ToString(), true);
            }

            if (ViewState["record"] != null)
            {
                clsuserdefined.populateRecordDetails(ref holderUserdefined, clstables.GetTableByID(tbl.UserDefinedTableID), (SortedList<int, object>)ViewState["record"], excludeAdminFields: !bAdministrator);
            }
            // }

            if (bHideButtons == true)
            {
                pnlBtns.Style.Add(HtmlTextWriterStyle.Display, "none");
            }

            if (!this.IsPostBack)
            {
                cCarsBase.AddCarEngineTypesToDropDownList(currentUser, ref this.cmbcartype);    
            }

            if (!this.IsPostBack)
            {
                cCarsBase.AddVehicleTypesToDropDownList(ref this.cmbvehicletype);
        }
        }

        #region Properties
        public int AccountID
        {
            get
            {
                if (ViewState["accountid"] == null)
                {
                    return 0;
                }
                else
                {
                    return (int)ViewState["accountid"];
                }
            }
            set
            {
                ViewState["accountid"] = value;
            }
        }

        public int EmployeeID
        {
            get
            {
                if (ViewState["employeeid"] == null)
                {
                    return 0;
                }
                else
                {
                    return (int)ViewState["employeeid"];
                }
            }
            set
            {
                ViewState["employeeid"] = value;
            }
        }

        public int CarID
        {
            get
            {
                if (ViewState["carid"] == null)
                {
                    return 0;
                }
                else
                {
                    return (int)ViewState["carid"];
                }
            }
            set
            {
                ViewState["carid"] = value;
            }
        }

        public bool inModalPopup
        {
            get
            {
                if (ViewState["inModalPopup"] == null)
                {
                    return false;
                }
                else
                {
                    return (bool)ViewState["inModalPopup"];
                }
            }
            set
            {
                ViewState["inModalPopup"] = value;
            }
        }

        public bool isAeExpenses
        {
            get
            {
                if (ViewState["aeExpenses"] == null)
                {
                    return false;
                }
                else
                {
                    return (bool)ViewState["aeExpenses"];
                }
            }
            set
            {
                ViewState["aeExpenses"] = value;
            }
        }

        public aeCarPageAction Action
        {
            get
            {
                if (ViewState["aeAction"] == null)
                {
                    return aeCarPageAction.Add;
                }
                else
                {
                    return (aeCarPageAction)ViewState["aeAction"];
                }
            }
            set
            {
                ViewState["aeAction"] = value;
            }
        }

        public bool HideButtons
        {
            get
            {
                if (ViewState["hideButtons"] == null)
                {
                    return false;
                }
                else
                {
                    return (bool)ViewState["hideButtons"];
                }
            }

            set
            {
                ViewState["hideButtons"] = value;
            }
        }

        public string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                {
                    return "";
                }
                else
                {
                    return (string)ViewState["ReturnURL"];
                }
            }
            set
            {
                ViewState["ReturnURL"] = value;
            }
        }

        public bool EmployeeAdmin
        {
            get
            {
                if (ViewState["bAdministrator"] == null)
                {
                    return false;
                }
                else
                {
                    return (bool)ViewState["bAdministrator"];
                }
            }

            set
            {
                ViewState["bAdministrator"] = value;
            }
        }

        public bool isPoolCar
        {
            get
            {
                if (ViewState["isPoolCar"] == null)
                {
                    return false;
                }
                else
                {
                    return (bool)ViewState["isPoolCar"];
                }
            }
            set
            {
                ViewState["isPoolCar"] = value;
            }
        }

        public bool SendEmailWhenNewCarAdded
        {
            get
            {
                if (ViewState["bSendEmailForNewCar"] == null)
                {
                    return false;
                }
                else
                {
                    return (bool)ViewState["bSendEmailForNewCar"];
                }
            }
            set
            {
                ViewState["bSendEmailForNewCar"] = value;
            }
        }

        /// <summary>
        /// Gets or Sets whether the Car Start Date is shown without the End Date
        /// </summary>
        public bool ShowStartDateOnly {
            get
            {
                if (ViewState["bShowStartDateOnly"] == null)
                {
                    return false;
                }
                else
                {
                    return (bool)ViewState["bShowStartDateOnly"];
                }
            }
            set
            {
                ViewState["bShowStartDateOnly"] = value;
            }
        }

        /// <summary>
        /// Gets or Sets whether the Duty of Care section is displayed
        /// </summary>
        public bool ShowDutyOfCare
        {
            get
            {
                if (ViewState["bShowDutyOfCare"] == null)
                {
                    return false;
                }
                else
                {
                    return (bool)ViewState["bShowDutyOfCare"];
                }
            }
            set
            {
                ViewState["bShowDutyOfCare"] = value;
            }
        }
        #endregion

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

        protected void cmbvehicletype_SelectedIndexChanged(object sender, EventArgs e)
        {
            //this.txtmodel.Enabled = this.chkusecar.Checked;
            //this.txtmake.Enabled = this.chkusecar.Checked;

            this.reqreg.Enabled = this.IsVehicleBicycle() == true;
            this.rfEngineSize.Enabled = this.IsVehicleBicycle() == true;
            this.cvEngineSize.Enabled = this.IsVehicleBicycle() == true;
            
            this.txtregno.Enabled = this.IsVehicleBicycle();
            this.txtEngineSize.Enabled = this.IsVehicleBicycle();
            this.cmbcartype.Enabled = this.IsVehicleBicycle();
            this.rvEngineType.Enabled = this.IsVehicleBicycle();
        }
  
        /// <summary>
        /// The is vehicle bicycle.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected bool IsVehicleBicycle()
        {
            return Convert.ToInt32(cmbvehicletype.SelectedValue.ToString()) != (int)CarTypes.VehicleType.Bicycle ? true : false;
        }
    }

    public enum aeCarPageAction
    {
        Edit,Add
    }
}
