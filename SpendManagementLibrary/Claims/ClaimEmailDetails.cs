using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary.Claims
{
    /// <summary>
    /// The claim email details.
    /// </summary>
    public class ClaimEmailDetails
    {
        /// <summary>
        /// Employee Id
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// Approver's account Id
        /// </summary>
        public int AccountId { get; set; }
    }
}
