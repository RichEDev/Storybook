namespace SpendManagementLibrary.Enumerators.Expedite
{
    using System.ComponentModel.DataAnnotations;
    using SpendManagementLibrary.Expedite;
    
    /// <summary>
    /// Represents a status in the lifecycle of an <see cref="Envelope"/>.
    /// </summary>
    public enum EnvelopeStatus
    {
        /// <summary>
        /// Unknown. 
        /// </summary>
        [Display(Name = @"Envelope status unknown.")]
        Unknown = 0,

        /// <summary>
        /// Envelope Generated.
        /// </summary>
        [Display(Name = @"Envelope generated.")]
        Generated = 1,

        /// <summary>
        /// Envelope sent to the printers to print.
        /// </summary>
        [Display(Name = @"Envelope sent to the printers to print.")]
        SentToPrint = 10,

        /// <summary>
        /// Envelope returned from Printers for Quality Assurance.
        /// </summary>
        [Display(Name = @"Envelope returned from printers for quality assurance.")]
        PrintQA = 11,

        /// <summary>
        /// Envelope has passed printing Quality Assurance.
        /// </summary>
        [Display(Name = @"Envelope has passed printing quality assurance.")]
        PrintQAPassed = 12,

        /// <summary>
        /// Envelope has failed printing Quality Assurance.
        /// </summary>
        [Display(Name = @"Envelope has failed printing quality assurance.")]
        PrintQAFailed = 13,

        /// <summary>
        /// Envelope has been issued to a customer.
        /// </summary>
        [Display(Name = @"Envelope has been issued to a customer.")]
        IssuedToAccount = 20,

        /// <summary>
        /// Envelope has been dispatched to a customer.
        /// </summary>
        [Display(Name = @"Envelope has been dispatched to a customer.")]
        DispatchedToClient = 21,

        /// <summary>
        /// Claimant has attached the EnvelopeNumber to a Claim.
        /// </summary>
        [Display(Name = @"Claimant has attached the envelope number to a claim.")]
        AttachedToClaim = 22,

        /// <summary>
        /// Envelope successfully returned to SEL.
        /// </summary>
        [Display(Name = @"Envelope successfully returned to SEL.")]
        ReceivedBySEL = 23,

        /// <summary>
        /// Envelope has not been returned, and could be lost in the post.
        /// </summary>
        [Display(Name = @"Envelope has not been returned, and could be lost in the post.")]
        UnconfirmedNotSent = 24,

        /// <summary>
        /// User has confirmed that the envelope was returned, although we don't have it...
        /// </summary>
        [Display(Name = @"User has confirmed that the envelope was returned, although we don't have it.")]
        ConfirmedSent = 25,
        
        /// <summary>
        /// Envelope has been scanned.
        /// </summary>
        [Display(Name = @"Envelope has been scanned.")]
        Scanned = 30,

        /// <summary>
        /// The envelope has been scanned and is attached to an account, pending admin reassignment.
        /// </summary>
        [Display(Name = @"The envelope has been scanned and is attached to an account, pending admin reassignment.")]
        PendingAdminReassignment = 32,

        /// <summary>
        /// Envelope's contents have been attached to a Claim, Claim Line, or User.
        /// </summary>
        [Display(Name = @"Envelope's contents have been attached to a claim, claim line, or user.")]
        ReceiptsAttached = 40,

        /// <summary>
        /// Validation of Envelope completed.
        /// </summary>
        [Display(Name = @"Validation of envelope completed.")]
        Validated = 50,

        /// <summary>
        /// Envelope is ready to be put into storage.
        /// </summary>
        [Display(Name = @"Envelope is ready to be put into storage.")]
        ReadyForStorage = 60,

        /// <summary>
        /// Envelope is currently in storage.
        /// </summary>
        [Display(Name = @"Envelope is currently in storage.")]
        InStorage = 61,

        /// <summary>
        /// Envelope has been marked for destruction.
        /// </summary>
        [Display(Name = @"Envelope has been marked for destruction.")]
        ReadyForDestruction = 70,

        /// <summary>
        /// Envelope has been destroyed.
        /// </summary>
        [Display(Name = @"Envelope has been destroyed.")]
        Destroyed = 71

    }
}
