namespace EsrGo2FromNhs.ESR
{
    using System;
    using System.Globalization;

    using Action = EsrGo2FromNhs.Base.Action;

    /// <summary>
    /// The ESR organisation record.
    /// </summary>
    public class EsrOrganisationRecord : EsrOrganisation
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="EsrOrganisationRecord"/> class.
        /// </summary>
        public EsrOrganisationRecord()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="EsrOrganisationRecord"/> class.
        /// </summary>
        /// <param name="esrOrganisationId">
        /// The ESR organisation id.
        /// </param>
        /// <param name="organisationName">
        /// The organisation name.
        /// </param>
        /// <param name="organisationType">
        /// The organisation type.
        /// </param>
        /// <param name="effectiveFromDate">
        /// The effective from date.
        /// </param>
        /// <param name="effectiveToDate">
        /// The effective to date.
        /// </param>
        /// <param name="hierarchyVersionId">
        /// The hierarchy version id.
        /// </param>
        /// <param name="hierarchyVersionDateFrom">
        /// The hierarchy version date from.
        /// </param>
        /// <param name="hierarchyVersionDateTo">
        /// The hierarchy version date to.
        /// </param>
        /// <param name="defaultCostCentre">
        /// The default cost centre.
        /// </param>
        /// <param name="parentOrganisationId">
        /// The parent organisation id.
        /// </param>
        /// <param name="nacsCode">
        /// The NACS code.
        /// </param>
        /// <param name="esrLocationId">
        /// The ESR location id.
        /// </param>
        /// <param name="esrLastUpdateDate">
        /// The ESR last update date.
        /// </param>
        /// <param name="costCentreDescription">
        /// The cost centre description.
        /// </param>
        public EsrOrganisationRecord(
            long esrOrganisationId,
            string organisationName,
            string organisationType,
            DateTime effectiveFromDate,
            DateTime? effectiveToDate,
            long hierarchyVersionId,
            DateTime? hierarchyVersionDateFrom,
            DateTime? hierarchyVersionDateTo,
            string defaultCostCentre,
            long? parentOrganisationId,
            string nacsCode,
            long? esrLocationId,
            DateTime? esrLastUpdateDate,
            string costCentreDescription)
        {
            this.ESROrganisationId = esrOrganisationId;
            this.OrganisationName = organisationName;
            this.OrganisationType = organisationType;
            this.EffectiveFrom = effectiveFromDate;
            this.EffectiveTo = effectiveToDate;
            this.HierarchyVersionId = hierarchyVersionId;
            this.HierarchyVersionFrom = hierarchyVersionDateFrom;
            this.HierarchyVersionTo = hierarchyVersionDateTo;
            this.DefaultCostCentre = defaultCostCentre;
            this.ParentOrganisationId = parentOrganisationId;
            this.NACSCode = nacsCode;
            this.ESRLocationId = esrLocationId;
            this.ESRLastUpdateDate = esrLastUpdateDate;
            this.CostCentreDescription = costCentreDescription;
        }

        /// <summary>
        /// Enumeration of ESR Column references in the delimited record line
        /// </summary>
        private enum OrganisationRecordColumnsRef
        {
            RecordType = 0,
            OrganisationId,
            OrganisationName,
            OrganisationType,
            EffectiveFromDate,
            EffectiveToDate,
            HierarchyVersionId,
            HierarchyVersionDateFrom,
            HierarchyVersionDateTo,
            DefaultCostCentre,
            ParentOrgId,
            NacsCode,
            LocationId,
            LastUpdateDate,
            CostCentreDescription
        }

        /// <summary>
        /// Parses a delimited ESR Person record and transforms it into EsrOrganisationRecord data object
        /// </summary>
        /// <param name="recordLine">Delimited row from outbound file</param>
        /// <returns></returns>
        public static EsrOrganisationRecord ParseEsrOrganisationRecord(string recordLine)
        {
            #region record variables

            long esrOrganisationId = 0;
            string organisationName = string.Empty;
            string organisationType = string.Empty;
            DateTime effectiveFromDate = new DateTime(1900, 1, 1);
            DateTime? effectiveToDate = null;
            long hierarchyVersionId = 0;
            DateTime? hierarchyVersionDateFrom = null;
            DateTime? hierarchyVersionDateTo = null;
            string defaultCostCentre = string.Empty;
            long? parentOrganisationId = null;
            string nacsCode = string.Empty;
            long? esrLocationId = null;
            DateTime? esrLastUpdateDate = null;
            string costCentreDescription = string.Empty;

            #endregion record variable

            string[] rec = recordLine.Split(global::EsrGo2FromNhs.ESR.EsrFile.RecordDelimiter);
            if (rec.Length != ((int)OrganisationRecordColumnsRef.CostCentreDescription + 1) && rec.Length != global::EsrGo2FromNhs.ESR.EsrFile.RecordDeleteLength)
            {
                // insufficient columns in row
                return null;
            }

            Action recordAction;

            switch (rec[(int)OrganisationRecordColumnsRef.RecordType].ToUpper())
            {
                case EsrRecordTypes.EsrOrganisationUpdateRecordType:
                    recordAction = Action.Update;
                    long.TryParse(rec[(int)OrganisationRecordColumnsRef.OrganisationId], out esrOrganisationId);
                    organisationName = rec[(int)OrganisationRecordColumnsRef.OrganisationName];
                    organisationType = rec[(int)OrganisationRecordColumnsRef.OrganisationType];
                    DateTime tmpDate;
                    if (DateTime.TryParseExact(rec[(int)OrganisationRecordColumnsRef.EffectiveFromDate], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        effectiveFromDate = tmpDate;
                    }
                    if (DateTime.TryParseExact(rec[(int)OrganisationRecordColumnsRef.EffectiveToDate], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        effectiveToDate = tmpDate;
                    }
                    long.TryParse(rec[(int)OrganisationRecordColumnsRef.HierarchyVersionId], out hierarchyVersionId);
                    if (DateTime.TryParseExact(rec[(int)OrganisationRecordColumnsRef.HierarchyVersionDateFrom], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        hierarchyVersionDateFrom = tmpDate;
                    }
                    if (DateTime.TryParseExact(rec[(int)OrganisationRecordColumnsRef.HierarchyVersionDateTo], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        hierarchyVersionDateTo = tmpDate;
                    }
                    defaultCostCentre = rec[(int)OrganisationRecordColumnsRef.DefaultCostCentre];
                    long tmpParentOrgId = 0;
                    if (long.TryParse(rec[(int)OrganisationRecordColumnsRef.ParentOrgId], out tmpParentOrgId))
                    {
                        parentOrganisationId = tmpParentOrgId;
                    }
                    nacsCode = rec[(int)OrganisationRecordColumnsRef.NacsCode];
                    long tmpLocId;
                    if (long.TryParse(rec[(int)OrganisationRecordColumnsRef.LocationId], out tmpLocId))
                    {
                        esrLocationId = tmpLocId;
                    }
                    if (DateTime.TryParseExact(rec[(int)OrganisationRecordColumnsRef.LastUpdateDate], "yyyyMMdd HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        esrLastUpdateDate = tmpDate;
                    }
                    costCentreDescription = rec[(int)OrganisationRecordColumnsRef.CostCentreDescription];
                    break;
                    
                case EsrRecordTypes.EsrOrganisationDeleteRecordType:
                    recordAction = Action.Delete;
                    long.TryParse(rec[(int)OrganisationRecordColumnsRef.OrganisationId], out esrOrganisationId);
                    break;
                default:
                    return null;
            }

            return new EsrOrganisationRecord(
                esrOrganisationId,
                organisationName,
                organisationType,
                effectiveFromDate,
                effectiveToDate,
                hierarchyVersionId,
                hierarchyVersionDateFrom,
                hierarchyVersionDateTo,
                defaultCostCentre,
                parentOrganisationId,
                nacsCode,
                esrLocationId,
                esrLastUpdateDate,
                costCentreDescription) { Action = recordAction, SafeParentOrganisationId = parentOrganisationId };
        }

        /// <summary>
        /// Re-instantiates the record as a new object with its parent organisation id restored and noting the rowindex
        /// </summary>
        /// <param name="record"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public static EsrOrganisationRecord ResetOrganisationRecord(EsrOrganisationRecord record, int rowIndex)
        {
            return new EsrOrganisationRecord(
                record.ESROrganisationId,
                record.OrganisationName,
                record.OrganisationType,
                record.EffectiveFrom,
                record.EffectiveTo,
                record.HierarchyVersionId,
                record.HierarchyVersionFrom,
                record.HierarchyVersionTo,
                record.DefaultCostCentre,
                record.SafeParentOrganisationId,
                record.NACSCode,
                record.ESRLocationId,
                record.ESRLastUpdateDate,
                record.CostCentreDescription) { Action = record.Action, SafeParentOrganisationId = record.SafeParentOrganisationId, RecordPositionIndex = rowIndex };
        }
    }
}
