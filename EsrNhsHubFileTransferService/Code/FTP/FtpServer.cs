namespace EsrNhsHubFileTransferService.Code.FTP
{
    /// <summary>
    /// The class to hold information from the ftpLocations table 
    /// </summary>
    public class FtpServer
    {
        private string password;

        private string username;

        #region Public Properties

        public int FtpLocationId { get; set; }

        public string Hostname { get; set; }

        public string Password
        {
            get
            {
                var pass = "Password Not Decrypted";
                try
                {
                    pass = Utilities.Cryptography.ExpensesCryptography.Decrypt(this.password);
                }
                finally {}
                return pass;
            }
            set
            {
                this.password = value;
            }
        }

        public string Path { get; set; }

        public string Username
        {
            get
            {
                var user = "Username Not Decrypted";
                try
                {
                    user = Utilities.Cryptography.ExpensesCryptography.Decrypt(this.username);
                }
                finally {}
                return user;
            }
            set
            {
                this.username = value;
            }
        }

        #endregion
    }
}