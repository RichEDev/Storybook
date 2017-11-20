namespace SpendManagementApi.Models.Types
{
    using System;
    using Interfaces;
    using SpendManagementLibrary;

    /// <summary>
    /// Represents an NHS Trust. This is usually a regional sub-company or unit.
    /// </summary>
    public class NhsTrust : ArchivableBaseExternalType, IApiFrontForDbObject<cESRTrust, NhsTrust>
    {
        /// <summary>
        /// The unique Id of this NHS Trust in the expenses database.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The unique Id of this NHS Trust, internally for the NHS.
        /// </summary>
        public string TrustVpd { get; set; }

        /// <summary>
        /// The name of the NHS Trust.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <returns>An api Type</returns>
        public NhsTrust From(cESRTrust dbType, IActionContext actionContext)
        {
            Id = dbType.TrustID;
            TrustVpd = dbType.TrustVPD;
            Label = dbType.TrustName;
            Archived = dbType.Archived;
            
            AccountId = dbType.AccountID;
            CreatedOn = dbType.CreatedOn ?? DateTime.UtcNow;
            ModifiedOn = dbType.ModifiedOn;
            
            return this;
        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <returns>A data access layer Type</returns>
        public cESRTrust To(IActionContext actionContext)
        {
            return null;
            //new cESRTrust(Id, Label, TrustVpd, null, null, 0, null, null, null, Archived, CreatedOn, ModifiedOn, "", byte.Parse("1"));
        }
    }
}