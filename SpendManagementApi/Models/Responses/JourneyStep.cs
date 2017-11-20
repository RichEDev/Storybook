namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;

    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Types;

    /// <summary>
    /// A response containing a particular <see cref="JourneyStep">Journey step</see>.
    /// </summary>
    public class JourneyStepResponse : ApiResponse<JourneyStep>
    {

    }

    /// <summary>
    /// A response containing a list of <see cref="JourneyStep">Journey steps</see>s.
    /// </summary>
    public class JourneyStepsResponse : GetApiResponse<JourneyStep>
    {
        /// <summary>
        /// Creates a new JourneyStepsResponse.
        /// </summary>
        public JourneyStepsResponse()
        {
            List = new List<JourneyStep>();
        }
    }
}