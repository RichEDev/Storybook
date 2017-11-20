using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest2012Ultimate.ApiLibrary.DataObjects.ESR
{
    using global::EsrGo2FromNhsWcfLibrary.ESR;

    [TestClass]
    public class EsrTrailerRecordTest
    {
        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseTrailerRecordInvalidRecordType()
        {
            string dataRow = "TRA~1234";

            EsrTrailerRecord parsedRecord = EsrTrailerRecord.ParseEsrTrailerRecord(dataRow);

            Assert.IsNull(parsedRecord, "Expected Null record not returned from ParseEsrTrailerRecord()");
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseTrailerRecordInvalidNumberColumns()
        {
            string dataRow = "TRL~1234~xyz";

            EsrTrailerRecord parsedRecord = EsrTrailerRecord.ParseEsrTrailerRecord(dataRow);

            Assert.IsNull(parsedRecord, "Expected Null record not returned from ParseEsrTrailerRecord()");
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseTrailerRecordValid()
        {
            string dataRow = "TRL~1234";

            EsrTrailerRecord parsedRecord = EsrTrailerRecord.ParseEsrTrailerRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrTrailerRecord()");
            Assert.AreEqual(EsrRecordTypes.EsrTrailerRecordType, parsedRecord.RecordType);
            Assert.AreEqual(1234, parsedRecord.NumberOfRecords);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseTrailerRecordInvalidRecordCount()
        {
            string dataRow = "TRL~abc";

            EsrTrailerRecord parsedRecord = EsrTrailerRecord.ParseEsrTrailerRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrTrailerRecord()");
            Assert.AreEqual(EsrRecordTypes.EsrTrailerRecordType, parsedRecord.RecordType);
            Assert.AreEqual(0, parsedRecord.NumberOfRecords);
        }
    }
}
