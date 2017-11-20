using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spend_Management;
using SpendManagementLibrary;

namespace tempMobileUnitTests
{
    internal class cClaimObject
    {
        public static cClaim Template(int claimid = 0, int claimno = 1, int employeeid = 0, string name = default(string), string description = default(string), int stage = 1, bool approved = false, bool paid = false, DateTime datesubmitted = default(DateTime), DateTime datepaid = default(DateTime), ClaimStatus status = ClaimStatus.None, int teamid = 0, int checkerid = 0, bool submitted = false, int currencyid = default(int), SortedList<int, object> userdefined = null, string connString = default(string), SortedList<int, cExpenseItem> expenseItems = null)
        {
            ICurrentUser currentUser = Moqs.CurrentUser();

            name = (name == default(string) ? "Unit Test Claim " + DateTime.UtcNow.ToLongDateString() + DateTime.UtcNow.Ticks.ToString() : name);
            description = (description == default(string) ? "Unit Test Claim" : description);
            datepaid = (datepaid == default(DateTime) ? DateTime.Parse("01/01/1900") : datepaid);
            datesubmitted = (datesubmitted == default(DateTime) ? DateTime.Parse("01/01/1900") : datesubmitted);
            
            return new cClaim(currentUser.AccountID, claimid, claimno, employeeid, name, description, stage, approved, paid, datesubmitted, datepaid, status, teamid, checkerid, submitted, DateTime.UtcNow, currentUser.EmployeeID, DateTime.UtcNow, currentUser.EmployeeID, currencyid, userdefined, connString, expenseItems);

        }

        public static cClaim New(cClaim claim)
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cClaims clsClaims = new cClaims(currentUser.AccountID);

            int claimID = clsClaims.addClaim(claim.employeeid, claim.name, claim.description, claim.userdefined);

            cClaims newClaims = new cClaims(currentUser.AccountID);
            cClaim retClaim = newClaims.getClaimById(claimID);

            return retClaim;
        }

        public static void TearDown(int claimID)
        {
            if(claimID > 0)
            {
                try
                {
                    ICurrentUser currentUser = Moqs.CurrentUser();
                    cClaims clsClaims = new cClaims(currentUser.AccountID);
                    cClaim claim = clsClaims.getClaimById(claimID);
                    if(claim != null)
                    {
                        clsClaims.deleteClaim(claim);
                    }
                }
                catch(Exception e)
                {

                }
            }
            return;
        }
    }
}
