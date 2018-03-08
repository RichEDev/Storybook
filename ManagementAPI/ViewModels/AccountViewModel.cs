namespace ManagementAPI.ViewModels
{
    using ManagementAPI.Models;
    using System.Collections.Generic;

    public class AccountViewModel
    {
        public string Username;

        public Account Account { get; set; }

        public List<Hostname> Hostnames { get; set; }
        public List<int> SelectedHostnameIDs { get; set; }

        public List<LicensedElement> LicensedElements { get; set; }
        public List<int> SelectedLicensedElementIDs { get; set; }

        public List<DatabaseServer> DatabaseServers { get; set; }
    }
}