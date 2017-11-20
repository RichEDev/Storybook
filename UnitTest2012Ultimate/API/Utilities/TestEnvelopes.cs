namespace UnitTest2012Ultimate.API.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Expedite;
    using SpendManagementLibrary.Interfaces.Expedite;
    using SpendManagementLibrary.Enumerators.Expedite;
    using Syncfusion.XlsIO.Implementation.Collections;

    public class TestEnvelopes : IManageEnvelopes
    {
        public static IList<EnvelopeType> Types;
        public static IList<Envelope> Envelopes;
        public static IList<EnvelopePhysicalState> States;

        public static readonly InvalidDataException EnvelopeErrorDoesntExist = new InvalidDataException("An Envelope with this EnvelopeId was not found.");
        public static readonly InvalidDataException EnvelopeErrorIdIsNotZero = new InvalidDataException("You must set the Id of the envelope to 0 to add it.");
        public static readonly InvalidDataException EnvelopeErrorAttachedToClaim = new InvalidDataException("This Envelope is attached to a claim.");
        public static readonly InvalidDataException EnvelopeErrorIssuedToAccount = new InvalidDataException("An Account with this Id doesn't exist.");
        public static readonly InvalidDataException EnvelopeErrorIssueBatchToAccount = new InvalidDataException("No envelopes with this batch code were found.");
        public static readonly InvalidDataException EnvelopeErrorClaimAttachNoAccount = new InvalidDataException("You must set the AccountId of the envelope to a valid account before attaching to a claim.");
        
        public static readonly InvalidDataException EnvelopeErrorTypeDoesntExist = new InvalidDataException("An EnvelopeStatus with this EnvelopeStatusId was not found.");
        public static readonly InvalidDataException EnvelopeErrorTypeInUse = new InvalidDataException("This EnvelopeType is in use on one of more Envelopes.");
        public static readonly InvalidDataException EnvelopeErrorPhysicalStateInUse = new InvalidDataException("This EnvelopePhysicalState is in use on one of more Envelopes.");



        /// <summary>
        /// The size of the batch during creation. 
        /// Should be 1000 in production.
        /// </summary>
        public uint BatchSize { get; private set; }



        /// <summary>
        /// Creates a new Test Implementation of IManageEnvelopes.
        /// </summary>
        public TestEnvelopes()
        {
            BatchSize = 3;
            ResetData();
        }

        /// <summary>
        /// Resets the data in this Test Class.
        /// </summary>
        public void ResetData()
        {
            States = new List<EnvelopePhysicalState>
            {
                new EnvelopePhysicalState{ EnvelopePhysicalStateId = 1, Details = "Torn Edges" },
                new EnvelopePhysicalState{ EnvelopePhysicalStateId = 2, Details = "Rebagged" },
                new EnvelopePhysicalState{ EnvelopePhysicalStateId = 3, Details = "Has Staples" },
                new EnvelopePhysicalState{ EnvelopePhysicalStateId = 4, Details = "Obscured EnvelopeNumber" },
                new EnvelopePhysicalState{ EnvelopePhysicalStateId = 5, Details = "Obscured CRN" }
            };

            Types = new List<EnvelopeType>
            {
                new EnvelopeType { EnvelopeTypeId = 1, Label = "UK mainland prepaid" },
                new EnvelopeType { EnvelopeTypeId = 2, Label = "UK mainland billable" },
                new EnvelopeType { EnvelopeTypeId = 3, Label = "Ireland prepaid" },
                new EnvelopeType { EnvelopeTypeId = 4, Label = "Ireland billable" },
                new EnvelopeType { EnvelopeTypeId = 5, Label = "France prepaid" },
                new EnvelopeType { EnvelopeTypeId = 6, Label = "France billable" },
                new EnvelopeType { EnvelopeTypeId = 7, Label = "Germany prepaid" },
                new EnvelopeType { EnvelopeTypeId = 8, Label = "Germany billable" },
                new EnvelopeType { EnvelopeTypeId = 9, Label = "Spain prepaid" },
                new EnvelopeType { EnvelopeTypeId = 10, Label = "Spain billable" }
            };
            
            Envelopes = new List<Envelope>
            {
                new Envelope
                    {
                        EnvelopeId = 1,
                        AccountId = null,
                        ClaimId = null,
                        EnvelopeNumber = "A-ABC-001",
                        ClaimReferenceNumber = null,
                        Type = Types[0],
                        Status = EnvelopeStatus.Generated,
                        DateAssignedToClaim = null,
                        DateReceived = null,
                        OverpaymentCharge = null,
                        PhysicalState = new SFArrayList<EnvelopePhysicalState>{ States[0], States[1]},
                        PhysicalStateProofUrl = null
                    },
                    new Envelope
                    {
                        EnvelopeId = 2,
                        AccountId = null,
                        ClaimId = null,
                        EnvelopeNumber = "A-ABC-002",
                        ClaimReferenceNumber = null,
                        Type = Types[1],
                        Status = EnvelopeStatus.Generated,
                        DateAssignedToClaim = null,
                        DateReceived = null,
                        OverpaymentCharge = null,
                        PhysicalState = new SFArrayList<EnvelopePhysicalState>{ States[1], States[2]},
                        PhysicalStateProofUrl = null
                    },
                    new Envelope
                    {
                        EnvelopeId = 2,
                        AccountId = null,
                        ClaimId = null,
                        EnvelopeNumber = "A-ABC-003",
                        ClaimReferenceNumber = null,
                        Type = Types[2],
                        Status = EnvelopeStatus.Generated,
                        DateAssignedToClaim = null,
                        DateReceived = null,
                        OverpaymentCharge = null,
                        PhysicalState = new SFArrayList<EnvelopePhysicalState>{ States[3], States[1]},
                        PhysicalStateProofUrl = null
                    }
            };
        }


        /// <summary>
        /// Gets all envelopes in the system.
        /// </summary>
        /// <returns></returns>
        public IList<Envelope> GetAllEnvelopes()
        {
            return Envelopes;
        }

        /// <summary>
        /// Gets all envelopes with the matching AccountId.
        /// </summary>
        /// <param name="accountId">The AccountId of account to get envelopes for.</param>
        /// <returns>A list of envelopes with the matching AccountId.</returns>
        public IList<Envelope> GetEnvelopesByAccount(int accountId)
        {
            return Envelopes.Where(e => e.AccountId == accountId).ToList();
        }

        /// <summary>
        /// Gets all envelopes with the matching AccountId, which have a sent date older than the supplied date, but no received date.
        /// </summary>
        /// <param name="accountId">The AccountId of account to get envelopes for.</param>
        /// <param name="olderThanWhen">The date that determines whether the envelope is sent but not received within the threshold.</param>
        /// <returns>A list of envelopes with the matching AccountId.</returns>
        public IList<Envelope> GetEnvelopesByAccountWhichAreSentButNotReceived(int accountId, DateTime olderThanWhen)
        {
            return
                GetEnvelopesByAccount(accountId).Where(e =>
                            (e.DateAssignedToClaim != null && e.DateAssignedToClaim < olderThanWhen) 
                            && e.DateReceived == null).ToList();
        }

        /// <summary>
        /// Gets all envelopes with the matching batch (the central part of the EnvelopeNumber).
        /// </summary>
        /// <param name="batchCode"></param>
        /// <returns>A list of envelopes with the matching AccountId.</returns>
        public IList<Envelope> GetEnvelopesByBatch(string batchCode)
        {
            return Envelopes.Where(e => e.EnvelopeNumber.Contains(batchCode)).ToList();
        }

        /// <summary>
        /// Gets an envelope by it's EnvelopeId.
        /// </summary>
        /// <param name="id">The Id of the envelope.</param>
        /// <returns>The envelope with the matching Id.</returns>
        public Envelope GetEnvelopeById(int id)
        {
            return Envelopes.FirstOrDefault(e => e.EnvelopeId == id);
        }

        /// <summary>
        /// Gets an envelope by it's EnvelopeNumber. Note that here the return type is a list
        /// as there is a small chance multiple envelopes with the same EnvelopeNumber will be sent in.
        /// </summary>
        /// <param name="envelopeNumber">The EnvelopeNumber of the envelope.</param>
        /// <returns>The envelope with the matching EnvelopeNumber.</returns>
        public IList<Envelope> GetEnvelopesByEnvelopeNumber(string envelopeNumber)
        {
            return Envelopes.Where(e => e.EnvelopeNumber == envelopeNumber).ToList();
        }

        /// <summary>
        /// Gets all envelopes that have the matching ClaimReferenceNumber.
        /// </summary>
        /// <param name="claimReferenceNumber">The ClaimReferenceNumber of the envelope.</param>
        /// <returns>The envelopes with the matching ClaimReferenceNumber.</returns>
        public IList<Envelope> GetEnvelopesByClaimReferenceNumber(string claimReferenceNumber)
        {
            return Envelopes.Where(e => e.ClaimReferenceNumber == claimReferenceNumber).ToList();
        }

        /// <summary>
        /// Attempts to create the supplied envelope in the system and returns it once created.
        /// Ensure the EnvelopeId is 0 for the envelope or the method will fail.
        /// </summary>
        /// <param name="envelope">The envelope to attempt to create.</param>
        /// <param name="user">The current user.</param>
        /// <returns>The created envelope, with it's EnvelopeId property correctly set.</returns>
        public Envelope AddEnvelope(Envelope envelope, ICurrentUserBase user)
        {
            if (envelope.EnvelopeId != 0)
            {
                throw EnvelopeErrorIdIsNotZero;
            }

            TryGetEnvelopeTypeAndThrow(envelope.Type.EnvelopeTypeId);

            var lastEnvelope = Envelopes.OrderBy(e => e.EnvelopeId).LastOrDefault();
            envelope.EnvelopeId = lastEnvelope == null ? 1 : lastEnvelope.EnvelopeId + 1;
            Envelopes.Add(envelope);
            return envelope;
        }

        /// <summary>
        /// Creates a batch of envelopes in the system and returns the newly create envelopes.
        /// </summary>
        /// <param name="type">The type of the envelope, which should be a valid EnvelopeType.</param>
        /// <param name="user">The current user.</param>
        /// <returns>A list of the newly created envelopes.</returns>
        public IList<Envelope> AddEnvelopeBatch(int type, ICurrentUserBase user)
        {
            var list = new List<Envelope>();
            var typeCount = 0;
            for (var i = 0; i < BatchSize; i++)
            {
                var env = new Envelope
                {
                    EnvelopeId = 0,
                    AccountId = null,
                    ClaimId = null,
                    EnvelopeNumber = "A-ABC-00" + typeCount,
                    ClaimReferenceNumber = null,
                    Status = EnvelopeStatus.Generated,
                    Type = Types[type],
                    DateAssignedToClaim = null,
                    DateReceived = null,
                    OverpaymentCharge = null,
                    PhysicalState = new List<EnvelopePhysicalState>(),
                    PhysicalStateProofUrl = null
                };

                AddEnvelope(env, user);
                typeCount = (typeCount >= Types.Count - 1) ? 0 : typeCount + 1;
            }

            return list;
        }

        /// <summary>
        /// Attempts to create the supplied batch of envelopes in the system and returns them once created.
        /// Ensure the EnvelopeId is 0 for the envelopes or the method will fail.
        /// </summary>
        /// <param name="envelopes">The envelopes to attempt to create.</param>
        /// <param name="user">The current user.</param>
        /// <returns>The created envelopes, with their EnvelopeId property correctly set.</returns>
        public IList<Envelope> AddEnvelopeBatch(IList<Envelope> envelopes, ICurrentUserBase user)
        {
            envelopes.ToList().ForEach(e => AddEnvelope(e, user));
            return envelopes;
        }

        /// <summary>
        /// Edits an envelope. Ensure the EnvelopeId is set correctly.
        /// </summary>
        /// <param name="envelope">The envelope to edit.</param>
        /// <param name="user">The current user.</param>
        /// <returns>The edited envelope.</returns>
        public Envelope EditEnvelope(Envelope envelope, ICurrentUserBase user)
        {
            var target = TryGetEnvelopeAndThrow(envelope.EnvelopeId);

            target.AccountId = envelope.AccountId;
            target.ClaimId = envelope.ClaimId;
            target.ClaimReferenceNumber = envelope.ClaimReferenceNumber;
            target.DateAssignedToClaim = envelope.DateAssignedToClaim;
            target.DateReceived = envelope.DateReceived;
            target.EnvelopeNumber = envelope.EnvelopeNumber;
            target.OverpaymentCharge = envelope.OverpaymentCharge;
            target.PhysicalState = envelope.PhysicalState;
            target.PhysicalStateProofUrl = envelope.PhysicalStateProofUrl;
            target.Status = envelope.Status;
            target.Type = envelope.Type;

            return envelope;
        }



        /// <summary>
        /// Edits a batch of envelopes. Ensure the EnvelopeId is set correctly in each.
        /// </summary>
        /// <param name="envelopes">The envelopes to edit.</param>
        /// <param name="user">The current user.</param>
        /// <returns>The edited envelopes.</returns>
        public IList<Envelope> EditEnvelopeBatch(IList<Envelope> envelopes, ICurrentUserBase user)
        {
            return envelopes.Select(envelope => EditEnvelope(envelope, user)).ToList();
        }

        /// <summary>
        /// Utility method for issuing a single envelope to an account.
        /// </summary>
        /// <param name="envelopeId">The EnvelopeId of the envelope to issue.</param>
        /// <param name="accountId">The AccountId of the account to issue the envelope to.</param>
        /// <param name="user">The current user.</param>
        /// <returns>The newly modified envelope.</returns>
        public Envelope IssueToAccount(int envelopeId, int accountId, ICurrentUserBase user)
        {
            if (accountId == 0 || accountId != GlobalTestVariables.AccountId)
            {
                throw EnvelopeErrorIssuedToAccount;
            }

            var envelope = TryGetEnvelopeAndThrow(envelopeId);

            envelope.AccountId = accountId;
            envelope.DateIssuedToClaimant = DateTime.UtcNow;
            envelope.Status = EnvelopeStatus.IssuedToAccount;

            return envelope;
        }

        /// <summary>
        /// Utility method for issuing multiple envelopes to an account. 
        /// You should specify the initial starting record, and an amount after that to assign.
        /// </summary>
        /// <param name="batchCode">The central part of the EnvelopeNumber (the batch) to assign.</param>
        /// <param name="accountId">The AccountId of the account to issue the envelopes to.</param>
        /// <param name="user">The current user.</param>
        /// <returns>The newly modified envelopes.</returns>
        public IList<Envelope> IssueBatchToAccount(string batchCode, int accountId, ICurrentUserBase user)
        {
            var envelopes = GetEnvelopesByBatch(batchCode).ToList();
            if (envelopes == null || envelopes.Count == 0)
            {
                throw EnvelopeErrorIssueBatchToAccount;
            }

            foreach (var envelope in envelopes)
            {
                IssueToAccount(envelope.EnvelopeId, accountId, user);
            }

            return envelopes;
        }

        /// <summary>
        /// Utility method for attaching a single envelope to a claim.
        /// </summary>
        /// <param name="envelopeId">The EnvelopeId of the envelope to attach.</param>
        /// <param name="claim">The claim to attach the envelope to.</param>
        /// <param name="user">The current user.</param>
        /// <param name="validateOnly">Whether to not go through with the save - and just validate only.</param>
        /// <returns>The newly modified envelope.</returns>
        public Envelope AttachToClaim(int envelopeId, cClaim claim, ICurrentUserBase user, bool validateOnly = false)
        {
            var envelope = TryGetEnvelopeAndThrow(envelopeId);

            if (!envelope.AccountId.HasValue)
            {
                throw EnvelopeErrorClaimAttachNoAccount;
            }

            envelope.ClaimId = claim.claimid;
            envelope.ClaimReferenceNumber = claim.ReferenceNumber;
            envelope.Status = EnvelopeStatus.AttachedToClaim;
            envelope.DateAssignedToClaim = DateTime.UtcNow;

            return envelope;
        }

        /// <summary>
        /// Utility method for detaching a single envelope from a claim.
        /// </summary>
        /// <param name="envelopeId">The EnvelopeId of the envelope to detach.</param>
        /// <param name="claim">The claim to detach the envelope from.</param>
        /// <param name="user">The current user.</param>
        /// <returns>The newly modified envelope.</returns>
        public Envelope DetachFromClaim(int envelopeId, cClaim claim, ICurrentUserBase user)
        {
            var envelope = TryGetEnvelopeAndThrow(envelopeId);

            envelope.ClaimId = null;
            envelope.ClaimReferenceNumber = null;
            envelope.Status = EnvelopeStatus.IssuedToAccount;
            envelope.DateAssignedToClaim = null;

            return envelope;
        }

        /// <summary>
        /// Utility method for marking a single envelope as received by SEL.
        /// </summary>
        /// <param name="id">The EnvelopeId of the envelope to issue.</param>
        /// <param name="user">The current user.</param>
        /// <returns>The newly modified envelope.</returns>
        public Envelope MarkReceived(int id, ICurrentUserBase user)
        {
            var envelope = TryGetEnvelopeAndThrow(id);
            envelope.Status = EnvelopeStatus.ReceivedBySEL;
            envelope.DateReceived = DateTime.UtcNow;
            return envelope;
        }

        /// <summary>
        /// Utility method for marking a single envelope as completed by SEL.
        /// </summary>
        /// <param name="id">The EnvelopeId of the envelope to mark complete.</param>
        /// <param name="user">The current user.</param>
        /// <returns>The newly modified envelope.</returns>
        public Envelope MarkComplete(int id, ICurrentUserBase user)
        {
            var envelope = TryGetEnvelopeAndThrow(id);
            envelope.Status = EnvelopeStatus.ReceiptsAttached;
            return envelope;
        }

        /// <summary>
        /// Utility method for updating the status of a single envelope.
        /// </summary>
        /// <param name="envelopeId">The EnvelopeId of the envelope to change the status of.</param>
        /// <param name="status">The new status of the envelope.</param>
        /// <param name="user">The current user.</param>
        /// <returns>The newly modified envelope.</returns>
        public Envelope UpdateEnvelopeStatus(int envelopeId, EnvelopeStatus status, ICurrentUserBase user)
        {
            var envelope = TryGetEnvelopeAndThrow(envelopeId);
            envelope.Status = status;
            return envelope;
        }
        
        /// <summary>
        /// Deletes an envelope.
        /// </summary>
        /// <param name="envelopeId">The EnvelopeId of the envelope to delete.</param>
        /// <param name="user">The user making the call.</param>
        /// <returns>Whether or not the operation was a success.</returns>
        public bool DeleteEnvelope(int envelopeId, ICurrentUserBase user)
        {
            var envelope = TryGetEnvelopeAndThrow(envelopeId);
            if (envelope.ClaimId != null)
            {
                throw EnvelopeErrorAttachedToClaim;
            }
            Envelopes.Remove(envelope);
            return true;
        }
        
        /// <summary>
        /// Gets all the envelope types in the system.
        /// </summary>
        /// <returns>A list of envelope types.</returns>
        public IList<EnvelopeType> GetAllEnvelopeTypes()
        {
            return Types;
        }

        /// <summary>
        /// Gets a single envelope type by it's Id.
        /// </summary>
        /// <param name="id">The Id of the type to get.</param>
        /// <returns>The envelope type.</returns>
        public EnvelopeType GetEnvelopeTypeById(int id)
        {
            return Types.FirstOrDefault(s => s.EnvelopeTypeId == id);
        }

        /// <summary>
        /// Adds an envelope type to the database.
        /// </summary>
        /// <param name="label">The new type (which will be come the label)</param>
        /// <returns>The new envelope type (with a populated Id).</returns>
        public EnvelopeType AddEnvelopeType(string label)
        {
            var lastType = Types.OrderBy(e => e.EnvelopeTypeId).LastOrDefault();
            var type = new EnvelopeType
            {
                EnvelopeTypeId = lastType == null ? 1 : lastType.EnvelopeTypeId + 1,
                Label = label
            };

            Types.Add(type);
            return type;
        }

        /// <summary>
        /// Edits an existing envelope type in the database.
        /// Be careful with this edit, as it affects the claim history.
        /// </summary>
        /// <param name="id">The Id of the type to modify.</param>
        /// <param name="label">The new type (which will be come the label)</param>
        /// <returns>The new envelope type (with a populated Id).</returns>
        public EnvelopeType EditEnvelopeType(int id, string label)
        {
            var type = TryGetEnvelopeTypeAndThrow(id);

            type.Label = label;
            return type;
        }

        /// <summary>
        /// Deletes an envelopeType. If the type is in use, an error is thrown.
        /// </summary>
        /// <param name="id">The Id of the type to delete.</param>
        /// <returns>Whether the delete succeeded.</returns>
        public bool DeleteEnvelopeType(int id)
        {
            var type = TryGetEnvelopeTypeAndThrow(id);

            if (Envelopes.Any(e => e.Type.EnvelopeTypeId == id))
            {
                throw EnvelopeErrorTypeInUse;
            }

            Types.Remove(type);
            return true;
        }

        /// <summary>
        /// Gets all the envelope PhysicalState in the system.
        /// </summary>
        /// <returns>A list of envelope types.</returns>
        public IList<EnvelopePhysicalState> GetAllEnvelopePhysicalStates()
        {
            return States;
        }

        /// <summary>
        /// Gets a single envelope PhysicalState by it's Id.
        /// </summary>
        /// <param name="id">The Id of the type to get.</param>
        /// <returns>The envelope type.</returns>
        public EnvelopePhysicalState GetEnvelopePhysicalStateById(int id)
        {
            return States.FirstOrDefault(x => x.EnvelopePhysicalStateId == id);
        }

        /// <summary>
        /// Adds an envelope PhysicalState to the database.
        /// </summary>
        /// <param name="details">The new type (which will be come the label)</param>
        /// <returns>The new envelope type (with a populated Id).</returns>
        public EnvelopePhysicalState AddEnvelopePhysicalState(string details)
        {
            var item = new EnvelopePhysicalState {EnvelopePhysicalStateId = States.Last().EnvelopePhysicalStateId + 1, Details = details};
            States.Add(item);
            return item;
        }

        /// <summary>
        /// Edits an existing envelope PhysicalState in the database.
        /// Be careful with this edit, as it affects the claim history.
        /// </summary>
        /// <param name="id">The Id of the EnvelopePhysicalState to modify.</param>
        /// <param name="details">The new EnvelopePhysicalState (which will be come the label)</param>
        /// <returns>The new envelope type (with a populated Id).</returns>
        public EnvelopePhysicalState EditEnvelopePhysicalState(int id, string details)
        {
            var state = TryGetEnvelopePhysicalStateAndThrow(id);
            state.Details = details;
            return state;
        }

        /// <summary>
        /// Updates the Envelope's physical states.
        /// </summary>
        /// <param name="envelopeId">The Envelope's Id.s</param>
        /// <param name="stateIds">The Ids of the Physical states.</param>
        /// <returns></returns>
        public Envelope UpdateEnvelopesPhysicalStates(int envelopeId, List<int> stateIds)
        {
            var envelope = TryGetEnvelopeAndThrow(envelopeId);
            envelope.PhysicalState = States.Where(x => stateIds.Contains(x.EnvelopePhysicalStateId)).ToList();
            return envelope;
        }

        /// <summary>
        /// Deletes a PhysicalState. If the type is in use, an error is thrown.
        /// </summary>
        /// <param name="id">The Id of the EnvelopePhysicalState to delete.</param>
        /// <returns>Whether the delete succeeded.</returns>
        public bool DeleteEnvelopePhysicalState(int id)
        {
            var state = TryGetEnvelopePhysicalStateAndThrow(id);

            if (Envelopes.Any(e => e.PhysicalState.Contains(state)))
            {
                throw EnvelopeErrorPhysicalStateInUse;
            }

            States.Remove(state);
            return true;
        }

        /// <summary>
        /// Gets all the history entries for an envelope.
        /// </summary>
        /// <param name="envelopeId">The Id of the envelope to fetch the history for.</param>
        /// <returns>A list of envelope histories.</returns>
        public IList<EnvelopeHistory> GetEnvelopeHistory(int envelopeId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether all the envelopes for a given CRN are marked as complete.
        /// </summary>
        /// <param name="claimReferenceNumber">The claim reference number. 
        /// This is generated when the claimant attaches an envelope in expenses.</param>
        /// <param name="completedCount">The number of completed envelopes.</param>
        /// <param name="totalCount">The total of count of the envelopes checked.</param>
        /// <param name="alreadyFetched">If you have already fetched the envelopes, pass them in here.</param>
        /// /// <returns>A boolean indicating whether the envelopes are all complete.</returns>
        public bool AreAllEnvelopesCompleteForClaim(string claimReferenceNumber, out int totalCount, out int completedCount, List<Envelope> alreadyFetched = null)
        {
            var envelopes = alreadyFetched ?? Envelopes.Where(e => e.ClaimReferenceNumber == claimReferenceNumber).ToList();
            totalCount = envelopes.Count();
            completedCount = envelopes.Count(e => (e.Status == EnvelopeStatus.ConfirmedSent && e.DeclaredLostInPost)
                                               || (e.Status >= EnvelopeStatus.ReceiptsAttached && e.DateReceived.HasValue));
            return completedCount == totalCount;
        }

        /// <summary>
        /// Attempts to get an Envelope, throwing an error if it doesn't exist.
        /// </summary>
        /// <param name="envelopeId">The EnvelopeId of the Envelope.</param>
        /// <returns>The envelope.</returns>
        /// <exception cref="InvalidDataException">If the EnvelopeId doesn't result in an envelope.</exception>
        private Envelope TryGetEnvelopeAndThrow(int envelopeId)
        {
            var envelope = GetEnvelopeById(envelopeId);
            if (envelope == null)
            {
                throw EnvelopeErrorDoesntExist;
            }
            return envelope;
        }
        
        /// <summary>
        /// Attempts to get an EnvelopeType, throwing an error if it doesn't exist.
        /// </summary>
        /// <param name="envelopeTypeId">The EnvelopeTypeId of the EnvelopeType.</param>
        /// <returns>The envelope type.</returns>
        /// <exception cref="InvalidDataException">If the EnvelopeTypeId doesn't result in an envelope type.</exception>
        private EnvelopeType TryGetEnvelopeTypeAndThrow(int envelopeTypeId)
        {
            var envelopeType = GetEnvelopeTypeById(envelopeTypeId);
            if (envelopeType == null)
            {
                throw EnvelopeErrorTypeDoesntExist;
            }
            return envelopeType;
        }
        
        /// <summary>
        /// Attempts to get an EnvelopePhysicalState, throwing an error if it doesn't exist.
        /// </summary>
        /// <param name="envelopePhysicalStateId">The Id of the EnvelopePhysicalState.</param>
        /// <returns>The EnvelopePhysicalState.</returns>
        /// <exception cref="InvalidDataException">If the Id doesn't result in an EnvelopePhysicalState.</exception>
        private EnvelopePhysicalState TryGetEnvelopePhysicalStateAndThrow(int envelopePhysicalStateId)
        {
            var envelopePhysicalState = GetEnvelopePhysicalStateById(envelopePhysicalStateId);
            if (envelopePhysicalState == null)
            {
                throw EnvelopeErrorTypeDoesntExist;
            }
            return envelopePhysicalState;
        }
    }
}
