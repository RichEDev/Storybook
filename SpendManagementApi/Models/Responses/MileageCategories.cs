using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpendManagementApi.Models.Common;
using SpendManagementApi.Models.Types;

namespace SpendManagementApi.Models.Responses
{
    /// <summary>
    /// Get Mileage Categories Response
    /// </summary>
    public class GetMileageCategoriesResponse : GetApiResponse<MileageCategory>
    {
    }

    /// <summary>
    /// Find Mileage Categories Response
    /// </summary>
    public class FindMileageCategoriesResponse : GetMileageCategoriesResponse
    {
        
    }

    /// <summary>
    /// Mileage Category Response
    /// </summary>
    public class MileageCategoryResponse : ApiResponse<MileageCategory>
    {   
    }

    /// <summary>
    /// Mileage Category Basic Response
    /// </summary>
    public class MileageCategoryBasicResponse : ApiResponse
    {
        /// <summary>
        /// Gets or sets the list of <see cref="MileageCategoryBasic">MileageCategoryBasic</see>
        /// </summary>
        public List<MileageCategoryBasic> List { get; set; }
    }
}