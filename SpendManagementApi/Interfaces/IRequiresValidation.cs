using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpendManagementApi.Interfaces
{
    internal interface IRequiresValidation
    {
        void Validate(IActionContext actionContext);
    }
}