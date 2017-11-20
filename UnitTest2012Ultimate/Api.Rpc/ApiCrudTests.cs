using System;
using System.Collections.Generic;

namespace UnitTest2012Ultimate.Api.Rpc
{
    using EsrGo2FromNhsWcfLibrary.Base;
    using EsrGo2FromNhsWcfLibrary.Crud;
    using EsrGo2FromNhsWcfLibrary.ESR;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The API CRUD TESTS.
    /// Update database by calling APICrudBase<typeparam name="DataClassBase"></typeparam> Directly
    /// </summary>
    [TestClass]
    public class ApiCrudTests
    {
        /// <summary>
        /// The test context instance.
        /// </summary>
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

        /// <summary>
        /// The my class initialize.
        /// </summary>
        /// <param name="testContext">
        /// The test context.
        /// </param>
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            GlobalAsax.Application_Start();
        }

        /// <summary>
        /// Use ClassCleanup to run code after all tests in a class have run
        /// </summary>
        [ClassCleanup]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }

        #endregion
       
        /// <summary>
        /// The API RPC create organisations.
        /// Create a lot of transactions to load test the service
        /// </summary>
        [TestMethod]
        [TestCategory("ApiRpc"), TestCategory("ApiCrudInterface")]
        public void ApiRpcCreateOrganisations()
        {
            const int RecordstoCreate = 100;
            var organisations = new List<EsrOrganisation>();
            for (int i = 1; i <= RecordstoCreate; i++)
            {
                var organisation = new EsrOrganisation
                                       {
                                           Action = Action.Create,
                                           ESROrganisationId = i,
                                           EffectiveFrom = DateTime.Now,
                                           OrganisationName = string.Format("UT_Loadtest_{0}", i)
                                       };
                organisations.Add(organisation);
            }

            var crudBase = new EsrOrganisationCrud("metabase", GlobalTestVariables.AccountId);

            try
            {    
                var result = crudBase.Create(organisations);
                Assert.IsTrue(result != null);
                Assert.IsTrue(result.Count == RecordstoCreate);
            }
            finally
            {
                foreach (EsrOrganisation organisation in organisations)
                {
                    organisation.Action = Action.Delete;
                    crudBase.Delete(organisation.ESROrganisationId);
                }
            }
        }
    }
}
