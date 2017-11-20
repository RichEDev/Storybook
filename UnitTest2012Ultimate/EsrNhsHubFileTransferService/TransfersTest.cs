namespace UnitTest2012Ultimate.EsrNhsHubFileTransferService
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using global::EsrNhsHubFileTransferService.Code.Transfers;

    /// <summary>
    ///     The Transfers test.
    /// </summary>
    [TestClass]
    public class TransfersTest
    {
        /// <summary>
        ///     Gets or sets the test context.
        /// </summary>
        public TestContext TestContext { get; set; }

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
            //GlobalAsax.Application_Start();
        }

        /// <summary>
        /// Use ClassCleanup to run code after all tests in a class have run.
        /// </summary>
        [ClassCleanup]
        public static void MyClassCleanup()
        {
            //GlobalAsax.Application_End();
        }

        /// <summary>
        /// Use TestInitialize to run code before running each test.
        /// </summary>
        [TestInitialize]
        public void MyTestInitialize() {}

        /// <summary>
        /// Use TestCleanup to run code after each test has run
        /// </summary>
        [TestCleanup]
        public void MyTestCleanup() {}
        
        #endregion

// ReSharper disable InconsistentNaming

        /// <summary>
        ///     A test for ValidateFileBasics
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESRv2"), TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Transfers_ValidateFileBasics_NullBytes()
        {
            byte[] bytes = null;
            string returnMessage;

            Transfers.ValidateFileBasics(ref bytes, out returnMessage);
        }

        /// <summary>
        ///     A test for ValidateFileBasics
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESRv2"), TestMethod]
        public void Transfers_ValidateFileBasics_NoBytes()
        {
            byte[] bytes = new byte[0];
            string returnMessage;

            bool actual = Transfers.ValidateFileBasics(ref bytes, out returnMessage);

            Assert.IsNotNull(actual);
            Assert.AreEqual(false, actual);
            Assert.AreEqual("Invalid file. Minimum number of rows (i.e. 3) are not present.", returnMessage);
        }

        /// <summary>
        ///     A test for ValidateFileBasics
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESRv2"), TestMethod]
        public void Transfers_ValidateFileBasics_TwoRowsWhitespaceFile()
        {
            /*
             * _
             * _
             */
            byte[] bytes = new byte[] { 0x20, 0x0A, 0x20 };
            string returnMessage;

            bool actual = Transfers.ValidateFileBasics(ref bytes, out returnMessage);

            Assert.IsNotNull(actual);
            Assert.AreEqual(false, actual);
            Assert.AreEqual("Invalid file. Minimum number of rows (i.e. 3) are not present.", returnMessage);
        }

        /// <summary>
        ///     A test for ValidateFileBasics
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESRv2"), TestMethod]
        public void Transfers_ValidateFileBasics_NotEnoughCharacters()
        {
            /*
             * 
             * 
             * 
             */
            byte[] bytes = new byte[]
                                {
                                    0x0a,
                                    0x0a,
                                    0x0a
                                };
            string returnMessage;

            bool actual = Transfers.ValidateFileBasics(ref bytes, out returnMessage);

            Assert.IsNotNull(actual);
            Assert.AreEqual(false, actual);
            Assert.AreEqual("Invalid file. The file does not contain enough characters to find a delimiter.", returnMessage);
        }

        /// <summary>
        ///     A test for ValidateFileBasics
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESRv2"), TestMethod]
        public void Transfers_ValidateFileBasics_InvalidDelimeter()
        {
            /*
             * HDR-GOBlahBlah-01
             * Some-Entries-In-The-Row
             * TRL-3
             */
            byte[] bytes = new byte[]
                                {
                                    0x48, 0x44, 0x52, 0x2d, 0x47, 0x4f, 0x42, 0x6c, 0x61, 0x68, 0x42, 0x6c, 0x61, 0x68, 0x2d, 0x30, 0x31, 0x0a,
                                    0x53, 0x6f, 0x6d, 0x65, 0x2d, 0x45, 0x6e, 0x74, 0x72, 0x69, 0x65, 0x73, 0x2d, 0x49, 0x6e, 0x2d, 0x54, 0x68, 0x65, 0x2d, 0x52, 0x6f, 0x77, 0x0a,
                                    0x54, 0x52, 0x4c, 0x2d, 0x33
                                };
            string returnMessage;

            bool actual = Transfers.ValidateFileBasics(ref bytes, out returnMessage);

            Assert.IsNotNull(actual);
            Assert.AreEqual(false, actual);
            Assert.AreEqual("Invalid file. An incorrect delimiter is being used.", returnMessage);
        }

        /// <summary>
        ///     A test for ValidateFileBasics
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESRv2"), TestMethod]
        public void Transfers_ValidateFileBasics_MultipleRowsWhitespaceFile()
        {
            /*
             * _
             * _
             * _
             */
            byte[] bytes = new byte[] { 0x20, 0x0A, 0x20, 0x0A, 0x20 };
            string returnMessage;

            bool actual = Transfers.ValidateFileBasics(ref bytes, out returnMessage);

            Assert.IsNotNull(actual);
            Assert.AreEqual(false, actual);
            Assert.AreEqual("Invalid file. An incorrect delimiter is being used.", returnMessage);
        }

        /// <summary>
        ///     A test for ValidateFileBasics
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESRv2"), TestMethod]
        public void Transfers_ValidateFileBasics_InvalidHeaderH()
        {
            /*
             * GDR~GOBlahBlah~01
             * Some~Entries~In~The~Row
             * TRL~3
             */
            byte[] bytes = new byte[]
                                {
                                    0x47, 0x44, 0x52, 0x7e, 0x47, 0x4f, 0x42, 0x6c, 0x61, 0x68, 0x42, 0x6c, 0x61, 0x68, 0x7e, 0x30, 0x31, 0x0a,
                                    0x53, 0x6f, 0x6d, 0x65, 0x7e, 0x45, 0x6e, 0x74, 0x72, 0x69, 0x65, 0x73, 0x7e, 0x49, 0x6e, 0x7e, 0x54, 0x68, 0x65, 0x7e, 0x52, 0x6f, 0x77, 0x0a,
                                    0x54, 0x52, 0x4c, 0x7e, 0x33
                                };
            string returnMessage;

            bool actual = Transfers.ValidateFileBasics(ref bytes, out returnMessage);

            Assert.IsNotNull(actual);
            Assert.AreEqual(false, actual);
            Assert.AreEqual("Invalid file. The first row must be a HDR row.", returnMessage);
        }

        /// <summary>
        ///     A test for ValidateFileBasics
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESRv2"), TestMethod]
        public void Transfers_ValidateFileBasics_InvalidHeaderD()
        {
            /*
             * HCR~GOBlahBlah~01
             * Some~Entries~In~The~Row
             * TRL~3
             */
            byte[] bytes = new byte[]
                                {
                                    0x48, 0x43, 0x52, 0x7e, 0x47, 0x4f, 0x42, 0x6c, 0x61, 0x68, 0x42, 0x6c, 0x61, 0x68, 0x7e, 0x30, 0x31, 0x0a,
                                    0x53, 0x6f, 0x6d, 0x65, 0x7e, 0x45, 0x6e, 0x74, 0x72, 0x69, 0x65, 0x73, 0x7e, 0x49, 0x6e, 0x7e, 0x54, 0x68, 0x65, 0x7e, 0x52, 0x6f, 0x77, 0x0a,
                                    0x54, 0x52, 0x4c, 0x7e, 0x33
                                };
            string returnMessage;

            bool actual = Transfers.ValidateFileBasics(ref bytes, out returnMessage);

            Assert.IsNotNull(actual);
            Assert.AreEqual(false, actual);
            Assert.AreEqual("Invalid file. The first row must be a HDR row.", returnMessage);
        }

        /// <summary>
        ///     A test for ValidateFileBasics
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESRv2"), TestMethod]
        public void Transfers_ValidateFileBasics_InvalidHeaderR()
        {
            /*
             * HDQ~GOBlahBlah~01
             * Some~Entries~In~The~Row
             * TRL~3
             */
            byte[] bytes = new byte[]
                                {
                                    0x48, 0x44, 0x51, 0x7e, 0x47, 0x4f, 0x42, 0x6c, 0x61, 0x68, 0x42, 0x6c, 0x61, 0x68, 0x7e, 0x30, 0x31, 0x0a,
                                    0x53, 0x6f, 0x6d, 0x65, 0x7e, 0x45, 0x6e, 0x74, 0x72, 0x69, 0x65, 0x73, 0x7e, 0x49, 0x6e, 0x7e, 0x54, 0x68, 0x65, 0x7e, 0x52, 0x6f, 0x77, 0x0a,
                                    0x54, 0x52, 0x4c, 0x7e, 0x33
                                };
            string returnMessage;

            bool actual = Transfers.ValidateFileBasics(ref bytes, out returnMessage);

            Assert.IsNotNull(actual);
            Assert.AreEqual(false, actual);
            Assert.AreEqual("Invalid file. The first row must be a HDR row.", returnMessage);
        }

        /// <summary>
        ///     A test for ValidateFileBasics
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESRv2"), TestMethod]
        public void Transfers_ValidateFileBasics_InvalidFooterT()
        {
            /*
             * HDR~GOBlahBlah~01
             * Some~Entries~In~The~Row
             * SRL~3
             */
            byte[] bytes = new byte[]
                                {
                                    0x48, 0x44, 0x52, 0x7e, 0x47, 0x4f, 0x42, 0x6c, 0x61, 0x68, 0x42, 0x6c, 0x61, 0x68, 0x7e, 0x30, 0x31, 0x0a,
                                    0x53, 0x6f, 0x6d, 0x65, 0x7e, 0x45, 0x6e, 0x74, 0x72, 0x69, 0x65, 0x73, 0x7e, 0x49, 0x6e, 0x7e, 0x54, 0x68, 0x65, 0x7e, 0x52, 0x6f, 0x77, 0x0a,
                                    0x53, 0x52, 0x4c, 0x7e, 0x33
                                };
            string returnMessage;

            bool actual = Transfers.ValidateFileBasics(ref bytes, out returnMessage);

            Assert.IsNotNull(actual);
            Assert.AreEqual(false, actual);
            Assert.AreEqual("Invalid file. The last row must be a TRL row.", returnMessage);
        }

        /// <summary>
        ///     A test for ValidateFileBasics
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESRv2"), TestMethod]
        public void Transfers_ValidateFileBasics_InvalidFooterR()
        {
            /*
             * HDR~GOBlahBlah~01
             * Some~Entries~In~The~Row
             * TQL~3
             */
            byte[] bytes = new byte[]
                                {
                                    0x48, 0x44, 0x52, 0x7e, 0x47, 0x4f, 0x42, 0x6c, 0x61, 0x68, 0x42, 0x6c, 0x61, 0x68, 0x7e, 0x30, 0x31, 0x0a,
                                    0x53, 0x6f, 0x6d, 0x65, 0x7e, 0x45, 0x6e, 0x74, 0x72, 0x69, 0x65, 0x73, 0x7e, 0x49, 0x6e, 0x7e, 0x54, 0x68, 0x65, 0x7e, 0x52, 0x6f, 0x77, 0x0a,
                                    0x54, 0x51, 0x4c, 0x7e, 0x33
                                };
            string returnMessage;

            bool actual = Transfers.ValidateFileBasics(ref bytes, out returnMessage);

            Assert.IsNotNull(actual);
            Assert.AreEqual(false, actual);
            Assert.AreEqual("Invalid file. The last row must be a TRL row.", returnMessage);
        }

        /// <summary>
        ///     A test for ValidateFileBasics
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESRv2"), TestMethod]
        public void Transfers_ValidateFileBasics_InvalidFooterL()
        {
            /*
             * HDR~GOBlahBlah~01
             * Some~Entries~In~The~Row
             * TRK~3
             */
            byte[] bytes = new byte[]
                                {
                                    0x48, 0x44, 0x52, 0x7e, 0x47, 0x4f, 0x42, 0x6c, 0x61, 0x68, 0x42, 0x6c, 0x61, 0x68, 0x7e, 0x30, 0x31, 0x0a,
                                    0x53, 0x6f, 0x6d, 0x65, 0x7e, 0x45, 0x6e, 0x74, 0x72, 0x69, 0x65, 0x73, 0x7e, 0x49, 0x6e, 0x7e, 0x54, 0x68, 0x65, 0x7e, 0x52, 0x6f, 0x77, 0x0a,
                                    0x54, 0x52, 0x4b, 0x7e, 0x33
                                };
            string returnMessage;

            bool actual = Transfers.ValidateFileBasics(ref bytes, out returnMessage);

            Assert.IsNotNull(actual);
            Assert.AreEqual(false, actual);
            Assert.AreEqual("Invalid file. The last row must be a TRL row.", returnMessage);
        }

        /// <summary>
        ///     A test for ValidateFileBasics
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESRv2"), TestMethod]
        public void Transfers_ValidateFileBasics_MissingRowCount()
        {
            /*
             * HDR~GOBlahBlah~01
             * Some~Entries~In~The~Row
             * TRL~
             */
            byte[] bytes = new byte[]
                                {
                                    0x48, 0x44, 0x52, 0x7e, 0x47, 0x4f, 0x42, 0x6c, 0x61, 0x68, 0x42, 0x6c, 0x61, 0x68, 0x7e, 0x30, 0x31, 0x0a,
                                    0x53, 0x6f, 0x6d, 0x65, 0x7e, 0x45, 0x6e, 0x74, 0x72, 0x69, 0x65, 0x73, 0x7e, 0x49, 0x6e, 0x7e, 0x54, 0x68, 0x65, 0x7e, 0x52, 0x6f, 0x77, 0x0a,
                                    0x54, 0x52, 0x4c, 0x7e
                                };
            string returnMessage;

            bool actual = Transfers.ValidateFileBasics(ref bytes, out returnMessage);

            Assert.IsNotNull(actual);
            Assert.AreEqual(false, actual);
            Assert.AreEqual("Invalid file. A rowcount cannot be found in the TRL row.", returnMessage);
        }

        /// <summary>
        ///     A test for ValidateFileBasics
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESRv2"), TestMethod]
        public void Transfers_ValidateFileBasics_InvalidRowCount()
        {
            /*
             * HDR~GOBlahBlah~01
             * Some~Entries~In~The~Row
             * TRL~*
             */
            byte[] bytes = new byte[]
                                {
                                    0x48, 0x44, 0x52, 0x7e, 0x47, 0x4f, 0x42, 0x6c, 0x61, 0x68, 0x42, 0x6c, 0x61, 0x68, 0x7e, 0x30, 0x31, 0x0a,
                                    0x53, 0x6f, 0x6d, 0x65, 0x7e, 0x45, 0x6e, 0x74, 0x72, 0x69, 0x65, 0x73, 0x7e, 0x49, 0x6e, 0x7e, 0x54, 0x68, 0x65, 0x7e, 0x52, 0x6f, 0x77, 0x0a,
                                    0x54, 0x52, 0x4c, 0x7e, 0x2a
                                };
            string returnMessage;

            bool actual = Transfers.ValidateFileBasics(ref bytes, out returnMessage);

            Assert.IsNotNull(actual);
            Assert.AreEqual(false, actual);
            Assert.AreEqual("Invalid file. A rowcount cannot be found in the TRL row.", returnMessage);
        }

        /// <summary>
        ///     A test for ValidateFileBasics
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESRv2"), TestMethod]
        public void Transfers_ValidateFileBasics_IncorrectRowCount()
        {
            /*
             * HDR~GOBlahBlah~01
             * Some~Entries~In~The~Row
             * TRL~2
             */
            byte[] bytes = new byte[]
                                {
                                    0x48, 0x44, 0x52, 0x7e, 0x47, 0x4f, 0x42, 0x6c, 0x61, 0x68, 0x42, 0x6c, 0x61, 0x68, 0x7e, 0x30, 0x31, 0x0a,
                                    0x53, 0x6f, 0x6d, 0x65, 0x7e, 0x45, 0x6e, 0x74, 0x72, 0x69, 0x65, 0x73, 0x7e, 0x49, 0x6e, 0x7e, 0x54, 0x68, 0x65, 0x7e, 0x52, 0x6f, 0x77, 0x0a,
                                    0x54, 0x52, 0x4c, 0x7e, 0x32
                                };
            string returnMessage;

            bool actual = Transfers.ValidateFileBasics(ref bytes, out returnMessage);

            Assert.IsNotNull(actual);
            Assert.AreEqual(false, actual);
            Assert.AreEqual("Invalid file. The rowcount from the last row does not match the number of rows found in the stream.", returnMessage);
        }

        /// <summary>
        ///     A test for ValidateFileBasics
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESRv2"), TestMethod]
        public void Transfers_ValidateFileBasics_ValidFileWithTrailingWhiteSpace()
        {
            /*
             * HDR~GOBlahBlah~01
             * Some~Entries~In~The~Row
             * TRL~3_
             */
            byte[] bytes = new byte[]
                                {
                                    0x48, 0x44, 0x52, 0x7e, 0x47, 0x4f, 0x42, 0x6c, 0x61, 0x68, 0x42, 0x6c, 0x61, 0x68, 0x7e, 0x30, 0x31, 0x0a,
                                    0x53, 0x6f, 0x6d, 0x65, 0x7e, 0x45, 0x6e, 0x74, 0x72, 0x69, 0x65, 0x73, 0x7e, 0x49, 0x6e, 0x7e, 0x54, 0x68, 0x65, 0x7e, 0x52, 0x6f, 0x77, 0x0a,
                                    0x54, 0x52, 0x4c, 0x7e, 0x33, 0x20
                                };
            string returnMessage;

            bool actual = Transfers.ValidateFileBasics(ref bytes, out returnMessage);

            Assert.IsNotNull(actual);
            Assert.AreEqual(true, actual);
            Assert.AreEqual("Valid file. Simple file validation passed.", returnMessage);
        }

        /// <summary>
        ///     A test for ValidateFileBasics
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESRv2"), TestMethod]
        public void Transfers_ValidateFileBasics_ValidFileWithTrailingLine()
        {
            /*
             * HDR~GOBlahBlah~01
             * Some~Entries~In~The~Row
             * TRL~3
             * 
             */
            byte[] bytes = new byte[]
                                {
                                    0x48, 0x44, 0x52, 0x7e, 0x47, 0x4f, 0x42, 0x6c, 0x61, 0x68, 0x42, 0x6c, 0x61, 0x68, 0x7e, 0x30, 0x31, 0x0a,
                                    0x53, 0x6f, 0x6d, 0x65, 0x7e, 0x45, 0x6e, 0x74, 0x72, 0x69, 0x65, 0x73, 0x7e, 0x49, 0x6e, 0x7e, 0x54, 0x68, 0x65, 0x7e, 0x52, 0x6f, 0x77, 0x0a,
                                    0x54, 0x52, 0x4c, 0x7e, 0x33,
                                    0x0a
                                };
            string returnMessage;

            bool actual = Transfers.ValidateFileBasics(ref bytes, out returnMessage);

            Assert.IsNotNull(actual);
            Assert.AreEqual(true, actual);
            Assert.AreEqual("Valid file. Simple file validation passed.", returnMessage);
        }

        /// <summary>
        ///     A test for ValidateFileBasics
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESRv2"), TestMethod]
        public void Transfers_ValidateFileBasics_ValidFileWithTrailingLineAndWhiteSpace()
        {
            /*
             * HDR~GOBlahBlah~01
             * Some~Entries~In~The~Row
             * TRL~3
             * __
             */
            byte[] bytes = new byte[]
                                {
                                    0x48, 0x44, 0x52, 0x7e, 0x47, 0x4f, 0x42, 0x6c, 0x61, 0x68, 0x42, 0x6c, 0x61, 0x68, 0x7e, 0x30, 0x31, 0x0a,
                                    0x53, 0x6f, 0x6d, 0x65, 0x7e, 0x45, 0x6e, 0x74, 0x72, 0x69, 0x65, 0x73, 0x7e, 0x49, 0x6e, 0x7e, 0x54, 0x68, 0x65, 0x7e, 0x52, 0x6f, 0x77, 0x0a,
                                    0x54, 0x52, 0x4c, 0x7e, 0x33,
                                    0x0a, 0x20, 0x20
                                };
            string returnMessage;

            bool actual = Transfers.ValidateFileBasics(ref bytes, out returnMessage);

            Assert.IsNotNull(actual);
            Assert.AreEqual(true, actual);
            Assert.AreEqual("Valid file. Simple file validation passed.", returnMessage);
        }

        /// <summary>
        ///     A test for ValidateFileBasics
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESRv2"), TestMethod]
        public void Transfers_ValidateFileBasics_ValidFile()
        {
            /*
             * HDR~GOBlahBlah~01
             * Some~Entries~In~The~Row
             * TRL~3
             */
            byte[] bytes = new byte[]
                                {
                                    0x48, 0x44, 0x52, 0x7e, 0x47, 0x4f, 0x42, 0x6c, 0x61, 0x68, 0x42, 0x6c, 0x61, 0x68, 0x7e, 0x30, 0x31, 0x0a,
                                    0x53, 0x6f, 0x6d, 0x65, 0x7e, 0x45, 0x6e, 0x74, 0x72, 0x69, 0x65, 0x73, 0x7e, 0x49, 0x6e, 0x7e, 0x54, 0x68, 0x65, 0x7e, 0x52, 0x6f, 0x77, 0x0a,
                                    0x54, 0x52, 0x4c, 0x7e, 0x33
                                };
            string returnMessage;

            bool actual = Transfers.ValidateFileBasics(ref bytes, out returnMessage);

            Assert.IsNotNull(actual);
            Assert.AreEqual(true, actual);
            Assert.AreEqual("Valid file. Simple file validation passed.", returnMessage);
        }
// ReSharper restore InconsistentNaming
    }
}