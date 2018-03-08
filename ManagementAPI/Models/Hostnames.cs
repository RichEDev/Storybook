namespace ManagementAPI.Models
{
    using System.ComponentModel;

    public class Hostname
    {
        [DisplayName("Hostname ID")]
        public int HostnameId { get; set; }

        [DisplayName("Hostname")]
        public string HostName { get; set; }

        [DisplayName("Module ID")]
        public int ModuleId { get; set; }

        public bool IsChecked { get; set; }
    }
}