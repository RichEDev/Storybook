using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest2012Ultimate.ApiLibrary.DataObjects.ESR
{
    using global::EsrGo2FromNhsWcfLibrary.ESR;

    [TestClass]
    public class EsrHeaderRecordTest
    {
        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseHeaderRecordValid()
        {
            const string dataRow = "HDR~GO_431_EXP_GOF_20121113_00000001.DAT~20121113 101356~431~20121013 161023~20121113 000000~01";

            EsrHeaderRecord parsedRecord = EsrHeaderRecord.ParseEsrHeaderRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrHeaderRecord()");
            Assert.AreEqual(EsrRecordTypes.EsrHeaderRecordType, parsedRecord.RecordType);
            Assert.AreEqual("GO_431_EXP_GOF_20121113_00000001.DAT", parsedRecord.Filename);
            Assert.AreEqual(new DateTime(2012,11,13,10,13,56), parsedRecord.CreationDate);
            Assert.AreEqual(431, parsedRecord.VpdNumber);
            Assert.AreEqual(new DateTime(2012, 10, 13, 16, 10, 23), parsedRecord.PreviousRunDate);
            Assert.AreEqual(new DateTime(2012,11,13,0,0,0), parsedRecord.RunDate);
            Assert.AreEqual("01", parsedRecord.InterfaceVersion);
            Assert.AreEqual('F', parsedRecord.FileTypeCode);
            Assert.AreEqual(1, parsedRecord.UniqueFileSequenceId);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseHeaderRecordInvalidVpd()
        {
            const string dataRow = "HDR~GO_431_EXP_GOF_20121113_00000001.DAT~20121113 101356~x31~20121013 161023~20121113 000000~01";

            EsrHeaderRecord parsedRecord = EsrHeaderRecord.ParseEsrHeaderRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrHeaderRecord()");
            Assert.AreEqual(EsrRecordTypes.EsrHeaderRecordType, parsedRecord.RecordType);
            Assert.AreEqual("GO_431_EXP_GOF_20121113_00000001.DAT", parsedRecord.Filename);
            Assert.AreEqual(new DateTime(2012, 11, 13, 10, 13, 56), parsedRecord.CreationDate);
            Assert.AreEqual(-1, parsedRecord.VpdNumber);
            Assert.AreEqual(new DateTime(2012, 10, 13, 16, 10, 23), parsedRecord.PreviousRunDate);
            Assert.AreEqual(new DateTime(2012, 11, 13, 0, 0, 0), parsedRecord.RunDate);
            Assert.AreEqual("01", parsedRecord.InterfaceVersion);
            Assert.AreEqual('F', parsedRecord.FileTypeCode);
            Assert.AreEqual(1, parsedRecord.UniqueFileSequenceId);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseHeaderRecordInvalidFileCreationDate()
        {
            const string dataRow = "HDR~GO_431_EXP_GOF_20121113_00000001.DAT~20121313 101356~431~20121013 161023~20121113 000000~01";

            EsrHeaderRecord parsedRecord = EsrHeaderRecord.ParseEsrHeaderRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrHeaderRecord()");
            Assert.AreEqual(EsrRecordTypes.EsrHeaderRecordType, parsedRecord.RecordType);
            Assert.AreEqual("GO_431_EXP_GOF_20121113_00000001.DAT", parsedRecord.Filename);
            Assert.AreEqual(DateTime.MinValue, parsedRecord.CreationDate);
            Assert.AreEqual(431, parsedRecord.VpdNumber);
            Assert.AreEqual(new DateTime(2012, 10, 13, 16, 10, 23), parsedRecord.PreviousRunDate);
            Assert.AreEqual(new DateTime(2012, 11, 13, 0, 0, 0), parsedRecord.RunDate);
            Assert.AreEqual("01", parsedRecord.InterfaceVersion);
            Assert.AreEqual('F', parsedRecord.FileTypeCode);
            Assert.AreEqual(1, parsedRecord.UniqueFileSequenceId);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseHeaderRecordInvalidFileRunDate()
        {
            const string dataRow = "HDR~GO_431_EXP_GOF_20121113_00000001.DAT~20121113 101356~431~20121013 161023~20121313 000000~01";

            EsrHeaderRecord parsedRecord = EsrHeaderRecord.ParseEsrHeaderRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrHeaderRecord()");
            Assert.AreEqual(EsrRecordTypes.EsrHeaderRecordType, parsedRecord.RecordType);
            Assert.AreEqual("GO_431_EXP_GOF_20121113_00000001.DAT", parsedRecord.Filename);
            Assert.AreEqual(new DateTime(2012, 11, 13, 10, 13, 56), parsedRecord.CreationDate);
            Assert.AreEqual(431, parsedRecord.VpdNumber);
            Assert.AreEqual(new DateTime(2012, 10, 13, 16, 10, 23), parsedRecord.PreviousRunDate);
            Assert.AreEqual(DateTime.MinValue, parsedRecord.RunDate);
            Assert.AreEqual("01", parsedRecord.InterfaceVersion);
            Assert.AreEqual('F', parsedRecord.FileTypeCode);
            Assert.AreEqual(1, parsedRecord.UniqueFileSequenceId);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseHeaderRecordInvalidFilePreviousRunDate()
        {
            const string dataRow = "HDR~GO_431_EXP_GOF_20121113_00000001.DAT~20121113 101356~431~20121313 161023~20121113 000000~01";

            EsrHeaderRecord parsedRecord = EsrHeaderRecord.ParseEsrHeaderRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrHeaderRecord()");
            Assert.AreEqual(EsrRecordTypes.EsrHeaderRecordType, parsedRecord.RecordType);
            Assert.AreEqual("GO_431_EXP_GOF_20121113_00000001.DAT", parsedRecord.Filename);
            Assert.AreEqual(new DateTime(2012, 11, 13, 10, 13, 56), parsedRecord.CreationDate);
            Assert.AreEqual(431, parsedRecord.VpdNumber);
            Assert.AreEqual(DateTime.MinValue, parsedRecord.PreviousRunDate);
            Assert.AreEqual(new DateTime(2012, 11, 13, 0, 0, 0), parsedRecord.RunDate);
            Assert.AreEqual("01", parsedRecord.InterfaceVersion);
            Assert.AreEqual('F', parsedRecord.FileTypeCode);
            Assert.AreEqual(1, parsedRecord.UniqueFileSequenceId);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseHeaderRecordInvalidNumberColumns()
        {
            const string dataRow = "HDR~GO_431_EXP_GOF_20121113_00000001.DAT~01";

            EsrHeaderRecord parsedRecord = EsrHeaderRecord.ParseEsrHeaderRecord(dataRow);

            Assert.IsNull(parsedRecord, "Expected Null record not returned from ParseEsrHeaderRecord()");
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseHeaderRecordInvalidRecordType()
        {
            const string dataRow = "HZR~GO_431_EXP_GOF_20121113_00000001.DAT~01";

            EsrHeaderRecord parsedRecord = EsrHeaderRecord.ParseEsrHeaderRecord(dataRow);

            Assert.IsNull(parsedRecord, "Expected Null record not returned from ParseEsrHeaderRecord()");
        }
    }
}
