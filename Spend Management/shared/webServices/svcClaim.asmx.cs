namespace Spend_Management.shared.webServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Script.Services;
    using System.Web.Services;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Expedite;
    using SpendManagementLibrary.Employees;

    /// <summary>
    /// The web service class for all cClaims web service methods.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    [ScriptService]
    public class svcClaim : WebService
    {
        /// <summary>
        /// Saves a list of envelope numbers against a claim
        /// </summary>
        /// <param name="envelopeNumbers">The envelope numbers to save</param>
        /// <param name="claimId">The ID of the claim to use</param>
        /// <param name="accountId">The current account ID</param>
        /// <returns>The reference number for the specified claim ID</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public ClaimEnvelopeAttachmentResults SaveClaimEnvelopeNumbers(List<ClaimEnvelopeInfo> envelopeNumbers, int claimId, int accountId)
        {
            ClaimEnvelopeAttachmentResults results;

            if (accountId < 1)
            {
                results = new ClaimEnvelopeAttachmentResults();
                results.AddStatus(ClaimEnvelopeAttachmentStatus.FailedNoAccount);
                return results;
            }

            // remove any blank envelope entries.
            envelopeNumbers = envelopeNumbers.Where(e => !string.IsNullOrWhiteSpace(e.EnvelopeNumber)).ToList();
                     
            var claims = new cClaims(accountId);
            results = claims.SaveClaimEnvelopeNumbers(envelopeNumbers, claimId, accountId);

            return results;
        }

        /// <summary>
        /// Get employee details by employeeid
        /// </summary>
        /// <param name="employeeId">requested employeeid</param>
        /// <returns>employee object</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public Employee getEmployeeById(int employeeId)
        {
            var user = cMisc.GetCurrentUser();
            var employee = new cEmployees(user.AccountID).GetEmployeeById(employeeId);
            return employee;
        }

        /// <summary>
        /// Get a list of existing claim envelope numbers.
        /// </summary>
        /// <param name="claimId">
        /// The claim ID to use.
        /// </param>
        /// <param name="accountId">
        /// The current account ID.
        /// </param>
        /// <returns>
        /// The list of existing envelope numbers for the specified claim ID.
        /// </returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public List<ClaimEnvelopeInfo> GetExistingClaimEnvelopeNumbers(int claimId, int accountId)
        {
            var claims = new cClaims(accountId);

            if (claimId < 1 || accountId < 1)
            {
                return new List<ClaimEnvelopeInfo>();
            }

            return claims.GetClaimEnvelopeNumbers(claimId);
        }

        /// <summary>
        /// This returns the claims summary grid for all submitted claims for the employee.
        /// </summary>
        /// <param name="employeeId">
        /// The employee id. 0 if using claim name search.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="claimName">
        /// The claim Name. pass employee id 0 if using the claim name search.
        /// </param>
        /// <param name="selectable">
        /// Whether to inlclude a radio / selection column.
        /// </param>
        /// <returns>
        /// Grid data as string array.
        /// </returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] GetApprovedGridForClaimSelector(int employeeId, int accountId, string claimName = "", bool selectable = false)
        {
            var claims = new cClaims(accountId);
            return claims.GetClaimSummaryGrid(employeeId, ClaimStage.Any, true, claimName, selectable);
        }
            }
}
