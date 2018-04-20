using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SpendManagementLibrary;
using System.Text;
using System.Web.Services;
using SpendManagementLibrary.Helpers;


namespace Spend_Management
{
    public partial class supplier_detailsPage : System.Web.UI.Page
    {
        public string mdlContact;
        public string cntlContactGrid;
        public string cntlAttList;
        public int udfCount;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Init(object sender, EventArgs e)
        {
            string script = "";
            if (Request.QueryString["t"] != null)
            {
                script = "setTimeout(setView, 0); ";
            }
            //if (Request.QueryString["e"] != null)
            //{
            //    script += "setTimeout(showDuplicateErrorMessage, 0);";
            //}

            ScriptManager.RegisterStartupScript(this, this.GetType(), "initScreen", "Sys.Application.add_load(function() { " + script + " });", true);
        }

        /// <summary>
        /// Page Load event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();

            cAccountSubAccounts subaccs = new cAccountSubAccounts(curUser.AccountID);
            cAccountProperties properties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties;

            curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SupplierDetails, true, true);
            
            lblLinkContactDetails.Visible = false;
            lblLinkSupplierContracts.Visible = false;

            if (!this.IsPostBack)
            {
                string header = properties.SupplierPrimaryTitle + " Details";
                Master.PageSubTitle = "General Details";
                //Master.enablenavigation = false;
                litSupplierText3.Text = properties.SupplierPrimaryTitle;

                // hide fields on new supplier
                lnkTaskSummary.Visible = false;
                lnkSupplierNotes.Visible = false;
                lnkAddTask.Visible = false;
                lnkNewContact.Visible = false;

                int view = 0;
                if (Request.QueryString["t"] != null)
                {
                    int.TryParse(Request.QueryString["t"], out view);
                }
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "setview", "var activeView = " + view.ToString() + "; ", true);

                int redir;
                if (int.TryParse(Request.QueryString["redir"], out redir))
                {
                    ViewState["redir"] = redir;
                }

                if (Request.QueryString["sid"] != null)
                {
                    cSuppliers suppliers = new cSuppliers(curUser.AccountID, curUser.CurrentSubAccountId);
                    int supplierId = 0;
                    int.TryParse(Request.QueryString["sid"], out supplierId);
                    if (supplierId > 0)
                    {
                        switch (curUser.CurrentActiveModule)
                        {
                            case Modules.contracts:
                                Master.helpid = 1060;
                                break;
                            default:
                                Master.helpid = 0;
                                break;
                        }
                        cSupplier supplier = suppliers.getSupplierById(supplierId);
                        if (supplier == null)
                        {
                            Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                        }
                        Title = properties.SupplierPrimaryTitle + ": " + supplier.SupplierName;
                        Session["NoteReturnURL"] = "shared/supplier_details.aspx?sid=" + supplierId.ToString();

                        if (curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.ContractDetails, true))
                        {
                            bool denyContractAdd = false;
                            if (supplier.SupplierStatus != null)
                            {
                                denyContractAdd = supplier.SupplierStatus.DenyContractAdd;
                            }

                            if (!denyContractAdd)
                            {
                                litAddContract.Text = "<a class=\"submenuitem\" onmouseover=\"window.status='Open New Contract screen';return true;\" onmouseout=\"window.status='Done';\" href=\"../ContractSummary.aspx?id=0&action=add&tab=0&supplierid=" + supplier.SupplierId.ToString() + "\">Add Contract</a>";
                            }
                            else
                            {
                                litAddContract.Text = "<a class=\"submenuitem\" onmouseover=\"window.status='Add New Contract denied due to " + properties.SupplierPrimaryTitle + " status';return true;\" onmouseout=\"window.status='Done';\" href=\"javascript:alert('Adding of a New Contract is denied due to " + properties.SupplierPrimaryTitle + " Status');\">Add Contract</a>";
                            }
                        }

                        imgContactEmail.Attributes.Add("onclick", "launchSupplierEmail('" + txtemail.ClientID + "');");

                        lnkAddTask.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Tasks, true);
                        lnkAddTask.ToolTip = "Create a task associated with the current " + properties.SupplierPrimaryTitle;
                        lnkAddTask.Attributes.Add("onmouseover", "window.status='Create a task associated with the current " + properties.SupplierPrimaryTitle + "';return true;");
                        lnkAddTask.Attributes.Add("onmouseout", "window.status='Done';");

                        lnkTaskSummary.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Tasks, true);

                        lnkNewContact.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.SupplierContacts, true);
                        lnkSupplierNotes.Visible = curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SupplierNotes, true);
                    }
                    else
                    {
                        Title = properties.SupplierPrimaryTitle + ": New";

                        switch (curUser.CurrentActiveModule)
                        {
                            case Modules.contracts:
                                Master.helpid = 1059;
                                break;
                            default:
                                Master.helpid = 0;
                                break;
                        }
                    }

                    SupplierId = supplierId;
                }

                AccountId = curUser.AccountID;
                UserId = curUser.EmployeeID;
                SubAccountID = curUser.CurrentSubAccountId;

                ViewState["title"] = Title;
                

                RenderSupplierDetails();

                //renderUDFs();

                lblpcounty.Text = properties.SupplierRegionTitle;
                lblbcounty.Text = properties.SupplierRegionTitle;

            }
            else
            {
                RenderSupplierDetails();
            }

            if (ViewState["title"] != null)
            {
                Master.title = ViewState["title"].ToString();
            }

            #region buttons
            // buttons
            bool showOkbtn = true;

            if (SupplierId > 0)
            {
                showOkbtn = curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.SupplierDetails, true);
            }
            else
            {
                showOkbtn = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.SupplierDetails, true);
            }

            if (showOkbtn)
            {
                ImageButton cmdOK = new ImageButton();
                cmdOK.ID = "cmdSDOK";
                cmdOK.ImageUrl = "~/shared/images/buttons/btn_save.png";
                cmdOK.Click += new ImageClickEventHandler(cmdOK_Click);
                cmdOK.OnClientClick = "validateform('sdetails');";
                cmdOK.ToolTip = "Update supplier details";
                cmdOK.ValidationGroup = "sdetails";
                cmdOK.CausesValidation = false;
                pnlButtons.Controls.Add(cmdOK);

                Literal spaces = new Literal();
                spaces.Text = "&nbsp;&nbsp;";
                pnlButtons.Controls.Add(spaces);
            }

            ImageButton cmdCancel = new ImageButton();
            cmdCancel.ID = "cmdSDCancel";
            cmdCancel.ImageUrl = "~/shared/images/buttons/cancel_up.gif";
            cmdCancel.ToolTip = "Abort and exit " + properties.SupplierPrimaryTitle + " details";
            cmdCancel.CausesValidation = false;
            cmdCancel.Click += new ImageClickEventHandler(cmdCancel_Click);
            pnlButtons.Controls.Add(cmdCancel);
            // buttons
            #endregion buttons

            if (SupplierId > 0)
            {
                if (curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SupplierContacts, true, false))
                {
                    lblLinkContactDetails.Visible = true;
                    RenderContacts();
                    if (curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractDetails, true))
                    {
                        lblLinkSupplierContracts.Visible = true;
                        GetSupplierContracts();
                    }
                }
            }

            int curTab = 0;
            if (Request.QueryString["tab"] != null)
            {
                curTab = int.Parse(Request.QueryString["tab"]);
            }

            supplierTabs.ActiveTabIndex = curTab;

            //
            // end of user control
            //

        }

        /// <summary>
        /// Link click event to add a new supplier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkNew_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/shared/supplier_details.aspx?sid=0", true);
        }

        /// <summary>
        /// Link click event to add a new note
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkSupplierNotes_Click(object sender, EventArgs e)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            cSuppliers suppliers = new cSuppliers(curUser.AccountID, curUser.CurrentSubAccountId);
            cSupplier supplier = suppliers.getSupplierById(SupplierId);
            Response.Redirect("~/ViewNotes.aspx?notetype=Supplier&supplierid=" + supplier.SupplierId.ToString() + "&id=-1&item=" + supplier.SupplierName + "&ret=" + (cMisc.Path + "/shared/supplier_details.aspx?t=0&sid=" + SupplierId.ToString()).Base64Encode(), true);
        }

        /// <summary>
        /// Link to add a task for the supplier record
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkAddTask_Click(object sender, EventArgs e)
        {
            string varURL = "tid=0&rid=" + SupplierId.ToString() + "&rtid=" + ((int)AppliesTo.VendorDetails).ToString() + "&ret=" + (cMisc.Path + "/shared/supplier_details.aspx?t=0&sid=" + SupplierId.ToString()).Base64Encode();
            Response.Redirect(cMisc.Path + "/shared/tasks/ViewTask.aspx?" + varURL, true);
        }

        /// <summary>
        /// Direct link to the Task Summary
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkTaskSummary_Click(object sender, EventArgs e)
        {
            Response.Redirect(cMisc.Path + "/shared/tasks/TaskSummary.aspx?paa=" + ((int)AppliesTo.VendorDetails).ToString() + "&pid=" + SupplierId.ToString() + "&ret=" + (Request.Url.PathAndQuery).Base64Encode(), true);
        }

        /// <summary>
        /// Method to render the supplier details fields to the screen
        /// </summary>               
        private void RenderSupplierDetails()
        {
            cSuppliers suppliers = new cSuppliers(AccountId, SubAccountID);
            cAccountSubAccounts subaccs = new cAccountSubAccounts(AccountId);
            cAccountProperties properties = subaccs.getSubAccountById(SubAccountID).SubAccountProperties;
            CurrentUser curUser = cMisc.GetCurrentUser();

            cSupplier curSupplier;
            if (SupplierId > 0)
            {
                curSupplier = suppliers.getSupplierById(SupplierId);
                ViewState["record"] = curSupplier.userdefined;
            }
            else
            {
                curSupplier = new cSupplier(0, SubAccountID, "", null, null, "", new cAddress(0, "", "", "", "", "", "", 0, "", "", false, DateTime.Now, UserId, DateTime.Now, UserId), "", 1, null, null, 0, 0, null, new SortedList<int, object>(), null, string.Empty, string.Empty, false, false);
                ViewState["record"] = null;
            }

            if (curSupplier == null)
            {
                Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
            }

            cCountries countries = new cCountries(curUser.AccountID, curUser.CurrentSubAccountId);
            cCurrencies currencies = new cCurrencies(curUser.AccountID, curUser.CurrentSubAccountId);

            StringBuilder str = new StringBuilder();

            #region div
            Literal lit = new Literal();
            lit.Text = "<div class=\"twocolumn\">\n";
            SDplaceholder.Controls.Add(lit);
            #endregion div

            GetCharCell(ref SDplaceholder, "suppliername", properties.SupplierPrimaryTitle + " Name", curSupplier.SupplierName, 150, "S", true, 1, "sdetails");

            cBaseDefinitions clsBaseDefs = null;

            int sstatus = 0;
            if (curSupplier.SupplierStatus != null)
            {
                sstatus = curSupplier.SupplierStatus.ID;
            }

            clsBaseDefs = new cBaseDefinitions(AccountId, SubAccountID, SpendManagementElement.SupplierStatus);
            GetListCell(ref SDplaceholder, "supplierstatus", "Status", clsBaseDefs.CreateDropDown(true, sstatus), sstatus, 8, "sdetails", "9E75D92D-C8F8-4A17-A7BD-C4A7E406C94C", properties.SupplierStatusEnforced);

            #region div
            lit = new Literal();
            lit.Text = "</div><div class=\"twocolumn\">\n";
            SDplaceholder.Controls.Add(lit);
            #endregion div

            GetCharCell(ref SDplaceholder, "suppliercode", properties.SupplierPrimaryTitle + " Code", curSupplier.SupplierCode, 50, "S", false, 2, "sdetails");

            int scat = 0;
            if (curSupplier.SupplierCategory != null)
            {
                scat = curSupplier.SupplierCategory.ID;
            }

            clsBaseDefs = new cBaseDefinitions(AccountId, SubAccountID, SpendManagementElement.SupplierCategory);
            GetListCell(ref SDplaceholder, "suppliercategory", properties.SupplierCatTitle, clsBaseDefs.CreateDropDown(true, scat), scat, 9, "sdetails", "05F1E205-7108-45DE-BD71-6EAD2673A434", properties.SupplierCatMandatory);


            #region div
            lit = new Literal();
            lit.Text = "</div><div class=\"twocolumn\">\n";
            SDplaceholder.Controls.Add(lit);
            #endregion div

            int currencyid = 0;
            if (curSupplier.TurnoverCurrencyId.HasValue)
            {
                currencyid = curSupplier.TurnoverCurrencyId.Value;
            }
            GetListCell(ref SDplaceholder, "currency", properties.SupplierPrimaryTitle + " Currency", currencies.CreateDropDown(), currencyid, 3, "sdetails");

            if (properties.SupplierTurnoverEnabled)
            {
                GetCharCell(ref SDplaceholder, "turnover", "Turnover", curSupplier.AnnualTurnover.ToString(), 12, "F", false, 10, "sdetails");
            }

            #region div
            lit = new Literal();
            lit.Text = "</div><div class=\"twocolumn\">\n";
            SDplaceholder.Controls.Add(lit);
            #endregion div

            cBaseDefinitions clsBaseDefinitions = new cBaseDefinitions(AccountId, SubAccountID, SpendManagementElement.FinancialStatus);
            int finstatusid = 0;
            if (curSupplier.LastFinancialStatus != null)
            {
                finstatusid = curSupplier.LastFinancialStatus.ID;
            }
            if (properties.SupplierLastFinStatusEnabled)
            {
                GetListCell(ref SDplaceholder, "financialstatus", "Financial Status", clsBaseDefinitions.CreateDropDown(true, finstatusid), finstatusid, 4, "sdetails", "612D8546-BC36-433D-9C6D-79D876329124");
            }

            if (properties.SupplierLastFinCheckEnabled)
            {
                string lastchkdate = string.Empty;
                if (curSupplier.CreditRefChecked.HasValue)
                {
                    lastchkdate = curSupplier.CreditRefChecked.Value.ToShortDateString();
                }
                GetCharCell(ref SDplaceholder, "financialstatuschecked", "Status Last Checked", lastchkdate, 10, "D", false, 11, "sdetails");
            }

            #region div
            lit = new Literal();
            lit.Text = "</div><div class=\"twocolumn\">\n";
            SDplaceholder.Controls.Add(lit);
            #endregion div

            if (properties.SupplierFYEEnabled)
            {
                GetListCell(ref SDplaceholder, "financialye", "Financial Year End", getMonths(), curSupplier.FinancialYearEnd, 5, "sdetails");
            }

            if (properties.SupplierIntContactEnabled)
            {
                GetCharCell(ref SDplaceholder, "internalcontact", "Internal Contact", curSupplier.InternalContact, 250, "S", false, 12, "sdetails");
            }

            #region div
            lit = new Literal();
            lit.Text = "</div><div class=\"twocolumn\">\n";
            SDplaceholder.Controls.Add(lit);
            #endregion div

            getSupplierAttachmentList(ref SDplaceholder, "SDAttachments", "Attachments", 6, "sdetails");

            if (properties.SupplierNumEmployeesEnabled)
            {
                GetCharCell(ref SDplaceholder, "numemployees", "No. Employees", curSupplier.NumberOfEmployees.ToString(), 9, "N", false, 13, "sdetails");
            }


            #region div
            lit = new Literal();
            lit.Text = "</div><div class=\"twocolumn\">\n";
            SDplaceholder.Controls.Add(lit);
            #endregion div

            GetCheckboxCell(ref SDplaceholder, "issupplier", "Is Supplier", curSupplier.IsSupplier, 7, "sdetails");

            GetCheckboxCell(ref SDplaceholder, "isreseller", "Is Reseller", curSupplier.IsReseller, 14, "sdetails");

            #region div
            lit = new Literal();
            lit.Text = "</div><div class=\"sectiontitle\">" + properties.SupplierPrimaryTitle + " Contact Details</div><div class=\"twocolumn\">\n";
            SDplaceholder.Controls.Add(lit);
            #endregion div

            GetCharCell(ref SDplaceholder, "switchboard", "Switchboard", curSupplier.PrimaryAddress.Switchboard, 30, "S", false, 15, "sdetails");

            GetCharCell(ref SDplaceholder, "supplieremail", properties.SupplierPrimaryTitle + " Email", curSupplier.SupplierEmail, 300, "E", false, 21, "sdetails");
            
            #region div
            lit = new Literal();
            lit.Text = "</div><div class=\"twocolumn\">\n";
            SDplaceholder.Controls.Add(lit);
            #endregion div

            GetCharCell(ref SDplaceholder, "fax", "Fax", curSupplier.PrimaryAddress.Fax, 30, "S", false, 16, "sdetails");

            GetCharCell(ref SDplaceholder, "weburl", "Web Address", curSupplier.WebURL, 250, "W", false, 18, "sdetails", "346F84FE-EAA7-4575-A39C-75034B32930D");

            #region div
            lit = new Literal();
            lit.Text = "</div><div class=\"sectiontitle\">" + properties.SupplierPrimaryTitle + " Address Details</div><div class=\"twocolumn\">\n";
            SDplaceholder.Controls.Add(lit);
            #endregion div

			GetCharCell(ref SDplaceholder, "addresstitle", "Address Title", curSupplier.PrimaryAddress.AddressTitle, 250, "S", false, 17, "sdetails");

            GetCharCell(ref SDplaceholder, "county", properties.SupplierRegionTitle, curSupplier.PrimaryAddress.County, 150, "S", false, 23, "sdetails");

            #region div
            lit = new Literal();
            lit.Text = "</div><div class=\"twocolumn\">\n";
            SDplaceholder.Controls.Add(lit);
            #endregion div

            GetCharCell(ref SDplaceholder, "address1", "Address Line 1", curSupplier.PrimaryAddress.AddressLine1, 150, "S", false, 18, "sdetails");

            GetCharCell(ref SDplaceholder, "postcode", "Postcode", curSupplier.PrimaryAddress.PostCode, 10, "S", false, 24, "sdetails");

            #region div
            lit = new Literal();
            lit.Text = "</div><div class=\"twocolumn\">\n";
            SDplaceholder.Controls.Add(lit);
            #endregion div

            GetCharCell(ref SDplaceholder, "address2", "Address Line 2", curSupplier.PrimaryAddress.AddressLine2, 150, "S", false, 19, "sdetails");

            GetListCell(ref SDplaceholder, "country", "Country", countries.CreateDropDown(), curSupplier.PrimaryAddress.CountryId, 25, "sdetails");

            #region div
            lit = new Literal();
            lit.Text = "</div><div class=\"twocolumn\">\n";
            SDplaceholder.Controls.Add(lit);
            #endregion div

            GetCharCell(ref SDplaceholder, "town", "City", curSupplier.PrimaryAddress.Town, 150, "S", false, 20, "sdetails");

            #region div
            lit = new Literal();
            lit.Text = "</div>";
            SDplaceholder.Controls.Add(lit);
            #endregion div


            return;
        }

        /// <summary>
        /// Create the user defined field panel fot the additional details
        /// </summary>
        private void renderUDFs()
        {
            cUserdefinedFields ufields = new cUserdefinedFields(AccountId);
            cTables tables = new cTables(AccountId);
            cTable sdtable = tables.GetTableByName("supplier_details");

            hiddenSupplierId.Value = SupplierId.ToString();

            int category = -1;


            if (hdnSupCatID.Value != "-1")
            {
                int.TryParse(hdnSupCatID.Value, out category);
            }
            else
            {
                DropDownList tmpLst = (DropDownList)SDetailsTab.FindControl("suppliercategory");
                if (tmpLst != null)
                {
                    int.TryParse(tmpLst.SelectedItem.Value, out category);
                }
            }

            StringBuilder udfscript;
            phSDUserFields.EnableViewState = false;
            ufields.createFieldPanel(ref phSDUserFields, tables.GetTableByID(sdtable.UserDefinedTableID), "sdetails", out udfscript, category, tooltipOnHover: true, groupType: GroupingOutputType.UnGroupedOnly, outJavascriptIncludesAutocompleteBindingsOnly: true);
            StringBuilder udfScriptTwo;
            ufields.createFieldPanel(ref phSAUserFields, tables.GetTableByID(sdtable.UserDefinedTableID), "sdetails", out udfScriptTwo, category, tooltipOnHover: true, groupType: GroupingOutputType.GroupedOnly, outJavascriptIncludesAutocompleteBindingsOnly: true);

            if (udfscript.Length > 0 || udfScriptTwo.Length > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "supplierUdfs", udfscript.Append(udfScriptTwo.ToString()).ToString(), true);
            }

            udfCount = phSAUserFields.Controls.Count;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "hideSDAdditionTab", "checkUDF(" + udfCount + ", '" + SDAdditionalTab.ClientID + "');", true);

            if (ViewState["record"] != null)
            {
                ufields.populateRecordDetails(ref phSDUserFields, tables.GetTableByID(sdtable.UserDefinedTableID), (SortedList<int, object>)ViewState["record"], groupType: GroupingOutputType.UnGroupedOnly);
                ufields.populateRecordDetails(ref phSAUserFields, tables.GetTableByID(sdtable.UserDefinedTableID), (SortedList<int, object>)ViewState["record"], groupType: GroupingOutputType.GroupedOnly);
            }
        }

        /// <summary>
        /// Method to render the supplier contact fields into the modal
        /// </summary>
        private void RenderContacts()
        {
            cSupplierContacts contacts = new cSupplierContacts(AccountId, SupplierId);
            cFields fields = new cFields(AccountId);
            CurrentUser curUser = cMisc.GetCurrentUser();
            cUserdefinedFields userdefined = new cUserdefinedFields(AccountId);

            // contactid, supplierid, contactname, position, email, mobile, business_addressid, home_addressid, comments
            cGridNew contactGrid = new cGridNew(AccountId, UserId, "contactgrid", contacts.getGridSQL);

            Literal litGrid = new Literal();
            contactGrid.enablearchiving = false;
            contactGrid.enabledeleting = curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.SupplierContacts, false, false);
            if (contactGrid.enabledeleting)
            {
                contactGrid.deletelink = "javascript:if(confirm('Click OK to confirm deletion')){deleteContact({contactid});}";
            }
            else
            {
                contactGrid.deletelink = "";
            }

            contactGrid.enableupdating = curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.SupplierContacts, false, false);

            if (contactGrid.enableupdating)
            {
                contactGrid.editlink = "javascript:editContact({contactid})";
            }
            else
            {
                contactGrid.editlink = "";
            }
            contactGrid.addEventColumn("viewnote", "images/icons/16/plain/folder_closed.png", "../ViewNotes.aspx?item={contactname}&notetype=SupplierContact&id=-1&contactid={contactid}&ret=" + (cMisc.Path + "/shared/supplier_details.aspx?t=1&sid=" + SupplierId.ToString()).Base64Encode(), "View Contact Notes", "Access contact notes");
            contactGrid.getColumnByName("contactid").hidden = true;
            contactGrid.SortedColumn = contactGrid.getColumnByName("contactname");
            contactGrid.CssClass = "datatbl";
            //contactGrid.getColumnByName("archived").hidden = true;
            contactGrid.KeyField = "contactid";
            contactGrid.EmptyText = "No contacts currently defined";

            ((cFieldColumn)contactGrid.getColumnByName("main_contact")).addValueListItem(0, "&nbsp;");
            ((cFieldColumn)contactGrid.getColumnByName("main_contact")).addValueListItem(1, "Yes");

            //contactGrid.addEventColumn(cMisc.path + "/shared/images/icons/16/plain/mail_earth.png", "", "Email Contact", "Email Contact");

            contactGrid.addFilter(fields.GetFieldByID(new Guid(ReportKeyFields.SupplierDetails_SupplierId)), ConditionType.Equals, new object[] { SupplierId }, new object[] { }, ConditionJoiner.None);


            string[] contactGridData = contactGrid.generateGrid();
            litGrid.Text = contactGridData[1];

            // set the sel.grid javascript variables
            Page.ClientScript.RegisterStartupScript(this.GetType(), "contactsGridVars", cGridNew.generateJS_init("contactsGridVars", new List<string>() { contactGridData[0] }, curUser.CurrentActiveModule), true);

            SCplaceholder.Controls.Add(litGrid);

            cntlContactGrid = contactGrid.GridID;

            cCountries countries = new cCountries(curUser.AccountID, curUser.CurrentSubAccountId);
            lstbcountry.Items.Clear();
            lstbcountry.Items.AddRange(countries.CreateDropDown().ToArray());

            lstpcountry.Items.Clear();
            lstpcountry.Items.AddRange(countries.CreateDropDown().ToArray());

            StringBuilder sbJavascript = new StringBuilder();
            cTables tables = new cTables(AccountId);
            cTable scontactTable = tables.GetTableByName("supplier_contacts");
            userdefined.createFieldPanel(ref phSCUserFields, tables.GetTableByID(scontactTable.UserDefinedTableID), "scontacts", out sbJavascript, tooltipOnHover: true);
            Page.ClientScript.RegisterStartupScript(this.GetType(), "udfs", sbJavascript.ToString(), true);
            return;
        }

        void cmdSCCancel_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/shared/supplier_details.aspx", true);
        }

        void cmdCancel_Click(object sender, ImageClickEventArgs e)
        {
            ViewState["supplierid"] = null;

            int contractID, redir;
            if (ViewState["redir"] != null && int.TryParse(ViewState["redir"].ToString(), out redir) && Session["ActiveContract"] != null && int.TryParse(Session["ActiveContract"].ToString(), out contractID))
            {
                ViewState["redir"] = null;
                if (redir == 1)
                {
                    Response.Redirect("~/ContractSummary.aspx?id=" + contractID, true);
                }
                else
                {
                    Response.Redirect("~/shared/suppliers.aspx", true);
                }
            }
            else
            {
                Response.Redirect("~/shared/suppliers.aspx", true);
            }
        }

        void cmdOK_Click(object sender, ImageClickEventArgs e)
        {
            TextBox tmpTxt;
            DropDownList tmpLst;
            CheckBox tmpChk;

            // ensure supplier not changed via ajax
            if (hiddenSupplierId.Value != SupplierId.ToString())
            {
                if (hiddenSupplierId.Value == "")
                {
                    hiddenSupplierId.Value = SupplierId.ToString();
                }
                else
                {
                    SupplierId = int.Parse(hiddenSupplierId.Value);
                }
            }

            // adding a new supplier
            string suppliername = "";
            tmpTxt = (TextBox)SDetailsTab.FindControl("suppliername");
            if (tmpTxt != null)
            {
                suppliername = tmpTxt.Text;
            }
            int status = 0;
            tmpLst = (DropDownList)SDetailsTab.FindControl("supplierstatus");
            if (tmpLst != null)
            {
                status = int.Parse(tmpLst.SelectedItem.Value);
            }
            string addr_title = "";
            tmpTxt = (TextBox)SDetailsTab.FindControl("addresstitle");
            if (tmpTxt != null)
            {
                addr_title = tmpTxt.Text;
            }
            int category = 0;
            tmpLst = (DropDownList)SDetailsTab.FindControl("suppliercategory");
            if (tmpLst != null)
            {
                category = int.Parse(tmpLst.SelectedItem.Value);
            }
            string addr1 = "";
            tmpTxt = (TextBox)SDetailsTab.FindControl("address1");
            if (tmpTxt != null)
            {
                addr1 = tmpTxt.Text;
            }
            string addr2 = "";
            tmpTxt = (TextBox)SDetailsTab.FindControl("address2");
            if (tmpTxt != null)
            {
                addr2 = tmpTxt.Text;
            }
            string town = "";
            tmpTxt = (TextBox)SDetailsTab.FindControl("town");
            if (tmpTxt != null)
            {
                town = tmpTxt.Text;
            }
            string postcode = "";
            tmpTxt = (TextBox)SDetailsTab.FindControl("postcode");
            if (tmpTxt != null)
            {
                postcode = tmpTxt.Text;
            }
            string county = "";
            tmpTxt = (TextBox)SDetailsTab.FindControl("county");
            if (tmpTxt != null)
            {
                county = tmpTxt.Text;
            }
            int numemployees = 0;
            tmpTxt = (TextBox)SDetailsTab.FindControl("numemployees");
            if (tmpTxt != null)
            {
                int tmpInt;
                if (int.TryParse(tmpTxt.Text, out tmpInt))
                {
                    numemployees = int.Parse(tmpTxt.Text);
                }
            }
            string switchboard = "";
            tmpTxt = (TextBox)SDetailsTab.FindControl("switchboard");
            if (tmpTxt != null)
            {
                switchboard = tmpTxt.Text;
            }
            string fax = "";
            tmpTxt = (TextBox)SDetailsTab.FindControl("fax");
            if (tmpTxt != null)
            {
                fax = tmpTxt.Text;
            }
            short financialye = 0;
            tmpLst = (DropDownList)SDetailsTab.FindControl("financialye");
            if (tmpLst != null)
            {
                short tmpShort;
                if (short.TryParse(tmpLst.SelectedItem.Value, out tmpShort))
                {
                    financialye = short.Parse(tmpLst.SelectedItem.Value);
                }
            }
            double turnover = 0;
            tmpTxt = (TextBox)SDetailsTab.FindControl("turnover");
            if (tmpTxt != null)
            {
                double tmpDouble;
                if (double.TryParse(tmpTxt.Text, out tmpDouble))
                {
                    turnover = double.Parse(tmpTxt.Text);
                }
            }
            string weburl = "";
            tmpTxt = (TextBox)SDetailsTab.FindControl("weburl");
            if (tmpTxt != null)
            {
                weburl = tmpTxt.Text;
            }
            int? currency = null;
            tmpLst = (DropDownList)SDetailsTab.FindControl("currency");
            if (tmpLst != null)
            {
                currency = int.Parse(tmpLst.SelectedItem.Value);
            }
            int country = 0;
            tmpLst = (DropDownList)SDetailsTab.FindControl("country");
            if (tmpLst != null)
            {
                country = int.Parse(tmpLst.SelectedItem.Value);
            }
            int financialstatus = 0;
            tmpLst = (DropDownList)SDetailsTab.FindControl("financialstatus");
            if (tmpLst != null)
            {
                financialstatus = int.Parse(tmpLst.SelectedItem.Value);
            }
            string suppliercode = "";
            tmpTxt = (TextBox)SDetailsTab.FindControl("suppliercode");
            if (tmpTxt != null)
            {
                suppliercode = tmpTxt.Text;
            }
            string suppEmail = "";
            tmpTxt = (TextBox)SDetailsTab.FindControl("supplieremail");
            if (tmpTxt != null)
            {
                suppEmail = tmpTxt.Text;
            }
            string intContact = "";
            tmpTxt = (TextBox)SDetailsTab.FindControl("internalcontact");
            if (tmpTxt != null)
            {
                intContact = tmpTxt.Text;
            }
            DateTime? credRefChecked = null;
            tmpTxt = (TextBox)SDetailsTab.FindControl("financialstatuschecked");
            if (tmpTxt != null)
            {
                if (string.IsNullOrEmpty(tmpTxt.Text) == false)
                {
                    DateTime tmpdt;
                    if (DateTime.TryParse(tmpTxt.Text, out tmpdt))
                    {
                        credRefChecked = tmpdt;
                    }
                }
            }
            bool issupp = false;
            tmpChk = (CheckBox)SDetailsTab.FindControl("issupplier");
            if (tmpChk != null)
            {
                issupp = tmpChk.Checked;
            }

            bool isresell = false;
            tmpChk = (CheckBox)SDetailsTab.FindControl("isreseller");
            if (tmpChk != null)
            {
                isresell = tmpChk.Checked;
            }

            cUserdefinedFields ufields = new cUserdefinedFields(AccountId);
            cTables tables = new cTables(AccountId);
            cTable sdtable = tables.GetTableByName("supplier_details");
            SortedList<int, object> udfSD = ufields.getItemsFromPanel(ref phSDUserFields, tables.GetTableByID(sdtable.UserDefinedTableID), groupType: GroupingOutputType.UnGroupedOnly);
            SortedList<int, object> udfSA = ufields.getItemsFromPanel(ref phSAUserFields, tables.GetTableByID(sdtable.UserDefinedTableID), groupType: GroupingOutputType.GroupedOnly);

            cSuppliers suppliers = new cSuppliers(AccountId, SubAccountID);
            cSupplierAddresses addresses = new cSupplierAddresses(AccountId);
            Dictionary<string, cSupplierContact> contacts = null;
            cBaseDefinitions clsBaseDefinitions = null;

            int addressid = 0;

            if (SupplierId > 0)
            {
                cSupplier curSupplier = suppliers.getSupplierById(SupplierId);
                addressid = curSupplier.PrimaryAddress.AddressId;
                contacts = curSupplier.SupplierContacts;
            }
            else
            {
                contacts = new Dictionary<string, cSupplierContact>();
            }

            cAddress newaddress = new cAddress(addressid, addr_title, addr1, addr2, town, county, postcode, country, switchboard, fax, false, DateTime.Now, UserId, DateTime.Now, UserId);

            cFinancialStatus newFinStatus = null;
            if (financialstatus > 0)
            {
                clsBaseDefinitions = new cBaseDefinitions(AccountId, SubAccountID, SpendManagementElement.FinancialStatus);
                newFinStatus = (cFinancialStatus)clsBaseDefinitions.GetDefinitionByID(financialstatus);
            }

            cSupplierStatus newStatus = null;
            if (status > 0)
            {
                clsBaseDefinitions = new cBaseDefinitions(AccountId, SubAccountID, SpendManagementElement.SupplierStatus);
                newStatus = (cSupplierStatus)clsBaseDefinitions.GetDefinitionByID(status);
            }

            cSupplierCategory newCategory = null;
            if (category > 0)
            {
                clsBaseDefinitions = new cBaseDefinitions(AccountId, SubAccountID, SpendManagementElement.SupplierCategory);
                newCategory = (cSupplierCategory)clsBaseDefinitions.GetDefinitionByID(category);
            }

            // need to merge the grouped and ungrouped udf values into a single set, so add the ungrouped values into the fields from the grouped.
            foreach (KeyValuePair<int, object> kvp in udfSD)
            {
                udfSA.Add(kvp.Key, kvp.Value);
            }
            
            cSupplier newsupplier = new cSupplier(SupplierId, SubAccountID, suppliername, newStatus, newCategory, suppliercode, newaddress, weburl, financialye, newFinStatus, currency, turnover, numemployees, contacts, udfSA, credRefChecked, intContact, suppEmail, issupp, isresell);
            int newSupplierId = suppliers.UpdateSupplier(newsupplier);

            if (newSupplierId > 0)
            {
                Response.Redirect("~/shared/suppliers.aspx");
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "errorModal", "Sys.Application.add_load(function() { setTimeout(showDuplicateErrorMessage,0); });", true);
            }
        }

        /// <summary>
        /// Function used for autocomplete
        /// </summary>
        public static void gotSupplier()
        {
            HttpApplication appinfo = (HttpApplication)HttpContext.Current.ApplicationInstance;
            svcAutoComplete acservice = new Spend_Management.svcAutoComplete();
        }

        /// <summary>
        /// Populate a grid to display contracts held with the current supplier
        /// </summary>
        private void GetSupplierContracts()
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            cAccountSubAccounts subaccs = new cAccountSubAccounts(AccountId);
            cAccountProperties properties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties;
            cSuppliers suppliers = new cSuppliers(curUser.AccountID, curUser.CurrentSubAccountId);
            cSupplier curSupplier = suppliers.getSupplierById(SupplierId);
            DBConnection db = new DBConnection(cAccounts.getConnectionString(curUser.AccountID));

            cFields fields = new cFields(AccountId);
            cTables tables = new cTables(AccountId);
            Guid contractTableID = tables.GetTableByName("contract_details").TableID;

            int deniedContractCount = 0;
            string sql = "select count(*) from contract_details where supplierId = @supplierId and dbo.CheckContractAccess(@employeeId,contractId, @subAccountId) = 0";
            db.sqlexecute.Parameters.AddWithValue("@supplierId", SupplierId);
            db.sqlexecute.Parameters.AddWithValue("@employeeId", curUser.EmployeeID);
            db.sqlexecute.Parameters.AddWithValue("@subAccountId", properties.SubAccountID);
            deniedContractCount = db.getcount(sql);

            db.sqlexecute.Parameters.Clear();
            sql = "select contractId, contractKey, contractNumber, contractDescription, endDate, contractValue, contractCurrency, codes_contractstatus.isArchive from dbo.contract_details left join codes_contractstatus on codes_contractstatus.statusId = contract_details.contractStatusId where supplierId = @supplierId and dbo.CheckContractAccess(@employeeId, contractId, @subAccountId) > 0";
            List<cNewGridColumn> columns = new List<cNewGridColumn>();
            columns.Add(new cFieldColumn(fields.GetBy(contractTableID, "contractId")));
            columns.Add(new cFieldColumn(fields.GetBy(contractTableID, "contractKey")));
            columns.Add(new cFieldColumn(fields.GetBy(contractTableID, "contractNumber")));
            columns.Add(new cFieldColumn(fields.GetBy(contractTableID, "contractDescription")));
            columns.Add(new cFieldColumn(fields.GetBy(contractTableID, "endDate")));
            columns.Add(new cFieldColumn(fields.GetBy(contractTableID, "contractValue")));
            columns.Add(new cFieldColumn(fields.GetBy(contractTableID, "contractCurrency")));
            columns.Add(new cFieldColumn(fields.GetBy(tables.GetTableByName("codes_contractstatus").TableID, "isArchive")));

            db.sqlexecute.Parameters.AddWithValue("@supplierId", SupplierId);
            db.sqlexecute.Parameters.AddWithValue("@employeeId", curUser.EmployeeID);
            db.sqlexecute.Parameters.AddWithValue("@subAccountId", properties.SubAccountID);
            System.Data.DataSet ds = db.GetDataSet(sql);

            int contractCount = 0;
            if (ds != null)
            {
                int.TryParse(ds.Tables[0].Rows.Count.ToString(), out contractCount);
            }
            litSupplierContractMsg.Text = "<img align=\"absmiddle\" alt=\"Information\" src=\"" + cMisc.Path + "\\shared\\images\\icons\\about.png\" />&nbsp;" + contractCount.ToString() + " Contracts held, " + deniedContractCount.ToString() + " denied access by audience";

            cGridNew contractsGrid = new cGridNew(curUser.AccountID, curUser.EmployeeID, "supplierContractsGrid", tables.GetTableByName("contract_details"), columns, ds);
            contractsGrid.CurrencyColumnName = "contractCurrency";
            contractsGrid.getColumnByName("contractCurrency").hidden = true;
            contractsGrid.enabledeleting = false;
            contractsGrid.enablearchiving = false;
            contractsGrid.EmptyText = "There are not currently any contracts defined";
            contractsGrid.editlink = "../ContractSummary.aspx?afs=1&id={contractId}&tab=0";
            contractsGrid.enableupdating = curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractDetails, true);
            contractsGrid.getColumnByName("contractId").hidden = true;
            contractsGrid.getColumnByName("contractValue").hidden = !curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ViewFinancials, true);
            contractsGrid.enablepaging = true;
            contractsGrid.CssClass = "datatbl";
            contractsGrid.KeyField = "contractDescription";

            Literal litGrid = new Literal();
            string[] gridData = contractsGrid.generateGrid();
            litGrid.Text = gridData[1];

            // set the sel.grid javascript variables
            Page.ClientScript.RegisterStartupScript(this.GetType(), "contractsGridVars", cGridNew.generateJS_init("contractsGridVars", new List<string>() { gridData[0] }, curUser.CurrentActiveModule), true);
            
            phSupplierContracts.Controls.Add(litGrid);
            return;
        }

        private void getSupplierAttachmentList(ref PlaceHolder targetHolder, string id, string label, short tabIndex, string validationgroup)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            Literal lit = new Literal();

            lit.Text = "<label id=\"lbl" + id + "\" for=\"" + id + "\">" + label + "</label><span class=\"inputs\">";
            targetHolder.Controls.Add(lit);

            DropDownList lst = new DropDownList {ID = id, TabIndex = tabIndex, ValidationGroup = validationgroup, CssClass = "fillspan"};
            lst.Attributes.Add("onchange", "javascript:openAttachment();");
            lst.Items.Add(new ListItem("Please select...", "0"));

            if (SupplierId > 0)
            {
                DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountId));
                // must check for access control on the attachment and also on the parent supplier
                const string sql = "select attachmentId, description, filename from attachments where attachmentarea = @appArea and referenceNumber = @supplierId and dbo.CheckAttachmentAccess(attachmentId, @employeeId) > 0";
                db.sqlexecute.Parameters.AddWithValue("@supplierId", SupplierId);
                db.sqlexecute.Parameters.AddWithValue("@appArea", (int)AttachmentArea.VENDOR);
                db.sqlexecute.Parameters.AddWithValue("@employeeId", curUser.EmployeeID);
                cSecureData crypt = new cSecureData();

                using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(sql))
                {
                    while (reader.Read())
                    {
                        int attId = reader.GetInt32(0);
                        string desc = null, fname = null;
                        if (!reader.IsDBNull(1))
                        {
                            desc = reader.GetString(1);
                        }
                        if (!reader.IsDBNull(2))
                        {
                             fname = reader.GetString(2);
                        }
                        lst.Items.Add(new ListItem((desc == "" ? fname : desc), crypt.Encrypt(attId.ToString())));
                    }
                    reader.Close();
                }
            }
            targetHolder.Controls.Add(lst);
            cntlAttList = lst.ClientID;

            lit = new Literal {Text = "</span><span class=\"inputicon\">"};

            if (SupplierId > 0)
            {
                lit.Text += "<img src=\"images/icons/16/plain/paperclip.png\" alt=\"Attach\" onclick=\"javascript:ShowAttachments('" + SupplierId.ToString() + "');\" />";
            }
            else
            {
                lit.Text += "&nbsp;";
            }
            lit.Text += "</span><span class=\"inputtooltipfield\">&nbsp;</span>";
            lit.Text += "<span class=\"inputvalidatorfield\">&nbsp;</span>";
            targetHolder.Controls.Add(lit);
            return;
        }

        #region FieldCells
        private void GetCharCell(ref PlaceHolder targetHolder, string id, string label, string value, int maxlength, string fieldtype, bool ismandatory, short tabIndex, string validationgroup, string tooltipID = "")
        {
            Literal lit = new Literal();
            lit.Text = "<label id=\"lbl" + id + "\" ";
            if (ismandatory)
            {
                lit.Text += "class=\"mandatory\" ";
            }
            lit.Text += "for=\"" + id + "\">" + label;
            if (ismandatory)
            {
                lit.Text += "*";
            }
            lit.Text += "</label>";
            targetHolder.Controls.Add(lit);

            lit = new Literal();
            lit.Text = "<span class=\"inputs\">";
            targetHolder.Controls.Add(lit);

            TextBox char_cntl = new TextBox();
            char_cntl.ID = id;
            char_cntl.CssClass = "fillspan";
            char_cntl.TabIndex = tabIndex;
            char_cntl.ValidationGroup = validationgroup;

            if (maxlength > 0)
            {
                char_cntl.MaxLength = maxlength;
            }
            if (value != "")
            {
                char_cntl.Text = value;
            }
            targetHolder.Controls.Add(char_cntl);
            lit = new Literal();
            lit.Text = "</span><span class=\"inputicon\">";
            targetHolder.Controls.Add(lit);

            switch (fieldtype)
            {
                case "D":
                    ImageButton calendarImage = new ImageButton();
                    calendarImage.ID = "calImgSupp" + id;
                    calendarImage.ImageUrl = "~/shared/images/icons/cal.gif";
                    calendarImage.CausesValidation = false;
                    targetHolder.Controls.Add(calendarImage);

                    AjaxControlToolkit.CalendarExtender calendarExtender = new AjaxControlToolkit.CalendarExtender();
                    calendarExtender.ID = "calExtSupp" + id;
                    calendarExtender.Format = "dd/MM/yyyy";
                    calendarExtender.PopupButtonID = calendarImage.ID;
                    calendarExtender.TargetControlID = char_cntl.ID;
                    targetHolder.Controls.Add(calendarExtender);
                    break;
                case "W":
                    Image imageWeb = new Image();
                    imageWeb.ImageUrl = "~/shared/images/icons/16/Plain/earth.png";
                    imageWeb.ID = "webImgSupp" + id;
                    imageWeb.Attributes.Add("onclick", "launchSupplierURL('" + char_cntl.ClientID + "');");
                    targetHolder.Controls.Add(imageWeb);
                    break;
                case "E":
                    Image imageEmail = new Image();
                    imageEmail.ImageUrl = "~/shared/images/icons/16/Plain/mail_earth.png";
                    imageEmail.ID = "emailImgSupp" + id;
                    imageEmail.Attributes.Add("onclick", "launchSupplierEmail('" + char_cntl.ClientID + "');");
                    targetHolder.Controls.Add(imageEmail);
                    break;
                default:
                    lit = new Literal();
                    lit.Text = "&nbsp;";
                    targetHolder.Controls.Add(lit);
                    break;
            }


            lit = new Literal();
            lit.Text = "</span><span class=\"inputtooltipfield\">";
            targetHolder.Controls.Add(lit);

            if (!string.IsNullOrEmpty(tooltipID))
            {
                Image tooltipImage = new Image();
                tooltipImage.ID = "imgtooltip" + id;
                tooltipImage.ImageUrl = "~/shared/images/icons/16/plain/tooltip.png";
                tooltipImage.Attributes.Add("onmouseover", "SEL.Tooltip.Show('" + tooltipID + "', 'sm', this);");
                targetHolder.Controls.Add(tooltipImage);
            }

            lit = new Literal();
            lit.Text = "</span>";
            targetHolder.Controls.Add(lit);



            lit = new Literal();
            lit.Text = "<span class=\"inputvalidatorfield\">";
            targetHolder.Controls.Add(lit);

            bool hasValidator = true;
            if (ismandatory)
            {
                RequiredFieldValidator req = new RequiredFieldValidator();
                req.ID = "req" + id;
                req.ControlToValidate = char_cntl.ID;
                req.Text = "*";
                req.SetFocusOnError = true;
                req.ErrorMessage = label + " field is mandatory";
                req.ValidationGroup = validationgroup;
                targetHolder.Controls.Add(req);

                //AjaxControlToolkit.ValidatorCalloutExtender reqex = new AjaxControlToolkit.ValidatorCalloutExtender();
                //reqex.ID = "reqex" + id;
                //reqex.TargetControlID = req.ID;
                //targetHolder.Controls.Add(reqex);
            }
            else
            {
                hasValidator = false;
            }

            switch (fieldtype)
            {
                case "D":
                    CompareValidator cmpdate = new CompareValidator();
                    cmpdate.ID = "cmpdate" + id;
                    cmpdate.ControlToValidate = char_cntl.ID;
                    cmpdate.Type = ValidationDataType.Date;
                    cmpdate.ErrorMessage = "Invalid date entered";
                    cmpdate.Text = "*";
                    cmpdate.Operator = ValidationCompareOperator.DataTypeCheck;
                    cmpdate.ValidationGroup = validationgroup;
                    targetHolder.Controls.Add(cmpdate);

                    //AjaxControlToolkit.ValidatorCalloutExtender cmpexdate = new AjaxControlToolkit.ValidatorCalloutExtender();
                    //cmpexdate.ID = "cmpexdate" + id;
                    //cmpexdate.TargetControlID = cmpdate.ID;
                    //targetHolder.Controls.Add(cmpexdate);
                    break;
                case "N":
                    CompareValidator cmpnum = new CompareValidator();
                    cmpnum.ID = "cmp" + id;
                    cmpnum.ControlToValidate = char_cntl.ID;
                    cmpnum.Operator = ValidationCompareOperator.DataTypeCheck;
                    cmpnum.Type = ValidationDataType.Integer;
                    cmpnum.Text = "*";
                    cmpnum.ErrorMessage = "Invalid numeric value entered";
                    cmpnum.ValidationGroup = validationgroup;
                    targetHolder.Controls.Add(cmpnum);

                    //AjaxControlToolkit.ValidatorCalloutExtender cmpexnum = new AjaxControlToolkit.ValidatorCalloutExtender();
                    //cmpexnum.ID = "cmpexnum" + id;
                    //cmpexnum.TargetControlID = cmpnum.ID;
                    //targetHolder.Controls.Add(cmpexnum);
                    break;
                case "F":
                case "C":
                    CompareValidator cmpfloat = new CompareValidator();
                    cmpfloat.ID = "cmpfloat" + id;
                    cmpfloat.ControlToValidate = char_cntl.ID;
                    cmpfloat.Operator = ValidationCompareOperator.DataTypeCheck;
                    cmpfloat.Type = ValidationDataType.Currency;
                    cmpfloat.Text = "*";
                    cmpfloat.ErrorMessage = "Invalid decimal amount entered";
                    cmpfloat.ValidationGroup = validationgroup;
                    targetHolder.Controls.Add(cmpfloat);

                    //AjaxControlToolkit.ValidatorCalloutExtender cmpexfloat = new AjaxControlToolkit.ValidatorCalloutExtender();
                    //cmpexfloat.ID = "cmpex" + id;
                    //cmpexfloat.TargetControlID = cmpfloat.ID;
                    //targetHolder.Controls.Add(cmpexfloat);
                    break;
                case "W": // web address validation
                    RegularExpressionValidator regweb = new RegularExpressionValidator();
                    regweb.ID = "regweb" + id;
                    regweb.ControlToValidate = char_cntl.ID;
                    regweb.Text = "*";
                    regweb.ErrorMessage = "Invalid web address entered";
                    //regweb.ValidationExpression = "([\\w-]+\\.)+[\\w-]+(/[\\w- ./?%&amp;=]*)?";
                    regweb.ValidationExpression = @"^(https?://){0,1}(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?[\w\-\./\?\%\&amp\;\=]*";
                    //@"^(https?|ftps?)://(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?.*|^mailto:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?"
                    regweb.ValidationGroup = validationgroup;
                    targetHolder.Controls.Add(regweb);

                    //AjaxControlToolkit.ValidatorCalloutExtender regexweb = new AjaxControlToolkit.ValidatorCalloutExtender();
                    //regexweb.ID = "regexweb" + id;
                    //regexweb.TargetControlID = regweb.ID;
                    //targetHolder.Controls.Add(regexweb);
                    break;
                case "E": // email address validation
                    RegularExpressionValidator regEmail = new RegularExpressionValidator();
                    regEmail.ID = "regEmail" + id;
                    regEmail.ControlToValidate = char_cntl.ID;
                    regEmail.Text = "*";
                    regEmail.ErrorMessage = "Invalid email address specified";
                    regEmail.ValidationExpression = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
                    regEmail.ValidationGroup = validationgroup;
                    regEmail.Display = ValidatorDisplay.Dynamic;
                    targetHolder.Controls.Add(regEmail);
                    break;
                default:
                    if (!ismandatory)
                    {
                        hasValidator = false;
                    }
                    break;
            }

            lit = new Literal();
            if (!hasValidator)
            {
                lit.Text = "&nbsp;";
            }
            lit.Text += "</span>";
            targetHolder.Controls.Add(lit);

            return;
        }

        private void GetListCell(ref PlaceHolder targetHolder, string id, string label, System.Web.UI.WebControls.ListItem[] listitems, int selectedId, short tabIndex, string validationgroup, string tooltipID = "", bool mandatory = false)
        {
            string mandatoryClass = string.Empty;
            string mandatoryStar = string.Empty;

            if (mandatory)
            {
                mandatoryClass = " class=\"mandatory\"";
                mandatoryStar = "*";
            }

            Literal lit = new Literal();
            lit.Text = "<label id=\"lbl" + id + "\" for=\"" + id + "\"" + mandatoryClass + ">" + label + mandatoryStar + "</label>";
            targetHolder.Controls.Add(lit);

            lit = new Literal();
            lit.Text = "<span class=\"inputs\">";
            targetHolder.Controls.Add(lit);

            DropDownList celllist = new DropDownList();
            celllist.ID = id;
            celllist.TabIndex = tabIndex;
            celllist.ValidationGroup = validationgroup;

            celllist.Items.AddRange(listitems);
            if (celllist.Items.FindByValue(selectedId.ToString()) != null)
            {
                celllist.SelectedIndex = celllist.Items.IndexOf(celllist.Items.FindByValue(selectedId.ToString()));
            }
            targetHolder.Controls.Add(celllist);

            if (id == "suppliercategory")
            {
                celllist.Attributes.Add("onchange", "refreshAdditionalDetailsUDFs('" + celllist.ClientID + "');");
                //celllist.AutoPostBack = true;
                //celllist.SelectedIndexChanged += new EventHandler(celllist_SelectedIndexChanged);
            }

            lit = new Literal();
            lit.Text = "</span><span class=\"inputicon\">&nbsp;</span><span class=\"inputtooltipfield\">";
            targetHolder.Controls.Add(lit);

            if (!string.IsNullOrEmpty(tooltipID))
            {
                Image tooltipImage = new Image();
                tooltipImage.ID = "imgtooltip" + id;
                tooltipImage.ImageUrl = "~/shared/images/icons/16/plain/tooltip.png";
                tooltipImage.Attributes.Add("onmouseover", "SEL.Tooltip.Show('" + tooltipID + "', 'sm', this);");
                targetHolder.Controls.Add(tooltipImage);
            }

            lit = new Literal();
            lit.Text = "</span>";
            targetHolder.Controls.Add(lit);

            lit = new Literal();
            lit.Text += "<span class=\"inputvalidatorfield\">";
            targetHolder.Controls.Add(lit);

            if (mandatory && celllist.Items.Count > 0)
            {
                CompareValidator compVal = new CompareValidator();
                compVal.ID = "cv" + id;
                compVal.ControlToValidate = celllist.ID;
                compVal.ValueToCompare = "0";
                compVal.Operator = ValidationCompareOperator.GreaterThan;
                compVal.Type = ValidationDataType.Integer;
                compVal.Text = "*";
                compVal.ErrorMessage = label + " is a mandatory value, please choose an option from the list.";
                compVal.ValidationGroup = validationgroup;
                targetHolder.Controls.Add(compVal);
            }

            lit = new Literal();
            lit.Text += "</span>";
            targetHolder.Controls.Add(lit);

            return;
        }

        private void GetListCell(ref PlaceHolder targetHolder, string id, string label, List<ListItem> listitems, int selectedId, short tabIndex, string validationgroup, string tooltipID = "")
        {
            Literal lit = new Literal();
            lit.Text = "<label id=\"lbl" + id + "\" for=\"" + id + "\">" + label + "</label>";
            targetHolder.Controls.Add(lit);

            lit = new Literal();
            lit.Text = "<span class=\"inputs\">";
            targetHolder.Controls.Add(lit);

            DropDownList celllist = new DropDownList();
            celllist.ID = id;
            celllist.TabIndex = tabIndex;
            celllist.ValidationGroup = validationgroup;

            for (int x = 0; x < listitems.Count; x++)
            {
                celllist.Items.Add(listitems[x]);
            }

            if (celllist.Items.FindByValue(selectedId.ToString()) != null)
            {
                celllist.Items.FindByValue(selectedId.ToString()).Selected = true;
            }
            else
            {
                cCountries clscountries = new cCountries(AccountId, SubAccountID);
                if (clscountries.list.ContainsKey(selectedId))
                {
                    celllist.Items.Add(clscountries.GetListItem(selectedId));
                    celllist.Items.FindByValue(selectedId.ToString()).Selected = true;
                }
            }
            targetHolder.Controls.Add(celllist);

            if (id == "suppliercategory")
            {
                celllist.Attributes.Add("onchange", "refreshAdditionalDetailsUDFs('" + celllist.ClientID + "');");
                //celllist.AutoPostBack = true;
                //celllist.SelectedIndexChanged += new EventHandler(celllist_SelectedIndexChanged);
            }

            lit = new Literal();
            lit.Text = "</span><span class=\"inputicon\">&nbsp;</span><span class=\"inputtooltipfield\">";
            targetHolder.Controls.Add(lit);

            if (!string.IsNullOrEmpty(tooltipID))
            {
                Image tooltipImage = new Image();
                tooltipImage.ID = "imgtooltip" + id;
                tooltipImage.ImageUrl = "~/shared/images/icons/16/plain/tooltip.png";
                tooltipImage.Attributes.Add("onmouseover", "SEL.Tooltip.Show('" + tooltipID + "', 'sm', this);");
                targetHolder.Controls.Add(tooltipImage);
            }

            lit = new Literal();
            lit.Text = "</span>";
            targetHolder.Controls.Add(lit);

            lit = new Literal();
            lit.Text += "<span class=\"inputvalidatorfield\">&nbsp;</span>";
            targetHolder.Controls.Add(lit);

            return;
        }

        /// <summary>
        /// This method is called by the update panel to cause a partial postback and update the fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void UpdatePanel_Load(object sender, EventArgs e)
        {
            renderUDFs();
        }

        void celllist_SelectedIndexChanged(object sender, EventArgs e)
        {
            renderUDFs();
        }

        private TableCell GetTwoColTitle(string cellTitle, HorizontalAlign alignment, bool bold, bool underline)
        {
            TableCell titleCell = new TableCell();
            titleCell.ColumnSpan = 3;

            titleCell.Text = cellTitle;
            titleCell.Font.Underline = underline;
            titleCell.Font.Bold = bold;
            titleCell.HorizontalAlign = alignment;

            return titleCell;
        }

        private void GetTextCell(ref PlaceHolder targetHolder, string id, string label, string value, int maxlength, string fieldtype, bool ismandatory, int textrows, short tabIndex, string validationgroup)
        {
            Literal lit = new Literal();
            lit.Text = "<label id=\"lbl" + id + "\" ";
            if (ismandatory)
            {
                lit.Text += "class=\"mandatory\" ";
            }
            lit.Text += "for=\"" + id + "\">" + label;
            if (ismandatory)
            {
                lit.Text += "*";
            }
            lit.Text += "</label>";
            targetHolder.Controls.Add(lit);

            lit = new Literal();
            lit.Text = "<span class=\"inputs\">";
            targetHolder.Controls.Add(lit);

            TextBox char_cntl = new TextBox();
            char_cntl.ID = id;
            char_cntl.TextMode = TextBoxMode.MultiLine;
            char_cntl.Rows = textrows;
            char_cntl.CssClass = "fillspan";
            char_cntl.TabIndex = tabIndex;
            char_cntl.Text = value;
            char_cntl.ValidationGroup = validationgroup;
            targetHolder.Controls.Add(char_cntl);

            lit = new Literal();
            lit.Text = "</span><span class=\"inputicon\">&nbsp;</span><span class=\"inputtooltipfield\">&nbsp;</span>";
            lit.Text += "<span class=\"inputvalidatorfield\">";
            targetHolder.Controls.Add(lit);

            if (ismandatory)
            {
                RequiredFieldValidator req = new RequiredFieldValidator();
                req.ID = "req" + id;
                req.ControlToValidate = char_cntl.ID;
                req.Text = "*";
                req.SetFocusOnError = true;
                req.ErrorMessage = label + " field is mandatory";
                req.ValidationGroup = validationgroup;
                targetHolder.Controls.Add(req);

                //AjaxControlToolkit.ValidatorCalloutExtender reqex = new AjaxControlToolkit.ValidatorCalloutExtender();
                //reqex.ID = "reqex" + id;
                //reqex.TargetControlID = req.ID;
                //targetHolder.Controls.Add(reqex);
            }

            lit = new Literal();
            if (!ismandatory)
            {
                lit.Text += "&nbsp;";
            }
            lit.Text += "</span>";
            targetHolder.Controls.Add(lit);

            return;
        }

        private void GetCheckboxCell(ref PlaceHolder targetHolder, string id, string label, bool ischecked, short tabIndex, string validationgroup)
        {
            Literal lit = new Literal();
            lit.Text = "<label id=\"lbl" + id + "\" for=\"" + id + "\">" + label + "</label>";
            targetHolder.Controls.Add(lit);

            lit = new Literal();
            lit.Text = "<span class=\"inputs\">";
            targetHolder.Controls.Add(lit);

            CheckBox chk = new CheckBox();
            chk.ID = id;
            chk.Checked = ischecked;
            chk.TabIndex = tabIndex;
            chk.ValidationGroup = validationgroup;
            targetHolder.Controls.Add(chk);

            lit = new Literal();
            lit.Text = "</span><span class=\"inputicon\">&nbsp;</span><span class=\"inputtooltipfield\">&nbsp;</span>";
            lit.Text += "<span class=\"inputvalidatorfield\">&nbsp;</span>";
            targetHolder.Controls.Add(lit);
            return;
        }

        private List<ListItem> getMonths()
        {
            List<ListItem> m = new List<ListItem>();
            m.Add(new ListItem("January", "1"));
            m.Add(new ListItem("February", "2"));
            m.Add(new ListItem("March", "3"));
            m.Add(new ListItem("April", "4"));
            m.Add(new ListItem("May", "5"));
            m.Add(new ListItem("June", "6"));
            m.Add(new ListItem("July", "7"));
            m.Add(new ListItem("August", "8"));
            m.Add(new ListItem("September", "9"));
            m.Add(new ListItem("October", "10"));
            m.Add(new ListItem("November", "11"));
            m.Add(new ListItem("December", "12"));

            return m;
        }
        #endregion

        #region properties

        public int SupplierId
        {
            get
            {
                if (ViewState["supplierid"] == null)
                {
                    return 0;
                }
                else
                {
                    return (int)ViewState["supplierid"];
                }
            }

            set
            {
                ViewState["supplierid"] = value;
            }
        }

        public int AccountId
        {
            get
            {
                if (ViewState["AccountID"] == null)
                {
                    return 0;
                }
                else
                {
                    return (int)ViewState["AccountID"];
                }
            }

            set { ViewState["AccountID"] = value; }
        }

        public int SubAccountID
        {
            get
            {
                if (ViewState["SubAccountID"] == null)
                {
                    return 0;
                }
                else
                {
                    return (int)ViewState["SubAccountID"];
                }
            }

            set { ViewState["SubAccountID"] = value; }
        }

        public int UserId
        {
            get
            {
                if (ViewState["UserID"] == null)
                {
                    return 0;
                }
                else
                {
                    return (int)ViewState["UserID"];
                }
            }

            set { ViewState["UserID"] = value; }
        }
        #endregion


    }

}

