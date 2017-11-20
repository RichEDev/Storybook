namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;
    using Common;
    using Types;

    /// <summary>
    /// Reponse with list of subcategories
    /// </summary>
    public class GetNhsTrustsResponse : GetApiResponse<NhsTrust>
    {
        /// <summary>
        /// Creates a new GetExpenseSubCategoriesResponse.
        /// </summary>
        public GetNhsTrustsResponse()
        {
            List = new List<NhsTrust>();
        }
    }

    /// <summary>
    /// Returns the added/ updated expense item
    /// </summary>
    public class NhsTrustResponse : ApiResponse<NhsTrust>
    {
    }
}