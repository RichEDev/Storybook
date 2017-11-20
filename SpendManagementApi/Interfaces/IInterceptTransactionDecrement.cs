using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpendManagementApi.Interfaces
{
    /// <summary>
    /// Interface indicating that the controller intercepts the transaction count
    /// </summary>
    public interface IInterceptTransactionDecrement
    {
        int TransactionCount { get; }
    }
}