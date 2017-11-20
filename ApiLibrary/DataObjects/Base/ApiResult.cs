namespace ApiLibrary.DataObjects.Base
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The API error.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    public class ApiResult 
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ApiResult"/> class.
        /// </summary>
        public ApiResult()
        {
            this.Result = ApiActionResult.NoAction;
            this.Message = string.Empty;
        }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the return value from the stored procedure.
        /// </summary>
        [DataMember]
        public ApiActionResult Result { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether lookup update failure has occured.
        /// </summary>
        [DataMember]
        public bool LookupUpdateFailure { get; set; }
    }
}
