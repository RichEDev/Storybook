
namespace ApiCrud.DataClasses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Web;

    /// <summary>
    /// The ESR person.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    public class EsrPerson : DataClassBase
    {
        /// <summary>
        /// The ESR first name.
        /// </summary>
        [DataMember]
        public string esrFirstName;
    }
}