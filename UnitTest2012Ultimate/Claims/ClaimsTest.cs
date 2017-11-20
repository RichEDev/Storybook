using Spend_Management.expenses.code.Claims;

namespace UnitTest2012Ultimate
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Enumerators;

    using Spend_Management;
    using Spend_Management.expenses.code.Claims;

    using Syncfusion.XlsIO.Implementation.Collections;

    /// <summary>
    /// This is a test class for Claims and, at present, only contains
    /// tests for receipt processing functionality
    /// </summary>
    [TestClass]
    public class ClaimsTest
    {
        private TestContext testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get
            {
                return this.testContextInstance;
            }

            set
            {
                this.testContextInstance = value;
            }
        }

		#region Additional test attributes

        // You can use the following additional attributes as you write your tests:

        // Use ClassInitialize to run code before running the first test in the class
		[ClassInitialize]
		public static void MyClassInitialize(TestContext testContext)
		{
			GlobalAsax.Application_Start();
		}

        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }

		// Use TestInitialize to run code before running each test
		// [TestInitialize()]
		// public void MyTestInitialize()
		// {
		// }
		 
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup()
		// {
		// }
		
		#endregion


        /// <summary>
        /// Test that an invalid claim ID does not break SetClaimReferenceNumber
        /// </summary>
		[TestCategory("Spend Management"), TestCategory("Claims"), TestMethod]
        public void Claims_SetClaimReferenceNumber_UsingInvalidClaimId()
        {
            ICurrentUserBase currentUser = Moqs.CurrentUser();

			var claims = new cClaims(currentUser.AccountID);

		    string result = claims.SetClaimReferenceNumber(0);

            Assert.AreEqual("-1", result);
        }


        /// <summary>
        /// Test that a reference number is correctly set agaisnt a claim where no reference number exists
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Claims"), TestMethod]
        public void Claims_SetClaimReferenceNumber_WhereReferenceNumberDoesNotExist()
        {
            cClaim claim = null;
            
            try
        {
            ICurrentUserBase currentUser = Moqs.CurrentUser();

            var claims = new cClaims(currentUser.AccountID);

                claim = cClaimObject.New(cClaimObject.Template(employeeid: currentUser.EmployeeID));

                Assert.IsTrue(claim != null && claim.claimid > 0, "ClaimId is not valid - issue with cClaimObject");

                string result = claims.SetClaimReferenceNumber(claim.claimid);

                Assert.IsTrue(result.Length == 11);

                Assert.IsTrue(result.IndexOf("-", StringComparison.Ordinal) == 3 && result.LastIndexOf("-", StringComparison.Ordinal) == 7);

                Assert.IsTrue(Regex.IsMatch(result.Substring(0, 3), @"^[0-9]+$"));

                Assert.IsTrue(Regex.IsMatch(result.Substring(4, 3), @"^[a-zA-Z]+$"));

                Assert.IsTrue(Regex.IsMatch(result.Substring(8, 3), @"^[a-zA-Z]+$"));
        }
            finally
            {
                if (claim != null && claim.claimid > 0)
                {
                    cClaimObject.TearDown(claim.claimid);
                }
            }
        }


        /// <summary>
        /// Test that the correct reference number is returned from a claim where a reference number exists
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Claims"), TestMethod]
        public void Claims_SetClaimReferenceNumber_WhereReferenceNumberExists()
        {
            cClaim claim = null;

            try
            {
                ICurrentUserBase currentUser = Moqs.CurrentUser();

                var claims = new cClaims(currentUser.AccountID);

                claim = cClaimObject.New(cClaimObject.Template(employeeid: currentUser.EmployeeID));

                Assert.IsTrue(claim != null && claim.claimid > 0, "ClaimId is not valid - issue with cClaimObject");

                string firstReferenceNumber = claims.SetClaimReferenceNumber(claim.claimid);

                Assert.IsTrue(firstReferenceNumber.Length == 11, "Incorrect Reference number returned");

                var updatedClaims = new cClaims(currentUser.AccountID);

                string result = updatedClaims.SetClaimReferenceNumber(claim.claimid);

                Assert.IsTrue(result.IndexOf("-", StringComparison.Ordinal) == 3 && result.LastIndexOf("-", StringComparison.Ordinal) == 7);

                Assert.IsTrue(Regex.IsMatch(result.Substring(0, 3), @"^[0-9]+$"));

                Assert.IsTrue(Regex.IsMatch(result.Substring(4, 3), @"^[a-zA-Z]+$"));

                Assert.IsTrue(Regex.IsMatch(result.Substring(8, 3), @"^[a-zA-Z]+$"));
            }
            finally
            {
                if (claim != null && claim.claimid > 0)
                {
                    cClaimObject.TearDown(claim.claimid);
                }
            }
        }


        /// <summary>
        /// Test that an invalid claim ID does not break SetClaimEnvelopeNumbers
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Claims"), TestMethod]
        public void Claims_SetClaimEnvelopeNumbers_UsingInvalidClaimId()
        {
            ICurrentUserBase currentUser = Moqs.CurrentUser();

            var claims = new cClaims(currentUser.AccountID);

            var envelopeInfoList = new List<ClaimEnvelopeInfo>
                                       {
                                           new ClaimEnvelopeInfo()
                                               {
                                                   ClaimEnvelopeId = 0,
                                                   EnvelopeNumber = "A-ABC-123"
                                               }
                                       };

            var result = claims.SetClaimEnvelopeNumbers(envelopeInfoList, -1);

            Assert.IsTrue(result.OverallResult == false);
            }


        /// <summary>
        /// Test that passing an empty list of envelope numbers does not break SetClaimEnvelopeNumbers
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Claims"), TestMethod]
        public void Claims_SetClaimEnvelopeNumbers_WhereNoEnvelopeNumbersArePassed()
        {
            ICurrentUserBase currentUser = Moqs.CurrentUser();

            var claims = new cClaims(currentUser.AccountID);

            var result = claims.SetClaimEnvelopeNumbers(new List<ClaimEnvelopeInfo>(), 69);

            Assert.IsTrue(result.OverallResult == false);
        }


        /// <summary>
        /// Test that using invalid envelope numbers does not break SetClaimEnvelopeNumbers
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Claims"), TestMethod]
        public void Claims_SetClaimEnvelopeNumbers_WhereInvalidEnvelopeNumberIsPassed()
        {
            cClaim claim = null;

            try
            {
                ICurrentUserBase currentUser = Moqs.CurrentUser();

                var claims = new cClaims(currentUser.AccountID);

                claim = cClaimObject.New(cClaimObject.Template(employeeid: currentUser.EmployeeID));

                Assert.IsTrue(claim != null && claim.claimid > 0, "ClaimId is not valid - issue with cClaimObject");

                var envelopeInfoList = new List<ClaimEnvelopeInfo>
                                       {
                                           new ClaimEnvelopeInfo()
                                               {
                                                   ClaimEnvelopeId = 0,
                                                   EnvelopeNumber = "A-ABC-123-999-A"
                                               }
                                       };

                var result = claims.SetClaimEnvelopeNumbers(envelopeInfoList, claim.claimid);

                Assert.IsFalse(result.OverallResult);
            }
            finally
            {
                if (claim != null && claim.claimid > 0)
                {
                    cClaimObject.TearDown(claim.claimid);
                }
            }
        }


        /// <summary>
        /// Test that envelope numbers are correctly saved against a claim
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Claims"), TestMethod]
        public void Claims_SetClaimEnvelopeNumbers_WhereEnvelopeNumbersDoNotExist()
        {
            cClaim claim = null;

            try
            {
                ICurrentUserBase currentUser = Moqs.CurrentUser();

                var claims = new cClaims(currentUser.AccountID);

                claim = cClaimObject.New(cClaimObject.Template(employeeid: currentUser.EmployeeID));

                Assert.IsTrue(claim != null && claim.claimid > 0, "ClaimId is not valid - issue with cClaimObject");

                var envelopeInfoList = new List<ClaimEnvelopeInfo>
                                       {
                                           new ClaimEnvelopeInfo()
                                               {
                                                   ClaimEnvelopeId = 0,
                                                   EnvelopeNumber = "A-ABC-123"
                                               }, 
                                               new ClaimEnvelopeInfo()
                                                   {
                                                       ClaimEnvelopeId = 0,
                                                       EnvelopeNumber = "B-CBA-321"
                                                   }
                                       };

                var result = claims.SetClaimEnvelopeNumbers(envelopeInfoList, claim.claimid);

                Assert.IsTrue(result.OverallResult);

                var updatedClaims = new cClaims(currentUser.AccountID);

                List<ClaimEnvelopeInfo> resultList = updatedClaims.GetClaimEnvelopeNumbers(claim.claimid);

                Assert.IsTrue(resultList.Count == 2);

                // Results are returned in reverse order
                Assert.AreEqual(envelopeInfoList[0].EnvelopeNumber, resultList[1].EnvelopeNumber);

                Assert.AreEqual(envelopeInfoList[1].EnvelopeNumber, resultList[0].EnvelopeNumber);

                Assert.IsTrue(resultList[0].ClaimEnvelopeId != 0);

                Assert.IsTrue(resultList[1].ClaimEnvelopeId != 0);
            }
            finally
            {
                if (claim != null && claim.claimid > 0)
                {
                    cClaimObject.TearDown(claim.claimid);
                }
            }
        }


        /// <summary>
        /// Test that envelope numbers can be successfully updated on a claim
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Claims"), TestMethod]
        public void Claims_SetClaimEnvelopeNumbers_WhereEnvelopeNumbersExist()
        {
            cClaim claim = null;

            try
            {
                ICurrentUserBase currentUser = Moqs.CurrentUser();

                var claims = new cClaims(currentUser.AccountID);

                claim = cClaimObject.New(cClaimObject.Template(employeeid: currentUser.EmployeeID));

                Assert.IsTrue(claim != null && claim.claimid > 0, "ClaimId is not valid - issue with cClaimObject");

                var envelopeInfoList = new List<ClaimEnvelopeInfo>
                {
                                           new ClaimEnvelopeInfo()
                                               {
                                                   ClaimEnvelopeId = 0,
                                                   EnvelopeNumber = "A-ABC-123"
                                               }, 
                                               new ClaimEnvelopeInfo()
                                                   {
                                                       ClaimEnvelopeId = 0,
                                                       EnvelopeNumber = "B-CBA-321"
                                                   }
                                       };

                var result = claims.SetClaimEnvelopeNumbers(envelopeInfoList, claim.claimid);

                Assert.IsTrue(result.OverallResult);

                var updatedClaims = new cClaims(currentUser.AccountID);

                List<ClaimEnvelopeInfo> resultList = updatedClaims.GetClaimEnvelopeNumbers(claim.claimid);

                Assert.IsTrue(resultList.Count == 2);

                var updatedEnvelopeList = new List<ClaimEnvelopeInfo>
                                       {
                                           new ClaimEnvelopeInfo()
                                               {
                                                   ClaimEnvelopeId = resultList[1].ClaimEnvelopeId,
                                                   EnvelopeNumber = "A-ABC-123"
                                               }, 
                                               new ClaimEnvelopeInfo()
                                                   {
                                                       ClaimEnvelopeId = resultList[0].ClaimEnvelopeId,
                                                       EnvelopeNumber = "B-CBA-321"
                                                   }
                                       };

                var updatedResult = updatedClaims.SetClaimEnvelopeNumbers(updatedEnvelopeList, claim.claimid);

                Assert.IsTrue(updatedResult.OverallResult);

                var finalClaims = new cClaims(currentUser.AccountID);

                List<ClaimEnvelopeInfo> updatedResultList = finalClaims.GetClaimEnvelopeNumbers(claim.claimid);

                Assert.IsTrue(updatedResultList.Count == 2);

                // Results are returned in reverse order                
                Assert.AreEqual(envelopeInfoList[0].EnvelopeNumber, updatedResultList[1].EnvelopeNumber);

                Assert.AreEqual(envelopeInfoList[1].EnvelopeNumber, updatedResultList[0].EnvelopeNumber);

                Assert.AreEqual(resultList[0].ClaimEnvelopeId, updatedResultList[0].ClaimEnvelopeId);

                Assert.AreEqual(resultList[1].ClaimEnvelopeId, updatedResultList[1].ClaimEnvelopeId);
            }
            finally
            {
                if (claim != null && claim.claimid > 0)
                {
                    cClaimObject.TearDown(claim.claimid);
                }
            }
        }


        /// <summary>
        /// Test that envelope numbers are correctly removed when they are deleted
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Claims"), TestMethod]
        public void Claims_SetClaimEnvelopeNumbers_WhereEnvelopeNumbersHaveBeenRemoved()
        {
            cClaim claim = null;

            try
            {
                ICurrentUserBase currentUser = Moqs.CurrentUser();

                var claims = new cClaims(currentUser.AccountID);

                claim = cClaimObject.New(cClaimObject.Template(employeeid: currentUser.EmployeeID));

                Assert.IsTrue(claim != null && claim.claimid > 0, "ClaimId is not valid - issue with cClaimObject");

                var envelopeInfoList = new List<ClaimEnvelopeInfo>
                                       {
                                           new ClaimEnvelopeInfo()
                                               {
                                                   ClaimEnvelopeId = 0,
                                                   EnvelopeNumber = "A-ABC-123"
                                               }, 
                                               new ClaimEnvelopeInfo()
                                                   {
                                                       ClaimEnvelopeId = 0,
                                                       EnvelopeNumber = "B-CBA-321"
                                                   }
                                       };

                var result = claims.SetClaimEnvelopeNumbers(envelopeInfoList, claim.claimid);

                Assert.IsTrue(result.OverallResult);

                var updatedClaims = new cClaims(currentUser.AccountID);

                List<ClaimEnvelopeInfo> resultList = updatedClaims.GetClaimEnvelopeNumbers(claim.claimid);

                Assert.IsTrue(resultList.Count == 2);

                var updatedEnvelopeList = new List<ClaimEnvelopeInfo>
                                       {
                                           new ClaimEnvelopeInfo()
                                               {
                                                   ClaimEnvelopeId = resultList[1].ClaimEnvelopeId,
                                                   EnvelopeNumber = "A-ABC-123"
                                               }
                                       };

                var updatedResult = updatedClaims.SetClaimEnvelopeNumbers(updatedEnvelopeList, claim.claimid);

                Assert.IsTrue(updatedResult.OverallResult);

                var finalClaims = new cClaims(currentUser.AccountID);

                List<ClaimEnvelopeInfo> updatedResultList = finalClaims.GetClaimEnvelopeNumbers(claim.claimid);

                Assert.IsTrue(updatedResultList.Count == 1);

                // Results are returned in reverse order                
                Assert.AreEqual(envelopeInfoList[0].EnvelopeNumber, updatedResultList[0].EnvelopeNumber);

                Assert.AreEqual(resultList[1].ClaimEnvelopeId, updatedResultList[0].ClaimEnvelopeId);
            }
            finally
            {
                if (claim != null && claim.claimid > 0)
                {
                    cClaimObject.TearDown(claim.claimid);
                }
            }
        }


        /// <summary>
        /// Test that an invalid claim ID does not break GetClaimEnvelopeNumbers
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Claims"), TestMethod]
        public void Claims_GetClaimEnvelopeNumbers_UsingInvalidClaimId()
        {
            ICurrentUserBase currentUser = Moqs.CurrentUser();

                var claims = new cClaims(currentUser.AccountID);

            List<ClaimEnvelopeInfo> resultList = claims.GetClaimEnvelopeNumbers(-1);

            Assert.IsTrue(resultList.Count == 0);
        }


        /// <summary>
        /// Test that no envelope numbers are returned if none exist on a claim
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Claims"), TestMethod]
        public void Claims_GetClaimEnvelopeNumbers_WhereEnvelopeNumberDoNotExist()
        {
            cClaim claim = null;

            try
            {
                ICurrentUserBase currentUser = Moqs.CurrentUser();

                var claims = new cClaims(currentUser.AccountID);

                claims.ApproveClaim(claim, currentUser.EmployeeID, null, false);
                claims.payClaim(claim, currentUser.EmployeeID, null);
                var postClaimsCountAny = claims.getCount(currentUser.EmployeeID, ClaimStage.Any);
                var postClaimsCountSubmitted = claims.getCount(currentUser.EmployeeID, ClaimStage.Submitted);
                var postClaimsCountCurrent = claims.getCount(currentUser.EmployeeID, ClaimStage.Current);
                var postClaimsCountPrevious = claims.getCount(currentUser.EmployeeID, ClaimStage.Previous);
                claim = cClaimObject.New(cClaimObject.Template(employeeid: currentUser.EmployeeID));

                Assert.IsTrue(claim != null && claim.claimid > 0, "ClaimId is not valid - issue with cClaimObject");

                List<ClaimEnvelopeInfo> resultsList = claims.GetClaimEnvelopeNumbers(claim.claimid);

                Assert.IsTrue(resultsList.Count == 0);
            }
            finally
            {
                if (claim != null && claim.claimid > 0)
                {
                    cClaimObject.TearDown(claim.claimid);
                }
            }
        }


        /// <summary>
        /// Test that envelope numbers are correctly retrieved.
        /// This is pretty much the same test as Claims_SetClaimEnvelopeNumbers_WhereEnvelopeNumbersExist
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Claims"), TestMethod]
        public void Claims_GetClaimEnvelopeNumbers_WhereEnvelopeNumbersExist()
        {
            cClaim claim = null;

            try
            {
                ICurrentUserBase currentUser = Moqs.CurrentUser();

                var claims = new cClaims(currentUser.AccountID);

                claim = cClaimObject.New(cClaimObject.Template(employeeid: currentUser.EmployeeID));

                Assert.IsTrue(claim != null && claim.claimid > 0, "ClaimId is not valid - issue with cClaimObject");

                var envelopeInfoList = new List<ClaimEnvelopeInfo>
                                       {
                                           new ClaimEnvelopeInfo()
                                               {
                                                   ClaimEnvelopeId = 0,
                                                   EnvelopeNumber = "A-ABC-123"
                                               }, 
                                               new ClaimEnvelopeInfo()
                                                   {
                                                       ClaimEnvelopeId = 0,
                                                       EnvelopeNumber = "B-CBA-321"
                                                   }
                                       };

                var result = claims.SetClaimEnvelopeNumbers(envelopeInfoList, claim.claimid);

                Assert.IsTrue(result.OverallResult);

                var updatedClaims = new cClaims(currentUser.AccountID);

                List<ClaimEnvelopeInfo> resultList = updatedClaims.GetClaimEnvelopeNumbers(claim.claimid);

                Assert.IsTrue(resultList.Count == 2);

                claims.ApproveClaim(claim, currentUser.EmployeeID, null, false);
                claims.payClaim(claim, currentUser.EmployeeID, null);
                var postClaimsCount = claims.getCount();
                // Results are returned in reverse order
                Assert.AreEqual(envelopeInfoList[0].EnvelopeNumber, resultList[1].EnvelopeNumber);

                Assert.AreEqual(envelopeInfoList[1].EnvelopeNumber, resultList[0].EnvelopeNumber);

                Assert.IsTrue(resultList[0].ClaimEnvelopeId != 0);

                Assert.IsTrue(resultList[1].ClaimEnvelopeId != 0);
            }
            finally
            {
                if (claim != null && claim.claimid > 0)
                {
                    cClaimObject.TearDown(claim.claimid);
                }
            }
        }

        [TestMethod, TestCategory("Claims"), TestCategory("Spend Management")]
        public void cClaims_CheckerIsOnHoliday()
        {
            cClaim claim = null;

            try
        {
                var claims = new cClaims(GlobalTestVariables.AccountId);
                var currentUser = Moqs.CurrentUser();
                var claimSubmission = new ClaimSubmission(currentUser);
                claim = cClaimObject.New(cClaimObject.Template(employeeid: currentUser.EmployeeID));

                var signoffType = SignoffType.None;
                var relid = 1;
                var commenter = GlobalTestVariables.EmployeeId;
                var holidaySignOffType = SignoffType.None;
                var holidayid = 998877;

                for (byte i = 0; i < 4; i++)
            {
                    var includestage = false;

                    TestSignOffTypeToHoliday(claim, claimSubmission, ref includestage, ref signoffType, ref relid, commenter, i, holidaySignOffType, holidayid);

                    holidaySignOffType = SignoffType.Team;
                    TestSignOffTypeToHoliday(claim, claimSubmission, ref includestage, ref signoffType, ref relid, commenter, i, holidaySignOffType, holidayid);

                    holidaySignOffType = SignoffType.LineManager;
                    TestSignOffTypeToHoliday(claim, claimSubmission, ref includestage, ref signoffType, ref relid, commenter, i, holidaySignOffType, holidayid);

                    holidaySignOffType = SignoffType.Employee;
                    TestSignOffTypeToHoliday(claim, claimSubmission, ref includestage, ref signoffType, ref relid, commenter, i, holidaySignOffType, holidayid);

                    holidaySignOffType = SignoffType.DeterminedByClaimantFromApprovalMatrix;
                    TestSignOffTypeToHoliday(claim, claimSubmission, ref includestage, ref signoffType, ref relid, commenter, i, holidaySignOffType, holidayid);

                    holidaySignOffType = SignoffType.CostCodeOwner;
                    TestSignOffTypeToHoliday(claim, claimSubmission, ref includestage, ref signoffType, ref relid, commenter, i, holidaySignOffType, holidayid);

                    holidaySignOffType = SignoffType.ClaimantSelectsOwnChecker;
                    TestSignOffTypeToHoliday(claim, claimSubmission, ref includestage, ref signoffType, ref relid, commenter, i, holidaySignOffType, holidayid);

                    holidaySignOffType = SignoffType.BudgetHolder;
                    TestSignOffTypeToHoliday(claim, claimSubmission, ref includestage, ref signoffType, ref relid, commenter, i, holidaySignOffType, holidayid);

                    holidaySignOffType = SignoffType.AssignmentSignOffOwner;
                    TestSignOffTypeToHoliday(claim, claimSubmission, ref includestage, ref signoffType, ref relid, commenter, i, holidaySignOffType, holidayid);

                    holidaySignOffType = SignoffType.ApprovalMatrix;
                    TestSignOffTypeToHoliday(claim, claimSubmission, ref includestage, ref signoffType, ref relid, commenter, i, holidaySignOffType, holidayid);
                }
            }
            finally
            {
                if (claim != null && claim.claimid > 0)
            {
                    cClaimObject.TearDown(claim.claimid);
                }
            }
        }

        //public void TestCostCodeOwnerInTeamCannotApproveOwnClaim()
        //{
        //    var currentUser = Moqs.CurrentUser();
        //    var claimSubmission = new ClaimSubmission(currentUser);
        //    var expenseItems = new SortedList<int, cExpenseItem>();

        //    var claim = cClaimObject.New(cClaimObject.Template(employeeid: currentUser.EmployeeID,expenseItems:expenseItems));
        //    var costCodeBreakdown = new List<cDepCostItem>();
        //    var costCode = new cDepCostItem(0, 1, 0, 0);
        //    var expItem = cExpenseItemObject.Template(1 ,claim.claimid, costcodebreakdown:costCodeBreakdown);
        //    var result = claimSubmission.GetClaimItemCheckerIds(claim.claimid, GlobalTestVariables.EmployeeId, claim.employeeid,
        //        GlobalTestVariables.SubAccountId);
        //}

        private static void TestSignOffTypeToHoliday(cClaim claim, ClaimSubmission claimSubmission, ref bool includestage, ref SignoffType signoffType, ref int relid, int commenter, byte i, SignoffType holidaySignOffType, int holidayid)
        {
            var stage = new cStage(
                1,
                SignoffType.Employee,
                1,
                1,
                StageInclusionType.Always,
                0,
                1,
                1,
                i,
                holidaySignOffType,
                holidayid,
                0,
                false,
                false,
                false,
                false,
                DateTime.Now,
                GlobalTestVariables.EmployeeId,
                DateTime.Now,
                0,
                false,
                false,
                false,
                false,
                false,
                null);
            var startSignOffType = signoffType;
            var startRelId = relid;
            claimSubmission.CheckerIsOnHoliday(claim, stage, ref includestage, ref signoffType, ref relid, commenter);
            if (i == 1 || i == 3)
            {
                Assert.IsTrue(includestage, "include stage should be true for onholiday =  {0}", i);
            }
            else
            {
                Assert.IsFalse(includestage, "include stage should be false for onholiday =  {0}", i);
            }

            if (i == 3)
            {
                Assert.IsTrue(signoffType == holidaySignOffType, "Signoff type should be set to {0}, but is actually {1}", signoffType, holidaySignOffType);
                Assert.IsTrue(relid == holidayid, "relid should be set to {0}, but is actually {1}", relid, holidayid);
            }
            else
            {
                Assert.IsTrue(signoffType == startSignOffType, "Signoff type should be set to {0}, but is actually {1}", signoffType, startSignOffType);
                Assert.IsTrue(relid == startRelId, "relid should be set to {0}, but is actually {1}", relid, startRelId);
            }
        }

    }
}