namespace UnitTest2012Ultimate.API
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SpendManagementApi.Controllers;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Repositories;
    using Spend_Management;
    using Utilities;

    public abstract class BaseTests<T, TResponse, TDal>
        where T : BaseExternalType, new()
        where TResponse : ApiResponse<T>, new()
        where TDal : class 
    {

        #region Consts / Vars

        protected string Label = "Unit Test Label";
        protected string Description = "Unit Test Description";
        protected string LabelDescriptionMod = " Modified";
        protected ICurrentUser User;
        protected ControllerFactory<T> Factory;
        protected TestActionContext ActionContext;
        protected List<int> InitialIds;

        /// <summary>
        /// Getting this purposefully recreates the underlying Dal type, 
        /// since usually the cache needs resetting.
        /// </summary>
        protected TDal Dal
        {
            get
            {
                return (TDal) ActionContext.GetType()
                                        .GetProperties()
                                        .First(x => x.PropertyType == typeof (TDal))
                                        .GetValue(ActionContext);
            }
        }

        #endregion Consts / Vars

        #region Init / DeInit

        [TestInitialize]
        public void Initialise()
        {   
            GlobalAsax.Application_Start();
            
            // get the initial setup Ids
            var accountId = int.Parse(ConfigurationManager.AppSettings.Get("AccountID"));
            var userId = int.Parse(ConfigurationManager.AppSettings.Get("EmployeeID"));

            // get the account and user details
            User = cMisc.GetCurrentUser(accountId + "," + userId);
            
            // grab the factory
            if (ActionContext == null)
            {
                ActionContext = new TestActionContext();
            }

            var repo = RepositoryFactory.GetRepository<T>(new object[]{User, ActionContext});
            Factory = new ControllerFactory<T>(repo);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            GlobalAsax.Application_End();
        }

        #endregion Init / DeInit

        protected void AddEditDeleteFullCycle(T objectToCreate, Action<T> creationValidation, Func<T, T> propertiesToModify, Action<T> modificationValidation, Func<T, int> deleteId, Action<T> deletionValidation)
        {
            var response = Factory.Post<TResponse>(objectToCreate, true);
            Assert.AreEqual(response.ResponseInformation.Errors.Count, 0);
            creationValidation.Invoke(response.Item);
            propertiesToModify.Invoke(response.Item);
            response = Factory.Put<TResponse>(response.Item, true);
            Assert.AreEqual(response.ResponseInformation.Errors.Count, 0);
            modificationValidation.Invoke(response.Item);
            var id = deleteId.Invoke(response.Item);
            response = Factory.Delete<TResponse>(id, true);
            Assert.AreEqual(response.ResponseInformation.Errors.Count, 0);
            deletionValidation.Invoke(response.Item);
        }

        protected TResponse AddWithAssertions(T objectToAdd, Action<TResponse> assertions = null, bool initWithRealContext = true)
        {
            var response = Factory.Post<TResponse>(objectToAdd, initWithRealContext);
            if (assertions != null)
            {
                assertions.Invoke(response);
            }
            return response;
        }

        protected TResponse UpdateWithAssertions(T objectToUpdate, Action<TResponse> assertions = null, bool initWithRealContext = true)
        {
            var response = Factory.Put<TResponse>(objectToUpdate, initWithRealContext);
            if (assertions != null)
            {
                assertions.Invoke(response);
            }
            return response;
        }

        protected TResponse DeleteWithAssertions(int id, Action<TResponse> assertions = null, bool initWithRealContext = true)
        {
            var response = Factory.Delete<TResponse>(id, initWithRealContext);
            if (assertions != null)
            {
                assertions.Invoke(response);
            }
            return response;
        }
    }
}
