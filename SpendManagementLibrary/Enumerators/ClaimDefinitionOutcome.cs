namespace SpendManagementLibrary.Enumerators
{ 
    /// <summary>
    /// The result of a<see cref="ClaimDefinition"> ClaimDefinition</see> action
    /// </summary>
    public enum ClaimDefinitionOutcome

    {
        /// <summary>
        /// The account only permits one active claim at a time
        /// </summary>
        SingleClaimOnly = 0,

        /// <summary>
        /// A duplicate claim name exists for the employee
        /// </summary>
        DuplicateClaimName = 1,

        /// <summary>
        /// The action on the claim definition was a success
        /// </summary>
        Success = 2,

        /// <summary>
        /// The action on the claim definition was a failure
        /// </summary>
        Fail = 3
    }
}

