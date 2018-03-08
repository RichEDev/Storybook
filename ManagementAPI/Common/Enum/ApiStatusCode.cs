namespace ManagementAPI.Common.Enum
{
        /// <summary>
        /// Represents the result of an operation in the API.
        /// </summary>
        public enum ApiStatusCode
        {
            /// <summary>
            /// Failure.
            /// </summary>
            Failure = 0,

            /// <summary>
            /// Success.
            /// </summary>
            Success = 1,

            /// <summary>
            /// Partial success with warnings.
            /// </summary>
            PartialSuccessWithWarnings = 2,

            /// <summary>
            /// Internal error.
            /// </summary>
            InternalError = 1000,

            /// <summary>
            /// Generic error.
            /// </summary>
            GenericError = 1001,

            /// <summary>
            /// Validation error.
            /// </summary>
            ValidationError = 1002,
        }
    }