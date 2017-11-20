namespace EsrGo2FromNhsWcfLibrary.ESR
{
    using System;
    using System.Globalization;
    using System.Runtime.Serialization;

    using Action = EsrGo2FromNhsWcfLibrary.Base.Action;

    /// <summary>
    /// The ESR location record.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    public class EsrLocationRecord : EsrLocation
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="EsrLocationRecord"/> class.
        /// </summary>
        public EsrLocationRecord()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="EsrLocationRecord"/> class.
        /// </summary>
        /// <param name="esrLocationId">
        /// The ESR location id.
        /// </param>
        /// <param name="locationCode">
        /// The location code.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="inactiveDate">
        /// The inactive date.
        /// </param>
        /// <param name="assignmentAddressLine1">
        /// The assignment address line 1.
        /// </param>
        /// <param name="assignmentAddressLine2">
        /// The assignment address line 2.
        /// </param>
        /// <param name="assignmentAddressLine3">
        /// The assignment address line 3.
        /// </param>
        /// <param name="assignmentAddressTown">
        /// The assignment address town.
        /// </param>
        /// <param name="assignmentAddressCounty">
        /// The assignment address county.
        /// </param>
        /// <param name="assignmentAddressPostcode">
        /// The assignment address postcode.
        /// </param>
        /// <param name="assignmentAddressCountry">
        /// The assignment address country.
        /// </param>
        /// <param name="telephone">
        /// The telephone.
        /// </param>
        /// <param name="fax">
        /// The fax.
        /// </param>
        /// <param name="payslipDeliveryPoint">
        /// The payslip delivery point.
        /// </param>
        /// <param name="siteCode">
        /// The site code.
        /// </param>
        /// <param name="welshLocationTranslation">
        /// The welsh location translation
        /// </param>
        /// <param name="welshAddressLine1">
        /// The welsh address line 1.
        /// </param>
        /// <param name="welshAddressLine2">
        /// The welsh address line 2.
        /// </param>
        /// <param name="welshAddressLine3">
        /// The welsh address line 3.
        /// </param>
        /// <param name="welshAddressTown">
        /// The welsh address town.
        /// </param>
        /// <param name="lastUpdateDate">
        /// The last update date.
        /// </param>
        public EsrLocationRecord(
            long esrLocationId,
            string locationCode,
            string description,
            DateTime? inactiveDate,
            string assignmentAddressLine1,
            string assignmentAddressLine2,
            string assignmentAddressLine3,
            string assignmentAddressTown,
            string assignmentAddressCounty,
            string assignmentAddressPostcode,
            string assignmentAddressCountry,
            string telephone,
            string fax,
            string payslipDeliveryPoint,
            string siteCode,
            string welshLocationTranslation,
            string welshAddressLine1,
            string welshAddressLine2,
            string welshAddressLine3,
            string welshAddressTown,
            DateTime? lastUpdateDate)
        {
            this.ESRLocationId = esrLocationId;
            this.LocationCode = locationCode;
            this.Description = description;
            this.InactiveDate = inactiveDate;
            this.AddressLine1 = assignmentAddressLine1;
            this.AddressLine2 = assignmentAddressLine2;
            this.AddressLine3 = assignmentAddressLine3;
            this.Town = assignmentAddressTown;
            this.County = assignmentAddressCounty;
            this.Postcode = assignmentAddressPostcode;
            this.Country = assignmentAddressCountry;
            this.Telephone = telephone;
            this.Fax = fax;
            this.PayslipDeliveryPoint = payslipDeliveryPoint;
            this.SiteCode = siteCode;
            this.WelshLocationTranslation = welshLocationTranslation;
            this.WelshAddress1 = welshAddressLine1;
            this.WelshAddress2 = welshAddressLine2;
            this.WelshAddress3 = welshAddressLine3;
            this.WelshTownTranslation = welshAddressTown;
            this.ESRLastUpdate = lastUpdateDate;
        }

        /// <summary>
        /// Enumeration of ESR Column references in the delimited record line
        /// </summary>
        private enum LocationRecordColumnsRef
        {
            /// <summary>
            /// The record type.
            /// </summary>
            RecordType = 0,

            /// <summary>
            /// The location id.
            /// </summary>
            LocationId,

            /// <summary>
            /// The location code.
            /// </summary>
            LocationCode,

            /// <summary>
            /// The location description.
            /// </summary>
            LocationDescription,

            /// <summary>
            /// The inactive date.
            /// </summary>
            InactiveDate,

            /// <summary>
            /// The address line 1.
            /// </summary>
            AddressLine1,

            /// <summary>
            /// The address line 2.
            /// </summary>
            AddressLine2,

            /// <summary>
            /// The address line 3.
            /// </summary>
            AddressLine3,

            /// <summary>
            /// The address town.
            /// </summary>
            AddressTown,

            /// <summary>
            /// The address county.
            /// </summary>
            AddressCounty,

            /// <summary>
            /// The address postcode.
            /// </summary>
            AddressPostcode,

            /// <summary>
            /// The address country.
            /// </summary>
            AddressCountry,

            /// <summary>
            /// The telephone.
            /// </summary>
            Telephone,

            /// <summary>
            /// The fax.
            /// </summary>
            Fax,

            /// <summary>
            /// The payslip delivery point.
            /// </summary>
            PayslipDeliveryPoint,

            /// <summary>
            /// The site code.
            /// </summary>
            SiteCode,

            /// <summary>
            /// The Welsh Translation
            /// </summary>
            WelshLocationTranslation,

            /// <summary>
            /// The welsh address line 1.
            /// </summary>
            WelshAddressLine1,

            /// <summary>
            /// The welsh address line 2.
            /// </summary>
            WelshAddressLine2,

            /// <summary>
            /// The welsh address line 3.
            /// </summary>
            WelshAddressLine3,

            /// <summary>
            /// The welsh town.
            /// </summary>
            WelshTown,

            /// <summary>
            /// The last update date.
            /// </summary>
            LastUpdateDate
        }

        /// <summary>
        /// Parses a delimited ESR Person record and transforms it into EsrLocationRecord data object
        /// </summary>
        /// <param name="recordLine">Delimited row from outbound file</param>
        /// <returns></returns>
        public static EsrLocationRecord ParseEsrLocationRecord(string recordLine)
        {
            #region record variables

            Int64 esrLocationId;
            string locationCode = string.Empty;
            string description = string.Empty;
            DateTime? inactiveDate = null;
            string assignmentAddressLine1 = string.Empty;
            string assignmentAddressLine2 = string.Empty;
            string assignmentAddressLine3 = string.Empty;
            string assignmentAddressTown = string.Empty;
            string assignmentAddressCounty = string.Empty;
            string assignmentAddressPostcode = string.Empty;
            string assignmentAddressCountry = string.Empty;
            string telephone = string.Empty;
            string fax = string.Empty;
            string payslipDeliveryPoint = string.Empty;
            string siteCode = string.Empty;
            string welshLocationTranslation = string.Empty;
            string welshAddressLine1 = string.Empty;
            string welshAddressLine2 = string.Empty;
            string welshAddressLine3 = string.Empty;
            string welshAddressTown = string.Empty;
            DateTime? lastUpdateDate = null;

            #endregion record variable

            string[] rec = recordLine.Split(EsrFile.RecordDelimiter);
            Action recordAction;

            switch (rec[(int)LocationRecordColumnsRef.RecordType].ToUpper())
            {
                case EsrRecordTypes.EsrLocationUpdateRecordType:
                    if (rec.Length != ((int)LocationRecordColumnsRef.LastUpdateDate + 1)) // insufficient columns in row
                    {
                        return null;
                    }

                    recordAction = Action.Update;
                    Int64.TryParse(rec[(int)LocationRecordColumnsRef.LocationId], out esrLocationId);
                    locationCode = rec[(int)LocationRecordColumnsRef.LocationCode];
                    description = rec[(int)LocationRecordColumnsRef.LocationDescription];
                    DateTime tmpInactiveDate;
                    if (EsrDateHelpers.TryParseExact(rec[(int)LocationRecordColumnsRef.InactiveDate], "yyyyMMdd", out tmpInactiveDate))
                    {
                        inactiveDate = tmpInactiveDate;
                    }

                    assignmentAddressLine1 = rec[(int)LocationRecordColumnsRef.AddressLine1];
                    assignmentAddressLine2 = rec[(int)LocationRecordColumnsRef.AddressLine2];
                    assignmentAddressLine3 = rec[(int)LocationRecordColumnsRef.AddressLine3];
                    assignmentAddressTown = rec[(int)LocationRecordColumnsRef.AddressTown];
                    assignmentAddressCounty = rec[(int)LocationRecordColumnsRef.AddressCounty];
                    assignmentAddressPostcode = rec[(int)LocationRecordColumnsRef.AddressPostcode];
                    assignmentAddressCountry = rec[(int)LocationRecordColumnsRef.AddressCountry];
                    telephone = rec[(int)LocationRecordColumnsRef.Telephone];
                    fax = rec[(int)LocationRecordColumnsRef.Fax];
                    payslipDeliveryPoint = rec[(int)LocationRecordColumnsRef.PayslipDeliveryPoint];
                    siteCode = rec[(int)LocationRecordColumnsRef.SiteCode];
                    welshLocationTranslation = rec[(int)LocationRecordColumnsRef.WelshLocationTranslation];
                    welshAddressLine1 = rec[(int)LocationRecordColumnsRef.WelshAddressLine1];
                    welshAddressLine2 = rec[(int)LocationRecordColumnsRef.WelshAddressLine2];
                    welshAddressLine3 = rec[(int)LocationRecordColumnsRef.WelshAddressLine3];
                    welshAddressTown = rec[(int)LocationRecordColumnsRef.WelshTown];
                    DateTime tmpLastUpdateDate;
                    if (EsrDateHelpers.TryParseExact(rec[(int)LocationRecordColumnsRef.LastUpdateDate], "yyyyMMdd HHmmss", out tmpLastUpdateDate))
                    {
                        lastUpdateDate = tmpLastUpdateDate;
                    }

                    break;
                case EsrRecordTypes.EsrLocationDeleteRecordType:
                    if (rec.Length < EsrFile.RecordDeleteLength) // insufficient columns in row
                    {
                        return null;
                    }

                    recordAction = Action.Delete;
                    long.TryParse(rec[EsrFile.RecordDeletionColumnRef], out esrLocationId);
                    break;
                default:
                    return null;
            }

            return new EsrLocationRecord(
                esrLocationId,
                locationCode,
                description,
                inactiveDate,
                assignmentAddressLine1,
                assignmentAddressLine2,
                assignmentAddressLine3,
                assignmentAddressTown,
                assignmentAddressCounty,
                assignmentAddressPostcode,
                assignmentAddressCountry,
                telephone,
                fax,
                payslipDeliveryPoint,
                siteCode,
                welshLocationTranslation,
                welshAddressLine1,
                welshAddressLine2,
                welshAddressLine3,
                welshAddressTown,
                lastUpdateDate) { Action = recordAction };
        }
    }
}
