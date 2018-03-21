namespace Spend_Management.expenses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Script.Serialization;
    using System.Web.UI.WebControls;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Enumerators.Expedite;
    using SpendManagementLibrary.Expedite;
    using SpendManagementLibrary.Employees;
    using System.Web.UI;
    using System.Globalization;

    public partial class claimViewer : System.Web.UI.Page
    {
        /// <summary>
        /// The number of days that the current account has set to wait before bringing up the envelope missing dialogue.
        /// </summary>
        public int DaysToWaitUntilEnvelopeCanBeDeclaredMissing = 5;

        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            if (!IsPostBack)
            {
                Master.UseDynamicCSS = true;
                cMisc misc = new cMisc(user.AccountID);
                cGlobalProperties properties = misc.GetGlobalProperties(user.AccountID);
                cClaims claims = new cClaims(user.AccountID);
                cExpenseItems expenseItems = new cExpenseItems(user.AccountID);
                StringBuilder js = new StringBuilder();
                bool useMobileDevices = false;
                bool fromClaimSelector = this.Request["claimSelector"] != null;
                int claimId = this.Request.QueryString["claimid"] != null ? int.Parse(this.Request.QueryString["claimid"]) : claims.getDefaultClaim(ClaimStage.Current, user.EmployeeID);

                cClaim claim = claims.getClaimById(claimId);
                if (claim == null)
                {
                    Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                }

                int employeeID;
                int viewOwnerIdForQueryString = 0;

                if (string.IsNullOrWhiteSpace(Request.QueryString["employeeid"]) == false)
                {
                    int.TryParse(Request.QueryString["employeeid"], out employeeID);
                    viewOwnerIdForQueryString = employeeID;
                }
                else
                {
                    employeeID = user.EmployeeID;
                }

                if (employeeID == 0 || (claim.employeeid != employeeID && !fromClaimSelector))
                {
                    Response.Redirect("~/restricted.aspx", true);
                }

                if (!claims.IsUserClaimsCurrentApprover(user, claim, fromClaimSelector))
                            {
                                Response.Redirect("~/restricted.aspx", true);
                            }

                #region determine view type
                UserView viewType;
                if (!fromClaimSelector)
                {
                    switch (claim.ClaimStage)
                    {
                        case ClaimStage.Current:
                            viewType = UserView.Current;
                            break;
                        case ClaimStage.Submitted:
                            viewType = UserView.Submitted;
                            break;
                        default:
                            viewType = UserView.Previous;
                            break;
                    }
                }
                else
                {
                    viewType = UserView.Previous;
                }

                #endregion

                

                if (claim != null)
                {
                    #region menu, pagetitles and breadcrumbs

                    this.litmenu.Text = GenerateMenu(claim.claimid, claim.ClaimStage, viewOwnerIdForQueryString, fromClaimSelector);
                    this.Master.title = "Claim: " + claim.name;
                    this.Title = "Claim Details";
                    this.Master.PageSubTitle = "Claim Details";

                    #endregion

                    #region general details
                    cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
                    cCurrencies clscurrencies = new cCurrencies(user.AccountID, user.CurrentSubAccountId);
                    cCurrency reqcurrency = clscurrencies.getCurrencyById(claim.currencyid);

                    string symbol = reqcurrency != null ? clsglobalcurrencies.getGlobalCurrencyById(reqcurrency.globalcurrencyid).symbol : "£";

                    js.Append("SEL.Claims.General.symbol = '" + symbol + "';\n");
                    this.lblname.Text = (claim.name.Length > 25 ? claim.name.Substring(0, 25) + "..." : claim.name).Replace("\n", "&nbsp;");

                    this.lbldescription.Text = claim.description.Length > 200 ? claim.description.Substring(0, 200) + "..." : claim.description;

                    this.lblnumitems.Text = claim.NumberOfItems.ToString();
                    this.lbltotal.Text = Math.Round(claim.Total, 2, MidpointRounding.AwayFromZero).ToString(symbol + "###,###,##0.00");
                    this.lblAmountPay.Text = Math.Round(claim.AmountPayable, 2, MidpointRounding.AwayFromZero).ToString(symbol + "###,###,##0.00");

                    switch (claim.ClaimStage)
                    {
                        case ClaimStage.Current:
                            this.lblstage.Text = "This claim has not yet been submitted.";
                            break;
                        case ClaimStage.Submitted:
                            this.lbldate.Text = claim.datesubmitted.ToLongDateString();
                            this.lblapprover.Text = claim.CurrentApprover;
                            this.lblstage.Text = string.Format("{0} of {1}", claim.stage, claim.TotalStageCount);
                            js.Append("$g('divDateSubmittedRow').style.display = '';\n");
                            this.lblcurrentapprover.Visible = true;
                            js.Append("$g('spanCurrentApproverInputs').style.display = '';\n");
                            js.Append("$g('spanCurrentApproverIcon').style.display = '';\n");
                            js.Append("$g('spanCurrentApproverTooltip').style.display = '';\n");
                            js.Append("$g('spanCurrentApproverValidator').style.display = '';\n");

                            if (claim.HasReturnedItems && user.EmployeeID == claim.employeeid)
                            {
                                this.expenseReturnNotice.Visible = true;

                                if (user.Account.ValidationServiceEnabled)
                                {
                                    // grab the current stage of the claim
                                    var allClaimStageTypes = new cGroups(claim.accountid).GetGroupById(user.Employee.SignOffGroupID).stages.Values.Select(x => x.signofftype).ToList();
                                    if (allClaimStageTypes[claim.stage - 1] == SignoffType.SELValidation)
                                    {
                                        this.expenseValidationReturnedNotice.Visible = true;
                                    }
                                }
                            }

                            break;
                        case ClaimStage.Previous:
                            this.lblpaid.Text = claim.datepaid.ToLongDateString();
                            this.lbldatepaid.Visible = true;
                            js.Append("$g('spanDatePaidInputs').style.display = '';\n");
                            js.Append("$g('spanDatePaidIcon').style.display = '';\n");
                            js.Append("$g('spanDatePaidTooltip').style.display = '';\n");
                            js.Append("$g('spanDatePaidValidator').style.display = '';\n");
                            break;
                    }

                    #endregion

                    #region item filter
                    if (!claim.HasCreditCardItems && !claim.HasPurchaseCardItems)
                    {
                        js.Append("$g('divViewFilter').style.display = 'none';\n");
                    }
                    else if (!claim.HasCreditCardItems)
                    {
                        js.Append("$g('creditCardItemLink').style.display = 'none';\n");
                    }
                    else if (!claim.HasPurchaseCardItems)
                    {
                        js.Append("$g('purchaseCardItemLink').style.display = 'none';\n");
                    }
                    #endregion

                    #region expense items grid
                    cEmployeeCorporateCards cards = new cEmployeeCorporateCards(user.AccountID);
                    SortedList<int, cEmployeeCorporateCard> employeeCards = cards.GetEmployeeCorporateCards(user.EmployeeID);
                    bool enableCorporateCards = employeeCards != null && employeeCards.Count > 0;
                    string[] gridData = expenseItems.generateClaimGrid(user.EmployeeID, claim, "gridExpenses", viewType, Filter.None, false, properties.attachreceipts, properties.allowmultipledestinations, enableCorporateCards, symbol, claimEmployeeId: viewOwnerIdForQueryString, fromClaimSelector: fromClaimSelector, claimStage: claim.ClaimStage);
                    this.litClaimGrid.Text = gridData[1];

                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "ClaimGridVars", cGridNew.generateJS_init("ClaimGridVars", new System.Collections.Generic.List<string>() { gridData[0] }, user.CurrentActiveModule), true);
                    #endregion

                    #region claim history
                    if (claim.HasClaimHistory)
                    {
                        string[] grid = claims.generateClaimHistoryGrid(claimId, user.EmployeeID);
                        this.lithistory.Text = grid[1];
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "gridHistoryVars", cGridNew.generateJS_init("gridHistoryVars", new List<string> { grid[0] }, user.CurrentActiveModule), true);
                    }
                    else
                    {
                        js.Append("$g('divHistory').style.display = 'none';\n");
                    }
                    #endregion

                    #region my mobile items
                    if (properties.UseMobileDevices && user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.MobileDevices, true) && (viewType == UserView.Current))
                    {
                        useMobileDevices = true;
                        string[] grid = claims.generateMobileItemsGrid(claim);
                        this.litmobileitems.Text = grid[1];
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "gridMobileItemsVars", cGridNew.generateJS_init("gridMobileItemsVars", new List<string> { grid[0] }, user.CurrentActiveModule), true);

                        grid = claims.GenerateMobileJourneysGrid(user.EmployeeID, user.AccountID, claim.claimid);
                        this.litMobileJourneys.Text = grid[1];
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "gridMobileJourneysVars", cGridNew.generateJS_init("gridMobileJourneysVars", new List<string> { grid[0] }, user.CurrentActiveModule), true);
                    }
                    else
                    {
                        js.Append("$g('divMobileItems').style.display = 'none';\n");
                    }
                    #endregion

                    #region corporate cards
                    js.Append("SEL.Claims.IDs.popupTransactionId = '" + this.popuptransaction.ClientID + "';\n");
                    if (viewType == UserView.Current)
                    {
                        cCardStatements statements = new cCardStatements(user.AccountID);
                        ListItem[] corporateCardStatements = statements.createStatementDropDown(user.EmployeeID);
                        this.ddlstStatements.Items.AddRange(corporateCardStatements);
                        this.ddlstStatements.Attributes.Add("onchange", "SEL.Claims.ClaimViewer.DdlstStatement_onchange()");
                        if (this.ddlstStatements.Items.Count > 0)
                        {
                            js.Append("SEL.Claims.General.statementId = " + this.ddlstStatements.Items[0].Value + ";\n");

                            js.Append("SEL.Claims.IDs.modUnallocatedId = '" + this.modUnallocated.ClientID + "';\n");
                            js.Append("SEL.Claims.IDs.ddlstStatementId = '" + this.ddlstStatements.ClientID + "';\n");
                            string[] grid = statements.generateCardTransactionGrid(Convert.ToInt32(this.ddlstStatements.Items[0].Value), user.EmployeeID);
                            this.litCardTransactionsGrid.Text = grid[1];
                            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "gridTransactionsVars", cGridNew.generateJS_init("gridTransactionsVars", new List<string> { grid[0] }, user.CurrentActiveModule), true);

                            this.AuditViewClaimCorporateCardStatements(claim, corporateCardStatements, user);
                        }
                        else
                        {
                            js.Append("$g('divCardItems').style.display = 'none';\n");
                        }
                    }
                    else
                    {
                        js.Append("$g('divCardItems').style.display = 'none';\n");
                    }

                    #endregion

                    #region Envelopes & Receipts

                    // hide the envelope divs
                    this.divEnvelopeMissingNotice.Visible = false;
                    this.EnvelopeAttachInfo.Visible = false;
                    this.referenceNumberRow.Visible = false;
                    this.ScanAttachCompletionNotice.Visible = false;

                    // get receipts and the count of how many are in the header.
                    var receipts = new Receipts(user.AccountID, user.EmployeeID);
                
                    // keep a flag here to set in the next block. It will affect whether the claim header notice below is shown.
                    var expediteEnabledAndWillShowMatchingNotice = false;

                    if (user.Account.ReceiptServiceEnabled)
                    {
                        // make sure the user has Scan/Attach before showing boxes.
                        var claimStages = claims.GetSignoffStagesAsTypes(claim);

                        /*if (claimStages.Contains(SignoffType.SELScanAttach))   // uncomment after FriendsLife migration
                    {*/
                        // display any missing envelopes if necessary
                        var envelopeManager = new Envelopes();
                        var claimHasCrn = !string.IsNullOrEmpty(claim.ReferenceNumber);

                        if (claimHasCrn)
                        {
                            var envelopes = envelopeManager.GetEnvelopesByClaimReferenceNumber(claim.ReferenceNumber).ToList();

                            this.referenceNumberRow.Visible = true;
                            this.referenceNumber.Text = claim.ReferenceNumber;

                            // determine any missing or damaged envelopes.
                            var missingEnvelopes = envelopes.Where(env => env.Status == EnvelopeStatus.UnconfirmedNotSent).ToList();

                            if (missingEnvelopes != null && missingEnvelopes.Any() && !fromClaimSelector)
                            {
                                this.DaysToWaitUntilEnvelopeCanBeDeclaredMissing = user.Account.DaysToWaitUntilSentEnvelopeIsMissing ?? 5;

                                // ReSharper disable once PossibleInvalidOperationException
                                var items = missingEnvelopes.Select(env =>
                                    new EnvelopeMissingStatus
                                    {
                                        Id = env.EnvelopeId,
                                        EnvelopeNumber = env.EnvelopeNumber,
                                        HasSent = null,
                                        DatePosted = env.DateAssignedToClaim.Value.ToShortDateString()
                                    });

                                var serializer = new JavaScriptSerializer();
                                js.Append("SEL.Claims.General.MissingEnvelopes = " + serializer.Serialize(items) + ";\n");
                                js.Append("SEL.Claims.IDs.modEnvelopeMissingId = '" + this.modEnvelopeMissing.ClientID + "';\n");
                                this.divEnvelopeMissingNotice.Visible = true;
                            }


                            // set the envelope summary in the claim info.
                            var envelopeSummary = claims.GetClaimEnvelopeSummary(claimId);
                            this.lblEnvelopeNumbers.Text = envelopeSummary.LabelText;
                            this.envelopeNumbers.InnerHtml = envelopeSummary.FirstEnvelopeNumber;
                            this.additionalEnvelopeLink.InnerText = envelopeSummary.AdditionalEnvelopesText;

                            foreach (var spanElement in envelopeSummary.AdditionalEnvelopeList)
                            {
                                this.additionalEnvelopeNumbers.Controls.Add(spanElement);
                            }

                            // if we are in Scan & Attach, check we aren't waiting on the user to declare matching is complete for validation in future.
                            if (claim.stage > 0 && claimStages[claim.stage - 1] == SignoffType.SELScanAttach && claimStages.Contains(SignoffType.SELValidation))
                            {
                                int total, complete;

                                // if all the envelopes have had their contents scanned and attached...
                                if (envelopeManager.AreAllEnvelopesCompleteForClaim(claim.ReferenceNumber, out total, out complete, envelopes))
                                {
                                    // then check if any item in the claim has no receipts and the claim header has some
                                    if (!receipts.CheckIfAllValidatableClaimLinesHaveReceiptsAttached(claim.claimid))
                                    {
                                        this.ScanAttachCompletionNotice.InnerHtml = string.Format(this.ScanAttachCompletionNotice.InnerHtml, claimId);
                                        expediteEnabledAndWillShowMatchingNotice = this.ScanAttachCompletionNotice.Visible = true;
                                    }
                                }
                            }
                        }

                        // add envelope info here if the claim has not been submitted yet
                        if (claim.ClaimStage == ClaimStage.Current && !fromClaimSelector)
                        {
                            // load the file from static content using JS on the page
                            var infoUrl = GlobalVariables.GetAppSetting("EnvelopeAttachmentInstructionsUrl").Replace(" ", "%20");
                            js.Append("$( document ).ready(function() {$('#EnvelopeAttachInfo').load('" + infoUrl + "');});");
                            this.EnvelopeAttachInfo.Visible = true;
                        }
                        /*}*/ // uncomment after FriendsLife migration
                    }

                    // display the claim-level receipts message if necessary
                    if (!expediteEnabledAndWillShowMatchingNotice && !fromClaimSelector)
                    {
                        var claimHeaderReceiptCount = receipts.GetByClaim(claimId, false).Count;

                        if (claimHeaderReceiptCount == 1)
                        {
                            this.receiptsContainerSingular.InnerHtml = string.Format(this.receiptsContainerSingular.InnerHtml, claimId);
                            this.receiptsContainerSingular.Visible = true;
                        }
                        else if (claimHeaderReceiptCount > 1)
                        {
                            this.receiptsContainerPlural.InnerHtml = string.Format(this.receiptsContainerPlural.InnerHtml, claimHeaderReceiptCount, claimId);
                            this.receiptsContainerPlural.Visible = true;
                        }
                    }
                }

                #endregion

                js.Append("SEL.Claims.General.claimId = " + claimId + ";\n");
                js.Append("SEL.Claims.General.enableReceiptAttachments = " + properties.attachreceipts.ToString().ToLower() + ";\n");
                js.Append("SEL.Claims.General.enableJourneyDetailsLink = " + properties.allowmultipledestinations.ToString().ToLower() + ";\n");
                js.Append("SEL.Claims.General.enableCorporateCardsLink = " + useMobileDevices.ToString().ToLower() + ";\n");
                js.Append("SEL.Claims.General.viewType = " + (byte)viewType + ";\n");
                js.Append("SEL.Claims.ClaimViewer.configureFlagModal();\n");
                js.Append("SEL.Claims.ClaimViewer.configureSubmitModal();\n");
                js.Append("SEL.Claims.ClaimViewer.configureDeclarationModal();\n");
                js.Append("SEL.Claims.ClaimViewer.configureApproverOnHolidayModal();\n");
                js.Append("SEL.Claims.IDs.modValidationId = '" + modValidation.ClientID + "';\n");
                js.Append("SEL.Claims.IDs.lblApproverId = '" + lblapprover.ClientID + "';\n");
                js.Append("SEL.Claims.IDs.txtClaimNameId = '" + txtClaimName.ClientID + "';\n");
                js.Append("SEL.Claims.IDs.txtClaimDescriptionId = '" + txtClaimDescription.ClientID + "';\n");
                js.AppendLine("SEL.Claims.IDs.ddlstBusinessMileageId = '" + ddlstBusinessMileage.ClientID + "';");
                js.AppendLine("SEL.Claims.IDs.compBusinessMileageId = '" + reqBusinessMileage.ClientID + "';");
                js.AppendLine("SEL.Claims.IDs.reqClaimID = '" + reqclaim.ClientID + "';");
                js.Append("SEL.Claims.General.ExpediteClient = " + user.Account.ReceiptServiceEnabled.ToString().ToLower() + ";\n");
                js.Append("SEL.Claims.General.displayDeclaration = " + (!properties.claimantdeclaration || string.IsNullOrEmpty(properties.declarationmsg) ? "false" : "true") + ";\n");
                if (user.Account.ReceiptServiceEnabled)
                {
                    this.ScriptManagerProxy1.Scripts.Add(
                        new ScriptReference(GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery.alphanum.js"));
                    this.ScriptManagerProxy1.Scripts.Add(
                        new ScriptReference(
                            GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery.address-1.6.min.js"));
                    this.submitClaimInfo.Attributes.Add("claimid", claimId.ToString(CultureInfo.InvariantCulture));
                    this.submitClaimInfo.Attributes.Add("accountid", user.AccountID.ToString(CultureInfo.InvariantCulture));
                    this.ClientScript.RegisterStartupScript(this.GetType(), "envelopeNumbers", "$(document).ready(function(){SEL.Claims.SubmitClaim.SetupSubmitClaimPage();});", true);
                    
                }

                js.Append("SEL.Claims.ClaimViewer.configureEnvelopeModal();\n");
                #region display flags if necessary
                if (Request.QueryString["expenseid"] != null)
                {

                    string expenseids = Request.QueryString["expenseid"];


                    if (!string.IsNullOrEmpty(expenseids))
                    {
                        js.Append("SEL.Claims.ClaimViewer.DisplayFlags('" + expenseids + "','claimViewer');");
                    }
                }
                #endregion

                string fromClaimSelectorAsString = fromClaimSelector ? "true" : "false";
                ClientScript.RegisterStartupScript(this.GetType(), "js", js.ToString(), true);
                ClientScript.RegisterStartupScript(this.GetType(), "claimselectorJS", "SEL.ClaimSelector.RootClaimSelector = '" + fromClaimSelectorAsString + "';", true);

                claims.AuditViewClaim(SpendManagementElement.Claims, claim, user);
            }

            var journeyDetailsControl = (journey_details)LoadControl("~/expenses/usercontrols/journey_details.ascx");
            journeyDetailsControl.AccountId = user.AccountID;
            journeyDetailsControl.CompanyId = user.Account.companyid;
            journeyDetailsControl.MapsEnabled = user.Account.MapsEnabled;
            journeyDetailsContainer.Controls.Add(journeyDetailsControl);

            var receiptsControl = (Spend_Management.Receipts)LoadControl("~/expenses/usercontrols/Receipts.ascx");
            receiptsControl.AjaxMode = true;
            receiptsContainer.Controls.Add(receiptsControl);
        }

        public string GetStaticLibraryPath()
        {
            return GlobalVariables.StaticContentLibrary;
        }

        /// <summary>
        /// Generates the menu displayed in the page options section of the page.
        /// </summary>
        /// <param name="claimStage">The stage the claim is at</param>
        /// <param name="viewowner">
        /// An employee ID if the claim is not the logged-in user's, otherwise 0
        /// </param>
        /// <param name="fromClaimSelector">
        /// Have we come from the claim selector? If so we can hide certain links that would be inappropriate.
        /// </param>
        /// <param name="claimId">The id of the claim to generate the menu for</param>
        /// <returns>
        /// A string containins the generated menu with links.
        /// </returns>
        private static string GenerateMenu(int claimId, ClaimStage claimStage, int viewowner, bool fromClaimSelector = false)
        {
            string claimSelectorUrlPart = fromClaimSelector ? "&claimSelector=true" : string.Empty;

            var builder = new StringBuilder();

            if (claimStage == ClaimStage.Current && !fromClaimSelector)
            {
                builder.AppendFormat("<a class=\"submenuitem\" href=\"../aeexpense.aspx?claimid={0}\">New Expense</a>", claimId);
            }

            if (!fromClaimSelector)
            {
                builder.AppendFormat("<a href=\"claimsummary.aspx?claimtype={0}\" class=\"submenuitem\">Claim List</a>", (byte)claimStage);
            }
            builder.AppendFormat("<a href=\"../setupview.aspx?viewid={0}&claimid={1}{2}{3}\" class=\"submenuitem\">Change View</a>", (byte)claimStage, claimId, viewowner == 0 ? string.Empty : string.Format("&viewowner={0}", viewowner), claimSelectorUrlPart);
            builder.AppendFormat("<a class=submenuitem target=\"_blank\" href=\"print.aspx?viewid={0}&claimid={1}\">Print</a>", (byte)claimStage, claimId);

            if (!fromClaimSelector)
            {
                switch (claimStage)
                {
                    case ClaimStage.Current:
                        builder.Append("<a href=\"javascript:SEL.Claims.ClaimViewer.DetermineIfClaimCanBeSubmitted();\" class=\"submenuitem\">Submit Claim</a>");
                        break;
                    case ClaimStage.Submitted:
                        builder.Append("<a href=\"javascript:SEL.Claims.ClaimViewer.UnsubmitClaim();\" class=\"submenuitem\">Unsubmit Claim</a>");
                        break;
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Navigate to previous page.
        /// </summary>
        /// <param name="sender">sender request</param>
        /// <param name="e">event</param>
        protected void cmdback_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (this.Request.QueryString["claimSelector"] != "true")
            {
                return;
            }
            string claimant = this.Request.QueryString["claimant"] != null ? this.Request.QueryString["claimant"] : string.Empty;
            string pageNumber = this.Request.QueryString["pageNumber"] != null ? this.Request.QueryString["pageNumber"] : string.Empty;
            string claimName = this.Request.QueryString["claimName"] != null ? this.Request.QueryString["claimName"] : string.Empty;
            string filterValue = this.Request.QueryString["filterValue"] != null ? this.Request.QueryString["filterValue"] : string.Empty;
            Response.Redirect("/expenses/claimSelector.aspx?claimSelector=true&claimant=" + claimant + "&pageNumber=" + pageNumber + "&claimName=" + claimName + "&filterValue=" + filterValue);
        }

        private void AuditViewClaimCorporateCardStatements(cClaim claim, ListItem[] corporateCardStatements, CurrentUser user)
        {
            if ((user.EmployeeID != claim.employeeid || (user.isDelegate && user.Delegate.EmployeeID != claim.employeeid)) && corporateCardStatements.Length > 0)
            {
                var statements = string.Empty;

                foreach (var statement in corporateCardStatements)
                {
                    statements += (statement.Text + ", ");
                }

                var auditLog = new cAuditLog();
                auditLog.ViewRecord(SpendManagementElement.Claims, $"Corporate Card Statements for claim {claim.name}: {statements.Remove(statements.Length - 2)}", user);
            }
        }
    }
}