namespace DutyOfCareAPI.DutyOfCare
{
    using System;

    /// <summary>
    /// The licence entitlement (sometimes known as the licence category) provides details of the specific driving entitlements for the specified licence
    /// </summary>
    public class EntitlementDetails
    {
        /// <summary>
        /// Code of entitlement category
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Indicates if entitlement is Provisional (P), Full (F), or Unclaimed Test Pass (U)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Date the entitlement is valid from
        /// </summary>
        public DateTime ValidFrom { get; set; }

        /// <summary>
        ///  Date the entitlement is valid to
        /// </summary>
        public DateTime ValidTo { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntitlementDetails"/> class. 
        /// Entilement constructor with below details
        /// </summary>
        /// <param name="code">
        /// Code of entitlement category
        /// </param>
        /// <param name="type">
        /// Indicates if entitlement is Provisional (P), Full (F), or Unclaimed Test Pass (U)
        /// </param>
        /// <param name="validFrom">
        /// Date the entitlement is valid from
        /// </param>
        /// <param name="validTo">
        /// Date the entitlement is valid to
        /// </param>
        public EntitlementDetails(string code,string type,DateTime validFrom,DateTime validTo)
        {
            this.Code = code;
            this.Type = type;
            this.ValidFrom = validFrom;
            this.ValidTo = validTo;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntitlementDetails"/> class.
        /// </summary>
        public EntitlementDetails()
        {
        }
    }
}