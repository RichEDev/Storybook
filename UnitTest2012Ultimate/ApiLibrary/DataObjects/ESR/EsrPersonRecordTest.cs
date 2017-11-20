using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest2012Ultimate.ApiLibrary.DataObjects.ESR
{
    using global::EsrGo2FromNhsWcfLibrary.ESR;
    using Action = global::EsrGo2FromNhsWcfLibrary.Base.Action;

    [TestClass]
    public class EsrPersonRecordTest
    {
        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParsePersonRecordInvalidRecordType()
        {
            string dataRow = "PER~10820689~20121011~~23519456~Mrs.~August~Stephanie~Jane~Wade~~~~19740421~JA230106C~4799829~20121011~~~~~~~~~~~~~~~~~~Employee~~~~20121112 115340~~~~~";

            EsrPersonRecord parsedRecord = EsrPersonRecord.ParseEsrPersonRecord(dataRow);

            Assert.IsNull(parsedRecord, "Expected Null record not returned from ParseEsrPersonRecord()");
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParsePersonRecordInvalidNumberColumns()
        {
            const string dataRow = "PRA~10820689~20121011~~23519456~Mrs.~August~Stephanie~Jane~Wade~~~~19740421~JA230106C~4799829~20121011~~~~~~~~~~~~~~~~~~Employee~~~~20121112 115340~~~~~~";

            EsrPersonRecord parsedRecord = EsrPersonRecord.ParseEsrPersonRecord(dataRow);

            Assert.IsNull(parsedRecord, "Expected Null record not returned from ParseEsrPersonRecord()");
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseEsrPersonValidDeleteRecord()
        {
            DateTime minDate = new DateTime(1900, 1, 1);
            const string dataRow = "PRD~10820689";

            EsrPersonRecord parsedRecord = EsrPersonRecord.ParseEsrPersonRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrPersonRecord()");
            Assert.AreEqual(Action.Delete, parsedRecord.Action);
            Assert.AreEqual(10820689, parsedRecord.ESRPersonId);
            Assert.AreEqual(minDate, parsedRecord.EffectiveStartDate);
            Assert.IsNull(parsedRecord.EffectiveEndDate);
            Assert.AreEqual(string.Empty, parsedRecord.EmployeeNumber);
            Assert.AreEqual(string.Empty, parsedRecord.Title);
            Assert.AreEqual(string.Empty, parsedRecord.LastName);
            Assert.AreEqual(string.Empty, parsedRecord.FirstName);
            Assert.AreEqual(string.Empty, parsedRecord.LastName);
            Assert.AreEqual(string.Empty, parsedRecord.MiddleNames);
            Assert.AreEqual(string.Empty, parsedRecord.MaidenName);
            Assert.AreEqual(string.Empty, parsedRecord.PreferredName);
            Assert.AreEqual(string.Empty, parsedRecord.PreviousLastName);
            Assert.AreEqual(string.Empty, parsedRecord.Gender);
            Assert.IsNull(parsedRecord.DateOfBirth);
            Assert.AreEqual(string.Empty, parsedRecord.NINumber);
            Assert.AreEqual(string.Empty, parsedRecord.NHSUniqueId);
            Assert.IsNull(parsedRecord.HireDate);
            Assert.IsNull(parsedRecord.ActualTerminationDate);
            Assert.AreEqual(string.Empty, parsedRecord.TerminationReason);
            Assert.AreEqual(string.Empty, parsedRecord.EmployeeStatusFlag);
            Assert.AreEqual(string.Empty, parsedRecord.WTROptOut);
            Assert.IsNull(parsedRecord.WTROptOutDate);
            Assert.AreEqual(string.Empty, parsedRecord.EthnicOrigin);
            Assert.AreEqual(string.Empty, parsedRecord.MaritalStatus);
            Assert.AreEqual(string.Empty, parsedRecord.CountryOfBirth);
            Assert.AreEqual(string.Empty, parsedRecord.PreviousEmployer);
            Assert.AreEqual(string.Empty, parsedRecord.PreviousEmployerType);
            Assert.IsNull(parsedRecord.CSD3Months);
            Assert.IsNull(parsedRecord.CSD12Months);
            Assert.AreEqual(string.Empty, parsedRecord.NHSCRSUUID);
            Assert.AreEqual(string.Empty, parsedRecord.SystemPersonType);
            Assert.AreEqual(string.Empty, parsedRecord.UserPersonType);
            Assert.AreEqual(string.Empty, parsedRecord.OfficeEmailAddress);
            Assert.IsNull(parsedRecord.NHSStartDate);
            Assert.IsNull(parsedRecord.ESRLastUpdateDate);
            Assert.AreEqual(string.Empty, parsedRecord.DisabilityFlag);
            Assert.AreEqual(string.Empty, parsedRecord.LegacyPayrollNumber);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseEsrPersonInvalidDeleteRecord()
        {
            DateTime minDate = new DateTime(1900, 1, 1);
            const string dataRow = "PRD~z0820689";

            EsrPersonRecord parsedRecord = EsrPersonRecord.ParseEsrPersonRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrPersonRecord()");
            Assert.AreEqual(Action.Delete, parsedRecord.Action);
            Assert.AreEqual(0, parsedRecord.ESRPersonId);
            Assert.AreEqual(minDate, parsedRecord.EffectiveStartDate);
            Assert.IsNull(parsedRecord.EffectiveEndDate);
            Assert.AreEqual(string.Empty, parsedRecord.EmployeeNumber);
            Assert.AreEqual(string.Empty, parsedRecord.Title);
            Assert.AreEqual(string.Empty, parsedRecord.LastName);
            Assert.AreEqual(string.Empty, parsedRecord.FirstName);
            Assert.AreEqual(string.Empty, parsedRecord.MiddleNames);
            Assert.AreEqual(string.Empty, parsedRecord.MaidenName);
            Assert.AreEqual(string.Empty, parsedRecord.PreferredName);
            Assert.AreEqual(string.Empty, parsedRecord.PreviousLastName);
            Assert.AreEqual(string.Empty, parsedRecord.Gender);
            Assert.IsNull(parsedRecord.DateOfBirth);
            Assert.AreEqual(string.Empty, parsedRecord.NINumber);
            Assert.AreEqual(string.Empty, parsedRecord.NHSUniqueId);
            Assert.IsNull(parsedRecord.HireDate);
            Assert.IsNull(parsedRecord.ActualTerminationDate);
            Assert.AreEqual(string.Empty, parsedRecord.TerminationReason);
            Assert.AreEqual(string.Empty, parsedRecord.EmployeeStatusFlag);
            Assert.AreEqual(string.Empty, parsedRecord.WTROptOut);
            Assert.IsNull(parsedRecord.WTROptOutDate);
            Assert.AreEqual(string.Empty, parsedRecord.EthnicOrigin);
            Assert.AreEqual(string.Empty, parsedRecord.MaritalStatus);
            Assert.AreEqual(string.Empty, parsedRecord.CountryOfBirth);
            Assert.AreEqual(string.Empty, parsedRecord.PreviousEmployer);
            Assert.AreEqual(string.Empty, parsedRecord.PreviousEmployerType);
            Assert.IsNull(parsedRecord.CSD3Months);
            Assert.IsNull(parsedRecord.CSD12Months);
            Assert.AreEqual(string.Empty, parsedRecord.NHSCRSUUID);
            Assert.AreEqual(string.Empty, parsedRecord.SystemPersonType);
            Assert.AreEqual(string.Empty, parsedRecord.UserPersonType);
            Assert.AreEqual(string.Empty, parsedRecord.OfficeEmailAddress);
            Assert.IsNull(parsedRecord.NHSStartDate);
            Assert.IsNull(parsedRecord.ESRLastUpdateDate);
            Assert.AreEqual(string.Empty, parsedRecord.DisabilityFlag);
            Assert.AreEqual(string.Empty, parsedRecord.LegacyPayrollNumber);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseEsrPersonValidUpdateRecord()
        {
            const string dataRow = "PRA~10820689~20121011~20121231~23519456~Mrs.~Smith~Julie~Jane~Wade~PrefName~PrevName~F~19740421~ZY230106L~1234567~20121011~20130103~TermReason~EmpStFlag~WOO~20100404~White~Single~England~NA~Nothing~20110303~20110202~123456789012~~~~EMP~Employee~me@myoffice.com~19990707~~20121112 115340~N~LegPayrollNum~British~~";

            EsrPersonRecord parsedRecord = EsrPersonRecord.ParseEsrPersonRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrPersonRecord()");
            Assert.AreEqual(Action.Update, parsedRecord.Action);
            Assert.AreEqual(10820689, parsedRecord.ESRPersonId);
            Assert.AreEqual(new DateTime(2012,10,11), parsedRecord.EffectiveStartDate);
            Assert.AreEqual(new DateTime(2012,12,31), parsedRecord.EffectiveEndDate);
            Assert.AreEqual("23519456", parsedRecord.EmployeeNumber);
            Assert.AreEqual("Mrs.", parsedRecord.Title);
            Assert.AreEqual("Smith", parsedRecord.LastName);
            Assert.AreEqual("Julie", parsedRecord.FirstName);
            Assert.AreEqual("Jane", parsedRecord.MiddleNames);
            Assert.AreEqual("Wade", parsedRecord.MaidenName);
            Assert.AreEqual("PrefName", parsedRecord.PreferredName);
            Assert.AreEqual("PrevName", parsedRecord.PreviousLastName);
            Assert.AreEqual("F", parsedRecord.Gender);
            Assert.AreEqual(new DateTime(1974,04,21), parsedRecord.DateOfBirth);
            Assert.AreEqual("ZY230106L", parsedRecord.NINumber);
            Assert.AreEqual("1234567", parsedRecord.NHSUniqueId);
            Assert.AreEqual(new DateTime(2012,10,11), parsedRecord.HireDate);
            Assert.AreEqual(new DateTime(2013,1,3), parsedRecord.ActualTerminationDate);
            Assert.AreEqual("TermReason", parsedRecord.TerminationReason);
            Assert.AreEqual("EmpStFlag", parsedRecord.EmployeeStatusFlag);
            Assert.AreEqual("WOO", parsedRecord.WTROptOut);
            Assert.AreEqual(new DateTime(2010,4,4),parsedRecord.WTROptOutDate);
            Assert.AreEqual("White", parsedRecord.EthnicOrigin);
            Assert.AreEqual("Single", parsedRecord.MaritalStatus);
            Assert.AreEqual("England", parsedRecord.CountryOfBirth);
            Assert.AreEqual("NA", parsedRecord.PreviousEmployer);
            Assert.AreEqual("Nothing", parsedRecord.PreviousEmployerType);
            Assert.AreEqual(new DateTime(2011,3,3), parsedRecord.CSD3Months);
            Assert.AreEqual(new DateTime(2011,2,2), parsedRecord.CSD12Months);
            Assert.AreEqual("123456789012", parsedRecord.NHSCRSUUID);
            Assert.AreEqual("EMP", parsedRecord.SystemPersonType);
            Assert.AreEqual("Employee", parsedRecord.UserPersonType);
            Assert.AreEqual("me@myoffice.com", parsedRecord.OfficeEmailAddress);
            Assert.AreEqual(new DateTime(1999,7,7), parsedRecord.NHSStartDate);
            Assert.AreEqual(new DateTime(2012,11,12,11,53,40), parsedRecord.ESRLastUpdateDate);
            Assert.AreEqual("N", parsedRecord.DisabilityFlag);
            Assert.AreEqual("LegPayrollNum", parsedRecord.LegacyPayrollNumber);
            Assert.AreEqual("British", parsedRecord.Nationality);
        }
    }
}
