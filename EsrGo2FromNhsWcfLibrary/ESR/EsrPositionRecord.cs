namespace EsrGo2FromNhsWcfLibrary.ESR
{
    using System;
    using System.Globalization;
    using System.Runtime.Serialization;

    using Action = EsrGo2FromNhsWcfLibrary.Base.Action;

    /// <summary>
    /// The ESR position record.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    public class EsrPositionRecord : EsrPosition
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="EsrPositionRecord"/> class.
        /// </summary>
        /// <param name="esrPositionId">
        /// The ESR position id.
        /// </param>
        /// <param name="effectiveFromDate">
        /// The effective from date.
        /// </param>
        /// <param name="effectiveToDate">
        /// The effective to date.
        /// </param>
        /// <param name="positionNumber">
        /// The position number.
        /// </param>
        /// <param name="positionName">
        /// The position name.
        /// </param>
        /// <param name="budgetedFte">
        /// The budgeted FTE.
        /// </param>
        /// <param name="subjectiveCode">
        /// The subjective code.
        /// </param>
        /// <param name="jobStaffGroup">
        /// The job staff group.
        /// </param>
        /// <param name="jobRole">
        /// The job role.
        /// </param>
        /// <param name="occupationCode">
        /// The occupation code.
        /// </param>
        /// <param name="payscale">
        /// The pay scale.
        /// </param>
        /// <param name="gradeStep">
        /// The grade step.
        /// </param>
        /// <param name="isaRegulatedPost">
        /// The ISA regulated post.
        /// </param>
        /// <param name="esrOrganisationId">
        /// The ESR organisation id.
        /// </param>
        /// <param name="hiringStatus">
        /// The hiring status.
        /// </param>
        /// <param name="positionType">
        /// The position type.
        /// </param>
        /// <param name="eligibleForOhProcessing">
        /// The eligible for oh processing.
        /// </param>
        /// <param name="eppFlag">
        /// The EPP flag.
        /// </param>
        /// <param name="deaneryPostNumber">
        /// The deanery post number.
        /// </param>
        /// <param name="managingDeaneryBody">
        /// The managing deanery body.
        /// </param>
        /// <param name="workplaceOrgCode">
        /// The workplace org code.
        /// </param>
        /// <param name="lastUpdateDate">
        /// The last update date.
        /// </param>
        /// <param name="subjectiveCodeDescription">
        /// The subjective code description.
        /// </param>
        public EsrPositionRecord(
            long esrPositionId,
            DateTime effectiveFromDate,
            DateTime? effectiveToDate,
            long positionNumber,
            string positionName,
            decimal budgetedFte,
            string subjectiveCode,
            string jobStaffGroup,
            string jobRole,
            string occupationCode,
            string payscale,
            string gradeStep,
            string isaRegulatedPost,
            long? esrOrganisationId,
            string hiringStatus,
            string positionType,
            string eligibleForOhProcessing,
            string eppFlag,
            string deaneryPostNumber,
            string managingDeaneryBody,
            string workplaceOrgCode,
            DateTime lastUpdateDate,
            string subjectiveCodeDescription)
        {
            this.ESRPositionId = esrPositionId;
            this.EffectiveFromDate = effectiveFromDate;
            this.EffectiveToDate = effectiveToDate;
            this.PositionNumber = positionNumber;
            this.PositionName = positionName;
            this.BudgetedFTE = budgetedFte;
            this.SubjectiveCode = subjectiveCode;
            this.JobStaffGroup = jobStaffGroup;
            this.JobRole = jobRole;
            this.OccupationCode = occupationCode;
            this.Payscale = payscale;
            this.GradeStep = gradeStep;
            this.ISARegulatedPost = isaRegulatedPost;
            this.ESROrganisationId = esrOrganisationId;
            this.HiringStatus = hiringStatus;
            this.PositionType = positionType;
            this.OHProcessingEligible = eligibleForOhProcessing;
            this.EPPFlag = eppFlag;
            this.DeaneryPostNumber = deaneryPostNumber;
            this.ManagingDeaneryBody = managingDeaneryBody;
            this.WorkplaceOrgCode = workplaceOrgCode;
            this.ESRLastUpdateDate = lastUpdateDate;
            this.SubjectiveCodeDescription = subjectiveCodeDescription;
        }

        public EsrPositionRecord()
        {
            
        }

        /// <summary>
        /// Enumeration of ESR Column references in the delimited record line
        /// </summary>
        private enum PositionRecordColumnsRef
        {
            RecordType = 0,
            PositionId,
            EffectiveFromDate,
            EffectiveToDate,
            PositionNumber,
            PositionName,
            BudgetedFte,
            SubjectiveCode,
            JobStaffGroup,
            JobRole,
            OccupationCode,
            Payscale,
            GradeStep,
            IsaRegulatedPost,
            EsrOrganisationId,
            HiringStatus,
            PositionType,
            ElibileForOhProcessing,
            EppFlag,
            DeaneryPostNumber,
            ManagingDeaneryBody,
            WorkplaceOrgCode,
            LastUpdateDate,
            SubjectiveCodeDescription
        }

        /// <summary>
        /// Parses a delimited ESR Person record and transforms it into EsrPositionRecord data object
        /// </summary>
        /// <param name="recordLine">Delimited row from outbound file</param>
        /// <returns></returns>
        public static EsrPositionRecord ParseEsrPositionRecord(string recordLine)
        {
            #region record variables

            long esrPositionId;
            var effectiveFromDate = new DateTime(1900, 1, 1);
            DateTime? effectiveToDate = null;
            long positionNumber = 0;
            string positionName = string.Empty;
            decimal budgetedFte = 0;
            string subjectiveCode = string.Empty;
            string jobStaffGroup = string.Empty;
            string jobRole = string.Empty;
            string occupationCode = string.Empty;
            string payscale = string.Empty;
            string gradeStep = string.Empty;
            string isaRegulatedPost = string.Empty;
            long? esrOrganisationId = null;
            string hiringStatus = string.Empty;
            string positionType = string.Empty;
            string eligibleForOhProcessing = string.Empty;
            string eppFlag = string.Empty;
            string deaneryPostNumber = string.Empty;
            string managingDeaneryBody = string.Empty;
            string workplaceOrgCode = string.Empty;
            var lastUpdateDate = new DateTime(1900, 1, 1);
            string subjectiveCodeDescription = string.Empty;

            #endregion record variables

            string[] rec = recordLine.Split(EsrFile.RecordDelimiter);
            Action recordAction;

            switch (rec[(int)PositionRecordColumnsRef.RecordType].ToUpper())
            {
                case EsrRecordTypes.EsrPositionUpdateRecordType:
                    if (rec.Length != ((int)PositionRecordColumnsRef.SubjectiveCodeDescription + 1)) // insufficient columns in row
                    {
                        return null;
                    }

                    recordAction = Action.Update;
                    long.TryParse(rec[(int)PositionRecordColumnsRef.PositionId], out esrPositionId);
                    DateTime tmpDate;
                    if (EsrDateHelpers.TryParseExact(rec[(int)PositionRecordColumnsRef.EffectiveFromDate], "yyyyMMdd", out tmpDate))
                    {
                        effectiveFromDate = tmpDate;
                    }

                    if (EsrDateHelpers.TryParseExact(rec[(int)PositionRecordColumnsRef.EffectiveToDate], "yyyyMMdd", out tmpDate))
                    {
                        effectiveToDate = tmpDate;
                    }
                    long.TryParse(rec[(int)PositionRecordColumnsRef.PositionNumber], out positionNumber);
                    positionName = rec[(int)PositionRecordColumnsRef.PositionName];
                    decimal tmpFte;
                    if (decimal.TryParse(rec[(int)PositionRecordColumnsRef.BudgetedFte], out tmpFte))
                    {
                        budgetedFte = tmpFte;
                    }
                    subjectiveCode = rec[(int)PositionRecordColumnsRef.SubjectiveCode];
                    jobStaffGroup = rec[(int)PositionRecordColumnsRef.JobStaffGroup];
                    jobRole = rec[(int)PositionRecordColumnsRef.JobRole];
                    occupationCode = rec[(int)PositionRecordColumnsRef.OccupationCode];
                    payscale = rec[(int)PositionRecordColumnsRef.Payscale];
                    gradeStep = rec[(int)PositionRecordColumnsRef.GradeStep];
                    isaRegulatedPost = rec[(int)PositionRecordColumnsRef.IsaRegulatedPost];
                    long tmpId;
                    if (long.TryParse(rec[(int)PositionRecordColumnsRef.EsrOrganisationId], out tmpId))
                    {
                        esrOrganisationId = tmpId;
                    }
                    hiringStatus = rec[(int)PositionRecordColumnsRef.HiringStatus];
                    positionType = rec[(int)PositionRecordColumnsRef.PositionType];
                    eligibleForOhProcessing = rec[(int)PositionRecordColumnsRef.ElibileForOhProcessing];
                    eppFlag = rec[(int)PositionRecordColumnsRef.EppFlag];
                    deaneryPostNumber = rec[(int)PositionRecordColumnsRef.DeaneryPostNumber];
                    managingDeaneryBody = rec[(int)PositionRecordColumnsRef.ManagingDeaneryBody];
                    workplaceOrgCode = rec[(int)PositionRecordColumnsRef.WorkplaceOrgCode];
                    if (EsrDateHelpers.TryParseExact(rec[(int)PositionRecordColumnsRef.LastUpdateDate], "yyyyMMdd HHmmss", out tmpDate))
                    {
                        lastUpdateDate = tmpDate;
                    }
                    subjectiveCodeDescription = rec[(int)PositionRecordColumnsRef.SubjectiveCodeDescription];
                    break;
                case EsrRecordTypes.EsrPositionDeleteType:
                    if (rec.Length < EsrFile.RecordDeleteLength) // insufficient columns in row
                    {
                        return null;
                    }

                    recordAction = Action.Delete;
                    long.TryParse(rec[EsrFile.RecordDeletionColumnRef], out esrPositionId);
                    break;
                default:
                    return null;
            }

            return new EsrPositionRecord(
                esrPositionId,
                effectiveFromDate,
                effectiveToDate,
                positionNumber,
                positionName,
                budgetedFte,
                subjectiveCode,
                jobStaffGroup,
                jobRole,
                occupationCode,
                payscale,
                gradeStep,
                isaRegulatedPost,
                esrOrganisationId,
                hiringStatus,
                positionType,
                eligibleForOhProcessing,
                eppFlag,
                deaneryPostNumber,
                managingDeaneryBody,
                workplaceOrgCode,
                lastUpdateDate,
                subjectiveCodeDescription) { Action = recordAction };
        }
    }
}
