namespace EsrGo2FromNhs.Spend_Management
{
    using System.Runtime.Serialization;

    using EsrGo2FromNhs.Base;

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
        public int subAccountId;

        /// <summary>
        /// The string key.
        /// </summary>
        [DataMember]
        public string stringKey;

        /// <summary>
        /// The string value.
        /// </summary>
        [DataMember]
        public string StringValue;
    }
}
