namespace UnitTest2012Ultimate
{
    using System;
    using System.Collections.Generic;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;

    using Spend_Management;

    internal class cClaimObject
    {
        public static cClaim Template(
            int claimid = 0,
            int claimno = 1,
            int employeeid = 0,
            string name = default(string),
            string description = default(string),
            int stage = 1,
            bool approved = false,
            bool paid = false,
            DateTime datesubmitted = default(DateTime),
            DateTime datepaid = default(DateTime),
            ClaimStatus status = ClaimStatus.None,
            int teamid = 0,
            int checkerid = 0,
            bool submitted = false,
            bool splitapprovalstage = false,
            int currencyid = default(int),
            SortedList<int, object> userdefined = null,
            string connString = default(string),
            SortedList<int, cExpenseItem> expenseItems = null)
        {
            ICurrentUser currentUser = Moqs.CurrentUser();

            name = name == default(string) ? "Unit Test Claim " + ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString() : name;
            description = (description == default(string) ? "Unit Test Claim" : description);
            datepaid = (datepaid == default(DateTime) ? DateTime.Parse("01/01/1900") : datepaid);
            datesubmitted = (datesubmitted == default(DateTime) ? DateTime.Parse("01/01/1900") : datesubmitted);
            
            return new cClaim(
                currentUser.AccountID,
                claimid,
                claimno,
                employeeid,
                name,
                description,
                stage,
                approved,
                paid,
                datesubmitted,
                datepaid,
                status,
                teamid,
                checkerid,
                submitted,
                splitapprovalstage,
                DateTime.UtcNow,
                currentUser.EmployeeID,
                DateTime.UtcNow,
                currentUser.EmployeeID,
                currencyid,
                string.Empty,
                false,
                string.Empty,
                1,
                false,
                false,
                false,
                false,
                false,
                5,
                DateTime.Now,
                DateTime.Now,
                0,
                0,
                0,
                0,
                0,
                0,
                false);
        }

        public static cClaim New(cClaim claim)
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cClaim retClaim;

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                connection.sqlexecute.Parameters.AddWithValue("@identity", System.Data.SqlDbType.Int);
                connection.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.Output;
                var strsql =
                    "INSERT INTO [claims_base] ([claimno] ,[employeeid] ,[approved] ,[paid] ,[datesubmitted] ,[description] ,[status]  ,[stage] ,[submitted] ,[name] ,[currencyid] ,[CreatedOn] ,[CreatedBy] ,[ModifiedOn] ,[ModifiedBy] ,[CacheExpiry] ,[ReferenceNumber] ,[splitApprovalStage]) VALUES (@claimno ,@employeeid ,@approved,@paid ,@datesubmitted,@description,@status,@stage,@submitted,@name,@currencyid,@createdon,@createdby,@modifiedon,@modifiedby,@cacheexpiry,@referencenumber,@splitapprovalstage);select @identity = @@identity";
                connection.sqlexecute.Parameters.AddWithValue("@claimno", claim.claimno);
                connection.sqlexecute.Parameters.AddWithValue("@employeeid", claim.employeeid);
                connection.sqlexecute.Parameters.AddWithValue("@approved", claim.approved);
                connection.sqlexecute.Parameters.AddWithValue("@paid", claim.paid);
                connection.sqlexecute.Parameters.AddWithValue("@datesubmitted", DateTime.Now);
                connection.sqlexecute.Parameters.AddWithValue("description", claim.description);
                connection.sqlexecute.Parameters.AddWithValue("@status", (int)claim.status);
                connection.sqlexecute.Parameters.AddWithValue("@stage", claim.stage);
                connection.sqlexecute.Parameters.AddWithValue("@submitted", claim.submitted);
                connection.sqlexecute.Parameters.AddWithValue("@name", claim.name);
                connection.sqlexecute.Parameters.AddWithValue("@currencyid", claim.currencyid);
                connection.sqlexecute.Parameters.AddWithValue("@createdon", claim.createdon);
                connection.sqlexecute.Parameters.AddWithValue("@createdby", claim.createdby);
                connection.sqlexecute.Parameters.AddWithValue("@modifiedon", claim.modifiedon);
                connection.sqlexecute.Parameters.AddWithValue("@modifiedby", claim.modifiedby);
                connection.sqlexecute.Parameters.AddWithValue("@cacheexpiry", DateTime.Now);
                connection.sqlexecute.Parameters.AddWithValue("@referencenumber", claim.ReferenceNumber ?? string.Empty);
                connection.sqlexecute.Parameters.AddWithValue("@splitapprovalstage", claim.splitApprovalStage);

                connection.ExecuteSQL(strsql);
                var claimId = (int)connection.sqlexecute.Parameters["@identity"].Value;
                if (claim.teamid > 0)
                {
                    strsql = string.Format(
                        "update [dbo].claims_base set teamid = {0} where claimid = {1}", claim.teamid, claimId);
                    connection.ExecuteSQL(strsql);
                }

                if (claim.checkerid > 0)
                {
                    strsql = string.Format(
                        "update [dbo].claims_base set checkerid = {0} where claimid = {1}", claim.checkerid, claimId);
                    connection.ExecuteSQL(strsql);
                }
                else
                {
                    strsql = string.Format(
                        "update [dbo].claims_base set checkerid = NULL where claimid = {0}", claimId);
                    connection.ExecuteSQL(strsql);
                }

                var newClaims = new cClaims(currentUser.AccountID);
                retClaim = newClaims.getClaimById(claimId);
            }

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
                        clsClaims.DeleteClaim(claim);
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
