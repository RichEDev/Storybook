using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest2012Ultimate.ApiLibrary.DataObjects.ESR
{
    using global::EsrGo2FromNhsWcfLibrary.ESR;
    using Action = global::EsrGo2FromNhsWcfLibrary.Base.Action;

    [TestClass]
    public class EsrAssignmentCostingRecordTest
    {
        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseAssignmentCostingRecordValidDeleteRecord()
        {
            DateTime minDate = new DateTime(1900, 1, 1);
            const string dataRow = "COD~87654321";

            EsrAssignmentCostingRecord parsedRecord = EsrAssignmentCostingRecord.ParseEsrAssignmentCostingRecord(dataRow);
            
            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrAssignmentCostingRecord()");
            Assert.AreEqual(Action.Delete, parsedRecord.Action);
            Assert.AreEqual(87654321, parsedRecord.ESRCostingAllocationId);
            Assert.AreEqual(0, parsedRecord.ESRPersonId);
            Assert.AreEqual(0, parsedRecord.ESRAssignmentId);
            Assert.AreEqual(minDate, parsedRecord.EffectiveStartDate);
            Assert.IsNull(parsedRecord.EffectiveEndDate);
            Assert.AreEqual(string.Empty, parsedRecord.EntityCode);
            Assert.AreEqual(string.Empty, parsedRecord.CharitableIndicator);
            Assert.AreEqual(string.Empty, parsedRecord.CostCentre);
            Assert.AreEqual(string.Empty, parsedRecord.Subjective);
            Assert.AreEqual(string.Empty, parsedRecord.Analysis1);
            Assert.AreEqual(string.Empty, parsedRecord.Analysis2);
            Assert.AreEqual(0, parsedRecord.ElementNumber);
            Assert.AreEqual(0, parsedRecord.PercentageSplit);
            Assert.IsNull(parsedRecord.ESRLastUpdate);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseAssignmentCostingRecordInvalidNumberOfColumns()
        {
            const string dataRow = "COA~10810330~11467515~7160446~20121031~~~~~~431AC04~~~~~~";

            EsrAssignmentCostingRecord parsedRecord = EsrAssignmentCostingRecord.ParseEsrAssignmentCostingRecord(dataRow);

            Assert.IsNull(parsedRecord, "Expected Null record not returned from ParseEsrAssignmentCostingRecord()");
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseAssignmentCostingInvalidRecordType()
        {
            const string dataRow = "COB~10810330~11467515~7160446~20121031~~~~~431AC04~~~~~~";

            EsrAssignmentCostingRecord parsedRecord = EsrAssignmentCostingRecord.ParseEsrAssignmentCostingRecord(dataRow);

            Assert.IsNull(parsedRecord, "Expected Null record not returned from ParseEsrAssignmentCostingRecord()");
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParsePhoneRecordValidUpdateRecord()
        {
            const string dataRow = "COA~12345678~55555555~7654321~20121031~20140101~EC~N~CostCentre001~SubjectiveField~431AC04~431AC05~999~Spare~25.75~20130107 160100";

            EsrAssignmentCostingRecord parsedRecord = EsrAssignmentCostingRecord.ParseEsrAssignmentCostingRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrPhoneRecord()");
            Assert.AreEqual(Action.Update, parsedRecord.Action);
            Assert.AreEqual(7654321, parsedRecord.ESRCostingAllocationId);
            Assert.AreEqual(12345678, parsedRecord.ESRPersonId);
            Assert.AreEqual(55555555, parsedRecord.ESRAssignmentId);
            Assert.AreEqual(new DateTime(2012,10,31), parsedRecord.EffectiveStartDate);
            Assert.AreEqual(new DateTime(2014,1,1), parsedRecord.EffectiveEndDate);
            Assert.AreEqual("EC", parsedRecord.EntityCode);
            Assert.AreEqual("N", parsedRecord.CharitableIndicator);
            Assert.AreEqual("CostCentre001", parsedRecord.CostCentre);
            Assert.AreEqual("SubjectiveField", parsedRecord.Subjective);
            Assert.AreEqual("431AC04", parsedRecord.Analysis1);
            Assert.AreEqual("431AC05", parsedRecord.Analysis2);
            Assert.AreEqual(999, parsedRecord.ElementNumber);
            Assert.AreEqual(Convert.ToDecimal(25.75), parsedRecord.PercentageSplit);
            Assert.AreEqual(new DateTime(2013,1,7,16,1,0), parsedRecord.ESRLastUpdate);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParsePhoneRecordInvalidStartDate()
        {
            DateTime minDate = new DateTime(1900, 1, 1);
            const string dataRow = "COA~12345678~55555555~7654321~20121331~20140101~EC~N~CostCentre001~SubjectiveField~431AC04~431AC05~999~Spare~25.75~20130107 160100";

            EsrAssignmentCostingRecord parsedRecord = EsrAssignmentCostingRecord.ParseEsrAssignmentCostingRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrPhoneRecord()");
            Assert.AreEqual(Action.Update, parsedRecord.Action);
            Assert.AreEqual(7654321, parsedRecord.ESRCostingAllocationId);
            Assert.AreEqual(12345678, parsedRecord.ESRPersonId);
            Assert.AreEqual(55555555, parsedRecord.ESRAssignmentId);
            Assert.AreEqual(minDate, parsedRecord.EffectiveStartDate);
            Assert.AreEqual(new DateTime(2014, 1, 1), parsedRecord.EffectiveEndDate);
            Assert.AreEqual("EC", parsedRecord.EntityCode);
            Assert.AreEqual("N", parsedRecord.CharitableIndicator);
            Assert.AreEqual("CostCentre001", parsedRecord.CostCentre);
            Assert.AreEqual("SubjectiveField", parsedRecord.Subjective);
            Assert.AreEqual("431AC04", parsedRecord.Analysis1);
            Assert.AreEqual("431AC05", parsedRecord.Analysis2);
            Assert.AreEqual(999, parsedRecord.ElementNumber);
            Assert.AreEqual(Convert.ToDecimal(25.75), parsedRecord.PercentageSplit);
            Assert.AreEqual(new DateTime(2013, 1, 7, 16, 1, 0), parsedRecord.ESRLastUpdate);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParsePhoneRecordInvalidEndDate()
        {
            const string dataRow = "COA~12345678~55555555~7654321~20121031~20140132~EC~N~CostCentre001~SubjectiveField~431AC04~431AC05~999~Spare~25.75~20130107 160100";

            EsrAssignmentCostingRecord parsedRecord = EsrAssignmentCostingRecord.ParseEsrAssignmentCostingRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrPhoneRecord()");
            Assert.AreEqual(Action.Update, parsedRecord.Action);
            Assert.AreEqual(7654321, parsedRecord.ESRCostingAllocationId);
            Assert.AreEqual(12345678, parsedRecord.ESRPersonId);
            Assert.AreEqual(55555555, parsedRecord.ESRAssignmentId);
            Assert.AreEqual(new DateTime(2012, 10, 31), parsedRecord.EffectiveStartDate);
            Assert.IsNull(parsedRecord.EffectiveEndDate);
            Assert.AreEqual("EC", parsedRecord.EntityCode);
            Assert.AreEqual("N", parsedRecord.CharitableIndicator);
            Assert.AreEqual("CostCentre001", parsedRecord.CostCentre);
            Assert.AreEqual("SubjectiveField", parsedRecord.Subjective);
            Assert.AreEqual("431AC04", parsedRecord.Analysis1);
            Assert.AreEqual("431AC05", parsedRecord.Analysis2);
            Assert.AreEqual(999, parsedRecord.ElementNumber);
            Assert.AreEqual(Convert.ToDecimal(25.75), parsedRecord.PercentageSplit);
            Assert.AreEqual(new DateTime(2013, 1, 7, 16, 1, 0), parsedRecord.ESRLastUpdate);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParsePhoneRecordInvalidUpdateDate()
        {
            const string dataRow = "COA~12345678~55555555~7654321~20121031~20140101~EC~N~CostCentre001~SubjectiveField~431AC04~431AC05~999~Spare~25.75~20130199 160100";

            EsrAssignmentCostingRecord parsedRecord = EsrAssignmentCostingRecord.ParseEsrAssignmentCostingRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrPhoneRecord()");
            Assert.AreEqual(Action.Update, parsedRecord.Action);
            Assert.AreEqual(7654321, parsedRecord.ESRCostingAllocationId);
            Assert.AreEqual(12345678, parsedRecord.ESRPersonId);
            Assert.AreEqual(55555555, parsedRecord.ESRAssignmentId);
            Assert.AreEqual(new DateTime(2012, 10, 31), parsedRecord.EffectiveStartDate);
            Assert.AreEqual(new DateTime(2014, 1, 1), parsedRecord.EffectiveEndDate);
            Assert.AreEqual("EC", parsedRecord.EntityCode);
            Assert.AreEqual("N", parsedRecord.CharitableIndicator);
            Assert.AreEqual("CostCentre001", parsedRecord.CostCentre);
            Assert.AreEqual("SubjectiveField", parsedRecord.Subjective);
            Assert.AreEqual("431AC04", parsedRecord.Analysis1);
            Assert.AreEqual("431AC05", parsedRecord.Analysis2);
            Assert.AreEqual(999, parsedRecord.ElementNumber);
            Assert.AreEqual(Convert.ToDecimal(25.75), parsedRecord.PercentageSplit);
            Assert.IsNull(parsedRecord.ESRLastUpdate);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParsePhoneRecordIOnvalidElementNumber()
        {
            const string dataRow = "COA~12345678~55555555~7654321~20121031~20140101~EC~N~CostCentre001~SubjectiveField~431AC04~431AC05~abc~Spare~25.75~20130107 160100";

            EsrAssignmentCostingRecord parsedRecord = EsrAssignmentCostingRecord.ParseEsrAssignmentCostingRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrPhoneRecord()");
            Assert.AreEqual(Action.Update, parsedRecord.Action);
            Assert.AreEqual(7654321, parsedRecord.ESRCostingAllocationId);
            Assert.AreEqual(12345678, parsedRecord.ESRPersonId);
            Assert.AreEqual(55555555, parsedRecord.ESRAssignmentId);
            Assert.AreEqual(new DateTime(2012, 10, 31), parsedRecord.EffectiveStartDate);
            Assert.AreEqual(new DateTime(2014, 1, 1), parsedRecord.EffectiveEndDate);
            Assert.AreEqual("EC", parsedRecord.EntityCode);
            Assert.AreEqual("N", parsedRecord.CharitableIndicator);
            Assert.AreEqual("CostCentre001", parsedRecord.CostCentre);
            Assert.AreEqual("SubjectiveField", parsedRecord.Subjective);
            Assert.AreEqual("431AC04", parsedRecord.Analysis1);
            Assert.AreEqual("431AC05", parsedRecord.Analysis2);
            Assert.AreEqual(0, parsedRecord.ElementNumber);
            Assert.AreEqual(Convert.ToDecimal(25.75), parsedRecord.PercentageSplit);
            Assert.AreEqual(new DateTime(2013, 1, 7, 16, 1, 0), parsedRecord.ESRLastUpdate);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParsePhoneRecordInvalidPercentageSplit()
        {
            const string dataRow = "COA~12345678~55555555~7654321~20121031~20140101~EC~N~CostCentre001~SubjectiveField~431AC04~431AC05~999~Spare~abc~20130107 160100";

            EsrAssignmentCostingRecord parsedRecord = EsrAssignmentCostingRecord.ParseEsrAssignmentCostingRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrPhoneRecord()");
            Assert.AreEqual(Action.Update, parsedRecord.Action);
            Assert.AreEqual(7654321, parsedRecord.ESRCostingAllocationId);
            Assert.AreEqual(12345678, parsedRecord.ESRPersonId);
            Assert.AreEqual(55555555, parsedRecord.ESRAssignmentId);
            Assert.AreEqual(new DateTime(2012, 10, 31), parsedRecord.EffectiveStartDate);
            Assert.AreEqual(new DateTime(2014, 1, 1), parsedRecord.EffectiveEndDate);
            Assert.AreEqual("EC", parsedRecord.EntityCode);
            Assert.AreEqual("N", parsedRecord.CharitableIndicator);
            Assert.AreEqual("CostCentre001", parsedRecord.CostCentre);
            Assert.AreEqual("SubjectiveField", parsedRecord.Subjective);
            Assert.AreEqual("431AC04", parsedRecord.Analysis1);
            Assert.AreEqual("431AC05", parsedRecord.Analysis2);
            Assert.AreEqual(999, parsedRecord.ElementNumber);
            Assert.AreEqual(0, parsedRecord.PercentageSplit);
            Assert.AreEqual(new DateTime(2013, 1, 7, 16, 1, 0), parsedRecord.ESRLastUpdate);
        }
    }
}
