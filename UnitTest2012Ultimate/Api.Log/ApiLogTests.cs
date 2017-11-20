namespace UnitTest2012Ultimate.Api.Log
{
    using System.Data.SqlClient;

    using EsrGo2FromNhsWcfLibrary.Interfaces;

    using global::EsrGo2FromNhsWcfLibrary.Base;
    using global::EsrGo2FromNhsWcfLibrary.Interfaces;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// The API log tests.
    /// </summary>
    [TestClass]
    public class ApiLogTests
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
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
            GlobalAsax.Application_Start();
        }

        /// <summary>
        /// Use Class Clean up to run code after all tests in a class have run
        /// </summary>
        [ClassCleanup]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }

        #endregion

        /// <summary>
        /// The API log write test (mocked).
        /// </summary>
        [TestMethod, TestCategory("ApiLog")]
        public void ApiLogWrite()
        {
            var apiLogData = new Mock<IApiDbConnection>();
            apiLogData.SetupAllProperties();
            apiLogData.Setup(x => x.ExecuteProc(It.IsAny<string>())).Returns(1);
            apiLogData.Setup(x => x.ConnectionStringValid).Returns(true);
            apiLogData.Setup(x => x.Sqlexecute).Returns(new SqlCommand());
            var apiLog = new Log(apiLogData.Object);

            var logRecord = new LogRecord
            {
                AccountId = GlobalTestVariables.AccountId,
                LogReason = LogRecord.LogReasonType.None,
                Filename = "filename",
                LogItemType = LogRecord.LogItemTypes.EsrServiceErrored,
                Message = "message",
                MetaBase = "expenses",
                Source = "source",
                TransferType = LogRecord.TransferTypes.EsrOutbound,
                LogId = 1
            };
            var result = apiLog.Write("expenses", "0", GlobalTestVariables.AccountId, logRecord.LogItemType, logRecord.TransferType, logRecord.LogId, logRecord.Filename, LogRecord.LogReasonType.None, logRecord.Message, logRecord.Source);
            Assert.IsTrue(result, "Should return true from write");
        }

        /// <summary>
        /// The API log write debug.
        /// </summary>
        [TestMethod, TestCategory("ApiLog")]
        public void ApiLogWriteDebug()
        {
            var apiLogData = new Mock<IApiDbConnection>();
            apiLogData.SetupAllProperties();
            apiLogData.Setup(x => x.ExecuteProc(It.IsAny<string>())).Returns(1);
            apiLogData.Setup(x => x.ConnectionStringValid).Returns(true);
            apiLogData.Setup(x => x.Sqlexecute).Returns(new SqlCommand());
            apiLogData.Setup(x => x.DebugLevel).Returns(Log.MessageLevel.Debug);
            var apiLog = new Log(apiLogData.Object);

            var logRecord = new LogRecord
            {
                AccountId = GlobalTestVariables.AccountId,
                LogReason = LogRecord.LogReasonType.SuccessAdd,
                Filename = "filename",
                LogItemType = LogRecord.LogItemTypes.EsrServiceErrored,
                Message = "message",
                MetaBase = "expenses",
                LogId = 1,
                Source = "source",
                TransferType = LogRecord.TransferTypes.EsrOutbound
            };
            var result = apiLog.WriteDebug("expenses", "0", GlobalTestVariables.AccountId, logRecord.LogItemType, logRecord.TransferType, logRecord.LogId, logRecord.Filename, LogRecord.LogReasonType.None, logRecord.Message, logRecord.Source);
            Assert.IsTrue(result, "Should return true from write");
        }

        /// <summary>
        /// The API log write extended.
        /// </summary>
        [TestMethod, TestCategory("ApiLog")]
        public void ApiLogWriteExtended()
        {
            var apiLogData = new Mock<IApiDbConnection>();
            apiLogData.SetupAllProperties();
            apiLogData.Setup(x => x.ExecuteProc(It.IsAny<string>())).Returns(1);
            apiLogData.Setup(x => x.Sqlexecute).Returns(new SqlCommand());
            apiLogData.Setup(x => x.DebugLevel).Returns(Log.MessageLevel.Extended);
            var apiLog = new Log(apiLogData.Object);

            var logRecord = new LogRecord
            {
                AccountId = GlobalTestVariables.AccountId,
                LogReason = LogRecord.LogReasonType.SuccessAdd,
                Filename = "filename",
                LogItemType = LogRecord.LogItemTypes.EsrServiceErrored,
                Message = "message",
                MetaBase = "expenses",
                LogId = 1,
                Source = "source",
                TransferType = LogRecord.TransferTypes.EsrOutbound
            };
            var result = apiLog.WriteExtra("expenses", "0", GlobalTestVariables.AccountId, logRecord.LogItemType, logRecord.TransferType, logRecord.LogId, logRecord.Filename, LogRecord.LogReasonType.None, logRecord.Message, logRecord.Source);
            Assert.IsTrue(result, "Should return true from write");
        }

        /// <summary>
        /// The API log write extended fail.
        /// </summary>
        [TestMethod, TestCategory("ApiLog")]
        public void ApiLogWriteExtendedFail()
        {
            var apiLogData = new Mock<IApiDbConnection>();
            apiLogData.SetupAllProperties();
            apiLogData.Setup(x => x.ExecuteProc(It.IsAny<string>())).Returns(0);
            apiLogData.Setup(x => x.Sqlexecute).Returns(new SqlCommand());
            apiLogData.Setup(x => x.ErrorMessage).Returns("Error from database layer");
            apiLogData.Setup(x => x.DebugLevel).Returns(Log.MessageLevel.Normal);
            var apiLog = new Log(apiLogData.Object);

            var logRecord = new LogRecord
            {
                AccountId = GlobalTestVariables.AccountId,
                LogReason = LogRecord.LogReasonType.SuccessAdd,
                Filename = "filename",
                LogItemType = LogRecord.LogItemTypes.EsrServiceErrored,
                Message = "message",
                MetaBase = "expenses",
                LogId = 1,
                Source = "source",
                TransferType = LogRecord.TransferTypes.EsrOutbound
            };
            var result = apiLog.WriteExtra("expenses", "0", GlobalTestVariables.AccountId, logRecord.LogItemType, logRecord.TransferType, logRecord.LogId, logRecord.Filename, LogRecord.LogReasonType.None, logRecord.Message, logRecord.Source);
            Assert.IsFalse(result, "Should return false from write");
        }

        /// <summary>
        /// The API log write debug fail.
        /// </summary>
        [TestMethod, TestCategory("ApiLog")]
        public void ApiLogWriteDebugFail()
        {
            var apiLogData = new Mock<IApiDbConnection>();
            apiLogData.SetupAllProperties();
            apiLogData.Setup(x => x.ExecuteProc(It.IsAny<string>())).Returns(0);
            apiLogData.Setup(x => x.DebugLevel).Returns(Log.MessageLevel.Normal);
            apiLogData.Setup(x => x.Sqlexecute).Returns(new SqlCommand());
            apiLogData.Setup(x => x.ErrorMessage).Returns("Error from database layer");
            var apiLog = new Log(apiLogData.Object);

            var logRecord = new LogRecord
            {
                AccountId = GlobalTestVariables.AccountId,
                LogReason = LogRecord.LogReasonType.Success,
                Filename = "filename",
                LogItemType = LogRecord.LogItemTypes.EsrServiceErrored,
                Message = "message",
                MetaBase = "expenses",
                LogId = 1,
                Source = "source",
                TransferType = LogRecord.TransferTypes.EsrOutbound
            };
            var result = apiLog.WriteDebug("expenses", "0", GlobalTestVariables.AccountId, logRecord.LogItemType, logRecord.TransferType, logRecord.LogId, logRecord.Filename, LogRecord.LogReasonType.None, logRecord.Message, logRecord.Source);
            Assert.IsFalse(result, "Should return false from write");
        }
    }
}
