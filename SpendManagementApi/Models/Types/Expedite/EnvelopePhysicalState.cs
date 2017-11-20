namespace SpendManagementApi.Models.Types.Expedite
{
    using System.ComponentModel.DataAnnotations;
    using Interfaces;
    using Utilities;
    
    /// <summary>
    /// Represents a physical envelope that may be sent 
    /// to a claimant and returned with receipts inside.
    /// </summary>
    public class EnvelopePhysicalState : BaseExternalType, IApiFrontForDbObject<SpendManagementLibrary.Expedite.EnvelopePhysicalState, EnvelopePhysicalState>
    {
        /// <summary>
        /// The unqiue primary key of this envelope.
        /// </summary>
        public int EnvelopePhysicalStateId { get; set; }

        /// <summary>
        /// The custom envelope number that is used by the claimant
        /// to generate a claim reference number (CRN).
        /// </summary>
        [MaxLength(100, ErrorMessage = ApiResources.ErrorMaxLength + @"100")]
        public string Details { get; set; }
        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The action context</param>
        /// <returns>An api Type</returns>
        public EnvelopePhysicalState From(SpendManagementLibrary.Expedite.EnvelopePhysicalState dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            return new EnvelopePhysicalState
            {
                EnvelopePhysicalStateId = dbType.EnvelopePhysicalStateId,
                Details = dbType.Details
            };
        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type
        /// </summary>
        /// <returns>A data access layer Type</returns>
        public SpendManagementLibrary.Expedite.EnvelopePhysicalState To(IActionContext actionContext)
        {
            var item = new SpendManagementLibrary.Expedite.EnvelopePhysicalState()
            {
                EnvelopePhysicalStateId = EnvelopePhysicalStateId,
                Details = Details
            };

            return item;
        }
    }
}
