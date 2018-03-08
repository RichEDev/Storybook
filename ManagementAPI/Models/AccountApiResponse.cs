namespace ManagementAPI.Models
{
    using System;
    using System.Collections.Generic;

    public class AccountApiResponse
    {
        public Account Account { get; set; }

        public List<Account> Accounts { get; set; }

        public int Id { get; set; }

        public bool Success { get; set; }

        public Exception Error { get; set; }
    }
}