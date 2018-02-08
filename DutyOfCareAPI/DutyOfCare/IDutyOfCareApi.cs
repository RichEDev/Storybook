namespace DutyOfCareAPI.DutyOfCare
{
    using System;

    using DutyOfCareLicenceCheckApi;

    /// <summary>
    /// Interface for all the Duty of care API operations
    /// </summary>
    public interface IDutyOfCareApi
    {
        /// <summary>
        /// Gets or sets the licence portal url of the licence check.
        /// </summary>
        string LicencePortalUrl { get; set; }

        /// <summary>
        /// GetConsentPortalAccess Method send request to Licence check API to get the Security code to access the Portal and submit the consent for an employee
        /// </summary>
        /// <param name="clientCode">DVLA look up key for the account</param>
        /// <param name="firstName">Firstname of the employee</param>
        /// <param name="lastName">Last name of the employee</param>
        /// <param name="drivingLicenceNumber">driving licence number</param>
        /// <param name="emailAdress">Email address of the employee</param>
        /// <param name="middleName">Middle name of the employee</param>
        /// <param name="dateOfBirth">Date of birth of the meployee</param>
        /// <param name="sex">Sex of employee</param>
        /// <returns>GetConsentPortalAccessResponse response object</returns>
        GetConsentPortalAccessResponse GetConsentPortalAccess(string clientCode, string firstName, string lastName, string drivingLicenceNumber, string emailAdress,string middleName, string sex, DateTime dateOfBirth);

        /// <summary>
        /// Invalidates the claimants consent. 
        /// </summary>
        /// <param name="securityKey">
        /// Security code for the claimants consent.
        /// </param>
        /// <returns>
        /// Whether the consent was revoked.
        /// </returns>
        bool InvalidateUsersConsent(Guid securityKey);

        /// <summary>
        /// Gets driving licence details from portal
        /// </summary>
        /// <param name="drivingLicenceNumber">
        /// Driving licence number
        /// </param>
        /// <param name="firstname">
        /// Firstname from driving licence
        /// </param>
        /// <param name="surname">
        /// Surname from driving licence
        /// </param>
        /// <param name="dateOfBirth">
        /// Date of birth from driving licence
        /// </param>
        /// <param name="sex">
        /// Sex of driving licence owner
        /// </param>
        /// <param name="driverId">
        /// Driver id which got while providing consent
        /// </param>
        /// <param name="employeeId">
        /// Owner of driving licence
        /// </param>
        /// <param name="initialLookUp">Indicates if it is Initial or consecutive lookup.Value is true then initial lookup</param>
        /// <param name="clientCode">DVLA look up key for the account</param>
        /// <returns>
        /// The <see cref="DrivingLicenceDetailsResponse"/>.
        /// Driving licence details
        /// </returns>
        DrivingLicenceDetailsResponse GetDrivingLicence(string drivingLicenceNumber, string firstname, string surname, DateTime dateOfBirth, Sex sex, long driverId, int employeeId,bool initialLookUp, string clientCode);

        /// <summary>
        /// Checks whether or not the provided driver has valid consent on record.
        /// </summary>
        /// <param name="driverId">The driver to check</param>
        /// <returns>true if consent is on record, false if not</returns>
        bool HasUserProvidedConsent(int driverId);

        /// <summary>
        /// Lookup a vehicle based on the registration number
        /// </summary>
        /// <param name="registrationNumber"></param>
        /// <param name="lookupLogger">An instance of <see cref="ILookupLogger"/></param>
        /// <returns>An instance of <see cref="IVehicleLookupResult"/></returns>
        IVehicleLookupResult Lookup(string registrationNumber, ILookupLogger lookupLogger);

    }
}
