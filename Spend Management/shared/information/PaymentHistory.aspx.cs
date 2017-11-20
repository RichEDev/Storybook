
namespace Spend_Management.shared.information
{
    using SpendManagementLibrary;
    using SpendManagementLibrary.Expedite;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Web;
    using System.Web.UI;

    /// <summary>
    /// This Page Display History of the Payment made through Expedite service for any claimant
    /// </summary>
    public partial class PaymentHistory : System.Web.UI.Page
    {
        CurrentUser currentUser = cMisc.GetCurrentUser();

        #region Page Events
        /// <summary>
        /// Page Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //if the company has not subscribed for expedite payment service, do not display the details.
                cAccount account = new cAccounts().GetAccountsWithPaymentServiceEnabled().Find(x => x.accountid == currentUser.AccountID);
                if (account == null)
                {
                    return;
                }

                Master.PageSubTitle = "Payment History";
                string[] gridData = FillGrid();
                litGrid.Text = gridData[2];
                //setting the javascript variables.
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Claims_baseVars", cGridNew.generateJS_init("PaymentHistory", new List<string> { gridData[1] }, currentUser.CurrentActiveModule), true);
            
            }
        }
            

        /// <summary>
        /// cmdClose_Click event for close and redirect to previous page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdClose_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string sPreviousURL = (SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url;
            Response.Redirect(sPreviousURL, true);
        }
        #endregion

        #region private methods
       
        /// <summary>
        /// Returns fund details as html
        /// </summary>
        /// <param name="transactionStartDate">start date</param>
        /// <param name="transactionEndDate">end date</param>
        /// <param name="transactionType">transaction type</param>
        /// <returns>grid with fund details</returns>
        public string[] FillGrid()
        {
            var clsfields = new cFields(currentUser.AccountID);
            const string SqlQuery = "SELECT ExpenseId,AmountPayable,DatePaid,EmployeeId FROM PaymentHistoryByEmployeeIdView";

            var gridPaymentHistory = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "PaymentHistory", SqlQuery)
            {
                 EmptyText = "There are currently no Payment History to display"
            };
      
            gridPaymentHistory.getColumnByName("ExpenseId").hidden = true;
            gridPaymentHistory.getColumnByName("EmployeeId").hidden = true;
            gridPaymentHistory.enabledeleting = false;
            gridPaymentHistory.enableupdating = false;

            if (currentUser.EmployeeID > 0 && currentUser.EmployeeID != 0)
                {
                gridPaymentHistory.addFilter(clsfields.GetFieldByID(new Guid("2B3E4E0E-E01E-4125-BF1D-0EE9C26DC30D")),
                    ConditionType.Equals, new object[] { currentUser.EmployeeID }, null, ConditionJoiner.None);
                }

            var retVals = new List<string> { gridPaymentHistory.GridID };
            retVals.AddRange(gridPaymentHistory.generateGrid());
            return retVals.ToArray();
        }
        #endregion
    }
}