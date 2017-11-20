namespace EsrGo2FromNhsWcfLibrary.ESR
{
    using System;
    using System.Globalization;
    using System.Runtime.Serialization;

    using Action = EsrGo2FromNhsWcfLibrary.Base.Action;

    /// <summary>
    /// The ESR vehicle record.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    public class EsrVehicleRecord : EsrVehicle
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="EsrVehicleRecord"/> class.
        /// </summary>
        public EsrVehicleRecord()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="EsrVehicleRecord"/> class.
        /// </summary>
        /// <param name="esrPersonId">
        /// The ESR person id.
        /// </param>
        /// <param name="esrAssignmentId">
        /// The ESR assignment id.
        /// </param>
        /// <param name="esrVehicleAllocationId">
        /// The ESR vehicle allocation id.
        /// </param>
        /// <param name="effectStartDate">
        /// The effect start date.
        /// </param>
        /// <param name="effectiveEndDate">
        /// The effective end date.
        /// </param>
        /// <param name="registrationNumber">
        /// The registration number.
        /// </param>
        /// <param name="make">
        /// The make.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="ownership">
        /// The ownership.
        /// </param>
        /// <param name="initialRegistrationDate">
        /// The initial registration date.
        /// </param>
        /// <param name="esrLastUpdate">
        /// The ESR last update.
        /// </param>
        /// <param name="enginecc">
        /// The engine cc.
        /// </param>
        /// <param name="userRatesTable">
        /// The user rates table.
        /// </param>
        /// <param name="fuelType">
        /// The fuel type.
        /// </param>
        public EsrVehicleRecord(
            long esrPersonId,
            long esrAssignmentId,
            long esrVehicleAllocationId,
            DateTime effectStartDate,
            DateTime? effectiveEndDate,
            string registrationNumber,
            string make,
            string model,
            string ownership,
            DateTime? initialRegistrationDate,
            DateTime esrLastUpdate,
            decimal enginecc,
            string userRatesTable,
            string fuelType)
        {
            this.ESRPersonId = esrPersonId;
            this.ESRAssignmentId = esrAssignmentId;
            this.ESRVehicleAllocationId = esrVehicleAllocationId;
            this.EffectiveStartDate = effectStartDate;
            this.EffectiveEndDate = effectiveEndDate;
            this.RegistrationNumber = registrationNumber;
            this.Make = make;
            this.Model = model;
            this.Ownership = ownership;
            this.InitialRegistrationDate = initialRegistrationDate;
            this.ESRLastUpdate = esrLastUpdate;

            // if passed in litres, convert to cc (i.e. 2.0 to 2000)
            this.EngineCC = (int)(enginecc < 10 ? enginecc * 1000 : enginecc);
            this.UserRatesTable = userRatesTable;
            this.FuelType = fuelType;
        }
    
        /// <summary>
        /// Enumeration of ESR Column references in the delimited record line
        /// </summary>
        private enum VehicleRecordColumnsRef
        {
            RecordType = 0,
            PersonId,
            AssignmentId,
            VehicleAllocationId,
            EffectiveStartDate,
            EffectiveEndDate,
            RegistrationNumber,
            Make,
            Model,
            Ownership,
            InitialRegistrationDate,
            LastUpdateDate,
            EngineCc,
            UserRatesTable,
            FuelType
        }

        /// <summary>
        /// Parses a delimited ESR Person record and transforms it into EsrPhoneRecord data object
        /// </summary>
        /// <param name="recordLine">Delimited row from outbound file</param>
        /// <returns></returns>
        public static EsrVehicleRecord ParseEsrVehicleRecord(string recordLine)
        {
            #region record variables

            long esrPersonId = 0;
            long esrAssignmentId = 0;
            long esrVehicleAllocationId;
            var effectStartDate = new DateTime(1900, 1, 1);
            DateTime? effectiveEndDate = null;
            string registrationNumber = string.Empty;
            string make = string.Empty;
            string model = string.Empty;
            string ownership = string.Empty;
            DateTime? initialRegistrationDate = null;
            var esrLastUpdate = new DateTime(1900, 1, 1, 0, 0, 0);
            decimal enginecc = 0;
            string userRatesTable = string.Empty;
            string fuelType = string.Empty;

            #endregion record variables

            string[] rec = recordLine.Split(EsrFile.RecordDelimiter);
            Action recordAction;

            switch (rec[(int)VehicleRecordColumnsRef.RecordType].ToUpper())
            {
                case EsrRecordTypes.EsrVehicleUpdateRecordType:
                    if (rec.Length != ((int)VehicleRecordColumnsRef.FuelType + 1)) // insufficient columns in row
                    {
                        return null;
                    }

                    recordAction = Action.Update;
                    long.TryParse(rec[(int)VehicleRecordColumnsRef.PersonId], out esrPersonId);
                    long.TryParse(rec[(int)VehicleRecordColumnsRef.AssignmentId], out esrAssignmentId);
                    long.TryParse(rec[(int)VehicleRecordColumnsRef.VehicleAllocationId], out esrVehicleAllocationId);
                    DateTime tmpDate;
                    if (EsrDateHelpers.TryParseExact(rec[(int)VehicleRecordColumnsRef.EffectiveStartDate], "yyyyMMdd", out tmpDate))
                    {
                        effectStartDate = tmpDate;
                    }

                    if (EsrDateHelpers.TryParseExact(rec[(int)VehicleRecordColumnsRef.EffectiveEndDate], "yyyyMMdd", out tmpDate))
                    {
                        effectiveEndDate = tmpDate;
                    }
                    registrationNumber = rec[(int)VehicleRecordColumnsRef.RegistrationNumber];
                    make = rec[(int)VehicleRecordColumnsRef.Make];
                    model = rec[(int)VehicleRecordColumnsRef.Model];
                    ownership = rec[(int)VehicleRecordColumnsRef.Ownership];
                    if (EsrDateHelpers.TryParseExact(rec[(int)VehicleRecordColumnsRef.InitialRegistrationDate], "yyyyMMdd", out tmpDate))
                    {
                        initialRegistrationDate = tmpDate;
                    }
                    if (EsrDateHelpers.TryParseExact(rec[(int)VehicleRecordColumnsRef.LastUpdateDate], "yyyyMMdd HHmmss", out tmpDate))
                    {
                        esrLastUpdate = tmpDate;
                    }
                    decimal tmpDec;
                    if (decimal.TryParse(rec[(int)VehicleRecordColumnsRef.EngineCc], out tmpDec))
                    {
                        enginecc = tmpDec;
                    }
                    userRatesTable = rec[(int)VehicleRecordColumnsRef.UserRatesTable];
                    fuelType = rec[(int)VehicleRecordColumnsRef.FuelType];
                    break;
                case EsrRecordTypes.EsrVehicleDeleteRecordType:
                    if (rec.Length < EsrFile.RecordDeleteLength) // insufficient columns in row
                    {
                        return null;
                    }

                    recordAction= Action.Delete;
                    long.TryParse(rec[EsrFile.RecordDeletionColumnRef], out esrVehicleAllocationId);
                    break;
                default:
                    return null;
            }

            return new EsrVehicleRecord(
                esrPersonId,
                esrAssignmentId,
                esrVehicleAllocationId,
                effectStartDate,
                effectiveEndDate,
                registrationNumber,
                make,
                model,
                ownership,
                initialRegistrationDate,
                esrLastUpdate,
                enginecc,
                userRatesTable,
                fuelType) { Action = recordAction };
        }
    }
}
