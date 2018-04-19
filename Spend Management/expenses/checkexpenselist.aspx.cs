namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web.UI;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;

    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;

    using Spend_Management.expenses.code.Claims;

    /// <summary>
    /// Summary description for checkexpenselist.
    /// </summary>
    public partial class checkexpenselist : Page
    {
        /// <summary>
        /// An instance of <see cref="IDataFactory{IGeneralOptions,Int32}"/> to get a <see cref="IGeneralOptions"/>
        /// </summary>
        [Dependency]
        public IDataFactory<IGeneralOptions, int> GeneralOptionsFactory { get; set; }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Title = "Expense Claim Details";
            Master.PageSubTitle = Title;

            Master.UseDynamicCSS = true;
            Response.Expires = 60;
            Response.ExpiresAbsolute = DateTime.Now.AddMinutes(-1);
            Response.AddHeader("pragma", "no-cache");
            Response.AddHeader("cache-control", "private");
            Response.CacheControl = "no-cache";

            Master.helpid = 1050;

            CurrentUser user = cMisc.GetCurrentUser();

            if (IsPostBack == false)
            {
                StringBuilder js = new StringBuilder();

                cEmployees clsemployees = new cEmployees(user.AccountID);

                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CheckAndPay, true, true);

                var generalOptions = this.GeneralOptionsFactory[user.CurrentSubAccountId].WithClaim().WithMileage();

                cClaims claims = new cClaims(user.AccountID);

                if (Request.QueryString["claimid"] == null)
                {
                    Response.Redirect("checkpaylist.aspx", true);
                }
                
                int claimId = Convert.ToInt32(Request.QueryString["claimid"]);
                cClaim claim = claims.getClaimById(claimId);
                cGroups clsgroups = new cGroups(user.AccountID);
                cGroup reqgroup = claims.GetGroupForClaim(clsgroups, claim);

                if (claim.splitApprovalStage)
                {
                    var claimSubmission = new ClaimSubmission(user);
                    if (!claimSubmission.HasItemCheckerWithEmployee(claim.claimid, user.EmployeeID))
                    {
                        Response.Redirect("checkpaylist.aspx", true);
                    }
                }
                else
                {
                    if (claim.checkerid != user.EmployeeID)
                    {
                        Response.Redirect("checkpaylist.aspx", true);
                    }
                }

                if (Request.QueryString["UpdateItemChecker"] == null)
                {
                    var reqstage = reqgroup.stages.Values[claim.stage - 1];
                    SignoffType signofftype = reqstage.signofftype;
                    if (SignoffType.Employee == signofftype || SignoffType.LineManager == signofftype || SignoffType.Team == signofftype || SignoffType.ApprovalMatrix == signofftype)
                    {
                        claims.ResetItemCheckerId(claim);
                    }
                }

                this.litdeclaration.Text = generalOptions.Claim.ApproverDeclarationMsg;
                if (claim.submitted == false)
                {
                    Response.Redirect("checkpaylist.aspx", true);
                }

                litMenu.Text = this.generateMenu(claim);

                #region claim history
                if (claim.HasClaimHistory)
                {
                    string[] grid = claims.generateClaimHistoryGrid(claimId, user.EmployeeID);
                    lithistory.Text = grid[1];
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "gridHistoryVars", cGridNew.generateJS_init("gridHistoryVars", new List<string> { grid[0] }, user.CurrentActiveModule), true);
                }
                else
                {
                    js.Append("$g('divHistory').style.display = 'none';\n");
                }
                #endregion

                cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
                cCurrencies clscurrencies = new cCurrencies(user.AccountID, user.CurrentSubAccountId);
                cCurrency reqcurrency = clscurrencies.getCurrencyById(claim.currencyid);

                litprevious.Text = claims.getPreviousClaimsDropDown(claim.employeeid);
                Employee claimant = clsemployees.GetEmployeeById(claim.employeeid);
                lblsubmitted.Text = claim.datesubmitted.ToLongDateString();
                lblsubmitted.Text = lblsubmitted.Text.Replace(" ", "&nbsp;");

                string symbol;
                if (reqcurrency == null)
                {
                    symbol = "£";
                }
                else
                {
                    symbol = clsglobalcurrencies.getGlobalCurrencyById(reqcurrency.globalcurrencyid).symbol;
                }

                lblamount.Text = claim.Total.ToString(symbol + "###,##0.00");
                lblstage.Text = claim.stage + " of " + claim.TotalStageCount;

                var claimStages = claims.GetSignoffStages(claim);
                var currentstage = claimStages[claim.stage - 1];
                
                this.divVerifyComment.Visible = currentstage.IsPostValidationCleanupStage;
                
                Master.title = "Claim: " + claim.name;
                lblclaimname.Text = claim.name;
                lblclaimname.Text = lblclaimname.Text.Replace(" ", "&nbsp;");
                lblclaimant.Text = claimant.Surname + ", " + claimant.Title + " " + claimant.Forename;
                lblclaimant.Text = lblclaimant.Text.Replace(" ", "&nbsp;");

                #region Grids
                cExpenseItems expenseItems = new cExpenseItems(user.AccountID);
                cEmployeeCorporateCards cards = new cEmployeeCorporateCards(user.AccountID);
                SortedList<int, cEmployeeCorporateCard> employeeCards = cards.GetEmployeeCorporateCards(claim.employeeid);
                bool enableCorporateCards = employeeCards != null && employeeCards.Count > 0;
                string[] gridData = expenseItems.generateClaimGrid(user.EmployeeID, claim, "gridExpenses", UserView.CheckAndPay, Filter.None, false, generalOptions.Claim.AttachReceipts, generalOptions.Mileage.AllowMultipleDestinations, enableCorporateCards, symbol, ItemState.Unapproved);
                litExpensesGrid.Text = gridData[1];
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ClaimGridVars", cGridNew.generateJS_init("ClaimGridVars", new System.Collections.Generic.List<string>() { gridData[0] }, user.CurrentActiveModule), true);

                gridData = expenseItems.generateClaimGrid(user.EmployeeID, claim, "gridReturned", UserView.CheckAndPay, Filter.None, false, generalOptions.Claim.AttachReceipts, generalOptions.Mileage.AllowMultipleDestinations, enableCorporateCards, symbol, ItemState.Returned);
                litReturnedGrid.Text = gridData[1];
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ReturnedGridVars", cGridNew.generateJS_init("ReturnedGridVars", new System.Collections.Generic.List<string>() { gridData[0] }, user.CurrentActiveModule), true);

                gridData = expenseItems.generateClaimGrid(user.EmployeeID, claim, "gridApproved", UserView.CheckAndPay, Filter.None, false, generalOptions.Claim.AttachReceipts, generalOptions.Mileage.AllowMultipleDestinations, enableCorporateCards, symbol, ItemState.Approved);
                litApprovedGrid.Text = gridData[1];
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ApprovedGridVars", cGridNew.generateJS_init("ApprovedGridVars", new System.Collections.Generic.List<string>() { gridData[0] }, user.CurrentActiveModule), true);
                #endregion

                #region Envelopes & Receipts

                // make sure the user has Scan/Attach before showing boxes.
                if (user.Account.ReceiptServiceEnabled/* && claimStages.Select(s => s.signofftype).Contains(SignoffType.SELScanAttach)*/) // uncomment after FriendsLife migration
                {
                    referenceNumberRow.Visible = true;

                    if (string.IsNullOrEmpty(claim.ReferenceNumber) == false)
                    {
                        this.referenceNumber.Text = claim.ReferenceNumber;
                    }

                    var envelopeSummary = claims.GetClaimEnvelopeSummary(claimId);

                    this.lblEnvelopeNumbers.Text = envelopeSummary.LabelText;

                    this.envelopeNumbers.InnerHtml = envelopeSummary.FirstEnvelopeNumber;

                    this.additionalEnvelopeLink.InnerText = envelopeSummary.AdditionalEnvelopesText;

                    foreach (var spanElement in envelopeSummary.AdditionalEnvelopeList)
                    {
                        this.additionalEnvelopeNumbers.Controls.Add(spanElement);
                    }
                }

                //display the claim-level receipts message if necessary
                var receiptCount = new SpendManagementLibrary.Expedite.Receipts(user.AccountID, user.EmployeeID).GetByClaim(claimId).Count;
                if (receiptCount == 1)
                {
                    receiptsContainerSingular.InnerHtml = string.Format(receiptsContainerSingular.InnerHtml, claimId);
                    receiptsContainerSingular.Visible = true;
                }
                else if (receiptCount > 1)
                {
                    receiptsContainerPlural.InnerHtml = string.Format(receiptsContainerPlural.InnerHtml, receiptCount, claimId);
                    receiptsContainerPlural.Visible = true;
                }

                #endregion

                js.Append("$(document).ready(function() {\n");
                js.Append("SEL.Claims.General.claimId = " + claimId + ";\n");
                js.Append("SEL.Claims.General.enableReceiptAttachments = " + generalOptions.Claim.AttachReceipts.ToString().ToLower() + ";\n");
                js.Append("SEL.Claims.General.enableJourneyDetailsLink = " + generalOptions.Mileage.AllowMultipleDestinations.ToString().ToLower() + ";\n");
                js.Append("SEL.Claims.General.viewType = " + (byte)UserView.CheckAndPay + ";\n");
                js.Append("SEL.Claims.IDs.modReturnId = '" + modReturn.ClientID + "';\n");
                js.Append("SEL.Claims.IDs.txtReturnReasonId = '" + txtReturnReason.ClientID + "';\n");
                js.Append("SEL.Claims.General.displayDeclaration = " + currentstage.displaydeclaration.ToString().ToLower() + ";\n");
                js.Append("SEL.Claims.IDs.modDeclarationId = '" + moddeclaration.ClientID + "';\n");
                js.Append("SEL.Claims.IDs.popupTransactionId = '" + popuptransaction.ClientID + "';\n");
                js.Append("SEL.Claims.IDs.modValidationId = '" + modValidation.ClientID + "';\n");
                js.Append("SEL.Claims.IDs.popupTransactionId = '" + popuptransaction.ClientID + "';\n");
                js.Append("SEL.Claims.ClaimViewer.configureFlagModal();\n");

                if (claim.NumberOfUnapprovedItems == 0 && !claim.HasReturnedItems)
                {
                    js.Append("$g('divCheckClaim').style.display = 'none';\n");
                }
                else
                {
                    js.Append("$g('divApproveClaim').style.display = 'none';\n");
                }

                if (claim.splitApprovalStage)
                {
                    js.Append("SEL.Claims.General.IsSplitClaim = true;\n");
                }

                js.Append("});");

                ClientScript.RegisterStartupScript(this.GetType(), "js", js.ToString(), true);

                claims.AuditViewClaim(SpendManagementElement.CheckAndPay, claim, user);
            }

            var journeyDetailsControl = (journey_details)LoadControl("~/expenses/usercontrols/journey_details.ascx");
            journeyDetailsControl.AccountId = user.AccountID;
            journeyDetailsControl.CompanyId = user.Account.companyid;
            journeyDetailsControl.MapsEnabled = user.Account.MapsEnabled;
            journeyDetailsContainer.Controls.Add(journeyDetailsControl);

            var receiptsControl = (Receipts)LoadControl("~/expenses/usercontrols/Receipts.ascx");
            receiptsControl.AjaxMode = true;
            receiptsContainer.Controls.Add(receiptsControl);
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

        public string generateMenu(cClaim claim)
        {
            StringBuilder menu = new StringBuilder();
            menu.Append("<div id=\"divCheckClaim\">")
                    .Append("<a href=\"javascript:SEL.Claims.CheckExpenseList.AllowSelected();\" class=\"submenuitem\">Allow Selected</a>")
                    .Append("<a href=\"javaScript:SEL.Claims.CheckExpenseList.ShowReturnModal();\" class=\"submenuitem\">Return Selected</a>")
                .Append("</div>")
                .Append("<a href=\"print.aspx?viewid=4&claimid=" + claim.claimid + "\" target=\"_blank\" class=\"submenuitem\">Print</a>")
                .Append("<a href=\"../setupview.aspx?viewid=4&claimid=" + claim.claimid + "\" class=\"submenuitem\">Change View</a>")
                .Append("<div id=\"divApproveClaim\">")
                    .Append("<a href=\"javascript:SEL.Claims.CheckExpenseList.ApproveClaim();\" class=\"submenuitem\">Approve Claim</a>")
                .Append("</div>")
                .Append("<div id=\"divUnsubmitClaim\">")
                .Append("<a href=\"javascript:SEL.Claims.CheckExpenseList.ShowUnsubmitClaimAsApproverModal();\" class=\"submenuitem\">Unsubmit Claim</a>")
                .Append("</div>")
                .Append("<a href=\"checkpaylist.aspx\" class=\"submenuitem\">Back</a>");

            return menu.ToString();
        }
    }
}
