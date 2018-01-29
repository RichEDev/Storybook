using SpendManagementLibrary.Helpers;
using SpendManagementLibrary.Interfaces;
using System.Collections.Generic;
using System.Text;

namespace SpendManagementLibrary.Claims
{
    /// <summary>
    /// Summary of pending claims of an approver
    /// </summary>
    public class ApproverClaimsSummary
    {
        /// <summary>
        /// get pending claim summary in the html table format 
        /// </summary>
        /// <param name="approverId">Approver Id whose claims summary is required </param>
        /// <param name="accountId">customer Id</param>
        /// <param name= "stringBuilder">initialized string builder object</param>
        /// <returns></returns>
        public string GetFormattedSummaryForEmailReminder(int approverId, int accountId, StringBuilder stringBuilder)
        {
            const string ThOpenTag = "<th style='padding:8px; color:#000'>";
            const string ThCloseTag = "</th>";
            const string ThCloseOpen = ThCloseTag + ThOpenTag;
            const string TdOpenTag = "<td style='padding:5px'>";
            const string TdCloseTag = "</td>";
            const string TrOpenTag = "<tr>";
            const string TrCloseTag = "</tr>";

            stringBuilder.Clear();
            stringBuilder.Append("<table>" + TrOpenTag + ThOpenTag + "Check Expenses" + ThCloseOpen + "Claimant" + ThCloseOpen + "Claim Name" + ThCloseOpen + "Claim Total" + ThCloseOpen + "Claim Submitted On" + ThCloseTag + TrCloseTag);

            foreach (var approverClaimsSummary in this.GetAllPendingClaims(approverId, accountId))
            {
                stringBuilder.Append(TrOpenTag);
                stringBuilder.Append(TdOpenTag + "<a href=\"https://[host][path]/expenses/checkexpenselist.aspx?claimid=" + approverClaimsSummary.ClaimId + "&stage=" + approverClaimsSummary.Stage + "\">Check Expenses</a>" + TdCloseTag);
                stringBuilder.Append(TdOpenTag + approverClaimsSummary.Claimant + TdCloseTag);
                stringBuilder.Append(TdOpenTag + approverClaimsSummary.ClaimName + TdCloseTag);
                stringBuilder.Append("<td style='padding:5px' align='right'>" + approverClaimsSummary.CurrencySymbol + string.Format("{0:0.00}", approverClaimsSummary.Total) + TdCloseTag);
                stringBuilder.Append(TdOpenTag + approverClaimsSummary.ClaimSubmittedOn.ToString("dd/MM/yyyy") + TdCloseTag);
                stringBuilder.Append(TrCloseTag);
            }

            stringBuilder.Append("</table>");
            return stringBuilder.ToString();
        }
        /// <summary>
        /// get the list of ApproverClaim for an approver
        /// </summary>
        /// <param name="approverId">Id of the approver</param>
        /// <param name="accountId">customer id</param>
        /// <returns></returns>
        private IEnumerable<ApproverClaim> GetAllPendingClaims(int approverId, int accountId)
        {
            const string Sql = "SELECT stage,claimId,empname, name,total,datesubmitted,currencySymbol from checkandpay where (checkerid = @approverId or itemCheckerId = @approverId) and checkandpay.status <> 4 and CheckerItemsApproved = 0";
            using (IDBConnection claimsData = new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                claimsData.AddWithValue("@approverId", approverId);
                using (var claimInfo = claimsData.GetReader(Sql))
                {
                    var approverClaimSummaryList = new List<ApproverClaim>();
                    var stageOrdinal = claimInfo.GetOrdinal("stage");
                    var claimIdOrdinal = claimInfo.GetOrdinal("claimId");
                    var claimantOrdinal = claimInfo.GetOrdinal("empname");
                    var claimNameOrdinal = claimInfo.GetOrdinal("name");
                    var claimTotalOrdinal = claimInfo.GetOrdinal("total");
                    var claimSubmittaldateOrdinal = claimInfo.GetOrdinal("datesubmitted");
                    var currencySymbolOrdinal = claimInfo.GetOrdinal("currencySymbol");
                    while (claimInfo.Read())
                    {
                        approverClaimSummaryList.Add(
                          new ApproverClaim(
                            approverId,
                            claimInfo.GetInt32(claimIdOrdinal),
                            claimInfo.GetInt32(stageOrdinal),
                            claimInfo.GetString(claimantOrdinal),
                            claimInfo.GetString(claimNameOrdinal),
                            claimInfo.GetDecimal(claimTotalOrdinal),
                            claimInfo.GetDateTime(claimSubmittaldateOrdinal),
                            $"&#{(int)claimInfo.GetString(currencySymbolOrdinal)[0]};"
                            ));
                    }

                    return approverClaimSummaryList;
                }
            }
        }
    }
}
