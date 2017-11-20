namespace SpendManagementApi.Models.Types
{
    using SpendManagementApi.Interfaces;
   
    /// <summary>
    /// The address label.
    /// </summary>
    public class AddressLabel : BaseExternalType, IBaseClassToAPIType<SpendManagementLibrary.Addresses.AddressLabel, AddressLabel>
    {
        /// <summary>
        /// Gets or sets the address location id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier from Postcode Anywhere
        /// </summary>
        public string GlobalIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the address friendly text.
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the label id.
        /// </summary>
        public int LabelId { get; set; }

        /// <summary>
        /// Gets or sets the primary account wide label Id.
        /// </summary>
        public int? PrimaryAccountWideLabel { get; set; }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="dbType">
        /// The db Type.
        /// </param>
        /// <param name="actionContext">
        /// The actionContext which contains DAL classes.
        /// </param>
        /// <returns>
        /// A API Type
        /// </returns>
        public AddressLabel ToApiType(SpendManagementLibrary.Addresses.AddressLabel dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            this.FriendlyName = dbType.AddressFriendlyName;
            this.GlobalIdentifier = dbType.GlobalIdentifier;
            this.Id = dbType.AddressID;
            this.Label = dbType.Text;
            this.LabelId = dbType.AddressLabelID;

            return this;
        }
    }
}