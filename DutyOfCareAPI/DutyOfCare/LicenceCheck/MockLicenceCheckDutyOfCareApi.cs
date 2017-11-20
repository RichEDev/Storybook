namespace DutyOfCareAPI.DutyOfCare.LicenceCheck
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    using DutyOfCareAPI.DutyOfCareLicenceCheckApi;

    /// <summary>
    /// The mock licence check duty of care api.
    /// </summary>
    public class MockLicenceCheckDutyOfCareApi : IDutyOfCareApi
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MockLicenceCheckDutyOfCareApi"/> class. 
        /// Create a new <see cref="LicenceCheckDutyOfCareApi"/>object.
        /// </summary>
        /// <param name="credentials">
        /// <see cref="NetworkCredential"/> stores Username and Password for customer/account to access the DVLA licence check api
        /// </param>
        /// <param name="licenceCheckUrl">
        /// Licence check URL based on test/live environment
        /// </param>
        public MockLicenceCheckDutyOfCareApi(NetworkCredential credentials, string licenceCheckUrl)
        {
            this.LicencePortalUrl = licenceCheckUrl;
        }

        /// <summary>
        /// Gets or sets the licence portal url.
        /// </summary>
        public string LicencePortalUrl { get; set; }

        /// <summary>
        /// The get consent portal access response.
        /// </summary>
        /// <param name="clientCode">
        /// The client code.
        /// </param>
        /// <param name="firstName">
        /// The first name.
        /// </param>
        /// <param name="lastName">
        /// The last name.
        /// </param>
        /// <param name="driverLicencePlate">
        /// The driver licence plate.
        /// </param>
        /// <param name="emailAdress">
        /// The email adress.
        /// </param>
        /// <param name="middleName">Middle name of the employee</param>
        /// <param name="dateOfBirth">Date of birth of the employee</param>
        /// <param name="sex">Sex of the employee</param>
        /// <returns>
        /// The <see cref="GetConsentPortalAccessResponse"/>.
        /// </returns>
        public GetConsentPortalAccessResponse GetConsentPortalAccess(
            string clientCode,
            string firstName,
            string lastName,
            string driverLicencePlate,
            string emailAdress, 
            string middleName,
            string sex,
            DateTime dateOfBirth)
        {
            Guard.ThrowIfNullOrWhiteSpace(nameof(clientCode), clientCode);
            Guard.ThrowIfNullOrWhiteSpace(nameof(firstName), firstName);
            Guard.ThrowIfNullOrWhiteSpace(nameof(lastName), lastName);
            Guard.ThrowIfNullOrWhiteSpace(nameof(driverLicencePlate), driverLicencePlate);
            Guard.ThrowIfNullOrWhiteSpace(nameof(emailAdress), emailAdress);
            Guard.ThrowIfNullOrWhiteSpace(nameof(middleName), middleName);
            Guard.ThrowIfNullOrWhiteSpace(nameof(sex), sex);
            var response = new GetConsentPortalAccessResponse
            {
                DriverId = new Random().Next(0, 1000)
            };

            if (emailAdress == "duplicate@email.com")
            {
                response.ResponseCode = "313";
                response.DriverId = 0;
            }
            else
            {
                switch (driverLicencePlate)
                {
                    case "BNB99002281O99MA":
                        response.ResponseCode = "312";
                        response.SecurityCode = null;
                        break;
                    case "BNB99002281O99MB":
                        response.ResponseCode = "312";
                        response.SecurityCode = Guid.NewGuid().ToString();
                        break;
                    case "BNB99002281O99ZZ":
                        response.ResponseCode = "301";
                        break;
                    case "BNB99002281O99XX":
                        response.ResponseCode = "314";
                        break;
                    case "BNB99002281O99YY":
                        response.ResponseCode = "401";
                        break;
                    case "BNB99002281O99PP":
                        response.ResponseCode = "504";
                        break;
                    case "BNB99002281O99QQ":
                        response.ResponseCode = "506";
                        break;
                }
            }

            if (string.IsNullOrEmpty(response.ResponseCode))
            {
                response.SecurityCode = Guid.NewGuid().ToString();
            }

            return response;
        }

        /// <summary>
        /// Gets driving licence details from portal
        /// </summary>
        /// <param name="driverNumber">
        /// Licence number
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
        /// <param name="isInitiallookUp">Indicates if it is initial or consecutive lookup.Value is true then initial lookup</param>
        /// <param name="clientCode">DVLA look up key for the account</param>
        /// <returns>
        /// The <see cref="DrivingLicenceDetailsResponse"/>
        /// Drivinglicence details.
        /// </returns>
        public DrivingLicenceDetailsResponse GetDrivingLicence(
            string driverNumber,
            string firstname,
            string surname,
            DateTime dateOfBirth,
            Sex sex,
            long driverId,
            int employeeId,
            bool isInitiallookUp,
            string clientCode)
        {
            Guard.ThrowIfNullOrWhiteSpace(nameof(driverNumber), driverNumber);
            Guard.ThrowIfNullOrWhiteSpace(nameof(firstname), firstname);
            Guard.ThrowIfNullOrWhiteSpace(nameof(surname), surname);

            var drivingLicence = new DrivingLicenceDetailsResponse();
            switch (driverNumber)
            {
                case "ERROR901060M99GG":
                    drivingLicence.EmployeeId = employeeId;
                    drivingLicence.ErrorStatus = "Validation Error";
                    drivingLicence.ResponseCode = "300";
                    break;
                case "ERROR902060M99FF":
                    drivingLicence.EmployeeId = employeeId;
                    drivingLicence.ErrorStatus = "Invalid driver or issue number";
                    drivingLicence.ResponseCode = "301";
                    break;
                case "ERROR901060M99EE":
                    drivingLicence.EmployeeId = employeeId;
                    drivingLicence.ErrorStatus = "Driver with provided ID does not exist";
                    drivingLicence.ResponseCode = "302";
                    break;
                case "ERROR902060M99II":
                    drivingLicence.EmployeeId = employeeId;
                    drivingLicence.ErrorStatus = "Unable to locate mandate";
                    drivingLicence.ResponseCode = "310";
                    break;
                case "ERROR901060M99CC":
                    drivingLicence.EmployeeId = employeeId;
                    drivingLicence.ErrorStatus = "Mandate awaiting approval";
                    drivingLicence.ResponseCode = "311";
                    break;
                case "ERROR902060M99BB":
                    drivingLicence.EmployeeId = employeeId;
                    drivingLicence.ErrorStatus = "Driver has no valid consent on record";
                    drivingLicence.ResponseCode = "303";
                    break;
                case "ERROR902060M99AA":
                    drivingLicence.EmployeeId = employeeId;
                    drivingLicence.ErrorStatus = "Driver not located in system";
                    drivingLicence.ResponseCode = "305";
                    break;
                case "ERROR902060M99MM":
                    drivingLicence.EmployeeId = employeeId;
                    drivingLicence.ErrorStatus = "DVLA Systems failed to respond";
                    drivingLicence.ResponseCode = "500";
                    break;
                case "ERROR901060M99LL":
                    drivingLicence.EmployeeId = employeeId;
                    drivingLicence.ErrorStatus = "Insufficient credits available";
                    drivingLicence.ResponseCode = "501";
                    break;
                case "ERROR902060M99KK":
                    drivingLicence.EmployeeId = employeeId;
                    drivingLicence.ErrorStatus = "Data Conversion Error";
                    drivingLicence.ResponseCode = "502";
                    break;
                case "BNB99002281O99PP":
                    drivingLicence.EmployeeId = employeeId;
                    drivingLicence.ErrorStatus = "Invalid username or password or Company Code provided";
                    drivingLicence.ResponseCode = "504";
                    break;
                case "ERROR901060M99DB":
                    drivingLicence.EmployeeId = employeeId;
                    drivingLicence.ErrorStatus = "Mandate awaiting approval";
                    drivingLicence.ResponseCode = "601";
                    break;
                case "ERROR902060M99DB":
                    drivingLicence.EmployeeId = employeeId;
                    drivingLicence.ErrorStatus = "Unable to locate mandate";
                    drivingLicence.ResponseCode = "602";
                    break;
                case "BNB99002281O99MM":
                    drivingLicence.EmployeeId = employeeId;
                    var endorsementsList = new List<EndorsementDetails>
                                           {
                                               new EndorsementDetails(
                                                   "DR10",
                                                   DateTime.Today.AddDays(-10).ToUniversalTime(),
                                                   true,
                                                   DateTime.Today.AddDays(-20).ToUniversalTime(),
                                                   0,
                                                   DateTime.Today.AddDays(-20).ToUniversalTime())
                                           };
                    endorsementsList.Add(new EndorsementDetails(
                                                   "SP30",
                                                   DateTime.Today.AddDays(-20).ToUniversalTime(),
                                                   true,
                                                   DateTime.Today.AddDays(-10).ToUniversalTime(),
                                                   3,
                                                   DateTime.Today.AddDays(-20).ToUniversalTime()));  
                    var entitlement = new List<EntitlementDetails>
                                           {
                                               new EntitlementDetails(
                                                   "A",
                                                   "P",
                                                   DateTime.Today.AddDays(-20),
                                                   DateTime.Today.AddDays(20))
                                           };
                    drivingLicence = new DrivingLicenceDetailsResponse(
                                                   "BNB99002281O99MM",
                                                   DateTime.Today.AddDays(-20),
                                                   DateTime.Today.AddDays(20),
                                                   "3",
                                                   "29",
                                                   "DS",
                                                   string.Empty,
                                                   endorsementsList,
                                                   entitlement,
                                                   employeeId,
                                                   string.Empty);

                    break;
                case "BNB99002281O99ER":
                    drivingLicence.EmployeeId = employeeId;
                    var endorsementsListForDisqualified = new List<EndorsementDetails>
                                           {
                                               new EndorsementDetails(
                                                   "DR10",
                                                   DateTime.Today.AddDays(-20).ToUniversalTime(),
                                                   true,
                                                   DateTime.Today.AddDays(-10).ToUniversalTime(),
                                                   0,
                                                   DateTime.Today.AddDays(-20).ToUniversalTime())
                                           };
                    endorsementsListForDisqualified.Add(new EndorsementDetails(
                        "SP30",
                        DateTime.Today.AddDays(-20).ToUniversalTime(),
                        true,
                        DateTime.Today.AddDays(-10).ToUniversalTime(),
                        3,
                        DateTime.Today.AddDays(-20).ToUniversalTime()));
                    entitlement = new List<EntitlementDetails>
                                      {
                                          new EntitlementDetails(
                                              "A",
                                              "P",
                                              DateTime.Today.AddDays(-20),
                                              DateTime.Today.AddDays(20))
                                      };
                    drivingLicence = new DrivingLicenceDetailsResponse(
                        "BNB99002281O99MM",
                        DateTime.Today.AddDays(-20),
                        DateTime.Today.AddDays(20),
                        "3",
                        "29",
                        "DS",
                        string.Empty,
                        endorsementsListForDisqualified,
                        entitlement,
                        employeeId,
                        string.Empty);
                    break;
                default:
                    var endorsements = new List<EndorsementDetails>
                    {
                        new EndorsementDetails("SP50", DateTime.Today.AddDays(-20), true, DateTime.Today.AddDays(-10), 10, DateTime.Today.AddDays(20)),
                        new EndorsementDetails("SP30", DateTime.Today.AddDays(-20), true, DateTime.Today.AddDays(-10), 10, DateTime.Today.AddDays(20))
                    };
                    var entitlements = new List<EntitlementDetails>
                    {
                        new EntitlementDetails("A", "P", DateTime.Today.AddDays(-20), DateTime.Today.AddDays(20)),
                        new EntitlementDetails("B","F",DateTime.Now.AddDays(-20),DateTime.Now.AddDays(20))
                    };

                    drivingLicence = new DrivingLicenceDetailsResponse(
                        "gfkde702271xb8mj",
                        DateTime.Today.AddDays(-20),
                        DateTime.Today.AddDays(20),
                        "1",
                        "11",
                        "FC",
                        string.Empty,
                        endorsements,
                        entitlements,
                        employeeId,
                        string.Empty);
                    break;
            }

            return drivingLicence;
        }

        /// <summary>
        /// Return a random chance of whether or not consent has been completed by the user.
        /// It's done this way rather than simply returning true to make sure the page polling works.
        /// </summary>
        /// <param name="driverId">The Driver to check for recorded consent</param>
        /// <returns></returns>
        public bool HasUserProvidedConsent(int driverId)
        {
            var randomNumber = new Random().Next(0, 3);

            return randomNumber == 1;
        }

        /// <summary>
        /// Invalidates the claimants consent. 
        /// </summary>
        /// <param name="securityKey">
        /// Security code for the claimants consent.
        /// </param>
        /// <returns>
        /// Whether the consent was revoked.
        /// </returns>
        public bool InvalidateUsersConsent(Guid securityKey)
        {
            Guard.ThrowIfNullOrWhiteSpace(nameof(securityKey), securityKey.ToString());
            return securityKey.ToString() != "4B54DFCB-9677-4C39-BE72-4EE4CB50B169";
        }
    }
}
