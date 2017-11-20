using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpendManagementApi.Controllers;
using SpendManagementApi.Models.Common;
using SpendManagementApi.Models.Requests;
using SpendManagementApi.Models.Responses;
using SpendManagementApi.Utilities;
using SpendManagementLibrary;
using SpendManagementLibrary.Enumerators;
using Spend_Management;
using Utilities.Cryptography;

namespace UnitTest2012Ultimate.API
{
    using System.Web;
    using System.Web.Http;

    using Moq;

    using SpendManagementApi.Controllers.V1;

    [TestClass]
    public class AccountTests
    {
        #region Consts / Vars

        private const string ApiAccessRoleName = "ApiUser";

        private int userId;
        private int accountId;
        private string userPw;

        private cAccounts accounts;
        private cAccount account;

        private cAccessRoles roles;
        private cAccessRole role;

        private ICurrentUser user;

        private LoginRequest request;
        private LoginResponse response;
        private AccountV1Controller controller;
        

        #endregion Consts / Vars

        #region Init / Cleanup

        [TestInitialize]
        public void Initialise()
        {
            GlobalAsax.Application_Start();

            // get the initial setup Ids
            accountId = int.Parse(ConfigurationManager.AppSettings.Get("AccountID"));
            userId = int.Parse(ConfigurationManager.AppSettings.Get("EmployeeID"));
            userPw = ExpensesCryptography.Decrypt("vlUnCTgO1bFJXxfib/suGQ==");
            
            // get the account and user details
            accounts = new cAccounts();
            accounts.CacheList();
            account = accounts.GetAccountByID(accountId);
            user = cMisc.GetCurrentUser(accountId + "," + userId);
            
            // make sure there is a role that permits the user to access the API.
            roles = new cAccessRoles(accountId, account.ConnectionString);
            role = roles.GetAccessRoleByName(ApiAccessRoleName);

            // create if doesn't exist
            if (role == null)
            {
                var apiElementAccess = new cElementAccess((int)SpendManagementElement.Api, true, true, true, true);
                var roleId = roles.SaveAccessRoleApi(user.EmployeeID, 0, ApiAccessRoleName, "Test Api Access", 0,
                    new List<cElementAccess> {apiElementAccess}, null, null, false, false, false, false, null, null, null, false, false, true);
                role = roles.GetAccessRoleByID(roleId);
            }

            // make sure the user has this role.
            if (!user.CheckAccessRoleApi(SpendManagementElement.None, AccessRoleType.View, AccessRequestType.Api))
            {
                user.Employee.GetAccessRoles().Add(new List<int> {role.RoleID}, user.CurrentSubAccountId, user);
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            
            GlobalAsax.Application_End();
        }

        #endregion Init / Cleanup

        #region Test Methods

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("AccountController")]
        public void CorrectCredentialsSucceedsWithAuthToken()
        {
            try
            {
                
                response = Login(user.Employee.Username, userPw, account.companyid);
                
                // assert
                Assert.IsNotNull(response.AuthToken);
                Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Success);
            }
            catch (HttpResponseException exception)
            {
                Assert.Fail("Login Failed: " + exception.Response);                
            }
        }

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("AccountController")]
        public void LoginFailsDueToBadAccount()
        {
            try
            {
                // expect this to throw...
                response = Login(user.Employee.Username, userPw, "0");
            }
            catch (HttpResponseException error)
            {
                // assert
                Assert.AreEqual(error.Response.StatusCode, HttpStatusCode.Unauthorized);
            }

            //HttpResponseException ex = Assert.Throws<HttpResponseException>(() => Login(user.Employee.Username, userPw, "0"));
            //Assert.That(ex.Response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("AccountController")]
        [ExpectedException(typeof(HttpResponseException), ApiResources.HttpStatusCodeUnauthorised)]
        public void LoginFailsDueToBadPassword()
        {
            // expect this to throw...
            response = Login(user.Employee.Username, "", account.companyid);            
        }

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("AccountController")]
        [ExpectedException(typeof(HttpResponseException), ApiResources.HttpStatusCodeForbidden)]
        public void LoginFailsDueToWrongAccessRole()
        {
            var accessRoles = new cAccessRoles(GlobalTestVariables.AccountId, cAccounts.getConnectionString(GlobalTestVariables.AccountId));
            // must remove the user from the role 
            if (user.CheckAccessRoleApi(SpendManagementElement.None, AccessRoleType.View, AccessRequestType.Api))
            {
                user.Employee.AdminOverride = false;
                
                foreach (var aRoleId in user.Employee.GetAccessRoles().GetBy(user.CurrentSubAccountId))
                {
                    cAccessRole accessRole = accessRoles.GetAccessRoleByID(aRoleId);
                    if (accessRole.ElementAccess.ContainsKey(SpendManagementElement.Api))
                    {
                        user.Employee.GetAccessRoles().Remove(aRoleId, user.CurrentSubAccountId, user);
                    }
                }
            }

            // expect this to throw...
            response = Login(user.Employee.Username, userPw, account.companyid);
        }

        

        #endregion Test Methods


        #region Utils

        private LoginResponse Login(string username, string password, string company)
        {
            using (controller = new AccountV1Controller(accounts))
            {
                controller.Request = new HttpRequestMessage(HttpMethod.Post, "Login");
                var moqObj = new Mock<HttpContextBase>();
                moqObj.SetupGet(r => r.Request.UserHostAddress).Returns("127.0.0.1");
                controller.Request.Properties.Add("MS_HttpContext", moqObj.Object);

                request = new LoginRequest
                {
                    Company = company,
                    Username = username,
                    Password = password
                };

                response = controller.Login(request);
            }

            return response;
        }

        #endregion Utils
    }
}
