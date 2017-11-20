using System.Collections.Generic;
using SpendManagementApi.Models.Common;
using SpendManagementApi.Models.Types;

namespace SpendManagementApi.Models.Responses
{
    public class GetCountriesResponse : GetApiResponse<Country>
    {   
    }

    public class FindCountriesResponse : GetCountriesResponse
    {
        
    }

    public class CountryResponse : ApiResponse<Country>
    {
    }
}