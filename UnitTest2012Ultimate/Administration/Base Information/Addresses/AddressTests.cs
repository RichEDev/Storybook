namespace UnitTest2012Ultimate.Administration.Base_Information.Addresses
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Addresses;
    using SpendManagementLibrary.Interfaces;

    using Spend_Management;

    [TestClass]
    public class AddressTests
    {
        #region Test Properties and methods

        public TestContext TestContext { get; set; }

        /// <summary>
        /// Clean up the testing environment.
        /// </summary>
        [ClassCleanup]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }

        /// <summary>
        /// Set up the testing environment.
        /// </summary>
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            GlobalAsax.Application_Start();
        }

        #endregion Test Properties and methods

        // ReSharper disable InconsistentNaming
        // ReSharper disable RedundantArgumentName

        #region Contructor Tests

        [TestMethod]
        [TestCategory("Spend Management Library")]
        [TestCategory("Addresses")]
        public void Address_Address_NewObject()
        {
            DateTime startDateTime = DateTime.UtcNow;

            Address address = new Address();

            Assert.IsNotNull(address);
            Assert.IsInstanceOfType(address, typeof(Address));
            Assert.AreEqual(0, address.Identifier);
            Assert.IsTrue(startDateTime <= address.CreatedOn);
        }

        #endregion

        #region Public Methods and Operators Tests

        [TestMethod]
        [TestCategory("Spend Management Library")]
        [TestCategory("Addresses")]
        public void Address_Get_IntMaxValueAddressIdentifier()
        {
            Assert.IsNull(Address.Get(accountIdentifier: GlobalTestVariables.AccountId, addressIdentifier: int.MaxValue, connection: GetBlankReadConnection()));
        }

        [TestMethod]
        [TestCategory("Spend Management Library")]
        [TestCategory("Addresses")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Address_Get_InvalidAddressIdentifier()
        {
            Address.Get(accountIdentifier: GlobalTestVariables.AccountId, addressIdentifier: -1);
        }

        [TestMethod]
        [TestCategory("Spend Management Library")]
        [TestCategory("Addresses")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Address_Get_InvalidGlobalIdentifier()
        {
            Address.Get(currentUser: Moqs.CurrentUser(), globalIdentifier: string.Empty, countries: null);
        }

        [TestMethod]
        [TestCategory("Spend Management Library")]
        [TestCategory("Addresses")]
        public void Address_GetByReservedKeyword_InvalidKeyword()
        {
            var subaccs = new cAccountSubAccounts(GlobalTestVariables.AccountId);
            cAccountSubAccount subaccount = subaccs.getSubAccountById(GlobalTestVariables.SubAccountId);
            cAccountProperties subAccountProperties = subaccount.SubAccountProperties.Clone();

            Assert.IsNull(Address.GetByReservedKeyword(currentUser: Moqs.CurrentUser(), keyword: string.Empty, date: DateTime.UtcNow, accountProperties: subAccountProperties));
        }

        [TestMethod]
        [TestCategory("Spend Management Library")]
        [TestCategory("Addresses")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Address_Delete_InvalidAddressObject()
        {
            Address.Delete(Moqs.CurrentUser(), -982091);
        }

        [TestMethod]
        [TestCategory("Spend Management Library")]
        [TestCategory("Addresses")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Address_ToggleArchive_InvalidAddressObject()
        {
            Address.ToggleArchive(Moqs.CurrentUser(), -982091);
        }

        [TestMethod]
        [TestCategory("Spend Management Library")]
        [TestCategory("Addresses")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Address_ToggleAccountWideFavourite_InvalidAddressObject()
        {
            Address.ToggleAccountWideFavourite(Moqs.CurrentUser(), -982091);
        }

        [TestMethod]
        [TestCategory("Spend Management Library")]
        [TestCategory("Addresses")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Address_Static_Save_InvalidAddressObject()
        {
            Address.Save(Moqs.CurrentUser(), -982091, string.Empty, "", "", "", "", "", -109, "", "", "", null, 0, false);
        }

        [TestMethod]
        [TestCategory("Spend Management Library")]
        [TestCategory("Addresses")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Address_Save_InvalidAddressObject()
        {
            Address address = new Address { Identifier = -982091, Line1 = "1 Line Address", Line2 = "Line 2", Line3 = "Line 3", City = "City", County = "County", Country = 0, Postcode = "PC1 1PC", Latitude = "", Longitude = "", GlobalIdentifier = string.Empty, CreationMethod = Address.AddressCreationMethod.ManualByAdministrator, AccountWideFavourite = false};

            address.Save(Moqs.CurrentUser());
        }

        #endregion

        #region Property Tests

        [TestMethod]
        [TestCategory("Spend Management Library")]
        [TestCategory("Addresses")]
        public void Address_FriendlyName_NewObject()
        {
            Address address = new Address();

            Assert.AreEqual(string.Empty, address.FriendlyName);
        }

        [TestMethod]
        [TestCategory("Spend Management Library")]
        [TestCategory("Addresses")]
        public void Address_FriendlyName_LineOneSetToEmpty()
        {
            Address address = new Address
            {
                Line1 = string.Empty
            };

            Assert.AreEqual(string.Empty, address.FriendlyName);
        }

        [TestMethod]
        [TestCategory("Spend Management Library")]
        [TestCategory("Addresses")]
        public void Address_FriendlyName_LineOneSetToWhitespace()
        {
            Address address = new Address
            {
                Line1 = "  "
            };

            Assert.AreEqual(string.Empty, address.FriendlyName);
        }

        [TestMethod]
        [TestCategory("Spend Management Library")]
        [TestCategory("Addresses")]
        public void Address_FriendlyName_NoLineOneAndPostcodeSetNormally()
        {
            Address address = new Address
            {
                Postcode = "PC1 1AB"
            };

            Assert.AreEqual("PC1 1AB", address.FriendlyName);
        }

        [TestMethod]
        [TestCategory("Spend Management Library")]
        [TestCategory("Addresses")]
        public void Address_FriendlyName_LineOneSetNormallyAndNoPostcode()
        {
            Address address = new Address
            {
                Line1 = "Line1"
            };

            Assert.AreEqual("Line1", address.FriendlyName);
        }

        [TestMethod]
        [TestCategory("Spend Management Library")]
        [TestCategory("Addresses")]
        public void Address_FriendlyName_PostcodeSetToEmpty()
        {
            Address address = new Address
            {
                Line1 = "Line1",
                Postcode = string.Empty
            };

            Assert.AreEqual("Line1", address.FriendlyName);
        }

        [TestMethod]
        [TestCategory("Spend Management Library")]
        [TestCategory("Addresses")]
        public void Address_FriendlyName_PostcodeSetToWhitespace()
        {
            Address address = new Address
            {
                Line1 = "Line1",
                Postcode = "  "
            };

            Assert.AreEqual("Line1", address.FriendlyName);
        }

        [TestMethod]
        [TestCategory("Spend Management Library")]
        [TestCategory("Addresses")]
        public void Address_FriendlyName_LineOneAndPostcodeSetNormally()
        {
            Address address = new Address
            {
                Line1 = "Line1",
                Postcode = "PC1 1AB"
            };

            Assert.AreEqual("Line1, PC1 1AB", address.FriendlyName);
        }

        #endregion

        // ReSharper restore RedundantArgumentName
        // ReSharper restore InconsistentNaming

        #region Methods

        private static IDBConnection GetBlankReadConnection()
        {
            Mock<IDBConnection> connection = new Mock<IDBConnection>();
            Mock<IDataReader> mockReader = new Mock<IDataReader>();

            connection.SetupAllProperties();
            mockReader.SetupAllProperties();

            mockReader.Setup(x => x.Read()).Returns(false);
            mockReader.Setup(x => x.GetOrdinal(It.IsAny<string>())).Returns(0);
            mockReader.Setup(x => x.IsDBNull(It.IsInRange(0, 20, Range.Inclusive))).Returns(true);
            
            connection.SetupGet(x => x.sqlexecute).Returns(new SqlCommand());
            connection.Setup(x => x.GetReader(It.IsAny<string>(), CommandType.StoredProcedure)).Returns(mockReader.Object);
            connection.Setup(x => x.GetReader(It.IsAny<string>(), CommandType.Text)).Returns(mockReader.Object);

            return connection.Object;
        }

        #endregion
    }
}