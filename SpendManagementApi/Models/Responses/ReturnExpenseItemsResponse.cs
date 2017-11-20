

using System.Collections.Generic;
using SpendManagementApi.Models.Common;
using SpendManagementApi.Models.Types;

namespace SpendManagementApi.Models.Responses
{

    /// <summary>
    /// 
    /// </summary>
    public class ReturnExpenseItemsResponse : GetApiResponse<ReturnedExpenseItem>
   
    {
        public ReturnExpenseItemsResponse()
        {
            this.List = new List<ReturnedExpenseItem>();
        }
       
    }
}