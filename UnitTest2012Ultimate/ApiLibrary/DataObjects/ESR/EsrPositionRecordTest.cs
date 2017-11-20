using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest2012Ultimate.ApiLibrary.DataObjects.ESR
{
    using global::EsrGo2FromNhsWcfLibrary.ESR;
    using Action = global::EsrGo2FromNhsWcfLibrary.Base.Action;

    [TestClass]
    public class EsrPositionRecordTest
    {
        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParsePositionRecordInvalidNumberColumns()
        {
            const string dataRow = "POA~1576747~20080401~~3560241~3560241|Consultant|021|General Surgery|~1~4311100~Medical and Dental~Consultant~021~YM72~YM72|1~Not Applicable~387169~Active~NONE~Yes~No~~~~20091109 154418~Consultants~~";

            EsrPositionRecord parsedRecord = EsrPositionRecord.ParseEsrPositionRecord(dataRow);

            Assert.IsNull(parsedRecord, "Expected Null record not returned from ParseEsrPositionRecord()");
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParsePositionValidUpdateRecord()
        {
            const string dataRow = "POA~654321~20080401~~3560241~3560241|Consultant|021|General Surgery|~1~4311100~Medical and Dental~Consultant~021~YM72~YM72|1~Not Applicable~387169~Active~NONE~Yes~No~DPNum~MDPNum~WOC~20091109 154418~Consultants";

            EsrPositionRecord parsedRecord = EsrPositionRecord.ParseEsrPositionRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrPositionRecord()");
            Assert.AreEqual(Action.Update, parsedRecord.Action);
            Assert.AreEqual(654321, parsedRecord.ESRPositionId);
            Assert.AreEqual(new DateTime(2008, 04, 01), parsedRecord.EffectiveFromDate);
            Assert.IsNull(parsedRecord.EffectiveToDate);
            Assert.AreEqual(3560241, parsedRecord.PositionNumber);
            Assert.AreEqual("3560241|Consultant|021|General Surgery|", parsedRecord.PositionName);
            Assert.AreEqual(1, parsedRecord.BudgetedFTE);
            Assert.AreEqual("4311100", parsedRecord.SubjectiveCode);
            Assert.AreEqual("Medical and Dental", parsedRecord.JobStaffGroup);
            Assert.AreEqual("Consultant", parsedRecord.JobRole);
            Assert.AreEqual("021", parsedRecord.OccupationCode);
            Assert.AreEqual("YM72", parsedRecord.Payscale);
            Assert.AreEqual("YM72|1", parsedRecord.GradeStep);
            Assert.AreEqual("Not Applicable", parsedRecord.ISARegulatedPost);
            Assert.AreEqual(387169, parsedRecord.ESROrganisationId);
            Assert.AreEqual("Active", parsedRecord.HiringStatus);
            Assert.AreEqual("NONE", parsedRecord.PositionType);
            Assert.AreEqual("Yes", parsedRecord.OHProcessingEligible);
            Assert.AreEqual("No", parsedRecord.EPPFlag);
            Assert.AreEqual("DPNum", parsedRecord.DeaneryPostNumber);
            Assert.AreEqual("MDPNum",parsedRecord.ManagingDeaneryBody);
            Assert.AreEqual("WOC", parsedRecord.WorkplaceOrgCode);
            Assert.AreEqual(new DateTime(2009, 11, 09, 15, 44, 18), parsedRecord.ESRLastUpdateDate);
            Assert.AreEqual("Consultants", parsedRecord.SubjectiveCodeDescription);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParsePositionRecordValidDeleteRecord()
        {
            DateTime minDate = new DateTime(1900, 1, 1);
            const string dataRow = "POD~654321";

            EsrPositionRecord parsedRecord = EsrPositionRecord.ParseEsrPositionRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrOrganisationRecord()");
            Assert.AreEqual(Action.Delete, parsedRecord.Action);
            Assert.AreEqual(654321, parsedRecord.ESRPositionId);
            Assert.AreEqual(minDate, parsedRecord.EffectiveFromDate);
            Assert.IsNull(parsedRecord.EffectiveToDate);
            Assert.AreEqual(0, parsedRecord.PositionNumber);
            Assert.AreEqual(string.Empty, parsedRecord.PositionName);
            Assert.AreEqual(0, parsedRecord.BudgetedFTE);
            Assert.AreEqual(string.Empty, parsedRecord.SubjectiveCode);
            Assert.AreEqual(string.Empty, parsedRecord.JobStaffGroup);
            Assert.AreEqual(string.Empty, parsedRecord.JobRole);
            Assert.AreEqual(string.Empty, parsedRecord.OccupationCode);
            Assert.AreEqual(string.Empty, parsedRecord.Payscale);
            Assert.AreEqual(string.Empty, parsedRecord.GradeStep);
            Assert.AreEqual(string.Empty, parsedRecord.ISARegulatedPost);
            Assert.IsNull(parsedRecord.ESROrganisationId);
            Assert.AreEqual(string.Empty, parsedRecord.HiringStatus);
            Assert.AreEqual(string.Empty, parsedRecord.PositionType);
            Assert.AreEqual(string.Empty, parsedRecord.OHProcessingEligible);
            Assert.AreEqual(string.Empty, parsedRecord.EPPFlag);
            Assert.AreEqual(string.Empty, parsedRecord.DeaneryPostNumber);
            Assert.AreEqual(string.Empty, parsedRecord.ManagingDeaneryBody);
            Assert.AreEqual(string.Empty, parsedRecord.WorkplaceOrgCode);
            Assert.AreEqual(minDate, parsedRecord.ESRLastUpdateDate);
            Assert.AreEqual(string.Empty, parsedRecord.SubjectiveCodeDescription);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParsePositionRecordInvalidRecordType()
        {
            const string dataRow = "POS~1576747~20080401~~3560241~3560241|Consultant|021|General Surgery|~1~4311100~Medical and Dental~Consultant~021~YM72~YM72|1~Not Applicable~387169~Active~NONE~Yes~No~~~~20091109 154418~Consultants";

            EsrPositionRecord parsedRecord = EsrPositionRecord.ParseEsrPositionRecord(dataRow);

            Assert.IsNull(parsedRecord, "Expected Null record not returned from ParseEsrPositionRecord()");
        }
    }
}
