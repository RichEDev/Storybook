
namespace ApiCrud.DataClasses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Web;

    /// <summary>
    /// The API action result.
    /// </summary>
    public enum ApiResult
    {
        /// <summary>
        /// The success return code.
        /// </summary>
        [EnumMember]
        Success = 0,

        /// <summary>
        /// The failure return code.
        /// </summary>
        [EnumMember]
        Failure = 1,

        /// <summary>
        /// The no action return code - no action taken.
        /// </summary>
        [EnumMember]
        NoAction = -1,

        /// <summary>
        /// The deleted return code.
        /// </summary>
        [EnumMember]
        Deleted = 2
    }

    /// <summary>
    /// The data class base.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    public partial class DataClassBase
    {
        /// <summary>
        /// The API action result.
        /// </summary>
        [DataMember]
        public ApiResult ActionResult;

        /// <summary>
        /// The uri of the data entity.
        /// </summary>
        [DataMember]
        public string Uri;
    }
}