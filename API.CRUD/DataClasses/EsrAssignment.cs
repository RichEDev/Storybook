namespace ApiCrud.DataClasses
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The ESR assignments Data Class.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    public class EsrAssignment : DataClassBase
    {
        /// <summary>
        /// The ESR assignment number.
        /// </summary>
        [DataMember]
        public int EsrAssignmentNumber;
    }
}