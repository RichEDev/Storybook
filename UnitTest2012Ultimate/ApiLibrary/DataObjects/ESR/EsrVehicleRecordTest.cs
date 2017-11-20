using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest2012Ultimate.ApiLibrary.DataObjects.ESR
{
    using global::EsrGo2FromNhsWcfLibrary.ESR;
    using Action = global::EsrGo2FromNhsWcfLibrary.Base.Action;

    /// <summary>
    /// Summary description for EsrVehicleRecordTest
    /// </summary>
    [TestClass]
    public class EsrVehicleRecordTest
    {
        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseVehicleRecordInvalidNumberOfColumns()
        {
            const string dataRow = "VHA~3570986~3023305~865397~20090203~~P361PPU~VAUXHALL~VECTRA~P~~20090318 151127~2000~NHS_MILEAGE_CASUAL~~P";

            EsrVehicleRecord parsedRecord = EsrVehicleRecord.ParseEsrVehicleRecord(dataRow);

            Assert.IsNull(parsedRecord, "Expected Null record not returned from ParseEsrVehicleRecord()");
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseVehicleRecordValidUpdateRecord()
        {
            const string dataRow = "VHA~3570986~3023305~865397~20090203~20130203~X123ABC~VAUXHALL~VECTRA~P~20010105~20090318 151127~2000~NHS_MILEAGE_CASUAL~P";

            EsrVehicleRecord parsedRecord = EsrVehicleRecord.ParseEsrVehicleRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrLocationRecord()");
            Assert.AreEqual(Action.Update, parsedRecord.Action);
            Assert.AreEqual(3570986, parsedRecord.ESRPersonId);
            Assert.AreEqual(3023305, parsedRecord.ESRAssignmentId);
            Assert.AreEqual(865397, parsedRecord.ESRVehicleAllocationId);
            Assert.AreEqual(new DateTime(2009, 02, 03), parsedRecord.EffectiveStartDate);
            Assert.AreEqual(new DateTime(2013, 02, 03), parsedRecord.EffectiveEndDate);
            Assert.AreEqual("X123ABC", parsedRecord.RegistrationNumber);
            Assert.AreEqual("VAUXHALL", parsedRecord.Make);
            Assert.AreEqual("VECTRA", parsedRecord.Model);
            Assert.AreEqual("P", parsedRecord.Ownership);
            Assert.AreEqual(new DateTime(2001, 1, 5), parsedRecord.InitialRegistrationDate);
            Assert.AreEqual(new DateTime(2009, 3, 18, 15, 11, 27), parsedRecord.ESRLastUpdate);
            Assert.AreEqual(2000, parsedRecord.EngineCC);
            Assert.AreEqual("NHS_MILEAGE_CASUAL", parsedRecord.UserRatesTable);
            Assert.AreEqual("P", parsedRecord.FuelType);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseVehicleRecordUpdateRecordWithInvalidDates()
        {
            const string dataRow = "VHA~3570986~3023305~865397~20090233~20131303~X123ABC~VAUXHALL~VECTRA~P~20010145~20090399 151127~2000~NHS_MILEAGE_CASUAL~P";

            EsrVehicleRecord parsedRecord = EsrVehicleRecord.ParseEsrVehicleRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrLocationRecord()");
            Assert.AreEqual(Action.Update, parsedRecord.Action);
            Assert.AreEqual(3570986, parsedRecord.ESRPersonId);
            Assert.AreEqual(3023305, parsedRecord.ESRAssignmentId);
            Assert.AreEqual(865397, parsedRecord.ESRVehicleAllocationId);
            Assert.AreEqual(new DateTime(1900, 1, 1), parsedRecord.EffectiveStartDate);
            Assert.IsNull(parsedRecord.EffectiveEndDate);
            Assert.AreEqual("X123ABC", parsedRecord.RegistrationNumber);
            Assert.AreEqual("VAUXHALL", parsedRecord.Make);
            Assert.AreEqual("VECTRA", parsedRecord.Model);
            Assert.AreEqual("P", parsedRecord.Ownership);
            Assert.IsNull(parsedRecord.InitialRegistrationDate);
            Assert.AreEqual(new DateTime(1900, 1, 1, 0, 0, 0), parsedRecord.ESRLastUpdate);
            Assert.AreEqual(2000, parsedRecord.EngineCC);
            Assert.AreEqual("NHS_MILEAGE_CASUAL", parsedRecord.UserRatesTable);
            Assert.AreEqual("P", parsedRecord.FuelType);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseVehicleRecordValidDeleteRecord()
        {
            const string dataRow = "VHD~123456";

            EsrVehicleRecord parsedRecord = EsrVehicleRecord.ParseEsrVehicleRecord(dataRow);

            Assert.IsNotNull(parsedRecord, "Expected valid record not returned from ParseEsrVehicleRecord()");
            Assert.AreEqual(Action.Delete, parsedRecord.Action);
            Assert.AreEqual(0, parsedRecord.ESRPersonId);
            Assert.AreEqual(0, parsedRecord.ESRAssignmentId);
            Assert.AreEqual(123456, parsedRecord.ESRVehicleAllocationId);
            Assert.AreEqual(new DateTime(1900, 1, 1), parsedRecord.EffectiveStartDate);
            Assert.IsNull(parsedRecord.EffectiveEndDate);
            Assert.AreEqual(string.Empty, parsedRecord.RegistrationNumber);
            Assert.AreEqual(string.Empty, parsedRecord.Make);
            Assert.AreEqual(string.Empty, parsedRecord.Model);
            Assert.AreEqual(string.Empty, parsedRecord.Ownership);
            Assert.IsNull(parsedRecord.InitialRegistrationDate);
            Assert.AreEqual(new DateTime(1900, 1, 1), parsedRecord.ESRLastUpdate);
            Assert.AreEqual(0, parsedRecord.EngineCC);
            Assert.AreEqual(string.Empty, parsedRecord.UserRatesTable);
            Assert.AreEqual(string.Empty, parsedRecord.FuelType);
        }

        [TestMethod]
        [TestCategory("ApiLibrary")]
        [TestCategory("EsrRecords")]
        public void ParseVehicleRecordInvalidRecordType()
        {
            const string dataRow = "VHX~3570986~3023305~865397~20090203~20130203~X123ABC~VAUXHALL~VECTRA~P~20010105~20090318 151127~2000~NHS_MILEAGE_CASUAL~P";

            EsrVehicleRecord parsedRecord = EsrVehicleRecord.ParseEsrVehicleRecord(dataRow);

            Assert.IsNull(parsedRecord, "Expected Null record not returned from ParseEsrVehicleRecord()");
        }
    }
}
