using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary.Enumerators
{
    public class AccountTypes
    {
        /// <summary>
        /// The account type Enumerations.
        /// </summary>
        public enum AccountType
        {
            /// <summary>
            /// No account type
            /// </summary>
            None=0,
            /// <summary>
            /// Savings
            /// </summary>
            Savings=1,
            /// <summary>
            /// Current
            /// </summary>
            Current=2,
            /// <summary>
            /// Credit Card
            /// </summary>
            CreditCard=3
        }

    }
}
