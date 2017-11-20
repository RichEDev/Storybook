using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest2012Ultimate.ApiLibrary.DataObjects.ESR
{
    using global::EsrGo2FromNhsWcfLibrary.ESR;
    using Action = global::EsrGo2FromNhsWcfLibrary.Base.Action;

    [TestClass]
    public class EsrPhoneRecordTest
    {
        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParsePhoneRecordValidDeleteRecord()
        {
            DateTime minDate = new DateTime(1900, 1, 1);
            const string dataRow = "PHD~87654321";

            EsrPhoneRecord parsedRecord = EsrPhoneRecord.ParseEsrPhoneRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrPhoneRecord()");
            Assert.AreEqual(Action.Delete, parsedRecord.Action);
            Assert.AreEqual(87654321, parsedRecord.ESRPhoneId);
            Assert.AreEqual(0, parsedRecord.ESRPersonId);
            Assert.AreEqual(minDate, parsedRecord.EffectiveStartDate);
            Assert.IsNull(parsedRecord.EffectiveEndDate);
            Assert.AreEqual(string.Empty, parsedRecord.PhoneNumber);
            Assert.AreEqual(string.Empty, parsedRecord.PhoneType);
            Assert.AreEqual(minDate, parsedRecord.ESRLastUpdate);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParsePhoneRecordInvalidDeleteRecord()
        {
            DateTime minDate = new DateTime(1900, 1, 1);
            const string dataRow = "PHD~x654321";

            EsrPhoneRecord parsedRecord = EsrPhoneRecord.ParseEsrPhoneRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrPhoneRecord()");
            Assert.AreEqual(Action.Delete, parsedRecord.Action);
            Assert.AreEqual(0, parsedRecord.ESRPhoneId);
            Assert.AreEqual(0, parsedRecord.ESRPersonId);
            Assert.AreEqual(minDate, parsedRecord.EffectiveStartDate);
            Assert.IsNull(parsedRecord.EffectiveEndDate);
            Assert.AreEqual(string.Empty, parsedRecord.PhoneNumber);
            Assert.AreEqual(string.Empty, parsedRecord.PhoneType);
            Assert.AreEqual(minDate, parsedRecord.ESRLastUpdate);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParsePhoneRecordInvalidNumberOfColumns()
        {
            const string dataRow = "PHA~10804728~18104606~H1~07407638863~20121105~~20121107 141337~~";

            EsrPhoneRecord parsedRecord = EsrPhoneRecord.ParseEsrPhoneRecord(dataRow);

            Assert.IsNull(parsedRecord, "Expected Null record not returned from ParseEsrPhoneRecord()");
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParsePhoneRecordInvalidRecordType()
        {
            const string dataRow = "PHX~10804728~18104606~H1~07407638863~20121105~~20121107 141337";

            EsrPhoneRecord parsedRecord = EsrPhoneRecord.ParseEsrPhoneRecord(dataRow);

            Assert.IsNull(parsedRecord, "Expected Null record not returned from ParseEsrPhoneRecord()");
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParsePhoneRecordValidUpdateRecord()
        {
            const string dataRow = "PHA~12345678~87654321~H1~07777654321~20121105~20130501~20121107 141337";

            EsrPhoneRecord parsedRecord = EsrPhoneRecord.ParseEsrPhoneRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrPhoneRecord()");
            Assert.AreEqual(Action.Update, parsedRecord.Action);
            Assert.AreEqual(87654321, parsedRecord.ESRPhoneId);
            Assert.AreEqual(12345678, parsedRecord.ESRPersonId);
            Assert.AreEqual("H1", parsedRecord.PhoneType);
            Assert.AreEqual("07777654321", parsedRecord.PhoneNumber);
            Assert.AreEqual(new DateTime(2012, 11, 5), parsedRecord.EffectiveStartDate);
            Assert.AreEqual(new DateTime(2013, 5, 1), parsedRecord.EffectiveEndDate);
            Assert.AreEqual(new DateTime(2012, 11, 7, 14, 13, 37), parsedRecord.ESRLastUpdate);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParsePhoneRecordInvalidStartDate()
        {
            DateTime minDate = new DateTime(1900, 1, 1);
            const string dataRow = "PHA~12345678~87654321~H1~07777654321~20121305~20130501~20121107 141337";

            EsrPhoneRecord parsedRecord = EsrPhoneRecord.ParseEsrPhoneRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrPhoneRecord()");
            Assert.AreEqual(Action.Update, parsedRecord.Action);
            Assert.AreEqual(87654321, parsedRecord.ESRPhoneId);
            Assert.AreEqual(12345678, parsedRecord.ESRPersonId);
            Assert.AreEqual("H1", parsedRecord.PhoneType);
            Assert.AreEqual("07777654321", parsedRecord.PhoneNumber);
            Assert.AreEqual(minDate, parsedRecord.EffectiveStartDate);
            Assert.AreEqual(new DateTime(2013, 5, 1), parsedRecord.EffectiveEndDate);
            Assert.AreEqual(new DateTime(2012, 11, 7, 14, 13, 37), parsedRecord.ESRLastUpdate);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParsePhoneRecordInvalidEndDate()
        {
            const string dataRow = "PHA~12345678~87654321~H1~07777654321~20121105~20131501~20121107 141337";

            EsrPhoneRecord parsedRecord = EsrPhoneRecord.ParseEsrPhoneRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrPhoneRecord()");
            Assert.AreEqual(Action.Update, parsedRecord.Action);
            Assert.AreEqual(87654321, parsedRecord.ESRPhoneId);
            Assert.AreEqual(12345678, parsedRecord.ESRPersonId);
            Assert.AreEqual("H1", parsedRecord.PhoneType);
            Assert.AreEqual("07777654321", parsedRecord.PhoneNumber);
            Assert.AreEqual(new DateTime(2012, 11, 5), parsedRecord.EffectiveStartDate);
            Assert.IsNull(parsedRecord.EffectiveEndDate);
            Assert.AreEqual(new DateTime(2012, 11, 7, 14, 13, 37), parsedRecord.ESRLastUpdate);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParsePhoneRecordInvalidUpdateDate()
        {
            DateTime minDate = new DateTime(1900, 1, 1);
            const string dataRow = "PHA~12345678~87654321~H1~07777654321~20121105~20130501~20121407 141337";

            EsrPhoneRecord parsedRecord = EsrPhoneRecord.ParseEsrPhoneRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrPhoneRecord()");
            Assert.AreEqual(Action.Update, parsedRecord.Action);
            Assert.AreEqual(87654321, parsedRecord.ESRPhoneId);
            Assert.AreEqual(12345678, parsedRecord.ESRPersonId);
            Assert.AreEqual("H1", parsedRecord.PhoneType);
            Assert.AreEqual("07777654321", parsedRecord.PhoneNumber);
            Assert.AreEqual(new DateTime(2012, 11, 5), parsedRecord.EffectiveStartDate);
            Assert.AreEqual(new DateTime(2013, 5, 1), parsedRecord.EffectiveEndDate);
            Assert.AreEqual(minDate, parsedRecord.ESRLastUpdate);
        }
    }
}
