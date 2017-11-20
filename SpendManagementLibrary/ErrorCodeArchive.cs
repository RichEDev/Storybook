using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
        /// <summary>
        /// Defines different error code while archiving custom entity
        /// </summary>
        public enum ErrorCodeArchive
        {
            /// <summary>
            /// Error while archiving
            /// </summary>
            ErrorOnArchive = -1,
            /// <summary>
            ///No Access role for archival
            /// </summary>
            NoArchiveAccess = -2,
            /// <summary>
            /// Allow archive flag is not set for greenlight view
            /// </summary>
            AllowArchiveOnViewDisabled = -3
        
    }
}
