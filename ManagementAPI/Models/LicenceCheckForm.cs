namespace ManagementAPI.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class LicenceCheckForm
    {
        [DisplayName("Company Name"), Required]
        [RegularExpression("^[a-zA-Z0-9'() ]*$", ErrorMessage = "Only alphanumeric characters and spaces allowed.")]
        public string CompanyName { get; set; }

        [DisplayName("Contact Name"), Required]
        public string ContactName { get; set; }

        [DisplayName("Address Line 1"), Required]
        public string AddressLine1 { get; set; }

        [DisplayName("Address Line 2"), Required]
        public string AddressLine2 { get; set; }

        [DisplayName("Address Line 3"), Required]
        public string AddressLine3 { get; set; }

        [DisplayName("Town"), Required]
        public string Town { get; set; }

        [DisplayName("County"), Required]
        public string County { get; set; }

        [DisplayName("Postcode"), Required]
        [RegularExpression("^[a-zA-Z0-9' ]*$", ErrorMessage = "Only alphanumeric characters and spaces allowed.")]
        public string Postcode { get; set; }

        [DisplayName("Credits"), Required]
        public int Credits { get; set; }
    }
}