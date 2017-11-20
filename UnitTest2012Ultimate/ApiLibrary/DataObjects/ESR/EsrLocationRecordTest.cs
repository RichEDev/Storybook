using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest2012Ultimate.ApiLibrary.DataObjects.ESR
{
    using global::EsrGo2FromNhsWcfLibrary.ESR;
    using Action = global::EsrGo2FromNhsWcfLibrary.Base.Action;

    [TestClass]
    public class EsrLocationRecordTest
    {
        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseLocationRecordInvalidNumberOfColumns()
        {
            string dataRow = "LCA~123456~888 z 0660 Management Accounts~Management Accounts~~A London Location~~~~~~United Kingdom~~~Yes~~~~~~20080403 183118";

            EsrLocationRecord parsedRecord = EsrLocationRecord.ParseEsrLocationRecord(dataRow);

            Assert.IsNull(parsedRecord, "Expected Null record not returned from ParseEsrLocationRecord()");
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseLocationRecordValidUpdateRecord()
        {
            string dataRow =
                "LCA~123456~888 z 0660 Management Accounts~Management Accounts~20140812~A London Location~Addr2~Addr3~MyTown~MyCounty~MyPostcode~United Kingdom~01234987654~0987654321~Yes~SC~WLocTranslation~WAddr1~WAddr2~WAddr3~WTown~20080403 183118";

            EsrLocationRecord parsedRecord = EsrLocationRecord.ParseEsrLocationRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrLocationRecord()");
            Assert.AreEqual(Action.Update, parsedRecord.Action);
            Assert.AreEqual(123456, parsedRecord.ESRLocationId);
            Assert.AreEqual("888 z 0660 Management Accounts", parsedRecord.LocationCode);
            Assert.AreEqual("Management Accounts", parsedRecord.Description);
            Assert.AreEqual(new DateTime(2014, 08, 12), parsedRecord.InactiveDate);
            Assert.AreEqual("A London Location", parsedRecord.AddressLine1);
            Assert.AreEqual("Addr2", parsedRecord.AddressLine2);
            Assert.AreEqual("Addr3", parsedRecord.AddressLine3);
            Assert.AreEqual("MyTown", parsedRecord.Town);
            Assert.AreEqual("MyCounty", parsedRecord.County);
            Assert.AreEqual("MyPostcode", parsedRecord.Postcode);
            Assert.AreEqual("United Kingdom", parsedRecord.Country);
            Assert.AreEqual("01234987654", parsedRecord.Telephone);
            Assert.AreEqual("0987654321", parsedRecord.Fax);
            Assert.AreEqual("Yes", parsedRecord.PayslipDeliveryPoint);
            Assert.AreEqual("SC", parsedRecord.SiteCode);
            Assert.AreEqual("WLocTranslation", parsedRecord.WelshLocationTranslation);
            Assert.AreEqual("WAddr1", parsedRecord.WelshAddress1);
            Assert.AreEqual("WAddr2", parsedRecord.WelshAddress2);
            Assert.AreEqual("WAddr3", parsedRecord.WelshAddress3);
            Assert.AreEqual("WTown", parsedRecord.WelshTownTranslation);
            Assert.AreEqual(new DateTime(2008,04,03,18,31,18), parsedRecord.ESRLastUpdate);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseLocationRecordValidDeleteRecord()
        {
            string dataRow = "LCD~123456";

            EsrLocationRecord parsedRecord = EsrLocationRecord.ParseEsrLocationRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrLocationRecord()");
            Assert.AreEqual(Action.Delete, parsedRecord.Action);
            Assert.AreEqual(123456, parsedRecord.ESRLocationId);
            Assert.AreEqual(string.Empty, parsedRecord.LocationCode);
            Assert.AreEqual(string.Empty, parsedRecord.Description);
            Assert.IsNull(parsedRecord.InactiveDate);
            Assert.AreEqual(string.Empty, parsedRecord.AddressLine1);
            Assert.AreEqual(string.Empty, parsedRecord.AddressLine2);
            Assert.AreEqual(string.Empty, parsedRecord.AddressLine3);
            Assert.AreEqual(string.Empty, parsedRecord.Town);
            Assert.AreEqual(string.Empty, parsedRecord.County);
            Assert.AreEqual(string.Empty, parsedRecord.Postcode);
            Assert.AreEqual(string.Empty, parsedRecord.Country);
            Assert.AreEqual(string.Empty, parsedRecord.Telephone);
            Assert.AreEqual(string.Empty, parsedRecord.Fax);
            Assert.AreEqual(string.Empty, parsedRecord.PayslipDeliveryPoint);
            Assert.AreEqual(string.Empty, parsedRecord.SiteCode);
            Assert.AreEqual(string.Empty, parsedRecord.WelshLocationTranslation);
            Assert.AreEqual(string.Empty, parsedRecord.WelshAddress1);
            Assert.AreEqual(string.Empty, parsedRecord.WelshAddress2);
            Assert.AreEqual(string.Empty, parsedRecord.WelshAddress3);
            Assert.AreEqual(string.Empty, parsedRecord.WelshTownTranslation);
            Assert.IsNull(parsedRecord.ESRLastUpdate);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseLocationRecordInvalidRecordType()
        {
            string dataRow = "LCC~123456~888 z 0660 Management Accounts~Management Accounts~~A London Location~~~~~~United Kingdom~~~Yes~~~~~~~20080403 183118";

            EsrLocationRecord parsedRecord = EsrLocationRecord.ParseEsrLocationRecord(dataRow);

            Assert.IsNull(parsedRecord, "Expected Null record not returned from ParseEsrLocationRecord()");
        }
    }
}
