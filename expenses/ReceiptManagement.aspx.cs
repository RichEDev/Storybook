namespace expenses
{
    using System;
    using System.Linq;
    using System.Web.UI;
    using SpendManagementLibrary;
    using Spend_Management;
    using Spend_Management.expenses.code.Claims;

    public partial class ReceiptManagement : Page
    {
        private const string KeyClaimId = "claimId";
        private const string KeyExpenseId = "expenseId";
        private const string KeyReturnTo = "returnTo";
        private const string KeySubAccountId = "subAccountId";
        private const string KeyViewForEmployeeId = "viewForEmployeeId";
        private const string KeyViewOwner = "viewOwner";
        private const string KeyStage = "stage";
        private const string KeyDeclare = "declare";
        private const string KeyFromClaimSelector = "claimSelector";

        protected int ClaimId;
        protected int? ExpenseId;
        protected bool? Declare;
        protected bool FromClaimSelector;
        
        /// <summary>
        /// Whether the user must provide a reason to delete the receipt.
        /// </summary>
        protected bool MustGiveDeleteReason;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Force IE out of compat mode
            Response.AddHeader("X-UA-Compatible", "IE=edge");
            
            Title = @"Receipt Management";
            Master.title = Title;
            Master.helpid = 666;
            Master.enablenavigation = false;
            Master.UseDynamicCSS = true;

            if (Page.Master != null)
            {
                var helpLink = Page.Master.FindControl("linkhelp");
            
                if (helpLink != null)
                {
                    helpLink.Visible = false;
                }
            }

            ClaimId = int.Parse(Request.QueryString[KeyClaimId]);
            ExpenseId = Request.QueryString[KeyExpenseId] == null ? (int?) null : int.Parse(Request.QueryString[KeyExpenseId]); 
            Declare = Request.QueryString[KeyDeclare] == null ? (bool?) null : bool.Parse(Request.QueryString[KeyDeclare]); 
            FromClaimSelector = Request.QueryString[KeyFromClaimSelector] != null && bool.Parse(Request.QueryString[KeyFromClaimSelector]); 
            
            ViewState[KeyClaimId] = ClaimId;
            ViewState[KeyExpenseId] = ExpenseId;
            ViewState[KeyDeclare] = Declare;
            ViewState[KeyFromClaimSelector] = FromClaimSelector;

            var user = cMisc.GetCurrentUser();
            ViewState[KeySubAccountId] = user.CurrentSubAccountId;
            ViewState[KeyReturnTo] = Request.QueryString[KeyReturnTo] == null ? (int?) null : int.Parse(Request.QueryString[KeyReturnTo]);
            ViewState[KeyViewForEmployeeId] = Request.QueryString[KeyViewForEmployeeId] == null ? (int?) null : int.Parse(Request.QueryString[KeyViewForEmployeeId]); 
            ViewState[KeyStage] = Request.QueryString[KeyStage] == null ? (int?) null : int.Parse(Request.QueryString[KeyStage]);
            ViewState[KeyViewOwner] = Request.QueryString[KeyViewOwner] == null ? (int?) null : int.Parse(Request.QueryString[KeyViewOwner]);

            var claims = new cClaims(user.AccountID);
            var claim = claims.getClaimById(ClaimId);
            
            if (claim == null)
            {
                Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
            }

            if (user.EmployeeID != claim.employeeid && !claims.IsUserClaimsCurrentApprover(user, claim, FromClaimSelector))
            {
                Response.Redirect("~/restricted.aspx", true);
            }

            // determine if deletion requires a reason
            MustGiveDeleteReason = claim.stage > 0 && claim.checkerid == (user.isDelegate ? user.Delegate.EmployeeID : user.EmployeeID);
            
            // show the declaration box and confirm modal if declare is set.
            modalDeclareMatchingComplete.Visible = DeclareMatchingCompleteNotice.Visible = (Declare == true);
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
            this.Save.Click += SaveClickHandler;
            this.Cancel.Click += CancelClickHandler;

        }
        #endregion

        private void CancelClickHandler(object sender, EventArgs e)
        {
            ReturnToPreviousPage();
        }

        private void SaveClickHandler(object sender, EventArgs e)
        {
            var wasDeclaration = (bool?)ViewState[KeyDeclare];
            
            if (wasDeclaration == true)
            {
                var user = cMisc.GetCurrentUser();
                var claims = new cClaims(user.AccountID);
                var claim = claims.getClaimById(ClaimId);
                var signoffStageTypes = claims.GetSignoffStagesAsTypes(claim);

                // advance the claim if it is in the Scan Attach stage.
                if (signoffStageTypes[claim.stage - 1] == SignoffType.SELScanAttach)
                {
                    var claimSubmission = new ClaimSubmission(user);
                    claims.UpdateClaimHistory(claim, "Claimant declared matching is complete. Claim advanced to the next stage.", claim.employeeid);
                    claimSubmission.SendClaimToNextStage(claim, false, user.EmployeeID, claim.employeeid, user.isDelegate ? user.Delegate.EmployeeID : (int?)null);
                }
            }


            ReturnToPreviousPage();
        }

        private void ReturnToPreviousPage()
        {
            var viewStateClaim = ViewState[KeyClaimId];
            var viewStateViewForEmployee = ViewState[KeyViewForEmployeeId];
            var viewStateReturnTo = ViewState[KeyReturnTo];
            var viewStateStage = ViewState[KeyStage];
            var viewStateClaimSelector = ((bool)ViewState[KeyFromClaimSelector]) ? "&claimSelector=true" : string.Empty;

            var viewOwnerEmployeeId = string.Empty;
            if (viewStateViewForEmployee != null && (int)viewStateViewForEmployee > 0)
            {
                viewOwnerEmployeeId = string.Format("employeeid={0}&", (int)viewStateViewForEmployee);
            }

            int returnto;
            if (viewStateReturnTo != null && int.TryParse(viewStateReturnTo.ToString(), out returnto))
            {

                switch ((int) viewStateReturnTo)
                {
                    case 1:
                        Response.Redirect(string.Format("expenses/claimViewer.aspx?{1}claimid={0}{2}", viewStateClaim,
                            viewOwnerEmployeeId, viewStateClaimSelector));
                        break;
                    case 3:
                        var currentStage = string.Empty;
                        if (viewStateStage != null)
                        {
                            currentStage = string.Format("&stage={0}", (int) viewStateStage);
                        }
                        Response.Redirect(
                            string.Format("expenses/checkexpenselist.aspx?claimid={0}{1}", viewStateClaim, currentStage),
                            true);
                        break;
                }
            }
            else
            {
                Response.Redirect(string.Format("ReceiptManagement.aspx?claimid={0}", viewStateClaim));
            }
        }
    }

}
