namespace SpendManagementLibrary.Mobile
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Holds the information for a mobile device pairing key
    /// </summary>
    public class PairingKey
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="PairingKey"/> class. 
        /// PairingKey constructor
        /// </summary>
        /// <param name="pairingKey">
        /// Pairing key to decode
        /// </param>
        public PairingKey(string pairingKey)
        {
            if (string.IsNullOrWhiteSpace(pairingKey))
            {
                this.PairingKeyValid = false;
                return;
            }

            this.Pairingkey = pairingKey;
            Regex regex = new Regex("^[0-9]{5}-[0-9]{5}-[0-9]{6}$", RegexOptions.Compiled);
            if (regex.IsMatch(pairingKey))
            {
                string[] splitParts = pairingKey.Split('-');
                this.AccountID = int.Parse(splitParts[0]);
                this.EmployeeID = int.Parse(splitParts[2]);
                this.Epoch5 = splitParts[1];
                this.PairingKeyValid = true;
            }
            else
            {
                this.PairingKeyValid = false;
            }
        }

        #region Properties

        /// <summary>
        /// Gets or sets the Account ID 
        /// </summary>
        public int AccountID { get; set; }

        /// <summary>
        /// Gets or sets EmployeeID
        /// </summary>
        public int EmployeeID { get; set; }

        /// <summary>
        /// Gets or sets Epoch
        /// </summary>
        public string Epoch5 { get; set; }

        /// <summary>
        /// Gets or sets Pairing key
        /// </summary>
        public string Pairingkey { get; set; }

        /// <summary>
        /// Gets or sets a boolean which indicates whether the paired key is a valid format or not
        /// </summary>
        public bool PairingKeyValid { get; set; }

        #endregion
    }

    /// <summary>
    /// Object to hold the pairing key and serial key combination passed in the mobile WebAPI methods
    /// </summary>
    public class PairingKeySerialKey
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="PairingKeySerialKey" /> class.
        /// </summary>
        /// <param name="pairingkey">The pairing key</param>
        /// <param name="serialkey">The serial key</param>
        public PairingKeySerialKey(PairingKey pairingkey, string serialkey)
        {
            this.PairingKey = pairingkey;
            this.SerialKey = serialkey;
        }

        /// <summary>
        /// Gets or sets the serial key
        /// </summary>
        public string SerialKey { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PairingKey"/>
        /// </summary>
        public PairingKey PairingKey { get; set; }
    }
}
