namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;

    using shared.webServices;

    using SpendManagementLibrary;
    using SpendManagementLibrary.FinancialYears;

    /// <summary>
    /// Summary description for aemileage.
    /// </summary>
    public partial class aemileage : Page
    {
        protected ImageButton cmdhelp;

        /// <summary>
        /// An instance of <see cref="IDataFactory{IGeneralOptions,Int32}"/> to get a <see cref="IGeneralOptions"/>
        /// </summary>
        [Dependency]
        public IDataFactory<IGeneralOptions, int> GeneralOptionsFactory { get; set; }

        protected void Page_Load(object sender, System.EventArgs e)
        {

            Title = "Add/Edit Vehicle Journey Rate Category";
            Master.title = Title;
            Master.PageSubTitle = "Vehicle Journey Rate Category Details";
            Master.helpid = 1011;

            if (IsPostBack == false)
            {
                bool allowDateRangeAdd = true;
                StringBuilder script = new StringBuilder();
                Master.enablenavigation = false;
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.VehicleJourneyRateCategories, true, true);

                cCurrencies clscurrencies = new cCurrencies(user.AccountID, user.CurrentSubAccountId);
                cmbCurrency.Items.AddRange(clscurrencies.CreateDropDown().ToArray());

                var generalOptions = this.GeneralOptionsFactory[user.CurrentSubAccountId].WithCurrency();

                cmbCurrency.SelectedValue = generalOptions.Currency.BaseCurrency.ToString();

                this.PopulateFinancialYearDropDown(user);
                this.PopulateVechicleEngineTypesDropDown(user);

                cmbRangeType.Attributes.Add("onchange", "SEL.VehicleJourneyRate.DateRange.Threshold.cmbRangeType_onchange();");
                cmbDateRangeType.Attributes.Add("onchange", "SEL.VehicleJourneyRate.DateRange.cmbDateRangeType_onchange();");
                int mileageid = 0;
                if (Request.QueryString["mileageid"] != null)
                {
                    int.TryParse(Request.QueryString["mileageid"], out mileageid);
                }
                ViewState["mileageid"] = mileageid;

                string[] gridData;
                if (mileageid > 0)
                {
                    cMileagecats clsmileagecats = new cMileagecats(user.AccountID);
                    cMileageCat reqmileage;

                    reqmileage = clsmileagecats.GetMileageCatById(mileageid);

                    cmbCurrency.SelectedValue = reqmileage.currencyid.ToString();

                    txtcarsize.Text = reqmileage.carsize;
                    txtdescription.Text = reqmileage.comment;

                    if (reqmileage.thresholdType == ThresholdType.Annual)
                    {
                        cmbThreshold.SelectedIndex = 0;
                    }
                    else
                    {
                        cmbThreshold.SelectedIndex = 1;
                    }

                    chkcalcmilestotal.Checked = reqmileage.calcmilestotal;

                    if (reqmileage.mileUom == MileageUOM.Mile)
                    {
                        cmbUom.SelectedIndex = 0;
                    }
                    else
                    {
                        cmbUom.SelectedIndex = 1;
                    }

                    // if range includes Any, don't permit any more additions
                    for (int x = 0; x < reqmileage.dateRanges.Count; x++)
                    {
                        cMileageDaterange curRange = (cMileageDaterange)reqmileage.dateRanges[x];
                        if (curRange.daterangetype == DateRangeType.Any)
                        {
                            allowDateRangeAdd = false;
                            break;
                        }
                    }

                    script.AppendFormat("SEL.VehicleJourneyRate.CurrentMileageId = {0};", reqmileage.mileageid);

                    gridData = new svcVehicleJourneyRate().DateRangeGrid(reqmileage.mileageid.ToString());

                    if (user.Account.IsNHSCustomer)
                    {
                        this.txtnhscode.Text = reqmileage.UserRatestable;
                        this.txtnhsstart.Text = reqmileage.UserRatesFromEngineSize.ToString();
                        this.txtnhsend.Text = reqmileage.UserRatesToEngineSize.ToString();
                    }

                    if (reqmileage.FinancialYearID != null)
                    {
                        foreach (ListItem item in this.ddlFinancialYear.Items)
                        {
                            if (item.Value == reqmileage.FinancialYearID.ToString())
                            {
                                item.Selected = true;
                            }
                        }
                    }
                }
                else
                {
                    gridData = new svcVehicleJourneyRate().DateRangeGrid("");
                    script.Append("SEL.VehicleJourneyRate.CurrentMileageId = 0;");
                }

                if (!user.Account.IsNHSCustomer)
                {
                    this.pannhs.Visible = false;
                    this.reqnhscode.Enabled = false;
                    this.reqnhsend.Enabled = false;
                    this.reqnhsstart.Enabled = false;
                }

                litDateRangeGrid.Text = gridData[2];

                // set the sel.grid javascript variables
                Page.ClientScript.RegisterStartupScript(this.GetType(), "mileageGridVars", cGridNew.generateJS_init("mileageGridVars", new List<string>() { gridData[1] }, user.CurrentActiveModule), true);

                if (allowDateRangeAdd)
                {
                    litAddDateRange.Text = "<a id=\"lnkAddDateRange\" href=\"javascript:SEL.VehicleJourneyRate.CurrentDateRangeId=0;SEL.VehicleJourneyRate.DateRange.ShowModal(true);\">Add Date Range</a>";
                }
                else
                {
                    litAddDateRange.Text = "<a id=\"lnkAddDateRange\" href=\"javascript:SEL.VehicleJourneyRate.CurrentDateRangeId=0;SEL.VehicleJourneyRate.DateRange.ShowModal(true);\" style= \"display:none;\">Add Date Range</a>";
                }

                this.ClientScript.RegisterStartupScript(this.GetType(), "startup", script.ToString(), true);

                string[] grid = aemileage.GenerateRatesGrid(user, "gridRates", 0);
                this.litRatesGrid.Text = grid[1];
                Page.ClientScript.RegisterStartupScript(this.GetType(), "gridRatesVars", cGridNew.generateJS_init("gridRatesVars", new List<string> { grid[0] }, user.CurrentActiveModule), true);

            }
        }

        public static string[] GenerateRatesGrid(ICurrentUser user, string gridId, int mileageThresholdId)
        {
            var grid = new cGridNew(user.AccountID, user.EmployeeID, gridId, "select MileageThresholdRates.MileageThresholdRateId, MileageThresholdRates.MileageThresholdId, VehicleEngineTypes.Name, MileageThresholdRates.RatePerUnit, MileageThresholdRates.AmountForVat from MileageThresholdRates")
            {
                KeyField = "MileageThresholdRateId",
                enableupdating = true,
                enabledeleting = true,
                editlink = "javascript:SEL.VehicleJourneyRate.DateRange.Threshold.Rate.Edit({MileageThresholdRateId});",
                deletelink = "javascript:SEL.VehicleJourneyRate.DateRange.Threshold.Rate.Delete({MileageThresholdRateId});",
                pagesize = GlobalVariables.DefaultModalGridPageSize,
                EmptyText = "There are currently no Fuel rates to display."
            };
            grid.addFilter(((cFieldColumn)grid.getColumnByName("MileageThresholdId")).field, ConditionType.Equals, new object[] { mileageThresholdId }, null, ConditionJoiner.None);
            grid.getColumnByName("MileageThresholdRateId").hidden = true;
            grid.getColumnByName("MileageThresholdId").hidden = true;
            grid.getColumnByName("RatePerUnit").Format = new cGridFormat { Format = AttributeFormat.FormattedText };
            grid.getColumnByName("AmountForVat").Format = new cGridFormat { Format = AttributeFormat.FormattedText };
            return grid.generateGrid();
        }

        private void PopulateFinancialYearDropDown(ICurrentUserBase user)
        {
            var financialYears = FinancialYears.ActiveYears(user);
            this.ddlFinancialYear.Items.Clear();
            foreach (FinancialYear year in financialYears)
            {
                this.ddlFinancialYear.Items.Add(new ListItem(year.Description, year.FinancialYearID.ToString()));
            }
        }

        private void PopulateVechicleEngineTypesDropDown(ICurrentUserBase user)
        {
            this.ddlVehicleEngineType.Items.Clear();
            this.ddlVehicleEngineType.Items.Add(new ListItem("[None]", "0"));
            this.ddlVehicleEngineType.Items.AddRange(VehicleEngineType.GetAll(user)
                .Select(vet => new ListItem(vet.Name, vet.VehicleEngineTypeId.ToString()))
                .ToArray());
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
            this.cmdcancel.Click += new System.Web.UI.ImageClickEventHandler(this.cmdcancel_Click);

        }
        #endregion

        private void cmdcancel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Response.Redirect("adminmileage.aspx", true);
        }
    }
}
