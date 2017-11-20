using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.HtmlControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpendManagementLibrary;
using Spend_Management;
using UnitTest2012Ultimate.DatabaseMock;

namespace UnitTest2012Ultimate.Shared
{
    [TestClass]
    public class cTeamsTests
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes

        [TestInitialize()]
        public void MyTestInitialize()
        {
        }
        [TestCleanup]
        public void TestCleanup()
        {
        }
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext context)
        {
            GlobalAsax.Application_Start();
        }

        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }

        #endregion
        [TestMethod, TestCategory("Spend Management"), TestCategory("Teams")]
        public void cTeamsGetTeamFromDatabaseTest()
        {
            const string teamSql = "select teamid, teamname, description, teamleaderid, createdon, createdby, modifiedon, modifiedby from dbo.teams WHERE teamid = @teamid";

            var team = new cTeam(GlobalTestVariables.AccountId, 999, "UTTestTeam", "UTTEstTEam",
                new List<int> {GlobalTestVariables.EmployeeId}, GlobalTestVariables.EmployeeId, DateTime.Now, null, null, null);

            var teams = new List<object> { team };
            var reader = Reader.MockReaderDataFromClassData(teamSql, teams).AddAlias<cTeam>("teamleaderid", t => team.teamLeaderId).AddAlias<cTeam>("createdon", d => DateTime.Now).AddAlias<cTeam>("createdby", d => GlobalTestVariables.EmployeeId).AddAlias<cTeam>("modifiedon", d => DateTime.Now).AddAlias<cTeam>("modifiedby", d => GlobalTestVariables.EmployeeId);

            const string teamEmpSql = "select employeeid from teamemps WHERE teamid = @teamid";

            var teamEmps = new List<object> { };
            var reader2 = Reader.MockReaderDataFromClassData(teamEmpSql, teamEmps);

            var database = Reader.NormalDatabase(new[] { reader, reader2 });
            var cteams = new cTeams(GlobalTestVariables.AccountId);
            var result = cteams.GetTeamFromDatabase(999, database.Object);
            cteams.RemoveCache(999);
            Assert.IsNotNull(result);
            Assert.AreEqual(team.description, result.description);
        }

        [TestMethod, TestCategory("Spend Management"), TestCategory("Teams")]
        public void cTeamsgetTeamidByNameTest()
        {
            const string teamSql = "select teamid from teams where teamname = @name";

            var team = new cTeam(GlobalTestVariables.AccountId, 999, "UTTestTeam", "UTTEstTEam",
                new List<int> { GlobalTestVariables.EmployeeId }, GlobalTestVariables.EmployeeId, DateTime.Now, null, null, null);

            var teams = new List<object> { team };
            var reader = Reader.MockReaderDataFromClassData(teamSql, teams).AddAlias<cTeam>("teamleaderid", t => team.teamLeaderId).AddAlias<cTeam>("createdon", d => DateTime.Now).AddAlias<cTeam>("createdby", d => GlobalTestVariables.EmployeeId).AddAlias<cTeam>("modifiedon", d => DateTime.Now).AddAlias<cTeam>("modifiedby", d => GlobalTestVariables.EmployeeId);

            var database = Reader.NormalDatabase(new[] { reader });
            var cteams = new cTeams(GlobalTestVariables.AccountId);
            var result = cteams.getTeamidByName("UTTestTeam", database.Object);
            cteams.RemoveCache(999);
            Assert.IsNotNull(result);
            Assert.AreEqual(999, result);
        }

        [TestMethod, TestCategory("Spend Management"), TestCategory("Teams")]
        public void cTeamsCreateDropDownTest()
        {
            const string teamSql = "select teamname, teamid from teams order by teamname";

            var team = new cTeam(GlobalTestVariables.AccountId, 999, "UTTestTeam", "UTTEstTEam",
                new List<int> { GlobalTestVariables.EmployeeId }, GlobalTestVariables.EmployeeId, DateTime.Now, null, null, null);
            var team2 = new cTeam(GlobalTestVariables.AccountId, 1999, "UTTestTeam2", "UTTEstTEam2",
               new List<int> { GlobalTestVariables.EmployeeId }, GlobalTestVariables.EmployeeId, DateTime.Now, null, null, null);
            var teams = new List<object> { team, team2 };
            var reader = Reader.MockReaderDataFromClassData(teamSql, teams).AddAlias<cTeam>("teamleaderid", t => team.teamLeaderId).AddAlias<cTeam>("createdon", d => DateTime.Now).AddAlias<cTeam>("createdby", d => GlobalTestVariables.EmployeeId).AddAlias<cTeam>("modifiedon", d => DateTime.Now).AddAlias<cTeam>("modifiedby", d => GlobalTestVariables.EmployeeId);

            var database = Reader.NormalDatabase(new[] { reader });
            var cteams = new cTeams(GlobalTestVariables.AccountId);
            var result = cteams.CreateDropDown(-1, database.Object);
            cteams.RemoveCache(999);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() == 2);
        }

        [TestMethod, TestCategory("Spend Management"), TestCategory("Teams")]
        public void cTeamsgetTeamMemberGridTest()
        {
            var team = new cTeam(GlobalTestVariables.AccountId, 9999, "UTTestTeam", "UTTEstTEam",
                new List<int> { GlobalTestVariables.EmployeeId }, GlobalTestVariables.EmployeeId, DateTime.Now, null, null, null);
            var cteams = new cTeams(GlobalTestVariables.AccountId);
            cteams.AddCache(team);
            var result = cteams.getTeamMemberGrid(9999);
            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrEmpty(result));
            cteams.RemoveCache(9999);
            try
            {
                var html = new HtmlElement {InnerHtml = result};
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}
