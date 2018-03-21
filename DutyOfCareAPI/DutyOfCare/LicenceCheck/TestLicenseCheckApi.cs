namespace DutyOfCareAPI.DutyOfCare.LicenceCheck
{
    using System.Net;

    using DutyOfCareAPI.DutyOfCare.LicenceCheck.VehicleLookup;

    /// <summary>
    /// Test class for LicenseCheckApi. Class is initialise if the mode is Test 
    /// </summary>
    public class TestLicenseCheckApi : LicenceCheckDutyOfCareApi
    {
        /// <summary>
        /// An instance of <see cref="IVehicleLookup"/> created as part of the bootstrap.
        /// </summary>
        public IVehicleLookup VehicleLookup { get; set; }

        /// <summary>
        /// Create a new TestLicenseCheckApi object.
        /// </summary>
        /// <param name="credential"><see cref="NetworkCredential"/>store the username and password for the account to acccess the api</param>
        /// <param name="licenceCheckUrl">Url to request the access to Licence check portal</param>
        public TestLicenseCheckApi(NetworkCredential credential,string licenceCheckUrl) : base(credential, Mode.Test, licenceCheckUrl)
        {

        }
    }
}
