namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;
    using Common;
    using Types;

    /// <summary>
    /// A response containing a list of <see cref="Claim">Claim</see>s.
    /// </summary>
    public class GetClaimsResponse : GetApiResponse<Claim>
    {
        /// <summary>
        /// Creates a new GetClaimsResponse.
        /// </summary>
        public GetClaimsResponse()
        {
            List = new List<Claim>();
        }
    }

    /// <summary>
    /// A response containing a particular <see cref="Claim">Claim</see>.
    /// </summary>
    public class ClaimResponse : ApiResponse<Claim>
    {

    }

    /// <summary>
    /// A response containing an integer.
    /// </summary>
    public class ClaimNumericResponse : ApiResponse<int>
    {

    }

    /// <summary>
    /// A response containing an integer.
    /// </summary>
    public class ClaimBasicResponse : GetApiResponse<ClaimBasic>
    {
        /// <summary>
        /// 
        /// </summary>
        public ClaimBasicResponse()
        {
            List = new List<ClaimBasic>();
        }
    }

    /// <summary>
    /// A response containing an claimResponse.
    /// </summary>
    public class ClaimBasicItemResponse : ApiResponse<ClaimBasic>
    {

    
    }

    /// <summary>
    /// A response containing a string of HTML.
    /// </summary>
    public class GetApproverResponse : ApiResponse<string>
    {

    }
}
