namespace ManagementAPI.Models
{
    using System.ComponentModel;

    public class LicensedElement
    {
        [DisplayName("Licensed Element ID")]
        public int LicensedElementId { get; set; }

        [DisplayName("Licensed Element")]
        public string LicensedElementName { get; set; }

        public bool IsChecked { get; set; }
    }
}