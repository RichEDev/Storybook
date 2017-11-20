using System.Linq;

namespace UnitTest2012Ultimate.API
{
    using System.Collections.Generic;
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Utilities;
    using Spend_Management;

    [TestClass]
    public class TeamTests : BaseTests<Team, TeamResponse, cTeams>
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("TeamsController")]
        public void RoundTrip()
        {
            var item = GenerateBasicWorkingItems();

            try
            {
                AddEditDeleteFullCycle(
                    item,
                    added =>
                    {
                        Assert.IsNotNull(added);
                        Assert.AreNotEqual(added.Id, 0);
                    },
                    toMod =>
                    {
                        toMod.Label += LabelDescriptionMod;
                        toMod.Description += LabelDescriptionMod;
                        return toMod;
                    },
                    modified =>
                    {
                        Assert.AreEqual(modified.Label, Label + LabelDescriptionMod);
                        Assert.AreEqual(modified.Description, Description + LabelDescriptionMod);
                    },
                    toDelete =>
                    {
                        InitialIds.Remove(toDelete.Id);
                        return toDelete.Id;
                    },
                    Assert.IsNull);
            }
            finally
            {
                CleanupAnyOutstanding();
            }
        }

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("TeamsController")]
        [ExpectedException(typeof(ApiException), ApiResources.ApiErrorWrongEmployeeIdMessage)]
        public void TeamWithBadEmployeeFails()
        {
            var item = GenerateBasicWorkingItems();
            item.TeamMembers.Add(-111);

            try
            {
                AddWithAssertions(item);
            }
            finally
            {
                CleanupAnyOutstanding();
            }
        }

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("TeamsController")]
        [ExpectedException(typeof(ApiException), ApiResources.ApiErrorWrongTeamLeaderIdMessage)]
        public void TeamWithBadLeaderFails()
        {
            var item = GenerateBasicWorkingItems();
            item.TeamLeaderId = -111;

            try
            {
                AddWithAssertions(item);
            }
            finally
            {
                CleanupAnyOutstanding();
            }
        }

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("TeamsController")]
        [ExpectedException(typeof(ApiException), ApiResources.ApiErrorRecordAlreadyExistsMessage)]
        public void TeamWithExistingNameFails()
        {
            var item = GenerateBasicWorkingItems();

            try
            {
                // add the first
                AddWithAssertions(item);

                item.Id = 0;

                // attempt to add the other
                AddWithAssertions(item);

            }
            finally
            {
                CleanupAnyOutstanding();
            }
        }

        #endregion Test Methods

        #region Utilities

        /// <summary>
        /// Returns and allowance with correct properties for AccountId, CurrencyId, EmployeeId, Label, and Description.
        /// The AllowanceRates property is set to null. The instance variables dal and intiialAllowanceIds are also set...
        /// </summary>
        /// <returns>A new Allowance object populated as per summary.</returns>
        private Team GenerateBasicWorkingItems()
        {
            CleanupAnyOutstanding();
            InitialIds = Dal.GetAllTeams().Select(x => x.teamid).ToList();

            return new Team
            {
                AccountId = User.AccountID,
                Label = Label,
                Description = Description,
                EmployeeId = User.EmployeeID
            };
        }

        private void CleanupAnyOutstanding()
        {
            var dal = Dal;

            // remove the original add.
            var newList = dal.GetAllTeams().Select(x => x.teamid).ToList();
            var toRemove = new List<int>();
            if (InitialIds != null)
            {
                newList.ForEach(id =>
                {
                    if (!InitialIds.Contains(id))
                    {
                        dal.DeleteTeam(id);
                        toRemove.Add(id);
                        dal = Dal;
                    }
                });
            }

            toRemove.ForEach(i => newList.Remove(i));
            newList.ForEach(id =>
            {
                var item = dal.GetTeamFromDatabase(id);
                if (item != null &&
                    (item.teamname == Label || item.teamname == Label + LabelDescriptionMod))
                {
                    dal.DeleteTeam(id);
                }
            });
        }

        #endregion Utilities
    }
}
