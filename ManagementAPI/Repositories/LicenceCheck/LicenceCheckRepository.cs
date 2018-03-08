namespace ManagementAPI.Repositories.LicenceCheck
{
    using ManagementAPI.LicenceCheckAPI;
    using ManagementAPI.Models;

    public class LicenceCheckRepository
    {
        /// <summary>
        /// The Selenity company code
        /// </summary>
        private static string CompanyCode = "MAC00852";

        /// <summary>
        /// The Selenity company code
        /// </summary>
        private static string AgentAccount = "MA00083";

        /// <summary>
        /// Our API username
        /// </summary>
        private static string Username = "Selenity";

        /// <summary>
        /// Our password
        /// </summary>
        private static string Password = "rOTw/tcK4a9q8IS6aTpLvg==";

        public SecureData crypt = new SecureData();

        /// <summary>
        /// Creates a company on the Licence Check API.
        /// </summary>
        /// <param name="licenceCheckForm"></param>
        /// <returns></returns>
        public CompanyResponse Setup(LicenceCheckForm licenceCheckForm)
        {
            var companyName = licenceCheckForm.CompanyName;
            var accountContact = licenceCheckForm.ContactName;
            var address1 = licenceCheckForm.AddressLine1;
            var address2 = licenceCheckForm.AddressLine2;
            var address3 = licenceCheckForm.AddressLine3;
            var town = licenceCheckForm.Town;
            var county = licenceCheckForm.County;
            var postcode = licenceCheckForm.Postcode;
            int creditsToTransfer = licenceCheckForm.Credits;

            using (var client = new QuickCheckClient())
            {
                var companyResponse = CreateCompany(client, companyName, accountContact, address1, address2, address3, town, county, postcode);

                companyResponse.Response.Success = true;

                if (companyResponse.Response.Success)
                {
                    var transferResponse = client.TransferCreditsToCompanyCode(Username, crypt.Decrypt(Password), AgentAccount, companyResponse.CompanyCode, creditsToTransfer);
                }

                return companyResponse;
            }
        }

        private CompanyResponse CreateCompany(QuickCheckClient client, string companyName, string accountContact, string address1, string address2, string address3, string town, string county, string postcode)
        {
            var createCompanyResponse = client.CreateNewCompany(Username, crypt.Decrypt(Password), new CreateCompanyData
            {
                AccountCompanyType = AccountCompanyType.Client,

                CompanyName = companyName,
                AccountContact = accountContact,
                Address1 = address1,
                Address2 = address2,
                Address3 = address3,
                PostalTown = town,
                County = county,
                Postcode = postcode,
                ExtensionData = null,
                ParentCompanyCode = AgentAccount,
                UseParentCompanyCredits = false
            });

            return createCompanyResponse;
        }

        private InformationResponse GetCredits(QuickCheckClient client, string company)
        {
            var credits = client.GetAvailableCredits(Username, crypt.Decrypt(Password), company);
            return credits;
        }
    }
}