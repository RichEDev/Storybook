using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest2012Ultimate.ApiLibrary.DataObjects.ESR
{
    using global::EsrGo2FromNhsWcfLibrary.ESR;
    using Action = global::EsrGo2FromNhsWcfLibrary.Base.Action;

    [TestClass]
    public class EsrAssignmentRecordTest
    {
        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseAssignmentRecordInvalidRecordType()
        {
            const string dataRow = "ASG~10820689~11479620~20121011~~20121011~E~23519456~~~~431 Weekly~Week~289587~~~~~~~~Yes~~~~~~386934~~3559596|Bank Nurse Band 5|N7B|Bank~~~~~Nursing and Midwifery Registered|Staff Nurse~431 z 0850 Bank Control|||~~~~~~~~~~~~~~";

            EsrAssignmentRecord parsedRecord = EsrAssignmentRecord.ParseEsrAssignmentRecord(dataRow);

            Assert.IsNull(parsedRecord, "Expected Null record not returned from ParseEsrAssignmentRecord()");
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseAssignmentRecordInvalidNumberColumns()
        {
            const string dataRow = "ASA~87654321~12345678~20121011~~20121011~E~23519456~~~~431 Weekly~Week~289587~~~~~~~~Yes~~~~~~386934~~3559596|Bank Nurse Band 5|N7B|Bank~~~~~Nursing and Midwifery Registered|Staff Nurse~431 z 0850 Bank Control|||~~~~~~~~~~~~";

            EsrAssignmentRecord parsedRecord = EsrAssignmentRecord.ParseEsrAssignmentRecord(dataRow);
            
            Assert.IsNull(parsedRecord, "Expected Null record not returned from ParseEsrAssignmentRecord()");
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseAssignmentRecordValidDeleteRecord()
        {
            const string dataRow = "ASD~12345678";
            DateTime minDate = new DateTime(1900, 1, 1);

            EsrAssignmentRecord parsedRecord = EsrAssignmentRecord.ParseEsrAssignmentRecord(dataRow);
            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrAssignmentRecord()");
            Assert.AreEqual(Action.Delete, parsedRecord.Action);
            Assert.AreEqual(0, parsedRecord.ESRPersonId);
            Assert.AreEqual(12345678, parsedRecord.AssignmentID);
            Assert.AreEqual(minDate, parsedRecord.EffectiveStartDate);
            Assert.IsNull(parsedRecord.EffectiveEndDate);
            Assert.AreEqual(minDate, parsedRecord.EarliestAssignmentStartDate);
            Assert.AreEqual(string.Empty, parsedRecord.AssignmentType);
            Assert.AreEqual(string.Empty, parsedRecord.AssignmentNumber);
            Assert.AreEqual(string.Empty, parsedRecord.SystemAssignmentStatus);
            Assert.AreEqual(string.Empty, parsedRecord.AssignmentStatus);
            Assert.AreEqual(string.Empty, parsedRecord.EmployeeStatusFlag);
            Assert.AreEqual(string.Empty, parsedRecord.PayrollName);
            Assert.AreEqual(string.Empty, parsedRecord.PayrollPeriodType);
            Assert.IsNull(parsedRecord.EsrLocationId);
            Assert.AreEqual(string.Empty, parsedRecord.SupervisorFlag);
            Assert.AreEqual(null, parsedRecord.SupervisorPersonId);
            Assert.AreEqual(null, parsedRecord.SupervisorAssignmentId);
            Assert.AreEqual(string.Empty, parsedRecord.SupervisorAssignmentNumber);
            Assert.IsNull(parsedRecord.DepartmentManagerAssignmentId);
            Assert.AreEqual(string.Empty, parsedRecord.EmployeeCategory);
            Assert.AreEqual(string.Empty, parsedRecord.AssignmentCategory);
            Assert.AreEqual(string.Empty, parsedRecord.PrimaryAssignmentString);
            Assert.AreEqual(0, parsedRecord.NormalHours);
            Assert.AreEqual(string.Empty, parsedRecord.NormalHoursFrequency);
            Assert.AreEqual(0, parsedRecord.GradeContractHours);
            Assert.AreEqual(0, parsedRecord.Fte);
            Assert.AreEqual(string.Empty, parsedRecord.FlexibleWorkingPattern);
            Assert.IsNull(parsedRecord.EsrOrganisationId);
            Assert.IsNull(parsedRecord.EsrPositionId);
            Assert.AreEqual(string.Empty, parsedRecord.PositionName);
            Assert.AreEqual(string.Empty, parsedRecord.Grade);
            Assert.AreEqual(string.Empty, parsedRecord.GradeStep);
            Assert.IsNull(parsedRecord.StartDateInGrade);
            Assert.AreEqual(0,parsedRecord.AnnualSalaryValue);
            Assert.AreEqual(string.Empty, parsedRecord.JobName);
            Assert.AreEqual(string.Empty, parsedRecord.Group);
            Assert.AreEqual(string.Empty, parsedRecord.TAndAFlag);
            Assert.AreEqual(string.Empty, parsedRecord.NightWorkerOptOut);
            Assert.IsNull(parsedRecord.ProjectedHireDate);
            Assert.IsNull(parsedRecord.VacancyID);
            Assert.IsNull(parsedRecord.ContractEndDate);
            Assert.IsNull(parsedRecord.IncrementDate);
            Assert.AreEqual(string.Empty, parsedRecord.MaximumPartTimeFlag);
            Assert.AreEqual(string.Empty, parsedRecord.AfcFlag);
            Assert.IsNull(parsedRecord.EsrLastUpdate);
            Assert.IsNull(parsedRecord.LastWorkingDay);
            Assert.AreEqual(string.Empty, parsedRecord.eKSFSpinalPoint);
            Assert.AreEqual(string.Empty, parsedRecord.ManagerFlag);
            Assert.IsNull(parsedRecord.FinalAssignmentEndDate);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseAssignmentRecordInvalidDeleteRecord()
        {
            DateTime minDate = new DateTime(1900, 1, 1);
            const string dataRow = "ASD~z2345678";

            EsrAssignmentRecord parsedRecord = EsrAssignmentRecord.ParseEsrAssignmentRecord(dataRow);
            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrAssignmentRecord()");
            Assert.AreEqual(Action.Delete, parsedRecord.Action);
            Assert.AreEqual(0, parsedRecord.AssignmentID);
            Assert.AreEqual(minDate, parsedRecord.EffectiveStartDate);
            Assert.IsNull(parsedRecord.EffectiveEndDate);
            Assert.AreEqual(minDate, parsedRecord.EarliestAssignmentStartDate);
            Assert.AreEqual(string.Empty, parsedRecord.AssignmentType);
            Assert.AreEqual(string.Empty, parsedRecord.AssignmentNumber);
            Assert.AreEqual(string.Empty, parsedRecord.SystemAssignmentStatus);
            Assert.AreEqual(string.Empty, parsedRecord.AssignmentStatus);
            Assert.AreEqual(string.Empty, parsedRecord.EmployeeStatusFlag);
            Assert.AreEqual(string.Empty, parsedRecord.PayrollName);
            Assert.AreEqual(string.Empty, parsedRecord.PayrollPeriodType);
            Assert.IsNull(parsedRecord.EsrLocationId);
            Assert.AreEqual(string.Empty, parsedRecord.SupervisorFlag);
            Assert.AreEqual(null, parsedRecord.SupervisorPersonId);
            Assert.AreEqual(null, parsedRecord.SupervisorAssignmentId);
            Assert.AreEqual(string.Empty, parsedRecord.SupervisorAssignmentNumber);
            Assert.IsNull(parsedRecord.DepartmentManagerAssignmentId);
            Assert.AreEqual(string.Empty, parsedRecord.EmployeeCategory);
            Assert.AreEqual(string.Empty, parsedRecord.AssignmentCategory);
            Assert.AreEqual(string.Empty, parsedRecord.PrimaryAssignmentString);
            Assert.AreEqual(0, parsedRecord.NormalHours);
            Assert.AreEqual(string.Empty, parsedRecord.NormalHoursFrequency);
            Assert.AreEqual(0, parsedRecord.GradeContractHours);
            Assert.AreEqual(0, parsedRecord.Fte);
            Assert.AreEqual(string.Empty, parsedRecord.FlexibleWorkingPattern);
            Assert.IsNull(parsedRecord.EsrOrganisationId);
            Assert.IsNull(parsedRecord.EsrPositionId);
            Assert.AreEqual(string.Empty, parsedRecord.PositionName);
            Assert.AreEqual(string.Empty, parsedRecord.Grade);
            Assert.AreEqual(string.Empty, parsedRecord.GradeStep);
            Assert.IsNull(parsedRecord.StartDateInGrade);
            Assert.AreEqual(0, parsedRecord.AnnualSalaryValue);
            Assert.AreEqual(string.Empty, parsedRecord.JobName);
            Assert.AreEqual(string.Empty, parsedRecord.Group);
            Assert.AreEqual(string.Empty, parsedRecord.TAndAFlag);
            Assert.AreEqual(string.Empty, parsedRecord.NightWorkerOptOut);
            Assert.IsNull(parsedRecord.ProjectedHireDate);
            Assert.IsNull(parsedRecord.VacancyID);
            Assert.IsNull(parsedRecord.ContractEndDate);
            Assert.IsNull(parsedRecord.IncrementDate);
            Assert.AreEqual(string.Empty, parsedRecord.MaximumPartTimeFlag);
            Assert.AreEqual(string.Empty, parsedRecord.AfcFlag);
            Assert.IsNull(parsedRecord.EsrLastUpdate);
            Assert.IsNull(parsedRecord.LastWorkingDay);
            Assert.AreEqual(string.Empty, parsedRecord.eKSFSpinalPoint);
            Assert.AreEqual(string.Empty, parsedRecord.ManagerFlag);
            Assert.IsNull(parsedRecord.FinalAssignmentEndDate);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseAssignmentRecordValidUpdateRecord()
        {
            const string dataRow =
                "ASA~87654321~12345678~20121011~20121231~20121011~E~55555555~ACTIVE_APL~Active~E~431 Weekly~Week~289587~N~88888888~99999999~00000000~11111111~EmpCat~AsgCat~Yes~37.5~NHFreq~38.25~37.25~Term Time~386934~7777~3559596|Bank Nurse Band 5|N7B|Bank~NHS|XR02|Review Body Band 2~GStep~20120906~17500~Nursing and Midwifery Registered|Staff Nurse~431 z 0850 Bank Control|||~GTA|BNK,GT1,NET|GT1,GT2~No~20121201~12345~20150404~20150101~Yes~X~20130103 160730~20150403~10~Y~20150405~";

            EsrAssignmentRecord parsedRecord = EsrAssignmentRecord.ParseEsrAssignmentRecord(dataRow);
            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrAssignmentRecord()");
            Assert.AreEqual(Action.Update, parsedRecord.Action);
            Assert.AreEqual(87654321, parsedRecord.ESRPersonId);
            Assert.AreEqual(12345678, parsedRecord.AssignmentID);
            Assert.AreEqual(new DateTime(2012, 10, 11), parsedRecord.EffectiveStartDate);
            Assert.AreEqual(new DateTime(2012, 12, 31), parsedRecord.EffectiveEndDate);
            Assert.AreEqual(new DateTime(2012, 10, 11), parsedRecord.EarliestAssignmentStartDate);
            Assert.AreEqual("E", parsedRecord.AssignmentType);
            Assert.AreEqual("55555555", parsedRecord.AssignmentNumber);
            Assert.AreEqual("ACTIVE_APL", parsedRecord.SystemAssignmentStatus);
            Assert.AreEqual("Active", parsedRecord.AssignmentStatus);
            Assert.AreEqual("E", parsedRecord.EmployeeStatusFlag);
            Assert.AreEqual("431 Weekly", parsedRecord.PayrollName);
            Assert.AreEqual("Week", parsedRecord.PayrollPeriodType);
            Assert.AreEqual(289587, parsedRecord.EsrLocationId);
            Assert.AreEqual("N", parsedRecord.SupervisorFlag);
            Assert.AreEqual(88888888, parsedRecord.SupervisorPersonId);
            Assert.AreEqual(99999999, parsedRecord.SupervisorAssignmentId);
            Assert.AreEqual("00000000", parsedRecord.SupervisorAssignmentNumber);
            Assert.AreEqual(11111111, parsedRecord.DepartmentManagerAssignmentId);
            Assert.AreEqual("EmpCat", parsedRecord.EmployeeCategory);
            Assert.AreEqual("AsgCat", parsedRecord.AssignmentCategory);
            Assert.AreEqual("Yes", parsedRecord.PrimaryAssignmentString);
            Assert.AreEqual(Convert.ToDecimal(37.5), parsedRecord.NormalHours);
            Assert.AreEqual("NHFreq", parsedRecord.NormalHoursFrequency);
            Assert.AreEqual(Convert.ToDecimal(38.25), parsedRecord.GradeContractHours);
            Assert.AreEqual(Convert.ToDecimal(37.25), parsedRecord.Fte);
            Assert.AreEqual("Term Time", parsedRecord.FlexibleWorkingPattern);
            Assert.AreEqual(386934, parsedRecord.EsrOrganisationId);
            Assert.AreEqual(7777, parsedRecord.EsrPositionId);
            Assert.AreEqual("3559596|Bank Nurse Band 5|N7B|Bank", parsedRecord.PositionName);
            Assert.AreEqual("NHS|XR02|Review Body Band 2", parsedRecord.Grade);
            Assert.AreEqual("GStep", parsedRecord.GradeStep);
            Assert.AreEqual(new DateTime(2012, 9, 6), parsedRecord.StartDateInGrade);
            Assert.AreEqual(17500, parsedRecord.AnnualSalaryValue);
            Assert.AreEqual("Nursing and Midwifery Registered|Staff Nurse", parsedRecord.JobName);
            Assert.AreEqual("431 z 0850 Bank Control|||", parsedRecord.Group);
            Assert.AreEqual("GTA|BNK,GT1,NET|GT1,GT2", parsedRecord.TAndAFlag);
            Assert.AreEqual("No", parsedRecord.NightWorkerOptOut);
            Assert.AreEqual(new DateTime(2012, 12, 01), parsedRecord.ProjectedHireDate);
            Assert.AreEqual(12345, parsedRecord.VacancyID);
            Assert.AreEqual(new DateTime(2015, 4, 4), parsedRecord.ContractEndDate);
            Assert.AreEqual(new DateTime(2015, 1, 1), parsedRecord.IncrementDate);
            Assert.AreEqual("Yes", parsedRecord.MaximumPartTimeFlag);
            Assert.AreEqual("X", parsedRecord.AfcFlag);
            Assert.AreEqual(new DateTime(2013, 1, 3, 16, 07, 30), parsedRecord.EsrLastUpdate);
            Assert.AreEqual(new DateTime(2015, 4, 3), parsedRecord.LastWorkingDay);
            Assert.AreEqual("10", parsedRecord.eKSFSpinalPoint);
            Assert.AreEqual("Y", parsedRecord.ManagerFlag);
            Assert.AreEqual(new DateTime(2015, 4, 5), parsedRecord.FinalAssignmentEndDate);
        }
    }
}
