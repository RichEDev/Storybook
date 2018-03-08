namespace ManagementAPI.Models
{
    public class HostnameLicensedElementBundle
    {
        public int ElementId { get; set; } // hostname or licensed element

        public int AccountId { get; set; }

        public string Username { get; set; }
    }
}