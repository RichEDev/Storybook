using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpendManagementApi.Interfaces
{
    internal interface ISupportsActionContext
    {
        IActionContext ActionContext { get; set; }
    }
}