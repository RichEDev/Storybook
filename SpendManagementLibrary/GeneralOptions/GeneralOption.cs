namespace SpendManagementLibrary.GeneralOptions
{   
    using System;

    [Serializable()]
    public class GeneralOption
    {
        /// <summary>
        /// The subaccountId 
        /// </summary> 
        public int SubaccountId { get; set; }

        /// <summary>
        /// The Key
        /// </summary>     
        public string Key { get; set; }

        /// <summary>
        /// The Value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The Postkey
        /// </summary>
        public string PostKey { get; set; }

        /// <summary>
        /// Specifies if this general option is applicable to all accounts.
        /// </summary>
        public bool IsGlobal { get; set; }


        public GeneralOption(int subaccountId, string key, string value, string postKey, bool isGlobal)
        {
            this.SubaccountId = subaccountId;
            this.Key = key;
            this.Value = value;
            this.PostKey = postKey;
            this.IsGlobal = isGlobal;
        }
    }
}
