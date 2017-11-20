namespace ApiLibrary.ApiObjects.ESR
{
    using System;
    using System.Globalization;
    using System.Runtime.Serialization;

    using ApiLibrary.DataObjects.ESR;
    using Action = ApiLibrary.DataObjects.Base.Action;

    /// <summary>
    /// The ESR assignment costing record.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    public class EsrAssignmentCostingRecord : EsrAssignmentCostings
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="EsrAssignmentCostingRecord"/> class.
        /// </summary>
        public EsrAssignmentCostingRecord()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="EsrAssignmentCostingRecord"/> class.
        /// </summary>
        /// <param name="esrPersonId">
        /// The ESR person id.
        /// </param>
        /// <param name="esrAssignmentId">
        /// The ESR assignment id.
        /// </param>
        /// <param name="esrCostingAllocationId">
        /// The ESR costing allocation id.
        /// </param>
        /// <param name="effectiveStartDate">
        /// The effective start date.
        /// </param>
        /// <param name="effectiveEndDate">
        /// The effective end date.
        /// </param>
        /// <param name="entityCode">
        /// The entity code.
        /// </param>
        /// <param name="charitableIndicator">
        /// The charitable indicator.
        /// </param>
        /// <param name="costCentre">
        /// The cost centre.
        /// </param>
        /// <param name="subjective">
        /// The subjective.
        /// </param>
        /// <param name="analysis1">
        /// The analysis 1.
        /// </param>
        /// <param name="analysis2">
        /// The analysis 2.
        /// </param>
        /// <param name="elementNumber">
        /// The element number.
        /// </param>
        /// <param name="percentageSplit">
        /// The percentage split.
        /// </param>
        /// <param name="esrLastUpdateDate">
        /// The ESR last update date.
        /// </param>
        public EsrAssignmentCostingRecord(
            long esrPersonId,
            long esrAssignmentId,
            long esrCostingAllocationId,
            DateTime effectiveStartDate,
            DateTime? effectiveEndDate,
            string entityCode,
            string charitableIndicator,
            string costCentre,
            string subjective,
            string analysis1,
            string analysis2,
            int elementNumber,
            decimal percentageSplit,
            DateTime? esrLastUpdateDate)
        {
            this.ESRPersonId = esrPersonId;
            this.ESRAssignmentId = esrAssignmentId;
            this.ESRCostingAllocationId = esrCostingAllocationId;
            this.EffectiveStartDate = effectiveStartDate;
            this.EffectiveEndDate = effectiveEndDate;
            this.EntityCode = entityCode;
            this.CharitableIndicator = charitableIndicator;
            this.CostCentre = costCentre;
            this.Subjective = subjective;
            this.Analysis1 = analysis1;
            this.Analysis2 = analysis2;
            this.ElementNumber = elementNumber;
            this.PercentageSplit = percentageSplit;
            this.ESRLastUpdate = esrLastUpdateDate;
        }

                        /// <summary>
        /// Enumeration of ESR Column references in the delimited record line
        /// </summary>
        private enum AssignmentCostingRecordColumnsRef
        {
            RecordType = 0,
            PersonId,
            AssignmentId,
            CostingAllocationId,
            EffectiveStartDate,
            EffectiveEndDate,
            EntityCode,
            CharitableIndicator,
            CostCentre,
            Subjective,
            Analysis1,
            Analysis2,
            ElementNumber,
            SpareSegment,
            PercentageSplit,
            LastUpdateDate
        }

        /// <summary>
        /// Parses a delimited ESR Person record and transforms it into EsrAssignmentCostingRecord data object
        /// </summary>
        /// <param name="recordLine">Delimited row from outbound file</param>
        /// <returns></returns>
        public static EsrAssignmentCostingRecord ParseEsrAssignmentCostingRecord(string recordLine)
        {
            #region record variables

            long esrPersonId = 0;
            long esrAssignmentId = 0;
            long esrAssignmentCostingAllocationId = 0;
            DateTime effectStartDate = new DateTime(1900, 1, 1);
            DateTime? effectiveEndDate = null;
            string entityCode = string.Empty;
            string charitableIndicator = string.Empty;
            string costCentre = string.Empty;
            string subjective = string.Empty;
            string analysis1 = string.Empty;
            string analysis2 = string.Empty;
            int elementNumber = 0;
            string spareSegment = string.Empty;
            decimal percentageSplit = 0;
            DateTime? esrLastUpdateDate = null;

            #endregion record variables

            string[] rec = recordLine.Split(EsrFile.RecordDelimiter);
            Action recordAction;

            switch (rec[(int)AssignmentCostingRecordColumnsRef.RecordType].ToUpper())
            {
                case EsrRecordTypes.EsrAssignmentCostingUpdateRecordType:
                    if (rec.Length != ((int)AssignmentCostingRecordColumnsRef.LastUpdateDate + 1)) // insufficient columns in row
                    {
                        return null;
                    }

                    recordAction = Action.Update;
                    long.TryParse(rec[(int)AssignmentCostingRecordColumnsRef.PersonId], out esrPersonId);
                    long.TryParse(rec[(int)AssignmentCostingRecordColumnsRef.AssignmentId], out esrAssignmentId);
                    long.TryParse(rec[(int)AssignmentCostingRecordColumnsRef.CostingAllocationId], out esrAssignmentCostingAllocationId);
                    DateTime tmpDate;
                    if (DateTime.TryParseExact(rec[(int)AssignmentCostingRecordColumnsRef.EffectiveStartDate], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        effectStartDate = tmpDate;
                    }

                    if (DateTime.TryParseExact(rec[(int)AssignmentCostingRecordColumnsRef.EffectiveEndDate], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        effectiveEndDate = tmpDate;
                    }
                    entityCode = rec[(int)AssignmentCostingRecordColumnsRef.EntityCode];
                    charitableIndicator = rec[(int)AssignmentCostingRecordColumnsRef.CharitableIndicator];
                    costCentre = rec[(int)AssignmentCostingRecordColumnsRef.CostCentre];
                    subjective = rec[(int)AssignmentCostingRecordColumnsRef.Subjective];
                    analysis1 = rec[(int)AssignmentCostingRecordColumnsRef.Analysis1];
                    analysis2 = rec[(int)AssignmentCostingRecordColumnsRef.Analysis2];
                    int tmpVal;
                    if (int.TryParse(rec[(int)AssignmentCostingRecordColumnsRef.ElementNumber], out tmpVal))
                    {
                        elementNumber = tmpVal;
                    }
                    spareSegment = rec[(int)AssignmentCostingRecordColumnsRef.SpareSegment];
                    decimal tmpDec;
                    if (decimal.TryParse(rec[(int)AssignmentCostingRecordColumnsRef.PercentageSplit], out tmpDec))
                    {
                        percentageSplit = tmpDec;
                    }
                    if (DateTime.TryParseExact(rec[(int)AssignmentCostingRecordColumnsRef.LastUpdateDate], "yyyyMMdd HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        esrLastUpdateDate = tmpDate;
                    }
                    break;
                case EsrRecordTypes.EsrAssignmentCostingDeleteRecordType:
                    if (rec.Length < EsrFile.RecordDeleteLength) // insufficient columns in row
                    {
                        return null;
                    }

                    recordAction = Action.Delete;
                    long.TryParse(rec[EsrFile.RecordDeletionColumnRef], out esrAssignmentCostingAllocationId);
                    break;
                default:
                    return null;
            }

            return new EsrAssignmentCostingRecord(
                esrPersonId,
                esrAssignmentId,
                esrAssignmentCostingAllocationId,
                effectStartDate,
                effectiveEndDate,
                entityCode,
                charitableIndicator,
                costCentre,
                subjective,
                analysis1,
                analysis2,
                elementNumber,
                percentageSplit,
                esrLastUpdateDate) { Action = recordAction };
        }
    }
}
