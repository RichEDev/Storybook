using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;

namespace SpendManagementUnitTests.Global_Objects
{
    public class cClaimObject
    {
        /// <summary>
        /// Global static variable for a claim object to be used in the unit tests
        /// </summary>
        /// <returns></returns>
        public static cClaim CreateCurrentClaim()
        {
            cClaims clsClaims = new cClaims(cGlobalVariables.AccountID);

            int ClaimID = clsClaims.addClaim(cGlobalVariables.EmployeeID, "Unit Test Claim", "Unit Test Claim", new SortedList<int, object>());
            clsClaims = new cClaims(cGlobalVariables.AccountID);
            cGlobalVariables.ClaimID = ClaimID;
            cClaim claim = clsClaims.getClaimById(ClaimID);
            cEmployeeObject.CreateUTDelegateEmployee();
            clsClaims.SendClaimToNextStage(claim, false, cGlobalVariables.DelegateID, cGlobalVariables.EmployeeID, null); ;
            return claim;
        }

        public static void deleteClaim()
        {
            cClaims clsClaims = new cClaims(cGlobalVariables.AccountID);
            cEmployeeObject.DeleteDelegateUTEmployee();
            cClaim claim = clsClaims.getClaimById(cGlobalVariables.ClaimID);
            clsClaims.deleteClaim(claim);
        }

    }
}
