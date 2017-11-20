using SpendManagementLibrary.Enumerators;

namespace SpendManagementLibrary
{
    using System.Collections.Generic;

    using SpendManagementLibrary.UserDefinedFields;

    /// <summary>
    /// A basic definition of the claims object.
    /// </summary>
    public class ClaimDefinition
    {
        /// <summary>
        /// The id number of the claim.
        /// </summary>
        public int ClaimId { get; set; }

        /// <summary>
        /// The name of the claim.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A description of the claim.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// A list of <see cref="UserDefinedFieldValue">user defined fields</see>./>
        /// </summary>
        public List<UserDefinedFieldValue> UserDefinedFields { get; set; }

        /// <summary>
        /// The result of a <see cref="ClaimDefinition">ClaimDefinition</see> action
        /// </summary>
        public ClaimDefinitionOutcome ClaimDefinitionOutcome { get; set; }
    }
}
