namespace EsrGo2FromNhs.ESR
{
    using System;
    using System.Globalization;
    using System.Runtime.Serialization;

    using Action = EsrGo2FromNhs.Base.Action;

    /// <summary>
    /// The ESR address record data class.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    public class EsrAddressRecord : EsrAddress
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="EsrAddressRecord"/> class.
        /// </summary>
        public EsrAddressRecord()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="EsrAddressRecord"/> class.
        /// </summary>
        /// <param name="esrPersonId">
        /// The ESR person id.
        /// </param>
        /// <param name="esrAddressId">
        /// The ESR address id.
        /// </param>
        /// <param name="addressType">
        /// The address type.
        /// </param>
        /// <param name="addressStyle">
        /// The address style.
        /// </param>
        /// <param name="primaryFlag">
        /// The primary flag.
        /// </param>
        /// <param name="addressLine1">
        /// The address line 1.
        /// </param>
        /// <param name="addressLine2">
        /// The address line 2.
        /// </param>
        /// <param name="addressLine3">
        /// The address line 3.
        /// </param>
        /// <param name="addressTown">
        /// The address town.
        /// </param>
        /// <param name="addressCounty">
        /// The address county.
        /// </param>
        /// <param name="addressPostcode">
        /// The address postcode.
        /// </param>
        /// <param name="addressCountry">
        /// The address country.
        /// </param>
        /// <param name="effectiveStartDate">
        /// The effective start date.
        /// </param>
        /// <param name="effectiveEndDate">
        /// The effective end date.
        /// </param>
        /// <param name="lastUpdateDate">
        /// The last update date.
        /// </param>
        public EsrAddressRecord(long esrPersonId, long esrAddressId, string addressType, string addressStyle, string primaryFlag, string addressLine1, string addressLine2, string addressLine3, string addressTown, string addressCounty, string addressPostcode, string addressCountry, DateTime effectiveStartDate, DateTime? effectiveEndDate, DateTime? lastUpdateDate)
        {
            this.ESRPersonId = esrPersonId;
            this.ESRAddressId = esrAddressId;
            this.AddressType = addressType;
            this.AddressStyle = addressStyle;
            this.PrimaryFlag = primaryFlag;
            this.AddressLine1 = addressLine1;
            this.AddressLine2 = addressLine2;
            this.AddressLine3 = addressLine3;
            this.AddressTown = addressTown;
            this.AddressCounty = addressCounty;
            this.AddressPostcode = addressPostcode;
            this.AddressCountry = addressCountry;
            this.EffectiveStartDate = effectiveStartDate;
            this.EffectiveEndDate = effectiveEndDate;
            this.ESRLastUpdate = lastUpdateDate;
        }

        /// <summary>
        /// Enumeration of ESR Column references in the delimited record line
        /// </summary>
        private enum AddressRecordColumnsRef
        {
            RecordType = 0,
            PersonId,
            AddressId,
            AddressType,
            AddressStyle,
            PrimaryFlag,
            AddressLine1,
            AddressLine2,
            AddressLine3,
            AddressTown,
            AddressCounty,
            AddressPostcode,
            AddressCountry,
            EffectiveStartDate,
            EffectiveEndDate,
            LastUpdateDate
        }

        /// <summary>
        /// Parses a delimited ESR Person record and transforms it into EsrPersonRecord data object
        /// </summary>
        /// <param name="recordLine">Delimited row from outbound file</param>
        /// <returns></returns>
        public static EsrAddressRecord ParseEsrAddressRecord(string recordLine)
        {
            #region record variables

            long esrPersonId = 0;
            long esrAddressId = 0;
            string addressType = string.Empty;
            string addressStyle = string.Empty;
            string primaryFlag = string.Empty;
            string addressLine1 = string.Empty;
            string addressLine2 = string.Empty; 
            string addressLine3 = string.Empty;
            string addressTown = string.Empty;
            string addressCounty = string.Empty;
            string addressPostcode = string.Empty;
            string addressCountry = string.Empty;
            DateTime effectiveStartDate = new DateTime(1900, 1, 1);
            DateTime? effectiveEndDate = null;
            DateTime? lastUpdateDate = null;

            #endregion record variables

            string[] rec = recordLine.Split(EsrFile.RecordDelimiter);
            if (rec.Length != ((int)AddressRecordColumnsRef.LastUpdateDate + 1) && rec.Length != 2) // 2 is delete record length (fixed)
            {
                // insufficient columns in row
                return null;
            }

            Action recordAction;

            switch (rec[(int)AddressRecordColumnsRef.RecordType].ToUpper())
            {
                case EsrRecordTypes.EsrAddressUpdateRecordType:
                    recordAction = Action.Update;
                    long.TryParse(rec[(int)AddressRecordColumnsRef.PersonId], out esrPersonId);
                    long.TryParse(rec[(int)AddressRecordColumnsRef.AddressId], out esrAddressId);
                    addressType = rec[(int)AddressRecordColumnsRef.AddressType];
                    addressStyle = rec[(int)AddressRecordColumnsRef.AddressStyle];
                    primaryFlag = rec[(int)AddressRecordColumnsRef.PrimaryFlag];
                    addressLine1 = rec[(int)AddressRecordColumnsRef.AddressLine1];
                    addressLine2 = rec[(int)AddressRecordColumnsRef.AddressLine2];
                    addressLine3 = rec[(int)AddressRecordColumnsRef.AddressLine3];
                    addressTown = rec[(int)AddressRecordColumnsRef.AddressTown];
                    addressCounty = rec[(int)AddressRecordColumnsRef.AddressCounty];
                    addressPostcode = rec[(int)AddressRecordColumnsRef.AddressPostcode];
                    addressCountry = rec[(int)AddressRecordColumnsRef.AddressCountry];
                    DateTime tmpDate;
                    if(DateTime.TryParseExact(rec[(int)AddressRecordColumnsRef.EffectiveStartDate], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None,  out tmpDate))
                    {
                        effectiveStartDate = tmpDate;
                    }

                    if (DateTime.TryParseExact(rec[(int)AddressRecordColumnsRef.EffectiveEndDate], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        effectiveEndDate = tmpDate;
                    }
                    if (DateTime.TryParseExact(rec[(int)AddressRecordColumnsRef.LastUpdateDate], "yyyyMMdd HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDate))
                    {
                        lastUpdateDate = tmpDate;
                    }
                    break;
                case EsrRecordTypes.EsrAddressDeleteRecordType:
                    recordAction = Action.Delete;
                    long.TryParse(rec[EsrFile.RecordDeletionColumnRef], out esrAddressId);
                    break;
                default:
                    return null;
            }

            return new EsrAddressRecord(
                esrPersonId,
                esrAddressId,
                addressType,
                addressStyle,
                primaryFlag,
                addressLine1,
                addressLine2,
                addressLine3,
                addressTown,
                addressCounty,
                addressPostcode,
                addressCountry,
                effectiveStartDate,
                effectiveEndDate,
                lastUpdateDate) { Action = recordAction };
        }
    }
}
