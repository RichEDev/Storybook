using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest2012Ultimate.ApiLibrary.DataObjects.ESR
{
    using global::EsrGo2FromNhsWcfLibrary.ESR;
    using Action = global::EsrGo2FromNhsWcfLibrary.Base.Action;

    [TestClass]
    public class EsrOrganisationRecordTest
    {
        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseOrganisationRecordInvalidNumberColumns()
        {
            string dataRow = "ORA~654321~999 Renal Nursing~NHS_DE~19510101~~51662~19510101~~43110684~387100~~289587~20111101 151656~43110684~~";

            EsrOrganisationRecord parsedRecord = EsrOrganisationRecord.ParseEsrOrganisationRecord(dataRow);

            Assert.IsNull(parsedRecord, "Expected Null record not returned from ParseEsrOrganisationRecord()");
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseOrganisationValidUpdateRecord()
        {
            string dataRow = "ORA~654321~999 Renal Nursing~NHS_DE~19510101~20140101~51662~19510101~20150101~43110684~387100~NAC~289587~20111101 151656~43110684";

            EsrOrganisationRecord parsedRecord = EsrOrganisationRecord.ParseEsrOrganisationRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrOrganisationRecord()");
            Assert.AreEqual(Action.Update, parsedRecord.Action);
            Assert.AreEqual(654321, parsedRecord.ESROrganisationId);
            Assert.AreEqual("999 Renal Nursing", parsedRecord.OrganisationName);
            Assert.AreEqual("NHS_DE", parsedRecord.OrganisationType);
            Assert.AreEqual(new DateTime(1951, 01, 01), parsedRecord.EffectiveFrom);
            Assert.AreEqual(new DateTime(2014, 01, 01), parsedRecord.EffectiveTo);
            Assert.AreEqual(51662, parsedRecord.HierarchyVersionId);
            Assert.AreEqual(new DateTime(1951, 01, 01), parsedRecord.HierarchyVersionFrom);
            Assert.AreEqual(new DateTime(2015, 01, 01), parsedRecord.HierarchyVersionTo);
            Assert.AreEqual("43110684", parsedRecord.DefaultCostCentre);
            Assert.AreEqual(387100, parsedRecord.ParentOrganisationId);
            Assert.AreEqual("NAC", parsedRecord.NACSCode);
            Assert.AreEqual(new DateTime(2011,11,01,15,16,56), parsedRecord.ESRLastUpdateDate);
            Assert.AreEqual("43110684", parsedRecord.CostCentreDescription);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseOrganisationRecordValidDeleteRecord()
        {
            DateTime minDate = new DateTime(1900, 1, 1);
            string dataRow = "ORD~654321";

            EsrOrganisationRecord parsedRecord = EsrOrganisationRecord.ParseEsrOrganisationRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrOrganisationRecord()");
            Assert.AreEqual(Action.Delete, parsedRecord.Action);
            Assert.AreEqual(654321, parsedRecord.ESROrganisationId);
            Assert.AreEqual(string.Empty, parsedRecord.OrganisationName);
            Assert.AreEqual(string.Empty, parsedRecord.OrganisationType);
            Assert.AreEqual(minDate, parsedRecord.EffectiveFrom);
            Assert.IsNull(parsedRecord.EffectiveTo);
            Assert.AreEqual(0, parsedRecord.HierarchyVersionId);
            Assert.IsNull(parsedRecord.HierarchyVersionFrom);
            Assert.IsNull(parsedRecord.HierarchyVersionTo);
            Assert.AreEqual(string.Empty, parsedRecord.DefaultCostCentre);
            Assert.IsNull(parsedRecord.ParentOrganisationId);
            Assert.AreEqual(string.Empty, parsedRecord.NACSCode);
            Assert.IsNull(parsedRecord.ESRLastUpdateDate);
            Assert.AreEqual(string.Empty, parsedRecord.CostCentreDescription);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseOrganisationRecordInvalidRecordType()
        {
            string dataRow = "ORG~654321~999 Renal Nursing~NHS_DE~19510101~~51662~19510101~~43110684~387100~~289587~20111101 151656~43110684";

            EsrOrganisationRecord parsedRecord = EsrOrganisationRecord.ParseEsrOrganisationRecord(dataRow);

            Assert.IsNull(parsedRecord, "Expected Null record not returned from ParseEsrOrganisationRecord()");
        }
    }
}
