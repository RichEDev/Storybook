using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiCrud.DataClasses
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The ESR phone.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    public class EsrPhone : DataClassBase
    {
        /// <summary>
        /// The ESR phone id.
        /// </summary>
        [DataMember]
        public int EsrPhoneId;
    }
}