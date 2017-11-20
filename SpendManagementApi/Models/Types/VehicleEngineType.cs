namespace SpendManagementApi.Models.Types
{
    using System;

    using SpendManagementApi.Interfaces;

    /// <summary>
    /// A class to hold details of a Vehicle Engine Type
    /// </summary>
    public class VehicleEngineType : BaseExternalType, IApiFrontForDbObject<SpendManagementLibrary.VehicleEngineType, VehicleEngineType>
    {
        /// <summary>
        /// Gets or sets the vehicle engine type id.
        /// </summary>
        public int? VehicleEngineTypeId { get; set; }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        public int? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the modified on.
        /// </summary>
        public DateTime? ModifiedOn { get; set; }

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>An api Type</returns>
        public VehicleEngineType From(SpendManagementLibrary.VehicleEngineType dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            this.VehicleEngineTypeId = dbType.VehicleEngineTypeId;        
            this.Code = dbType.Code;
            this.Name = dbType.Name;
            this.CreatedBy = dbType.CreatedBy;
            this.CreatedOn = dbType.CreatedOn;
            this.ModifiedBy = dbType.ModifiedBy;
            this.ModifiedOn = dbType.ModifiedOn;

            return this;
        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>A data access layer Type</returns>
        public SpendManagementLibrary.VehicleEngineType To(IActionContext actionContext)
        {
            throw new NotImplementedException();
        }
    }
}
