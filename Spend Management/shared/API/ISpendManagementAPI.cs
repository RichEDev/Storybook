using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using SpendManagementLibrary;
using System.Web.Script.Serialization;

namespace Spend_Management
{
    /// <summary>
    /// Interface for the Spend Management API
    /// </summary>
    [ServiceContract]
    public interface ISpendManagementAPI
    {
        [OperationContract]
        string DoWork();

        [OperationContract]
        Dictionary<int, string> GetEmployeeInfo(string companyID);
    }
}
