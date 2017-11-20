using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest2012Ultimate.ApiLibrary.DataObjects.ESR
{

    using global::EsrGo2FromNhsWcfLibrary.ESR;
    using Action = global::EsrGo2FromNhsWcfLibrary.Base.Action;

    [TestClass]
    public class EsrAddressRecordTest
    {
        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseAddressRecordValidDeleteRecord()
        {
            DateTime minDate = new DateTime(1900, 1, 1);
            const string dataRow = "ADD~87654321";

            EsrAddressRecord parsedRecord = EsrAddressRecord.ParseEsrAddressRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrAddressRecord()");
            Assert.AreEqual(Action.Delete, parsedRecord.Action);
            Assert.AreEqual(87654321, parsedRecord.ESRAddressId);
            Assert.AreEqual(0, parsedRecord.ESRPersonId);
            Assert.AreEqual(string.Empty, parsedRecord.AddressType);
            Assert.AreEqual(string.Empty, parsedRecord.AddressStyle);
            Assert.AreEqual(string.Empty, parsedRecord.PrimaryFlag);
            Assert.AreEqual(string.Empty, parsedRecord.AddressLine1);
            Assert.AreEqual(string.Empty, parsedRecord.AddressLine2);
            Assert.AreEqual(string.Empty, parsedRecord.AddressLine3);
            Assert.AreEqual(string.Empty, parsedRecord.AddressTown);
            Assert.AreEqual(string.Empty, parsedRecord.AddressCounty);
            Assert.AreEqual(string.Empty, parsedRecord.AddressPostcode);
            Assert.AreEqual(string.Empty, parsedRecord.AddressCountry);
            Assert.AreEqual(minDate, parsedRecord.EffectiveStartDate);
            Assert.IsNull(parsedRecord.EffectiveEndDate);
            Assert.IsNull(parsedRecord.ESRLastUpdate);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseAddressRecordInvalidDeleteRecord()
        {
            DateTime minDate = new DateTime(1900, 1, 1);
            const string dataRow = "ADD~x7654321";

            EsrAddressRecord parsedRecord = EsrAddressRecord.ParseEsrAddressRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrAddressRecord()");
            Assert.AreEqual(Action.Delete, parsedRecord.Action);
            Assert.AreEqual(0, parsedRecord.ESRAddressId);
            Assert.AreEqual(0, parsedRecord.ESRPersonId);
            Assert.AreEqual(string.Empty, parsedRecord.AddressType);
            Assert.AreEqual(string.Empty, parsedRecord.AddressStyle);
            Assert.AreEqual(string.Empty, parsedRecord.PrimaryFlag);
            Assert.AreEqual(string.Empty, parsedRecord.AddressLine1);
            Assert.AreEqual(string.Empty, parsedRecord.AddressLine2);
            Assert.AreEqual(string.Empty, parsedRecord.AddressLine3);
            Assert.AreEqual(string.Empty, parsedRecord.AddressTown);
            Assert.AreEqual(string.Empty, parsedRecord.AddressCounty);
            Assert.AreEqual(string.Empty, parsedRecord.AddressPostcode);
            Assert.AreEqual(string.Empty, parsedRecord.AddressCountry);
            Assert.AreEqual(minDate, parsedRecord.EffectiveStartDate);
            Assert.IsNull(parsedRecord.EffectiveEndDate);
            Assert.IsNull(parsedRecord.ESRLastUpdate);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseAddressRecordInvalidNumberOfColumns()
        {
            const string dataRow = "ADA~12345678~87654321~~GB~Yes~5 My Way~~~SomeTown~Lincolnshire~LN11 2HL~GB~20121001~~20121108 131846~~";

            EsrAddressRecord parsedRecord = EsrAddressRecord.ParseEsrAddressRecord(dataRow);

            Assert.IsNull(parsedRecord, "Expected Null record not returned from ParseEsrAddressRecord()");
        }


        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseAddressRecordInvalidRecordType()
        {
            const string dataRow = "ADX~12345678~87654321~~GB~Yes~5 My Way~~~SomeTown~Lincolnshire~LN11 2HL~GB~20121001~~20121108 131846";

            EsrAddressRecord parsedRecord = EsrAddressRecord.ParseEsrAddressRecord(dataRow);

            Assert.IsNull(parsedRecord, "Expected Null record not returned from ParseEsrAddressRecord()");
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseAddressRecordValidUpdateRecord()
        {
            const string dataRow = "ADA~12345678~87654321~Adr_Type~GB~Yes~5 My Way~My Street~My Area~SomeTown~Lincolnshire~LN11 2HL~GB~20121001~20140101~20121108 131846";

            EsrAddressRecord parsedRecord = EsrAddressRecord.ParseEsrAddressRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrPhoneRecord()");
            Assert.AreEqual(Action.Update, parsedRecord.Action);
            Assert.AreEqual(12345678, parsedRecord.ESRPersonId);
            Assert.AreEqual(87654321, parsedRecord.ESRAddressId);
            Assert.AreEqual("Adr_Type", parsedRecord.AddressType);
            Assert.AreEqual("GB", parsedRecord.AddressStyle);
            Assert.AreEqual("Yes", parsedRecord.PrimaryFlag);
            Assert.AreEqual("5 My Way", parsedRecord.AddressLine1);
            Assert.AreEqual("My Street", parsedRecord.AddressLine2);
            Assert.AreEqual("My Area", parsedRecord.AddressLine3);
            Assert.AreEqual("SomeTown", parsedRecord.AddressTown);
            Assert.AreEqual("Lincolnshire", parsedRecord.AddressCounty);
            Assert.AreEqual("LN11 2HL", parsedRecord.AddressPostcode);
            Assert.AreEqual("GB", parsedRecord.AddressCountry);
            Assert.AreEqual(new DateTime(2012, 10, 01), parsedRecord.EffectiveStartDate);
            Assert.AreEqual(new DateTime(2014, 01, 01), parsedRecord.EffectiveEndDate);
            Assert.AreEqual(new DateTime(2012, 11, 08, 13, 18, 46), parsedRecord.ESRLastUpdate);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseAddressRecordInvalidStartDate()
        {
            DateTime minDate = new DateTime(1900, 1, 1);
            const string dataRow = "ADA~12345678~87654321~Adr_Type~GB~Yes~5 My Way~My Street~My Area~SomeTown~Lincolnshire~LN11 2HL~GB~20121301~20140101~20121108 131846";

            EsrAddressRecord parsedRecord = EsrAddressRecord.ParseEsrAddressRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrPhoneRecord()");
            Assert.AreEqual(Action.Update, parsedRecord.Action);
            Assert.AreEqual(12345678, parsedRecord.ESRPersonId);
            Assert.AreEqual(87654321, parsedRecord.ESRAddressId);
            Assert.AreEqual("Adr_Type", parsedRecord.AddressType);
            Assert.AreEqual("GB", parsedRecord.AddressStyle);
            Assert.AreEqual("Yes", parsedRecord.PrimaryFlag);
            Assert.AreEqual("5 My Way", parsedRecord.AddressLine1);
            Assert.AreEqual("My Street", parsedRecord.AddressLine2);
            Assert.AreEqual("My Area", parsedRecord.AddressLine3);
            Assert.AreEqual("SomeTown", parsedRecord.AddressTown);
            Assert.AreEqual("Lincolnshire", parsedRecord.AddressCounty);
            Assert.AreEqual("LN11 2HL", parsedRecord.AddressPostcode);
            Assert.AreEqual("GB", parsedRecord.AddressCountry);
            Assert.AreEqual(minDate, parsedRecord.EffectiveStartDate);
            Assert.AreEqual(new DateTime(2014, 01, 01), parsedRecord.EffectiveEndDate);
            Assert.AreEqual(new DateTime(2012, 11, 08, 13, 18, 46), parsedRecord.ESRLastUpdate);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseAddressRecordInvalidEndDate()
        {
            const string dataRow = "ADA~12345678~87654321~Adr_Type~GB~Yes~5 My Way~My Street~My Area~SomeTown~Lincolnshire~LN11 2HL~GB~20121001~20141301~20121108 131846";

            EsrAddressRecord parsedRecord = EsrAddressRecord.ParseEsrAddressRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrPhoneRecord()");
            Assert.AreEqual(Action.Update, parsedRecord.Action);
            Assert.AreEqual(12345678, parsedRecord.ESRPersonId);
            Assert.AreEqual(87654321, parsedRecord.ESRAddressId);
            Assert.AreEqual("Adr_Type", parsedRecord.AddressType);
            Assert.AreEqual("GB", parsedRecord.AddressStyle);
            Assert.AreEqual("Yes", parsedRecord.PrimaryFlag);
            Assert.AreEqual("5 My Way", parsedRecord.AddressLine1);
            Assert.AreEqual("My Street", parsedRecord.AddressLine2);
            Assert.AreEqual("My Area", parsedRecord.AddressLine3);
            Assert.AreEqual("SomeTown", parsedRecord.AddressTown);
            Assert.AreEqual("Lincolnshire", parsedRecord.AddressCounty);
            Assert.AreEqual("LN11 2HL", parsedRecord.AddressPostcode);
            Assert.AreEqual("GB", parsedRecord.AddressCountry);
            Assert.AreEqual(new DateTime(2012, 10, 01), parsedRecord.EffectiveStartDate);
            Assert.IsNull(parsedRecord.EffectiveEndDate);
            Assert.AreEqual(new DateTime(2012, 11, 08, 13, 18, 46), parsedRecord.ESRLastUpdate);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseAddressRecordInvalidUpdateDate()
        {
            const string dataRow = "ADA~12345678~87654321~Adr_Type~GB~Yes~5 My Way~My Street~My Area~SomeTown~Lincolnshire~LN11 2HL~GB~20121001~20140101~20121308 131846";

            EsrAddressRecord parsedRecord = EsrAddressRecord.ParseEsrAddressRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrPhoneRecord()");
            Assert.AreEqual(Action.Update, parsedRecord.Action);
            Assert.AreEqual(12345678, parsedRecord.ESRPersonId);
            Assert.AreEqual(87654321, parsedRecord.ESRAddressId);
            Assert.AreEqual("Adr_Type", parsedRecord.AddressType);
            Assert.AreEqual("GB", parsedRecord.AddressStyle);
            Assert.AreEqual("Yes", parsedRecord.PrimaryFlag);
            Assert.AreEqual("5 My Way", parsedRecord.AddressLine1);
            Assert.AreEqual("My Street", parsedRecord.AddressLine2);
            Assert.AreEqual("My Area", parsedRecord.AddressLine3);
            Assert.AreEqual("SomeTown", parsedRecord.AddressTown);
            Assert.AreEqual("Lincolnshire", parsedRecord.AddressCounty);
            Assert.AreEqual("LN11 2HL", parsedRecord.AddressPostcode);
            Assert.AreEqual("GB", parsedRecord.AddressCountry);
            Assert.AreEqual(new DateTime(2012, 10, 01), parsedRecord.EffectiveStartDate);
            Assert.AreEqual(new DateTime(2014, 01, 01), parsedRecord.EffectiveEndDate);
            Assert.IsNull(parsedRecord.ESRLastUpdate);
        }
    }
}
