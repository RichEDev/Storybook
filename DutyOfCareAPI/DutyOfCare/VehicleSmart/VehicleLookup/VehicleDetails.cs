namespace DutyOfCareAPI.DutyOfCare.VehicleSmart.VehicleLookup
{
    /// <summary>
    /// part of the return data for a Vehicle Lookup using (https://www.getvehiclesmart.com/)
    /// </summary>
    public class VehicleDetails
    {
        /// <summary>
        /// Gets or sets the CO2Emissions value
        /// </summary>
        public string Co2Emissions { get; set; }

        /// <summary>
        /// Gets or sets the Vehicle Type Approval value
        /// </summary>
        public string VehicleTypeApproval { get; set; }

        /// <summary>
        /// Gets or sets the Vehicle Cylendar capacity.
        /// </summary>
        public long CylinderCapacity { get; set; }

        /// <summary>
        /// Gets or sets the Vehicle Fuel Type 
        /// </summary>
        public string Fuel { get; set; }

        /// <summary>
        /// Gets or sets the Vehicle Production Year
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// Gets or sets the Vehicle Registration date
        /// </summary>
        public string DateOfRegistration { get; set; }

        /// <summary>
        /// Gets or sets the Vehicle Make
        /// </summary>
        public string Make { get; set; }

        /// <summary>
        /// Gets or sets the Vehicle Export Marker
        /// </summary>
        public string ExportMarker { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the vehicle is taxed.
        /// </summary>
        public bool Taxed { get; set; }

        /// <summary>
        /// Gets or sets the Vehicle Tax Due Date
        /// </summary>
        public string TaxDueDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is sorn.
        /// </summary>
        public bool IsSorn { get; set; }

        /// <summary>
        /// Gets or sets the Vehicle Colour
        /// </summary>
        public string Colour { get; set; }

        /// <summary>
        /// Gets or sets the Vehicle Wheelplan
        /// </summary>
        public string Wheelplan { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether no mot data held by dvla.
        /// </summary>
        public bool NoMotDataHeldByDvla { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether mot exempt.
        /// </summary>
        public bool MotExempt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the vehicle is MOT'd.
        /// </summary>
        public bool Motd { get; set; }

        /// <summary>
        /// Gets or sets the mot expiry date.
        /// </summary>
        public string MotExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether dvla results.
        /// </summary>
        public bool DvlaResults { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether dvsa results.
        /// </summary>
        public bool DvsaResults { get; set; }

        /// <summary>
        /// Gets or sets the registration.
        /// </summary>
        public string Registration { get; set; }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the mot qty recorded in miles.
        /// </summary>
        public long MotQtyRecordedInMiles { get; set; }

        /// <summary>
        /// Gets or sets the mot qty recorded in kms.
        /// </summary>
        public long MotQtyRecordedInKms { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether mot miles and kms.
        /// </summary>
        public bool MotMilesAndKms { get; set; }

        /// <summary>
        /// Gets or sets the mot pass qty.
        /// </summary>
        public long MotPassQty { get; set; }

        /// <summary>
        /// Gets or sets the mot fail qty.
        /// </summary>
        public long MotFailQty { get; set; }

        /// <summary>
        /// Gets or sets the mot total advisory notices.
        /// </summary>
        public long MotTotalAdvisoryNotices { get; set; }

        /// <summary>
        /// Gets or sets the mot total failure notices.
        /// </summary>
        public long MotTotalFailureNotices { get; set; }

        /// <summary>
        /// Gets or sets the make dvsa.
        /// </summary>
        public string MakeDvsa { get; set; }

        /// <summary>
        /// Gets or sets the model dvsa.
        /// </summary>
        public string ModelDvsa { get; set; }

        /// <summary>
        /// Gets or sets the first used date dvsa.
        /// </summary>
        public string FirstUsedDateDvsa { get; set; }

        /// <summary>
        /// Gets or sets the mot expiry date dvsa.
        /// </summary>
        public string MotExpiryDateDvsa { get; set; }

        /// <summary>
        /// Gets or sets the fuel dvsa.
        /// </summary>
        public string FuelDvsa { get; set; }

        /// <summary>
        /// Gets or sets the colour dvsa.
        /// </summary>
        public string ColourDvsa { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether motd dvsa.
        /// </summary>
        public bool MotdDvsa { get; set; }

        /// <summary>
        /// Gets or sets the date of registration source.
        /// </summary>
        public string DateOfRegistrationSource { get; set; }
    }
}