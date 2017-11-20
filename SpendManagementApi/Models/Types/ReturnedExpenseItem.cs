using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpendManagementApi.Models.Types
{
    public class ReturnedExpenseItem : BaseExternalType
    {
        /// <summary>

        /// </summary>
        public int ExpenseId { get; set; }

        /// <summary>

        /// </summary>
        public bool Outcome { get; set; }
    }
}