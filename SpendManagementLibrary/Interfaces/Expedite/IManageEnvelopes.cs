namespace SpendManagementLibrary.Interfaces.Expedite
{
    using System;
    using System.Collections.Generic;
    using SpendManagementLibrary.Expedite;
    using Enumerators.Expedite;

    public interface IManageEnvelopes
    {
        /// <summary>
        /// The size of the batch during creation. 
        /// Should be 1000 in production.
        /// </summary>
        uint BatchSize { get; }

        #region Direct Envelope Management

        /// <summary>
        /// Gets all envelopes in the system.
        /// </summary>
        /// <returns></returns>
        IList<Envelope> GetAllEnvelopes();

        /// <summary>
        /// Gets all envelopes with the matching AccountId.
        /// </summary>
        /// <param name="accountId">The AccountId of account to get envelopes for.</param>
        /// <returns>A list of envelopes with the matching AccountId.</returns>
        IList<Envelope> GetEnvelopesByAccount(int accountId);

        /// <summary>
        /// Gets all envelopes with the matching AccountId, which have a sent date older than the supplied date, but no received date.
        /// </summary>
        /// <param name="accountId">The AccountId of account to get envelopes for.</param>
        /// <param name="olderThanWhen">The date that determines whether the envelope is sent but not received within the threshold.</param>
        /// <returns>A list of envelopes with the matching AccountId.</returns>
        IList<Envelope> GetEnvelopesByAccountWhichAreSentButNotReceived(int accountId, DateTime olderThanWhen);

        /// <summary>
        /// Gets all envelopes with the matching batch (the central part of the EnvelopeNumber).
        /// </summary>
        /// <param name="batchCode"></param>
        /// <returns>A list of envelopes with the matching AccountId.</returns>
        IList<Envelope> GetEnvelopesByBatch(string batchCode);

        /// <summary>
        /// Gets an envelope by it's EnvelopeId.
        /// </summary>
        /// <param name="id">The Id of the envelope.</param>
        /// <returns>The envelope with the matching Id.</returns>
        Envelope GetEnvelopeById(int id);

        /// <summary>
        /// Gets an envelope by it's EnvelopeNumber. Note that here the return type is a list
        /// as there is a small chance multiple envelopes with the same EnvelopeNumber will be sent in.
        /// </summary>
        /// <param name="envelopeNumber">The EnvelopeNumber of the envelope.</param>
        /// <returns>The envelope with the matching EnvelopeNumber.</returns>
        IList<Envelope> GetEnvelopesByEnvelopeNumber(string envelopeNumber);

        /// <summary>
        /// Gets all envelopes that have the matching ClaimReferenceNumber.
        /// </summary>
        /// <param name="claimReferenceNumber">The ClaimReferenceNumber of the envelope.</param>
        /// <returns>The envelopes with the matching ClaimReferenceNumber.</returns>
        IList<Envelope> GetEnvelopesByClaimReferenceNumber(string claimReferenceNumber);

        /// <summary>
        /// Attempts to create the supplied envelope in the system and returns it once created.
        /// Ensure the EnvelopeId is 0 for the envelope or the method will fail.
        /// </summary>
        /// <param name="envelope">The envelope to attempt to create.</param>
        /// <param name="user">The current user.</param>
        /// <returns>The created envelope, with it's EnvelopeId property correctly set.</returns>
        Envelope AddEnvelope(Envelope envelope, ICurrentUserBase user);

        /// <summary>
        /// Creates a batch of envelopes in the system and returns the newly create envelopes.
        /// </summary>    
        /// <param name="type">The type of the envelope, which should be a valid EnvelopeType.</param>
        /// <param name="user">The current user.</param>
        /// <returns>A list of the newly created envelopes.</returns>
        IList<Envelope> AddEnvelopeBatch(int type, ICurrentUserBase user);

        /// <summary>
        /// Attempts to create the supplied batch of envelopes in the system and returns them once created.
        /// Ensure the EnvelopeId is 0 for the envelopes or the method will fail.
        /// </summary>
        /// <param name="envelopes">The envelopes to attempt to create.</param>
        /// <param name="user">The current user.</param>
        /// <returns>The created envelopes, with their EnvelopeId property correctly set.</returns>
        IList<Envelope> AddEnvelopeBatch(IList<Envelope> envelopes, ICurrentUserBase user);

        /// <summary>
        /// Edits an envelope. Ensure the EnvelopeId is set correctly.
        /// </summary>
        /// <param name="envelope">The envelope to edit.</param>
        /// <param name="user">The current user.</param>
        /// <returns>The edited envelope.</returns>
        Envelope EditEnvelope(Envelope envelope, ICurrentUserBase user);

        /// <summary>
        /// Edits a batch of envelopes. Ensure the EnvelopeId is set correctly in each.
        /// </summary>
        /// <param name="envelopes">The envelopes to edit.</param>
        /// <param name="user">The current user.</param>
        /// <returns>The edited envelopes.</returns>
        IList<Envelope> EditEnvelopeBatch(IList<Envelope> envelopes, ICurrentUserBase user);

        /// <summary>
        /// Utility method for issuing a single envelope to an account.
        /// </summary>
        /// <param name="envelopeId">The EnvelopeId of the envelope to issue.</param>
        /// <param name="accountId">The AccountId of the account to issue the envelope to.</param>
        /// <param name="user">The current user.</param>
        /// <returns>The newly modified envelope.</returns>
        Envelope IssueToAccount(int envelopeId, int accountId, ICurrentUserBase user);

        /// <summary>
        /// Utility method for issuing multiple envelopes to an account. 
        /// You should specify the initial starting record, and an amount after that to assign.
        /// </summary>
        /// <param name="batchCode">The central part of the EnvelopeNumber (the batch) to assign.</param>
        /// <param name="accountId">The AccountId of the account to issue the envelopes to.</param>
        /// <param name="user">The current user.</param>
        /// <returns>The newly modified envelopes.</returns>
        IList<Envelope> IssueBatchToAccount(string batchCode, int accountId, ICurrentUserBase user);

        /// <summary>
        /// Utility method for attaching a single envelope to a claim, with the option to only validate that it can be attached.
        /// </summary>
        /// <param name="envelopeId">The EnvelopeId of the envelope to attach.</param>
        /// <param name="claim">The claim to attach the envelope to.</param>
        /// <param name="user">The current user.</param>
        /// <param name="validateOnly">Whether to not go through with the save - and just validate only.</param>
        /// <returns>The newly modified envelope.</returns>
        Envelope AttachToClaim(int envelopeId, cClaim claim, ICurrentUserBase user, bool validateOnly = false);

        /// <summary>
        /// Utility method for detaching a single envelope from a claim.
        /// </summary>
        /// <param name="envelopeId">The EnvelopeId of the envelope to detach.</param>
        /// <param name="claim">The claim to detach the envelope from.</param>
        /// <param name="user">The current user.</param>
        /// <returns>The newly modified envelope.</returns>
        Envelope DetachFromClaim(int envelopeId, cClaim claim, ICurrentUserBase user);

        /// <summary>
        /// Utility method for marking a single envelope as received by SEL.
        /// </summary>
        /// <param name="id">The EnvelopeId of the envelope to mark received.</param>
        /// <param name="user">The current user.</param>
        /// <returns>The newly modified envelope.</returns>
        Envelope MarkReceived(int id, ICurrentUserBase user);

        /// <summary>
        /// Utility method for marking a single envelope as completed by SEL.
        /// </summary>
        /// <param name="id">The EnvelopeId of the envelope to mark complete.</param>
        /// <param name="user">The current user.</param>
        /// <returns>The newly modified envelope.</returns>
        Envelope MarkComplete(int id, ICurrentUserBase user);

        /// <summary>
        /// Utility method for updating the status of a single envelope.
        /// </summary>
        /// <param name="envelopeId">The EnvelopeId of the envelope to change the status of.</param>
        /// <param name="status">The new status of the envelope.</param>
        /// <param name="user">The current user.</param>
        /// <returns>The newly modified envelope.</returns>
        Envelope UpdateEnvelopeStatus(int envelopeId, EnvelopeStatus status, ICurrentUserBase user);

        /// <summary>
        /// Deletes an envelope.
        /// </summary>
        /// <param name="envelopeId">The EnvelopeId of the envelope to delete.</param>
        /// <param name="user">The user making the call.</param>
        /// <returns>Whether or not the operation was a success.</returns>
        bool DeleteEnvelope(int envelopeId, ICurrentUserBase user);

        #endregion Direct Envelope Management

        #region Envelope Type Management

        /// <summary>
        /// Gets all the envelope types in the system.
        /// </summary>
        /// <returns>A list of envelope types.</returns>
        IList<EnvelopeType> GetAllEnvelopeTypes();

        /// <summary>
        /// Gets a single envelope type by it's Id.
        /// </summary>
        /// <param name="id">The Id of the type to get.</param>
        /// <returns>The envelope type.</returns>
        EnvelopeType GetEnvelopeTypeById(int id);

        /// <summary>
        /// Adds an envelope type to the database.
        /// </summary>
        /// <param name="label">The new type (which will be come the label)</param>
        /// <returns>The new envelope type (with a populated Id).</returns>
        EnvelopeType AddEnvelopeType(string label);

        /// <summary>
        /// Edits an existing envelope type in the database.
        /// Be careful with this edit, as it affects the claim history.
        /// </summary>
        /// <param name="id">The Id of the type to modify.</param>
        /// <param name="label">The new type (which will be come the label)</param>
        /// <returns>The new envelope type (with a populated Id).</returns>
        EnvelopeType EditEnvelopeType(int id, string label);

        /// <summary>
        /// Deletes an envelopeType. If the type is in use, an error is thrown.
        /// </summary>
        /// <param name="id">The Id of the type to delete.</param>
        /// <returns>Whether the delete succeeded.</returns>
        bool DeleteEnvelopeType(int id);

        #endregion Envelope Type Management

        #region Envelope Physical State Management

        /// <summary>
        /// Gets all the envelope PhysicalState in the system.
        /// </summary>
        /// <returns>A list of envelope types.</returns>
        IList<EnvelopePhysicalState> GetAllEnvelopePhysicalStates();

        /// <summary>
        /// Gets a single envelope PhysicalState by it's Id.
        /// </summary>
        /// <param name="id">The Id of the type to get.</param>
        /// <returns>The envelope type.</returns>
        EnvelopePhysicalState GetEnvelopePhysicalStateById(int id);

        /// <summary>
        /// Adds an envelope PhysicalState to the database.
        /// </summary>
        /// <param name="details">The new type (which will be come the label)</param>
        /// <returns>The new envelope type (with a populated Id).</returns>
        EnvelopePhysicalState AddEnvelopePhysicalState(string details);

        /// <summary>
        /// Edits an existing envelope PhysicalState in the database.
        /// Be careful with this edit, as it affects the claim history.
        /// </summary>
        /// <param name="id">The Id of the EnvelopePhysicalState to modify.</param>
        /// <param name="details">The new EnvelopePhysicalState (which will be come the label)</param>
        /// <returns>The new envelope type (with a populated Id).</returns>
        EnvelopePhysicalState EditEnvelopePhysicalState(int id, string details);

        /// <summary>
        /// Updates the Envelope's physical states.
        /// </summary>
        /// <param name="envelopeId">The Envelope's Id.s</param>
        /// <param name="stateIds">The Ids of the Physical states.</param>
        /// <returns>The updated Envelope.</returns>
        Envelope UpdateEnvelopesPhysicalStates(int envelopeId, List<int> stateIds);

        /// <summary>
        /// Deletes a PhysicalState. If the type is in use, an error is thrown.
        /// </summary>
        /// <param name="id">The Id of the EnvelopePhysicalState to delete.</param>
        /// <returns>Whether the delete succeeded.</returns>
        bool DeleteEnvelopePhysicalState(int id);

        #endregion Envelope Physical State Management
        
        #region Envelope History Management

        /// <summary>
        /// Gets all the history entries for an envelope.
        /// </summary>
        /// <param name="envelopeId">The Id of the envelope to fetch the history for.</param>
        /// <returns>A list of envelope histories.</returns>
        IList<EnvelopeHistory> GetEnvelopeHistory(int envelopeId);

        #endregion Envelope History Management
        
        #region Utilities

        /// <summary>
        /// Checks whether all the envelopes for a given CRN are marked as complete. 
        /// If you pass in a list of envelopes, these will be checked; if not the DB is hit.
        /// </summary>
        /// <param name="claimReferenceNumber">The claim reference number. 
        /// This is generated when the claimant attaches an envelope in expenses.</param>
        /// <param name="completedCount">The number of completed envelopes.</param>
        /// <param name="totalCount">The total of count of the envelopes checked.</param>
        /// <param name="alreadyFetched">If you have already fetched the envelopes, pass them in here.</param>
        /// <returns>A boolean indicating whether the envelopes are all complete.</returns>
        bool AreAllEnvelopesCompleteForClaim(string claimReferenceNumber, out int totalCount, out int completedCount, List<Envelope> alreadyFetched = null);

        #endregion Utilities
    }
}
