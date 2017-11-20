namespace EsrGo2FromNhsWcfLibrary.Spend_Management
{
    using System.Runtime.Serialization;

    using EsrGo2FromNhsWcfLibrary.Base;

    /// <summary>
    /// The account properties.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    public class AccountProperty : DataClassBase
    {
        /// <summary>
        /// The sub account id.
        /// </summary>
        [DataMember]
        public int SubAccountId;

        /// <summary>
        /// The string key.
        /// </summary>
        [DataMember]
        public string StringKey;

        /// <summary>
        /// The string value.
        /// </summary>
        [DataMember]
        public string StringValue;
    }
}
