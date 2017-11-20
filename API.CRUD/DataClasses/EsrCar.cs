namespace ApiCrud.DataClasses
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The ESR cars Data Class.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    public class EsrCar : DataClassBase
    {
        /// <summary>
        /// The ESR cars id.
        /// </summary>
        [DataMember]
        public int EsrCarsId;
    }
}