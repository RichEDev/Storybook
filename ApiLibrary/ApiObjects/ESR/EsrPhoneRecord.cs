namespace ApiLibrary.ApiObjects.ESR
{
    using System;
    using System.Globalization;
    using System.Runtime.Serialization;

    using ApiLibrary.DataObjects.ESR;

    using Action = ApiLibrary.DataObjects.Base.Action;

    /// <summary>
    /// The ESR phone record.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    public class EsrPhoneRecord : EsrPhone
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="EsrPhoneRecord"/> class.
        /// </summary>
        public EsrPhoneRecord()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="EsrPhoneRecord"/> class.
        /// </summary>
        /// <param name="esrPhoneId">
        /// The ESR phone id.
        /// </param>
        /// <param name="esrPersonId">
        /// The ESR person id.
        /// </param>
        /// <param name="phoneType">
        /// The phone type.
        /// </param>
        /// <param name="phoneNumber">
        /// The phone number.
        /// </param>
        /// <param name="effectiveStartDate">
        /// The effective start date.
        /// </param>
        /// <param name="effectiveEndDate">
        /// The effective end date.
        /// </param>
        /// <param name="esrLastUpdateDate">
        /// The ESR last update date.
        /// </param>
        public EsrPhoneRecord(long esrPhoneId, long esrPersonId, string phoneType, string phoneNumber, DateTime effectiveStartDate, DateTime? effectiveEndDate, DateTime esrLastUpdateDate)
        {
            this.ESRPhoneId = esrPhoneId;
            this.ESRPersonId = esrPersonId;
            this.PhoneType = phoneType;
            this.PhoneNumber = phoneNumber;
            this.EffectiveStartDate = effectiveStartDate;
            this.EffectiveEndDate = effectiveEndDate;
            this.ESRLastUpdate = esrLastUpdateDate;
        }

        #endregion

        /// <summary>
        /// Enumeration of ESR Column references in the delimited record line
        /// </summary>
        private enum PhoneRecordColumnsRef
        {
            RecordType = 0,
            PersonId,
            PhoneId,
            PhoneType,
            PhoneNumber,
            EffectiveStartDate,
            EffectiveEndDate,
            LastUpdateDate
        }

        /// <summary>
        /// Parses a delimited ESR Person record and transforms it into EsrPhoneRecord data object
        /// </summary>
        /// <param name="recordLine">Delimited row from outbound file</param>
        /// <returns></returns>
        public static EsrPhoneRecord ParseEsrPhoneRecord(string recordLine)
        {
            #region record variables

            long esrPhoneId = 0;
            long esrPersonId = 0;
            string phoneType = string.Empty;
            string phoneNumber = string.Empty;
            DateTime effectiveStartDate = new DateTime(1900, 1, 1);
            DateTime? effectiveEndDate = null;
            DateTime esrLastUpdateDate = new DateTime(1900, 1, 1);

            #endregion record variables

            string[] rec = recordLine.Split(EsrFile.RecordDelimiter);
            Action recordAction;

            switch (rec[(int)PhoneRecordColumnsRef.RecordType].ToUpper())
            {
                case EsrRecordTypes.EsrPhoneUpdateRecordType:
                    if (rec.Length != ((int)PhoneRecordColumnsRef.LastUpdateDate + 1)) // insufficient columns in row
                    {
                        return null;
                    }

                    recordAction = Action.Update;
                    long.TryParse(rec[(int)PhoneRecordColumnsRef.PersonId], out esrPersonId);
                    long.TryParse(rec[(int)PhoneRecordColumnsRef.PhoneId], out esrPhoneId);
                    phoneType = rec[(int)PhoneRecordColumnsRef.PhoneType];
                    phoneNumber = rec[(int)PhoneRecordColumnsRef.PhoneNumber];
                    DateTime tmpDate;
                    if (DateTime.TryParseExact(rec[(int)PhoneRecordColumnsRef.EffectiveStartDate], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        effectiveStartDate = tmpDate;
                    }

                    if (DateTime.TryParseExact(rec[(int)PhoneRecordColumnsRef.EffectiveEndDate], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        effectiveEndDate = tmpDate;
                    }
                    if (DateTime.TryParseExact(rec[(int)PhoneRecordColumnsRef.LastUpdateDate], "yyyyMMdd HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        esrLastUpdateDate = tmpDate;
                    }
                    break;
                case EsrRecordTypes.EsrPhoneDeleteRecordType:
                    if (rec.Length < EsrFile.RecordDeleteLength) // insufficient columns in row
                    {
                        return null;
                    }

                    recordAction = Action.Delete;
                    long.TryParse(rec[EsrFile.RecordDeletionColumnRef], out esrPhoneId);
                    break;
                default:
                    return null;
            }

            return new EsrPhoneRecord(esrPhoneId, esrPersonId, phoneType, phoneNumber, effectiveStartDate, effectiveEndDate, esrLastUpdateDate) { Action = recordAction };
        }
    }
}
