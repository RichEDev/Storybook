namespace SpendManagementApi.Models.Types.Expedite
{
    using System.ComponentModel.DataAnnotations;
    using Interfaces;
    using Utilities;

    /// <summary>
    /// Represents a type of Envelope, from the EnvelopeTypes table in metabase.
    /// </summary>
    public class EnvelopeType : BaseExternalType, IApiFrontForDbObject<SpendManagementLibrary.Expedite.EnvelopeType, EnvelopeType>
    {
        /// <summary>
        /// The unique primary key of this type.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The label for human consumption.
        /// </summary>
        [MaxLength(50, ErrorMessage = ApiResources.ErrorMaxLength + @"50")]
        public string Label { get; set; }

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext"></param>
        /// <returns>An api Type</returns>
        public EnvelopeType From(SpendManagementLibrary.Expedite.EnvelopeType dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            Id = dbType.EnvelopeTypeId;
            Label = dbType.Label;
            return this;
        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type
        /// </summary>
        /// <returns>A data access layer Type</returns>
        public SpendManagementLibrary.Expedite.EnvelopeType To(IActionContext actionContext)
        {
            return new SpendManagementLibrary.Expedite.EnvelopeType
            {
                EnvelopeTypeId = Id,
                Label = Label
            };
        }
    }
}
