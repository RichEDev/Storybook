
namespace DutyOfCareAPI.DutyOfCare.LicenceCheck
{
    using System;
    using System.Linq;
    using System.Net;
    using System.ServiceModel;
    using DutyOfCare;
    using DutyOfCareLicenceCheckApi;
    using DutyOfCareAPI.DutyOfCare.LicenceCheck.VehicleLookup;
    using DutyOfCareAPI.VDLVehicleLookup;


    /// <summary>
    /// Class represents the Various Licence Check API methods
    /// </summary>
    public class LicenceCheckDutyOfCareApi : IDutyOfCareApi
    {
        /// <summary>
        /// Username and Password for customer/account to access the DVLA licence check api
        /// </summary>
        private readonly NetworkCredential _credentials;

        /// <summary>
        /// Mode which defines if it is live or test environment
        /// </summary>
        private DutyOfCare.Mode _mode;

        /// <summary>
        ///Licence portal url base of Mode(Test/Live)
        /// </summary>
        public string LicencePortalUrl { get; set; }

        /// <summary>
        /// Create a new <see cref="LicenceCheckDutyOfCareApi"/>object.
        /// </summary>
        /// <param name="credentials"><see cref="NetworkCredential"/> stores Username and Password for customer/account to access the DVLA licence check api</param>
        /// <param name="licenceCheckUrl">Licence check URL based on test/live environment</param>
        public LicenceCheckDutyOfCareApi(NetworkCredential credentials, string licenceCheckUrl) : this(credentials, DutyOfCare.Mode.Live, licenceCheckUrl)
        {
            this.LicencePortalUrl = licenceCheckUrl;
        }

        /// <summary>
        /// Create a new LicenceCheckDutyOfCareApi object.
        /// </summary>
        /// <param name="credentials">object stores Username and Password for customer/account to access the DVLA licence check api</param>
        /// <param name="mode">Live or Test mode</param>
        /// <param name="licenceCheckUrl">Licence check URL based on test/live environment</param>
        protected LicenceCheckDutyOfCareApi(NetworkCredential credentials, DutyOfCare.Mode mode, string licenceCheckUrl)
        {
            Guard.ThrowIfNullOrWhiteSpace(nameof(credentials.Password), credentials.Password);
            Guard.ThrowIfNullOrWhiteSpace(nameof(credentials.UserName), credentials.UserName);
            this._credentials = credentials;
            this._mode = mode;
            this.LicencePortalUrl = licenceCheckUrl;
        }


        /// <summary>
        /// Method interact with the Licence Portal API and generate the securitycode and driver id
        /// </summary>
        /// <param name="clientCode">DVLA look up key for the account</param>
        /// <param name="firstName">Firstname of the employee</param>
        /// <param name="lastName">Last name of the employee</param>
        /// <param name="licenceNumber">The licence number on the user's driving licence</param>
        /// <param name="emailAdress">Email address of the employee</param>
        /// <param name="middleName">Middle name of the employee</param>
        /// <param name="dateOfBirth">Date of birth of the employee</param>
        /// <param name="sex">Sex of employee</param>
        /// <returns><see cref="GetConsentPortalAccessResponse"/> returns the api response information like security code, driverid and the reponse message</returns>
        public GetConsentPortalAccessResponse GetConsentPortalAccess(string clientCode, string firstName, string lastName, string licenceNumber, string emailAdress,string middleName, string sex, DateTime dateOfBirth)
        {
            Guard.ThrowIfNullOrWhiteSpace(nameof(clientCode), clientCode);
            Guard.ThrowIfNullOrWhiteSpace(nameof(firstName), firstName);
            Guard.ThrowIfNullOrWhiteSpace(nameof(lastName), lastName);
            Guard.ThrowIfNullOrWhiteSpace(nameof(licenceNumber), licenceNumber);
            Guard.ThrowIfNullOrWhiteSpace(nameof(emailAdress), emailAdress);
            Guard.ThrowIfNullOrWhiteSpace(nameof(middleName), middleName);

            GetConsentPortalAccessResponse response = null;
            using (var quickCheckClient = new QuickCheckClient())
            {
                quickCheckClient.Open();

                if (quickCheckClient.State == CommunicationState.Opened)
                {
                    var driverResponse = this.CreateSystemDriver(quickCheckClient, clientCode, firstName, lastName, licenceNumber, emailAdress, middleName,sex, dateOfBirth);

                    if (driverResponse != null)
                    {
                        response = new GetConsentPortalAccessResponse(driverResponse.SecurityCode == Guid.Empty ? string.Empty : driverResponse.SecurityCode.ToString(), driverResponse.DriverId, driverResponse.Response.Details, driverResponse.Response.Code);
                    }
                }
            }
            return response;
        }

        /// <summary>
        /// Invalidates the employees conent
        /// </summary>
        /// <param name="securityKey">Security code of an employee to access the consent portal.</param> 
        /// <returns>If the invalidation was successfull or not.</returns>
        public bool InvalidateUsersConsent(Guid securityKey)
        {
            Guard.ThrowIfNullOrWhiteSpace(nameof(securityKey), securityKey.ToString());
            using (var quickCheckClient = new QuickCheckClient())
            {
                quickCheckClient.Open();

                if (quickCheckClient.State == CommunicationState.Opened)
                {
                    var driverResponse = quickCheckClient.InvalidateEConsentBySecurityCode(this._credentials.UserName, this._credentials.Password, securityKey);

                    if (driverResponse != null && driverResponse.Response.Success && driverResponse.Response.Code != "520")
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Creates the system driver used for generating the consent
        /// </summary>
        /// <param name="client">Initialise the licence check object for accessing the api</param> 
        /// <param name="clientCode">DVLA look up key for the account</param>
        /// <param name="firstName">Firstname of the employee</param>
        /// <param name="lastName">Last name of the employee</param>
        /// <param name="licenceNumber">The licence number on the user's driving licence</param>
        /// <param name="emailAdress">Email address of the employee</param>
        /// <param name="middleName">Middle name of the employee</param>
        /// <param name="dateOfBirth">Date of birth of the employee</param>
        /// <param name="sex">Sex of the employee</param>
        /// <returns>DriverResponse class from the Licence check api include the security code and driverid required to login further to submit the consent</returns>
        private DriverResponse CreateSystemDriver(IQuickCheck client, string clientCode, string firstName, string lastName, string licenceNumber, string emailAdress, string middleName, string sex, DateTime dateOfBirth)
        {
            var data = new CreateDriverData
            {
                CompanyCode = clientCode,
                Firstname = firstName,
                Surname = lastName,
                EmailAddress = emailAdress,
                DriverNumber = licenceNumber,
                Middlename = middleName,
                Gender = sex == "Male" ? Sex.Male : Sex.Female,
                DateOfBirth =dateOfBirth
            };

            var driverResponse = client.CreateSystemDriver(this._credentials.UserName, this._credentials.Password, data);
            return driverResponse;
        }

        /// <summary>
        /// Gets driving licence details from portal
        /// </summary>
        /// <param name="drivingLicenceNumber">
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
        /// <param name="intialLookUp">Indicates if it is Initial or consecutive lookup.Value is true then initaila lookup</param>
        /// <param name="clientCode">DVLA look up key for the account</param>
        /// <returns>
        /// The <see cref="DrivingLicenceDetailsResponse"/>.
        /// Driving licence details
        /// </returns>
        public DrivingLicenceDetailsResponse GetDrivingLicence(string drivingLicenceNumber, string firstname, string surname,DateTime dateOfBirth,Sex sex,long driverId,int employeeId,bool intialLookUp,string clientCode)
        {
            Guard.ThrowIfNullOrWhiteSpace(nameof(drivingLicenceNumber), drivingLicenceNumber);
            Guard.ThrowIfNullOrWhiteSpace(nameof(firstname), firstname);
            Guard.ThrowIfNullOrWhiteSpace(nameof(surname), surname);

            using (var quickCheckClient = new QuickCheckClient())
            {
                try
                {
                    quickCheckClient.Open();

                    if (quickCheckClient.State != CommunicationState.Opened) return null;
                    if (!quickCheckClient.ValidateUser(this._credentials.UserName, this._credentials.Password).Success) return null;
                    CheckResponse response;
                    if (intialLookUp)
                    {
                        var data = new GetLicenceData
                        {
                            Username = this._credentials.UserName,
                            Password = this._credentials.Password,
                            DriverNumber = drivingLicenceNumber,
                            Surname = surname,
                            Firstname = firstname,
                            DateOfBirth = dateOfBirth,
                            Sex = sex,
                            DriverId = driverId
                        };

                        response = quickCheckClient.GetLicenceData(data);
                    }
                    else
                    {
                        var data = new CheckLicenceData
                        {
                            Username = this._credentials.UserName,
                            Password = this._credentials.Password,
                            DriverNumber = drivingLicenceNumber,
                            Surname = surname,
                            Firstname = firstname,
                            DateOfBirth = dateOfBirth,
                            Sex = sex,
                            DriverId = driverId,
                            CompanyCode= clientCode,
                              YourReference = new Guid().ToString()
                        };

                        response = quickCheckClient.CheckLicence(data, null);
                    }
                    return SetTheDrivingLicenceInformation(employeeId, response);
                    
                }
                catch(Exception ex)
                {
                    return new DrivingLicenceDetailsResponse(employeeId, "Error while fetching driving licence " + ex.Message, "-1");
                }
            }
        }

        /// <summary>
        /// Checks whether or not the user has valid consent on record at LicenceCheck
        /// </summary>
        /// <param name="driverId">The Driver to check</param>
        /// <returns>true if consent is on record, false if not</returns>
        public bool HasUserProvidedConsent(int driverId)
        {
            using (var quickCheckClient = new QuickCheckClient())
            {
                quickCheckClient.Open();

                if (quickCheckClient.State != CommunicationState.Opened) return false;

                var response = quickCheckClient.HasDriverGivenEConsent(this._credentials.UserName, this._credentials.Password, driverId);

                return response.Data;
            }
        }

        /// <summary>
        /// Lookup a vehicle based on the registration number
        /// </summary>
        /// <param name="registrationNumber"></param>
        /// <param name="lookupLogger">An instance of <see cref="ILookupLogger"/></param>
        /// <returns>An instance of <see cref="IVehicleLookupResult"/></returns>
        public IVehicleLookupResult Lookup(string registrationNumber, ILookupLogger lookupLogger)
        {
            var service = new VDLServiceClient();
            var response = service.GetAdvancedVehicleData(this._credentials.UserName, this._credentials.Password, registrationNumber);
            lookupLogger.Write(registrationNumber, response.ResponseMessage.Code, response.ResponseMessage.Description);
            if (response.Success)
            {
                var engineCapacity = 0;
                int.TryParse(response.VehicleData.ExactCc, out engineCapacity);
                if (engineCapacity == 0)
                {
                    int.TryParse(response.VehicleData.Cc, out engineCapacity);
                }
 
                var result = new VehicleLookupSuccess
                {
                    Message = response.ResponseMessage.Description,
                    Code = response.ResponseMessage.Code,
                    Vehicle = new Vehicle
                    {
                        RegistrationNumber = response.VehicleData.VRM,
                        Model = response.VehicleData.DvlaModel,
                        Make = response.VehicleData.DvlaMake,
                        FuelType = response.VehicleData.Fuel,
                        EngineCapacity = engineCapacity,
                        VehicleType = string.IsNullOrEmpty(response.VehicleData.VehicleType) ? response.VehicleData.BodyStyle : response.VehicleData.VehicleType
                    }
                };

                return result;
            }
            else
            {
                return new VehicleLookupFailed(response.ResponseMessage.Code, response.ResponseMessage.Description);
            }
        }

        private static DrivingLicenceDetailsResponse SetTheDrivingLicenceInformation(int employeeId, CheckResponse response)
        {
            if (response.Data == null) return new DrivingLicenceDetailsResponse(employeeId, response.Response.Details.Trim(), response.Response.Code);
            var endorsements = response.Data.result[0].licence.endorsements.Select(endorsement => new EndorsementDetails(endorsement.code, endorsement.convictionDate, endorsement.isDisqual, endorsement.disqualEndDate, endorsement.noOfPoints, endorsement.offenceDate)).ToList();
            var entitlements = response.Data.result[0].licence.entitlements.Select(entitlement => new EntitlementDetails(entitlement.code, entitlement.type, entitlement.validFrom, entitlement.validTo)).ToList();
            var drivingLicence = new DrivingLicenceDetailsResponse(
                response.DriverNumber,
                response.Data.result[0].licence.validFrom,
                response.Data.result[0].licence.validTo,
                response.Data.result[0].licence.directiveIdentifier,
                response.Data.result[0].licence.issueNumber,
                response.Data.result[0].licence.status,
                response.Response.Details.Trim(),
                endorsements,
                entitlements,
                employeeId,
                response.Response.Code ?? string.Empty);
            return drivingLicence;
        }
    }
}
