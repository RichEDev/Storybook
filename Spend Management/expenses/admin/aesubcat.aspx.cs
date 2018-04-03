using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using SpendManagementLibrary;
using System.Text;
using System.Web.Services;
using BusinessLogic;
using BusinessLogic.DataConnections;
using BusinessLogic.P11DCategories;
using SpendManagementLibrary.Expedite;
using SpendManagementLibrary.Helpers;
using Spend_Management.shared.webServices;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for aesubcat.
    /// </summary>
    public partial class aesubcat : Page
    {
        [Dependency]
        public IDataFactory<IP11DCategory, int> P11DCategoriesRepository { get; set; }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Master.helpid = 1023;

            CurrentUser user = cMisc.GetCurrentUser();
            SortedList<int, object> udfrecord = null;

            if (IsPostBack == false)
            {
                this.Master.UseDynamicCSS = true;
                this.ScriptMan.Scripts.Add(new ScriptReference(GlobalVariables.StaticContentLibrary +
                                                               "/js/jQuery/jquery-ui.datepicker-en-gb.js"));

                string[] rolesGrid;
                string[] vatGrid;
                string[] splititemGrid;
                string[] allowancesGrid;
                string[] modalSplitItemsGrid;

                Master.enablenavigation = false;
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ExpenseItems, true, true);

                var subcatid = 0;

                if (Request.QueryString["subcatid"] != null)
                {
                    int.TryParse(Request.QueryString["subcatid"], out subcatid);
                }

                cSubcats clsSubcats = new cSubcats(user.AccountID);
                List<SubcatBasic> subcats = clsSubcats.GetSortedList();
                cCategories clscategories = new cCategories(user.AccountID);
                cMisc clsmisc = new cMisc(user.AccountID);
                cMileagecats clsmileage = new cMileagecats(user.AccountID);
                ItemRoles clsroles = new ItemRoles(user.AccountID);
                ddlstItemRole.Items.AddRange(clsroles.CreateDropDown(0, false).ToArray());
                lblfrom.Text = clsmisc.GetGeneralFieldByCode("from").description;
                lblto.Text = clsmisc.GetGeneralFieldByCode("to").description;
                lblcompany.Text = clsmisc.GetGeneralFieldByCode("organisation").description;
                cmbentertainment.Items.Add(new ListItem("[None]", "0"));
                ListItem[] entertainmentItems = clsSubcats.CreateEntertainmentDropDown(subcats, subcatid).ToArray();
                ListItem[] entertainmentItems2 = clsSubcats.CreateEntertainmentDropDown(subcats, subcatid).ToArray();
                cmbentertainment.Items.AddRange(entertainmentItems);
                ListItem[] subsistenceItems = clsSubcats.CreateSubsistenceDropDown(subcats).ToArray();
                cmbsplitremote.Items.Add(new ListItem("[None]", "0"));
                cmbsplitremote.Items.AddRange(subsistenceItems);
                cmbsplitpersonal.Items.Add(new ListItem("[None]", "0"));
                cmbsplitpersonal.Items.AddRange(entertainmentItems2);
                ddlstMileageCategory.Items.Add(new ListItem("[None]", "0"));
                ddlstMileageCategory.Items.AddRange(clsmileage.CreateDropDown().ToArray());
                ddlstDateRange.Attributes.Add("onchange", "ddlstDateRange_onchange();");
                cmbReimbursableItems.Items.Add(new ListItem("[None]", "0"));
                cmbReimbursableItems.Items.AddRange(clsSubcats.CreateReimbursableItemsDropDown().ToArray());
                ddlstReimburseMileageCategory.Items.Add(new ListItem("[None]", "0"));
                ddlstReimburseMileageCategory.Items.AddRange(clsmileage.CreateDropDown().ToArray());
                ddlstPublicTransportRate.Items.Add(new ListItem("[None]", "0"));
                ddlstPublicTransportRate.Items.AddRange(clsmileage.CreateDropDown().ToArray());

                StringBuilder script = new StringBuilder();


                #region Validation Tab

                // enable validation tab if account has it.
                if (user.Account.ValidationServiceEnabled)
                {
                    tabValidation.Visible = true;

                    if (!user.Employee.AdminOverride)
                    {
                        txtValidatorNotes1.ReadOnly = txtValidatorNotes2.ReadOnly = txtValidatorNotes3.ReadOnly = true;
                    }
                }

                #endregion Validation Tab

                var subCatsService = new svcSubCats();
                if (subcatid > 0)
                {
                    cSubcat reqSubcat = clsSubcats.GetSubcatById(subcatid);

                    if (reqSubcat == null)
                    {
                        Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                    }

                    Master.title = "Expense Item: " + reqSubcat.subcat;
                    cmbcategories.Items.AddRange(clscategories.CreateDropDown(0));
                    cmbpdcats.Items.AddRange(this.CreateP11DDropDown(reqSubcat.pdcatid));
                    txtaccountcode.Text = reqSubcat.accountcode;
                    cmbcategories.Items.FindByValue(reqSubcat.categoryid.ToString()).Selected = true;
                    txtsubcat.Text = reqSubcat.subcat;
                    txtshortsubcat.Text = reqSubcat.shortsubcat;
                    txtdescription.Text = reqSubcat.description;

                    if (ddlstCalculation.Items.FindByValue(((byte) reqSubcat.calculation).ToString()) != null)
                    {
                        ddlstCalculation.Items.FindByValue(((byte) reqSubcat.calculation).ToString()).Selected = true;
                    }
                    chkreimbursable.Checked = reqSubcat.reimbursable;
                    if (reqSubcat.vatreceipt == true)
                    {
                        chkvatreceipt.Checked = true;
                        chknormalreceipt.Enabled = false;
                    }

                    this.txtStartDate.Text = reqSubcat.StartDate == null
                        ? string.Empty
                        : reqSubcat.StartDate.Value.ToString("dd/MM/yyyy");

                    this.txtEndDate.Text = reqSubcat.EndDate == null
                        ? string.Empty
                        : reqSubcat.EndDate.Value.ToString("dd/MM/yyyy");

                    switch (reqSubcat.calculation)
                    {
                        case CalculationType.PencePerMile:
                            chkmileage.Enabled = false;
                            break;
                        case CalculationType.Meal:
                            chkstaff.Enabled = false;
                            chkothers.Enabled = false;
                            break;
                        case CalculationType.FuelReceipt:
                            chkreimbursable.Checked = false;
                            chkreimbursable.Enabled = false;
                            break;
                        case CalculationType.PencePerMileReceipt:
                            chkmileage.Enabled = false;
                            break;
                        case CalculationType.FixedAllowance:
                            txtallowanceamount.Text = reqSubcat.allowanceamount.ToString("######0.00");
                            break;
                    }
                    if (reqSubcat.addasnet == true)
                    {
                        cmbtotaltype.Items.FindByText("NET").Selected = true;
                    }
                    else
                    {
                        cmbtotaltype.Items.FindByText("Gross").Selected = true;
                    }



                    chkmileage.Checked = reqSubcat.mileageapp;

                    chkstaff.Checked = reqSubcat.staffapp;

                    chkothers.Checked = reqSubcat.othersapp;



                    chkbmiles.Checked = reqSubcat.bmilesapp;

                    chkpassenger.Checked = reqSubcat.passengersapp;
                    chkHeavyBulky.Checked = reqSubcat.allowHeavyBulkyMileage;

                    chkpmiles.Checked = reqSubcat.pmilesapp;

                    chkhotelmand.Checked = reqSubcat.hotelmand;

                    chkeventinhome.Checked = reqSubcat.eventinhomeapp;

                    chktip.Checked = reqSubcat.tipapp;


                    chkattendees.Checked = reqSubcat.attendeesapp;

                    chknormalreceipt.Checked = reqSubcat.receiptapp;
                    chknopassengers.Checked = reqSubcat.nopassengersapp;
                    chkpassengernames.Checked = reqSubcat.passengernamesapp;
                    chknonights.Checked = reqSubcat.nonightsapp;
                    chkattendeesmand.Checked = reqSubcat.attendeesmand;
                    chknodirectors.Checked = reqSubcat.nodirectorsapp;

                    chkpersonalguests.Checked = reqSubcat.nopersonalguestsapp;
                    chkremoteworkers.Checked = reqSubcat.noremoteworkersapp;
                    chkhotel.Checked = reqSubcat.hotelapp;
                    chknorooms.Checked = reqSubcat.noroomsapp;
                    chkvatnumber.Checked = reqSubcat.vatnumberapp;
                    chkvatnumbermand.Checked = reqSubcat.vatnumbermand;
                    chkfrom.Checked = reqSubcat.fromapp;
                    chkto.Checked = reqSubcat.toapp;
                    chkcompany.Checked = reqSubcat.companyapp;

                    txtcomment.Text = reqSubcat.comment;
                    txtalternateaccountcode.Text = reqSubcat.alternateaccountcode;
                    chkentertainment.Checked = reqSubcat.splitentertainment;
                    chksplitpersonal.Checked = reqSubcat.splitpersonal;
                    chksplitremoteworkers.Checked = reqSubcat.splitremote;
                    if (cmbentertainment.Items.FindByValue(reqSubcat.entertainmentid.ToString()) != null)
                    {
                        cmbentertainment.Items.FindByValue(reqSubcat.entertainmentid.ToString()).Selected = true;
                    }
                    if (cmbsplitremote.Items.FindByValue(reqSubcat.remoteid.ToString()) != null)
                    {
                        cmbsplitremote.Items.FindByValue(reqSubcat.remoteid.ToString()).Selected = true;
                    }
                    if (cmbsplitpersonal.Items.FindByValue(reqSubcat.personalid.ToString()) != null)
                    {
                        cmbsplitpersonal.Items.FindByValue(reqSubcat.personalid.ToString()).Selected = true;
                    }


                    if (clsmisc.GetGeneralFieldByCode("reason").individual == false)
                    {
                        chkreason.Enabled = false;
                    }
                    else
                    {
                        chkreason.Enabled = true;
                        chkreason.Checked = reqSubcat.reasonapp;
                    }
                    if (clsmisc.GetGeneralFieldByCode("otherdetails").individual == false)
                    {
                        chkotherdetails.Enabled = false;
                    }
                    else
                    {
                        chkotherdetails.Enabled = true;
                        chkotherdetails.Checked = reqSubcat.otherdetailsapp;
                    }
                    chkIsRelocationMileage.Checked = reqSubcat.IsRelocationMileage;

                    createCountryGrid(reqSubcat);
                    allowancesGrid = createAllowancesGrid(reqSubcat);

                    vatGrid = subCatsService.createVATGrid(reqSubcat.subcatid.ToString());
                    splititemGrid = subCatsService.createSplitItemGrid(reqSubcat.subcatid.ToString());

                    modalSplitItemsGrid = subCatsService.getSplitItems(reqSubcat.subcatid.ToString());

                    createUDFGrid(reqSubcat);
                    rolesGrid = subCatsService.createRoleGrid(reqSubcat.subcatid.ToString());
                    this.chkenablehometooffice.Checked = reqSubcat.EnableHomeToLocationMileage;
                    this.chkHomeToOfficeAsZero.Checked = false;

                    switch (reqSubcat.HomeToLocationType)
                    {
                        case HomeToLocationType.CalculateHomeAndOfficeToLocationDiff:
                            optdeducthometooffice.Checked = true;
                            break;
                        case HomeToLocationType.FlagHomeAndOfficeToLocationDiff:
                            optflaghometooffice.Checked = true;
                            break;
                        case HomeToLocationType.DeductHomeToOfficeFromEveryJourney:
                            optDeductHomeToOfficeDistanceOnce.Checked = true;
                            break;
                        case HomeToLocationType.DeductHomeToOfficeEveryTimeHomeIsVisited:
                            optDeductHomeToOfficeDistanceAll.Checked = true;
                            break;
                        case HomeToLocationType.DeductHomeToOfficeIfStartOrFinishHome:
                            optDeductHomeToOfficeDistanceStart.Checked = true;
                            break;
                        case HomeToLocationType.DeductFirstAndLastHome:
                            optDeductFirstOrLastHome.Checked = true;
                            break;
                        case HomeToLocationType.DeductFullHomeToOfficeEveryTimeHomeIsVisited:
                            optDeductHomeToOfficeDistanceFull.Checked = true;
                            break;
                        case HomeToLocationType.DeductFullHomeToOfficeIfStartOrFinishHome:
                            this.optDeductFullHomeToOfficeDistanceStart.Checked = true;
                            break;
                        case HomeToLocationType.DeductFixedMilesIfStartOrFinishHome:
                            this.optDeductFixed.Checked = true;
                            this.txtDeductFixed.Text = reqSubcat.HomeToOfficeFixedMiles.ToString();
                            break;
                        case HomeToLocationType.JuniorDoctorRotation:
                            this.optRotationalMileage.Checked = true;
                            this.ddlstPublicTransportRate.Items.FindByValue(reqSubcat.PublicTransportRate.ToString())
                                .Selected = true;
                            break;
                    }

                    if (reqSubcat.HomeToOfficeAlwaysZero)
                    {
                        this.chkHomeToOfficeAsZero.Checked = true;
                    }

                    if (reqSubcat.EnforceToOfficeMileageCap)
                    {
                        this.chkEnforceMileageCap.Checked = true;
                        this.txtMileageCap.Visible = true;
                        this.txtMileageCap.Text = reqSubcat.HomeToOfficeMileageCap == null
                            ? string.Empty
                            : reqSubcat.HomeToOfficeMileageCap.ToString();
                    }

                    if (reqSubcat.MileageCategory != null &&
                        ddlstMileageCategory.Items.FindByValue(reqSubcat.MileageCategory.ToString()) != null)
                    {
                        ddlstMileageCategory.Items.FindByValue(reqSubcat.MileageCategory.ToString()).Selected = true;
                    }

                    if (reqSubcat.reimbursableSubcatID != null &&
                        cmbReimbursableItems.Items.FindByValue(reqSubcat.reimbursableSubcatID.ToString()) != null)
                    {
                        cmbReimbursableItems.Items.FindByValue(reqSubcat.reimbursableSubcatID.ToString()).Selected =
                            true;
                    }

                    if (reqSubcat.MileageCategory != null &&
                        ddlstReimburseMileageCategory.Items.FindByValue(reqSubcat.MileageCategory.ToString()) != null)
                    {
                        ddlstReimburseMileageCategory.Items.FindByValue(reqSubcat.MileageCategory.ToString()).Selected =
                            true;
                    }

                    udfrecord = reqSubcat.userdefined;

                    switch (reqSubcat.calculation)
                    {
                        case CalculationType.NormalItem:
                            break;
                        case CalculationType.Meal:
                            chkstaff.Enabled = false;
                            chkothers.Enabled = false;
                            chknodirectors.Enabled = false;
                            break;
                        case CalculationType.PencePerMile:
                            break;
                        case CalculationType.DailyAllowance:
                            break;
                        case CalculationType.FuelReceipt:
                            break;
                        case CalculationType.PencePerMileReceipt:

                            break;
                        case CalculationType.FixedAllowance:
                            break;
                    }

                    #region DOC Options

                    chkEnableDoc.Checked = reqSubcat.EnableDoC;
                    chkRequireClass1Insurance.Checked = reqSubcat.RequireClass1BusinessInsurance;

                    #endregion


                    #region Validation Properties

                    chkEnableValidation.Checked = reqSubcat.Validate;

                    if (reqSubcat.ValidationRequirements != null)
                    {
                        var requirements = reqSubcat.ValidationRequirements.OrderBy(x => x.Id);
                        var criterion = requirements.ElementAtOrDefault(0);

                        if (criterion != null)
                        {
                            validationCriterion1Id.Value = criterion.Id.ToString();
                            txtValidatorNotes1.Text = criterion.Requirements;
                        }

                        criterion = requirements.ElementAtOrDefault(1);

                        if (criterion != null)
                        {
                            validationCriterion2Id.Value = criterion.Id.ToString();
                            txtValidatorNotes2.Text = criterion.Requirements;
                        }

                        criterion = requirements.ElementAtOrDefault(2);

                        if (criterion != null)
                        {
                            validationCriterion3Id.Value = criterion.Id.ToString();
                            txtValidatorNotes3.Text = criterion.Requirements;
                        }
                    }

                    #endregion Validation Properties

                }
                else
                {
                    Master.title = "Expense Item: New";
                    cmbcategories.Items.AddRange(clscategories.CreateDropDown(0));
                    cmbpdcats.Items.AddRange(this.CreateP11DDropDown(0));
                    createCountryGrid(null);

                    if (clsmisc.GetGeneralFieldByCode("reason").individual == false)
                    {
                        chkreason.Enabled = false;
                    }

                    if (clsmisc.GetGeneralFieldByCode("otherdetails").individual == false)
                    {
                        chkotherdetails.Enabled = false;
                    }

                    allowancesGrid = createAllowancesGrid(null);
                    vatGrid = subCatsService.createVATGrid("");
                    splititemGrid = subCatsService.createSplitItemGrid("");

                    modalSplitItemsGrid = subCatsService.getSplitItems("");
                    createUDFGrid(null);
                    rolesGrid = subCatsService.createRoleGrid("0");

                }

                litRoles.Text = rolesGrid[2];
                litVatRates.Text = vatGrid[2];
                litsplit.Text = splititemGrid[2];
                litallowances.Text = allowancesGrid[2];

                Literal litModalSplit = new Literal();
                litModalSplit.Text = modalSplitItemsGrid[2];
                pnlSplitList.Controls.Add(litModalSplit);

                // set the sel.grid javascript variables
                List<string> jsBlockObjects = new List<string>();

                jsBlockObjects.Add(rolesGrid[1]);
                jsBlockObjects.Add(vatGrid[1]);
                jsBlockObjects.Add(splititemGrid[1]);
                jsBlockObjects.Add(allowancesGrid[1]);
                jsBlockObjects.Add(modalSplitItemsGrid[1]);

                Page.ClientScript.RegisterStartupScript(this.GetType(), "SubcatGridVars",
                    cGridNew.generateJS_init("SubcatGridVars", jsBlockObjects, user.CurrentActiveModule), true);

                script.Append("var subcatid = " + subcatid + ";\n");
                script.Append(getCountryList());
                script.Append(getUDFList());

                if (subcatid > 0)
                {
                    script.Append("setupEditCalculationDiv();\n");
                }
                this.ClientScript.RegisterStartupScript(this.GetType(), "startup", script.ToString(), true);

                Master.PageSubTitle = "Expense Item Details";
            }

            cUserdefinedFields clsuserdefined = new cUserdefinedFields(user.AccountID);
            cTables clstables = new cTables(user.AccountID);
            cTable tbl = clstables.GetTableByID(new Guid("401b44d7-d6d8-497b-8720-7ffcc07d635d"));
            StringBuilder javascript;
            clsuserdefined.createFieldPanel(ref holderUserdefined, clstables.GetTableByID(tbl.UserDefinedTableID),
                "vgSubcat", out javascript);
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "udfs", javascript.ToString(), true);
            if (udfrecord != null)
            {
                clsuserdefined.populateRecordDetails(ref holderUserdefined,
                    clstables.GetTableByID(tbl.UserDefinedTableID), udfrecord);
            }

        }

        /// <summary>
        /// Creates the P11D dropdown
        /// </summary>
        /// <param name="pdcatid">The Id of the P11D currently assignedto the subcat</param>
        /// <returns>A <see cref="ListItem"/> of P11D Categories</returns>
        public ListItem[] CreateP11DDropDown(int pdcatid)
        {
            var p11DCategoriesUnsorted = this.P11DCategoriesRepository.Get();
            var p11DCategoriesSorted = from p11DCategory in p11DCategoriesUnsorted
                orderby p11DCategory.Name
                select p11DCategory;
            var count = p11DCategoriesSorted.Count();
            var items = new ListItem[count + 1];
            items[0] = new ListItem();
            var i = 1;

            foreach (var p11DCategory in p11DCategoriesSorted)
            {
                items[i] = new ListItem
                {
                    Text = p11DCategory.Name,
                    Value = p11DCategory.Id.ToString()
                };
                if (p11DCategory.Id == pdcatid)
                {
                    items[i].Selected = true;
                }
                i++;
            }

            return items;
        }

        private string[] createAllowancesGrid(cSubcat subcat)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cAllowances clsallowances = new cAllowances(user.AccountID);
            DataSet ds = new DataSet();
            DataTable tbl = new DataTable();
            SortedList<string, cAllowance> allowances = clsallowances.sortList();
            tbl.Columns.Add("allowanceid", typeof(System.Int32));
            tbl.Columns.Add("allowance", typeof(System.String));
            foreach (cAllowance allowance in allowances.Values)
            {
                tbl.Rows.Add(new object[] {allowance.allowanceid, allowance.allowance});
            }

            ds.Tables.Add(tbl);

            cTables clstables = new cTables(user.AccountID);
            cFields clsfields = new cFields(user.AccountID);

            List<cNewGridColumn> columns = new List<cNewGridColumn>();
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("04031a74-115c-4d41-a31a-0727a3ea0198"))));
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("74bec6a1-5520-46bc-96d1-759200bc206f"))));
            cGridNew clsgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridAllowances",
                clstables.GetTableByID(new Guid("68a1116c-b8e7-45d9-824b-acfe82c25c54")), columns, ds);
            clsgrid.KeyField = "allowanceid";
            clsgrid.getColumnByName("allowanceid").hidden = true;
            clsgrid.EnableSorting = false;
            clsgrid.EnableSelect = true;
            if (subcat != null)
            {
                foreach (int i in subcat.allowances)
                {
                    clsgrid.SelectedItems.Add(i);
                }
            }

            List<string> retVals = new List<string>();
            retVals.Add(clsgrid.GridID);
            retVals.AddRange(clsgrid.generateGrid());
            return retVals.ToArray();
        }

        private void createUDFGrid(cSubcat subcat)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            bool newdiv = true;
            cUserdefinedFields clsudfs = new cUserdefinedFields(user.AccountID);
            List<cUserDefinedField> fields = clsudfs.GetSpecificUserDefinedFields();
            Literal lit;
            Label lbl;
            CheckBox chkbox;



            foreach (cUserDefinedField field in fields)
            {
                if (newdiv)
                {
                    lit = new Literal();
                    lit.Text = "<div class=\"twocolumn\">";
                    holderUDFs.Controls.Add(lit);
                }
                lbl = new Label();
                lbl.Text = field.attribute.displayname;
                lbl.AssociatedControlID = "chkudf" + field.userdefineid;
                holderUDFs.Controls.Add(lbl);
                lit = new Literal();
                lit.Text = "<span class=\"inputs\">";
                holderUDFs.Controls.Add(lit);
                chkbox = new CheckBox();
                chkbox.ID = "chkudf" + field.userdefineid;
                chkbox.InputAttributes["value"] = field.userdefineid.ToString();
                if (subcat != null && subcat.associatedudfs.Contains(field.userdefineid))
                {
                    chkbox.Checked = true;
                }
                holderUDFs.Controls.Add(chkbox);
                lit = new Literal();
                lit.Text =
                    "</span><span class=\"inputicon\"></span><span class=\"inputtooltipfield\"></span><span class=\"inputvalidatorfield\"></span>";
                holderUDFs.Controls.Add(lit);
                if (!newdiv)
                {
                    lit = new Literal();
                    lit.Text = "</div>";
                    holderUDFs.Controls.Add(lit);

                }
                newdiv = !newdiv;
            }
            if (!newdiv)
            {
                lit = new Literal();
                lit.Text = "</div>";
                holderUDFs.Controls.Add(lit);
            }
        }

        private void createCountryGrid(cSubcat subcat)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            bool newdiv = true;
            cCountrySubcat countrysubcat;
            Literal lit;
            Label lbl;
            TextBox txt;
            cCountries clscountries = new cCountries(user.AccountID, user.CurrentSubAccountId);
            cGlobalCountries clsglobalcountries = new cGlobalCountries();
            cGlobalCountry globalcountry;
            SortedList<string, cCountry> lstCountries = clscountries.sortList(true);

            foreach (cCountry country in lstCountries.Values)
            {
                if (newdiv)
                {
                    lit = new Literal();
                    lit.Text = "<div class=\"twocolumn\">";
                    holderCountries.Controls.Add(lit);
                }
                lbl = new Label();
                globalcountry = clsglobalcountries.getGlobalCountryById(country.GlobalCountryId);
                lbl.Text = globalcountry.Country;
                lbl.AssociatedControlID = "txtaccountcode" + country.CountryId;
                holderCountries.Controls.Add(lbl);
                lit = new Literal();
                lit.Text = "<span class=\"inputs\">";
                holderCountries.Controls.Add(lit);
                txt = new TextBox();
                txt.ID = "txtaccountcode" + country.CountryId;
                txt.MaxLength = 50;
                if (subcat != null)
                {
                    countrysubcat = subcat.getCountry(country.CountryId);
                    if (countrysubcat != null)
                    {
                        txt.Text = countrysubcat.accountcode;
                    }
                }
                holderCountries.Controls.Add(txt);
                lit = new Literal();
                lit.Text =
                    "</span><span class=\"inputicon\"></span><span class=\"inputtooltipfield\"></span><span class=\"inputvalidatorfield\"></span>";
                holderCountries.Controls.Add(lit);
                if (!newdiv)
                {
                    lit = new Literal();
                    lit.Text = "</div>";
                    holderCountries.Controls.Add(lit);

                }
                newdiv = !newdiv;
            }
            if (!newdiv)
            {
                lit = new Literal();
                lit.Text = "</div>";
                holderCountries.Controls.Add(lit);
            }
        }


        public string getCountryList()
        {
            StringBuilder output = new StringBuilder();

            output.Append("var lstCountries = new Array();\n");
            CurrentUser user = cMisc.GetCurrentUser();
            cCountries clscountries = new cCountries(user.AccountID, user.CurrentSubAccountId);
            List<int> countries = clscountries.getCountryIds();
            foreach (int i in countries)
            {
                output.Append("lstCountries.push(" + i + ");\n");
            }
            return output.ToString();
        }

        public string getUDFList()
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cUserdefinedFields clsudfs = new cUserdefinedFields(user.AccountID);
            List<cUserDefinedField> fields = clsudfs.GetSpecificUserDefinedFields();
            List<int> udfs = new List<int>();

            StringBuilder output = new StringBuilder();
            output.Append("var lstUDFs = new Array();\n");
            foreach (cUserDefinedField field in fields)
            {
                output.Append("lstUDFs.push(" + field.userdefineid + ");\n");
            }
            return output.ToString();
        }
    }
}
