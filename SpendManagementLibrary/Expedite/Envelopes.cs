namespace SpendManagementLibrary.Expedite
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.IO;
    using System.Linq;
    using Enumerators.Expedite;
    using Helpers;
    using Interfaces.Expedite;

    public class Envelopes : IManageEnvelopes
    {
        #region Private Constants / Column Names / Param Keys / SQL

        private const string ColumnNameEnvelopeId = "EnvelopeId";
        private const string ColumnNameEnvelopeAccountId = "AccountId";
        private const string ColumnNameEnvelopeClaimId = "ClaimId";
        private const string ColumnNameEnvelopeNumber = "EnvelopeNumber";
        private const string ColumnNameEnvelopeCrn = "CRN";
        private const string ColumnNameEnvelopeStatus = "EnvelopeStatus";
        private const string ColumnNameEnvelopeType = "EnvelopeType";
        private const string ColumnNameEnvelopeDateIssued = "DateIssuedToClaimant";
        private const string ColumnNameEnvelopeDateAssigned = "DateAssignedToClaim";
        private const string ColumnNameEnvelopeDateReceived = "DateReceived";
        private const string ColumnNameEnvelopeDateAttached = "DateAttachCompleted";
        private const string ColumnNameEnvelopeDateDestroyed = "DateDestroyed";
        private const string ColumnNameEnvelopeOverpayment = "OverpaymentCharge";
        private const string ColumnNameEnvelopePhysicalStateUrl = "PhysicalStateProofUrl";
        private const string ColumnNameEnvelopeLastModifiedBy = "LastModifiedBy";
        private const string ColumnNameEnvelopeDeclaredLostInPost = "DeclaredLostInPost";
        private const string ColumnNameTypeId = "EnvelopeTypeId";
        private const string ColumnNameTypeLabel = "Label";
        private const string ColumnNameId = "Id";
        private const string ColumnNamePhysicalStateId = "EnvelopePhysicalStateId";
        private const string ColumnNamePhysicalStateDetails = "Details";


        private const string ParamEnvelopeId = "@envelopeId";
        private const string ParamEnvelopeAccountId = "@accountId";
        private const string ParamEnvelopeClaimId = "@claimId";
        private const string ParamEnvelopeNumber = "@envelopeNumber";
        private const string ParamEnvelopeCrn = "@crn";
        private const string ParamEnvelopeStatus = "@envelopeStatus";
        private const string ParamEnvelopeType = "@envelopeType";
        private const string ParamEnvelopeDateIssued = "@dateIssuedToClaimant";
        private const string ParamEnvelopeDateAssigned = "@dateAssignedToClaim";
        private const string ParamEnvelopeDateReceived = "@dateReceived";
        private const string ParamEnvelopeDateAttached = "@dateAttachCompleted";
        private const string ParamEnvelopeDateDestroyed = "@dateDestroyed";
        private const string ParamEnvelopeOverpayment = "@overpaymentCharge";
        private const string ParamEnvelopePhysicalStateUrl = "@physicalStateProofUrl";
        private const string ParamEnvelopeLastModifiedBy = "@lastModifiedBy";
        private const string ParamEnvelopeDeclaredLostInPost = "@declaredLostInPost";
        private const string ParamEnvelopes = "@envelopes";
        private const string ParamId = "@id";
        private const string ParamLabel = "@label";
        private const string ParamPhysicalStateDetails = "@details";
        private const string ParamPhysicalStateIds = "@physicalStateIds";


        private const string StoredProcAddEnvelope = "AddEnvelope";
        private const string StoredProcAddEnvelopeBatch = "AddEnvelopeBatch";
        private const string StoredProcEditEnvelope = "EditEnvelope";
        private const string StoredProcUnlinkEnvelope = "UnlinkEnvelopeFromClaim";
        private const string StoredProcEditEnvelopeBatch = "EditEnvelopeBatch";
        private const string StoredProcDeleteEnvelope = "DeleteEnvelope";
        private const string StoredProcAddType = "AddEnvelopeType";
        private const string StoredProcEditType = "EditEnvelopeType";
        private const string StoredProcDeleteType = "DeleteEnvelopeType";
        private const string StoredProcSavePhysicalState = "SaveEnvelopePhysicalState";
        private const string StoredProcDeletePhysicalState = "DeleteEnvelopePhysicalState";
        private const string StoredProcUpdatePhysicalStates = "UpdateEnvelopesPhysicalStates";

        /*
            // Build the base SELECT SQL statement here - note the concat is done at compile time, not at runtime.
            // There is therefore no overhead, and this is (slightly) more readable than string builder is.
            // This is here so that there are not loads of strings littered throughout the class, and a column or param can be changed centrally.
        */
        private const string BaseGetEnvelopeSql =
                                "SELECT e." + ColumnNameEnvelopeId + ", e." + ColumnNameEnvelopeAccountId + ", e." + ColumnNameEnvelopeClaimId +
                                ", e." + ColumnNameEnvelopeNumber + ", e." + ColumnNameEnvelopeCrn + ", e." + ColumnNameEnvelopeStatus + 
                                ", t." + ColumnNameTypeId + ", t." + ColumnNameTypeLabel + 
                                ", e." + ColumnNameEnvelopeDateIssued + ", e." + ColumnNameEnvelopeDateAssigned + ", e." + ColumnNameEnvelopeDateReceived +
                                ", e." + ColumnNameEnvelopeDateAttached + ", e." + ColumnNameEnvelopeDateDestroyed + 
                                ", e." + ColumnNameEnvelopeOverpayment + ", e." + ColumnNameEnvelopePhysicalStateUrl + ", e." + ColumnNameEnvelopeDeclaredLostInPost +
                                ", p." + ColumnNamePhysicalStateId + ", p." + ColumnNamePhysicalStateDetails + 
                                " FROM [dbo].[Envelopes] AS e" +
                                " LEFT JOIN [dbo].[EnvelopeTypes] AS t ON e." + ColumnNameEnvelopeType + " = t." + ColumnNameTypeId + 
                                " LEFT JOIN [dbo].[EnvelopesPhysicalStates] AS j ON e." + ColumnNameEnvelopeId + " = j." + ColumnNameEnvelopeId +
                                " LEFT JOIN [dbo].[EnvelopePhysicalState] AS p ON j." + ColumnNamePhysicalStateId + " = p." + ColumnNamePhysicalStateId;
                                

        private const string BaseGetTypeSql = "SELECT " + ColumnNameTypeId + ", " + ColumnNameTypeLabel + " FROM [dbo].[EnvelopeTypes] ";  
        private const string BaseGetStateSql = "SELECT " + ColumnNamePhysicalStateId + ", " + ColumnNamePhysicalStateDetails + " FROM [dbo].[EnvelopePhysicalState] ";

        private static readonly InvalidDataException EnvelopeErrorIdIsZero = new InvalidDataException("Cannot edit an Envelope with an EnvelopeId of 0.");
        private static readonly InvalidDataException EnvelopeErrorIdIsNotZero = new InvalidDataException("You must set the Id of an envelope to 0 to add it.");
        private static readonly InvalidDataException EnvelopeErrorTypeDoesntExist = new InvalidDataException("An EnvelopeType with this EnvelopeTypeId was not found.");
        private static readonly InvalidDataException EnvelopeErrorAccountDoesntExist = new InvalidDataException("No Account was found with this Id.");
        private static readonly InvalidDataException EnvelopeErrorNoEnvelopesWithThisBatch = new InvalidDataException("No envelopes with this batch code were found.");


        #endregion Private Constants / Column Names

        #region Properties + Constructor

        /// <summary>
        /// The size of the batch during creation. 
        /// Should be 1000 in production.
        /// </summary>
        public uint BatchSize { get; private set; }

        /// <summary>
        /// Creates an instance of Envelopes.
        /// </summary>
        public Envelopes()
        {
            BatchSize = (uint) int.Parse(ConfigurationManager.AppSettings["EnvelopeBatchSize"] ?? "1000");
        }

        #endregion Properties + Constructor

        #region Direct Envelope Management

        /// <summary>
        /// Gets all envelopes in the system.
        /// </summary>
        /// <returns></returns>
        public IList<Envelope> GetAllEnvelopes()
        {
            return ExtractEnvelopesFromSql(BaseGetEnvelopeSql);
        }

        /// <summary>
        /// Gets all envelopes with the matching AccountId.
        /// </summary>
        /// <param name="accountId">The AccountId of account to get envelopes for.</param>
        /// <returns>A list of envelopes with the matching AccountId.</returns>
        public IList<Envelope> GetEnvelopesByAccount(int accountId)
        {
            const string sql = BaseGetEnvelopeSql + " WHERE " + ColumnNameEnvelopeAccountId + " = " + ParamEnvelopeAccountId;
            var args = new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(ParamEnvelopeAccountId, accountId) };
            return ExtractEnvelopesFromSql(sql, args);
        }

        /// <summary>
        /// Gets all envelopes with the matching AccountId, which have a sent date older than the supplied date, but no received date.
        /// </summary>
        /// <param name="accountId">The AccountId of account to get envelopes for.</param>
        /// <param name="olderThanWhen">The date that determines whether the envelope is sent but not received within the threshold.</param>
        /// <returns>A list of envelopes with the matching AccountId.</returns>
        public IList<Envelope> GetEnvelopesByAccountWhichAreSentButNotReceived(int accountId, DateTime olderThanWhen)
        {
            const string sql = BaseGetEnvelopeSql + " WHERE " + ColumnNameEnvelopeAccountId + " = " + ParamEnvelopeAccountId
                + " AND (" + ColumnNameEnvelopeStatus + " = " + ParamEnvelopeStatus + " AND " + ColumnNameEnvelopeDateAssigned +
                " < " + ParamEnvelopeDateAssigned + ") AND " + ColumnNameEnvelopeDateReceived + " IS NULL";

            var args = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(ParamEnvelopeAccountId, accountId),
                new KeyValuePair<string, object>(ParamEnvelopeStatus, (int)EnvelopeStatus.AttachedToClaim),
                new KeyValuePair<string, object>(ParamEnvelopeDateAssigned, olderThanWhen)
            };

            return ExtractEnvelopesFromSql(sql, args);
        }

        /// <summary>
        /// Gets all envelopes with the matching batch (the central part of the EnvelopeNumber).
        /// </summary>
        /// <param name="batchCode"></param>
        /// <returns>A list of envelopes with the matching AccountId.</returns>
        public IList<Envelope> GetEnvelopesByBatch(string batchCode)
        {
            // set up the SQL
            //var sql = BaseGetEnvelopeSql + " AND CONTAINS(" + ColumnNameEnvelopeNumber + ", '" + batchCode.ToUpperInvariant() + "')";
            var sql = BaseGetEnvelopeSql + " WHERE " + ColumnNameEnvelopeNumber + " LIKE '%" + batchCode.ToUpperInvariant() + "%'";

            // fetch the envelope range specified
            return ExtractEnvelopesFromSql(sql).ToList();
        }

        /// <summary>
        /// Gets an envelope by it's EnvelopeId.
        /// </summary>
        /// <param name="id">The Id of the envelope.</param>
        /// <returns>The envelope with the matching Id.</returns>
        public Envelope GetEnvelopeById(int id)
        {
            const string sql = BaseGetEnvelopeSql + " WHERE e." + ColumnNameEnvelopeId + " = " + ParamEnvelopeId;
            var args = new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(ParamEnvelopeId, id) };
            return ExtractEnvelopesFromSql(sql, args).LastOrDefault();
        }

        /// <summary>
        /// Gets an envelope by it's EnvelopeNumber. Note that here the return type is a list
        /// as there is a small chance multiple envelopes with the same EnvelopeNumber will be sent in.
        /// </summary>
        /// <param name="envelopeNumber">The EnvelopeNumber of the envelope.</param>
        /// <returns>The envelope with the matching EnvelopeNumber.</returns>
        public IList<Envelope> GetEnvelopesByEnvelopeNumber(string envelopeNumber)
        {
            const string sql = BaseGetEnvelopeSql + " WHERE " + ColumnNameEnvelopeNumber + " = " + ParamEnvelopeNumber;
            var args = new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(ParamEnvelopeNumber, envelopeNumber) };
            return ExtractEnvelopesFromSql(sql, args);
        }

        /// <summary>
        /// Gets all envelopes that have the matching ClaimReferenceNumber.
        /// </summary>
        /// <param name="claimReferenceNumber">The ClaimReferenceNumber of the envelope.</param>
        /// <returns>The envelopes with the matching ClaimReferenceNumber.</returns>
        public IList<Envelope> GetEnvelopesByClaimReferenceNumber(string claimReferenceNumber)
        {
            const string sql = BaseGetEnvelopeSql + " WHERE " + ColumnNameEnvelopeCrn + " = " + ParamEnvelopeCrn;
            var args = new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(ParamEnvelopeCrn, claimReferenceNumber) };
            return ExtractEnvelopesFromSql(sql, args);
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

            ValidateAccount(envelope.AccountId ?? user.AccountID);

            TryGetEnvelopeTypeAndThrow(envelope.Type.EnvelopeTypeId);

            // convert
            var args = ConvertEnvelopeToSqlParams(envelope, user);

            // remove the EnvelopeId arg
            args.Remove(args.First(a => a.Key == ParamEnvelopeId));

            // make the call, getting the result
            envelope.EnvelopeId = RunSingleStoredProcedure(StoredProcAddEnvelope, args);

            // include linkage to physical state
            UpdateEnvelopesPhysicalStates(envelope.EnvelopeId, envelope.PhysicalState.Select(x => x.EnvelopePhysicalStateId).ToList());

            return envelope;
        }

        /// <summary>
        /// Creates a batch of envelopes in the system and returns the newly create envelopes.
        /// </summary>
        /// <param name="type">The type of the envelope, which should be a valid EnvelopeType.</param>
        /// <returns>A list of the newly created envelopes.</returns>
        public IList<Envelope> AddEnvelopeBatch(int type)
        {
            var envelopeNumberBatch = new EnvelopeNumberBatch(BatchSize);
            List<string> envelopeNumbers = envelopeNumberBatch.GenerateEnvelopeNumbers();

            TryGetEnvelopeTypeAndThrow(type);

            // create output list
            var list = new List<Envelope>(); 

            // populate the envelopes
            for (var i = 0; i < BatchSize; i++)
            {
                // save this for the return for SQL
                var env = new Envelope
                {
                    EnvelopeId = 0,
                    AccountId = null,
                    ClaimId = null,
                    EnvelopeNumber = envelopeNumbers[i],
                    ClaimReferenceNumber = null,
                    Status = EnvelopeStatus.Generated,
                    Type = new EnvelopeType { EnvelopeTypeId = type },
                    DateIssuedToClaimant = null,
                    DateAssignedToClaim = null,
                    DateReceived = null,
                    OverpaymentCharge = null,
                    PhysicalState = new List<EnvelopePhysicalState>(),
                    PhysicalStateProofUrl = null,
                    LastModifiedBy = 0
                };

                // add to the list
                list.Add(env);
            }

            // execute proc on list
            ExecuteBatchProcedure(StoredProcAddEnvelopeBatch, list, true);

            // return modified
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
            // handle errors
            ValidateEnvelopeTypes(envelopes, true);

            // add and update Ids
            ExecuteBatchProcedure(StoredProcAddEnvelopeBatch, envelopes, true);

            // return
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
            // check account
            if (envelope.AccountId.HasValue)
            {
                ValidateAccount(envelope.AccountId.Value);
            }

            // handle error
            TryGetEnvelopeAndThrow(envelope.EnvelopeId);

            if (envelope.Type != null)
            {
                TryGetEnvelopeTypeAndThrow(envelope.Type.EnvelopeTypeId);
            }

            // convert
            var args = ConvertEnvelopeToSqlParams(envelope, user);

            // make the call, getting the result
            RunSingleStoredProcedure(StoredProcEditEnvelope, args);

            // include linkage to physical state
            UpdateEnvelopesPhysicalStates(envelope.EnvelopeId, envelope.PhysicalState.Select(x => x.EnvelopePhysicalStateId).ToList());

            // return
            return GetEnvelopeById(envelope.EnvelopeId);
        }

        /// <summary>
        /// Edits a batch of envelopes. Ensure the EnvelopeId is set correctly in each.
        /// </summary>
        /// <param name="envelopes">The envelopes to edit.</param>
        /// <returns>The edited envelopes.</returns>
        public IList<Envelope> EditEnvelopeBatch(IList<Envelope> envelopes)
        {
            // handle errors
            ValidateEnvelopeTypes(envelopes, false);

            // execute the batch edit and return
            ExecuteBatchProcedure(StoredProcEditEnvelopeBatch, envelopes);

            // return
            return envelopes.ToList().Select(e => GetEnvelopeById(e.EnvelopeId)).ToList();
        }

        /// <summary>
        /// Utility method for issuing a single envelope to an account.
        /// Will set the envelop's status to 6 (envelope issued).
        /// </summary>
        /// <param name="envelopeId">The EnvelopeId of the envelope to issue.</param>
        /// <param name="accountId">The AccountId of the account to issue the envelope to.</param>
        /// <param name="user">The current user.</param>
        /// <returns>The newly modified envelope.</returns>
        public Envelope IssueToAccount(int envelopeId, int accountId, ICurrentUserBase user)
        {
            // check account
            ValidateAccount(accountId);

            // get the envelope and throw if it doesn't exist
            var envelope = TryGetEnvelopeAndThrow(envelopeId);

            // and the envelopes haven't been assigned
            if (envelope.AccountId != null)
            {
                throw new InvalidDataException("This envelope is already assigned to another account.");
            }

            // set the new accountId
            envelope.AccountId = accountId;
            envelope.Status = EnvelopeStatus.IssuedToAccount;
            envelope.DateIssuedToClaimant = DateTime.UtcNow;

            // convert to sql params
            var args = ConvertEnvelopeToSqlParams(envelope, user);

            // execute SP
            RunSingleStoredProcedure(StoredProcEditEnvelope, args);

            // return
            return envelope;
        }

        /// <summary>
        /// Utility method for issuing multiple envelopes to an account. 
        /// You should specify the initial starting record, and an amount after that to assign.
        /// </summary>
        /// <param name="batchCode">The central part of the EnvelopeNumber (the batch) to assign.</param>
        /// <param name="accountId">The AccountId of the account to issue the envelopes to.</param>
        /// <returns>The newly modified envelopes.</returns>
        public IList<Envelope> IssueBatchToAccount(string batchCode, int accountId)
        {
            // check account
            ValidateAccount(accountId);

            // get the batch
            var envelopes = GetEnvelopesByBatch(batchCode).ToList();

            // check there are some.
            if (envelopes == null || envelopes.Count == 0)
            {
                throw EnvelopeErrorNoEnvelopesWithThisBatch;
            }

            // and the envelopes haven't been assigned
            var envelopeWithNonNullAccountId = envelopes.FirstOrDefault(e => e.AccountId != null);
            if (envelopeWithNonNullAccountId != null)
            {
                throw new InvalidDataException("At least one of these envelopes is already assigned to another account: " + envelopeWithNonNullAccountId.EnvelopeId);
            }

            // populate the rows (set the accountId and the status to assigned (5))
            envelopes.ForEach(env =>
            {
                env.AccountId = accountId;
              env.Status = EnvelopeStatus.IssuedToAccount;
                env.DateIssuedToClaimant = DateTime.UtcNow;
            });

            // exec
            ExecuteBatchProcedure(StoredProcEditEnvelopeBatch, envelopes);

            return envelopes;
        }

        /// <summary>
        /// Utility method for attaching a single envelope to a claim, with the option to only validate that it can be attached.
        /// </summary>
        /// <param name="envelopeId">The EnvelopeId of the envelope to attach.</param>
        /// <param name="claim">The claim to attach the envelope to. 
        /// NOTE: The claim Id is not checked, since this would require a reference to Spend_Management.</param>
        /// <param name="user">The current user.</param>
        /// <param name="validateOnly">Passing true for this parameter causes the Envelope to not be attached, but rather just validated.</param>
        /// <returns>The newly modified envelope.</returns>
        public Envelope AttachToClaim(int envelopeId, cClaim claim, ICurrentUserBase user, bool validateOnly = false)
        {
            // get the envelope and throw if it doesn't exist
            var envelope = TryGetEnvelopeAndThrow(envelopeId);

            if (!envelope.AccountId.HasValue)
            {
                throw new InvalidDataException("You must set the AccountId of the envelope to a valid account before attaching to a claim.");
            }

            if (envelope.ClaimId.HasValue && envelope.ClaimId != claim.claimid)
            {
                throw new InvalidDataException("This Envelope is already assigned to another claim.");
            }

            // if we're only validating, do not alter the DB.
            if (validateOnly)
            {
                return envelope;
            }

            // set the new claimId
            envelope.ClaimId = claim.claimid;
            envelope.Status = EnvelopeStatus.AttachedToClaim;
            envelope.ClaimReferenceNumber = claim.ReferenceNumber;
            envelope.DateAssignedToClaim = DateTime.UtcNow;

            // convert to sql params
            var args = ConvertEnvelopeToSqlParams(envelope, user);

            // execute SP
            RunSingleStoredProcedure(StoredProcEditEnvelope, args);

            // return
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
            // create args.
            var args = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(ParamEnvelopeId, envelopeId),
                new KeyValuePair<string, object>(ParamEnvelopeLastModifiedBy, user.EmployeeID)
            };

            // execute SP
            RunSingleStoredProcedure(StoredProcUnlinkEnvelope, args);

            // return
            return GetEnvelopeById(envelopeId);
        }

        /// <summary>
        /// Utility method for marking a single envelope as received by SEL.
        /// </summary>
        /// <param name="id">The EnvelopeId of the envelope to issue.</param>
        /// <param name="user">The current user.</param>
        /// <returns>The newly modified envelope.</returns>
        public Envelope MarkReceived(int id, ICurrentUserBase user)
        {
            // get the envelope and throw if it doesn't exist
            var envelope = TryGetEnvelopeAndThrow(id);

            // update status and date
            envelope.Status = EnvelopeStatus.ReceivedBySEL;
            envelope.DateReceived = DateTime.UtcNow;

            // convert to sql params
            var args = ConvertEnvelopeToSqlParams(envelope, user);

            // execute SP
            RunSingleStoredProcedure(StoredProcEditEnvelope, args);

            // return
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
            // get the envelope and throw if it doesn't exist
            var envelope = TryGetEnvelopeAndThrow(id);

            // update status and date
            envelope.Status = EnvelopeStatus.ReceiptsAttached;
            envelope.DateAttachCompleted = DateTime.UtcNow;

            // convert to sql params
            var args = ConvertEnvelopeToSqlParams(envelope, user);

            // execute SP
            RunSingleStoredProcedure(StoredProcEditEnvelope, args);

            // return
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
            // get the envelope and throw if it doesn't exist
            var envelope = TryGetEnvelopeAndThrow(envelopeId);

            // change the status
            envelope.Status = status;

            // convert to sql params
            var args = ConvertEnvelopeToSqlParams(envelope, user);

            // execute SP
            RunSingleStoredProcedure(StoredProcEditEnvelope, args);

            // return
            return envelope;
        }
        
        /// <summary>
        /// Deletes an envelope.
        /// </summary>
        /// <param name="envelopeId">The EnvelopeId of the envelope to delete.</param>
        /// <param name="user">The user making the call for audit purposes.</param>
        /// <returns>Whether or not the operation was a success.</returns>
        public bool DeleteEnvelope(int envelopeId, ICurrentUserBase user)
        {
            // get the envelope and throw if it doesn't exist
            var envelope = TryGetEnvelopeAndThrow(envelopeId);

            // throw if attached to claim
            if (envelope.ClaimId != null)
            {
                throw new InvalidDataException("This Envelope is attached to a claim.");
            }

            // convert to sql params
            var args = ConvertEnvelopeToSqlParams(envelope, user);

            // execute SP
            RunSingleStoredProcedure(StoredProcDeleteEnvelope, args);

            // return
            return true;
        }

        #endregion Direct Envelope Management

        #region Envelope Type Management

        /// <summary>
        /// Gets all the envelope types in the system.
        /// </summary>
        /// <returns>A list of envelope types.</returns>
        public IList<EnvelopeType> GetAllEnvelopeTypes()
        {
            return ExtractEnvelopeTypesFromSql(BaseGetTypeSql);
        }

        /// <summary>
        /// Gets a single envelope type by it's Id.
        /// </summary>
        /// <param name="id">The Id of the type to get.</param>
        /// <returns>The envelope type.</returns>
        public EnvelopeType GetEnvelopeTypeById(int id)
        {
            const string sql = BaseGetTypeSql + " WHERE " + ColumnNameTypeId + " = " + ParamEnvelopeType;
            var args = new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(ParamEnvelopeType, id) };
            return ExtractEnvelopeTypesFromSql(sql, args).FirstOrDefault();
        }

        /// <summary>
        /// Adds an envelope type to the database.
        /// </summary>
        /// <param name="label">The new type (which will be come the label)</param>
        /// <returns>The new envelope type (with a populated Id).</returns>
        public EnvelopeType AddEnvelopeType(string label)
        {
            var args = new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(ParamLabel, label) };
            var id = RunSingleStoredProcedure(StoredProcAddType, args);
            return GetEnvelopeTypeById(id);
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
            TryGetEnvelopeTypeAndThrow(id);
            var args = new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(ParamId, id), new KeyValuePair<string, object>(ParamLabel, label) };
            RunSingleStoredProcedure(StoredProcEditType, args);
            return GetEnvelopeTypeById(id);
        }

        /// <summary>
        /// Deletes an envelopeType. If the type is in use, an error is thrown.
        /// </summary>
        /// <param name="id">The Id of the type to delete.</param>
        /// <returns>Whether the delete succeeded.</returns>
        public bool DeleteEnvelopeType(int id)
        {
            TryGetEnvelopeTypeAndThrow(id);

            // check no items have the status
            const string sql = BaseGetEnvelopeSql + " AND " + ColumnNameTypeId + " = " + ParamEnvelopeType;
            var args = new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(ParamEnvelopeType, id) };
            var envelopesWithThisType = ExtractEnvelopesFromSql(sql, args);

            if (envelopesWithThisType.Any())
            {
                throw new InvalidDataException("You cannot delete this EnvelopeType as it is in use by an envelope.");
            }

            args = new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(ParamId, id) };
            var result = RunSingleStoredProcedure(StoredProcDeleteType, args);
            return result == 0;
        }

        #endregion Envelope Type Management

        #region Envelope Physical State Management

        /// <summary>
        /// Gets all the envelope PhysicalState in the system.
        /// </summary>
        /// <returns>A list of envelope types.</returns>
        public IList<EnvelopePhysicalState> GetAllEnvelopePhysicalStates()
        {
            return ExtractEnvelopePhysicalStatesFromSql(BaseGetStateSql);
        }

        /// <summary>
        /// Gets a single envelope PhysicalState by it's Id.
        /// </summary>
        /// <param name="id">The Id of the type to get.</param>
        /// <returns>The envelope type.</returns>
        public EnvelopePhysicalState GetEnvelopePhysicalStateById(int id)
        {
            const string sql = BaseGetStateSql + " WHERE " + ColumnNamePhysicalStateId + " = " + ParamId;
            var args = new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(ParamId, id) };
            return ExtractEnvelopePhysicalStatesFromSql(sql, args).FirstOrDefault();
        }

        /// <summary>
        /// Adds an envelope PhysicalState to the database.
        /// </summary>
        /// <param name="details">The new type (which will be come the label)</param>
        /// <returns>The new envelope type (with a populated Id).</returns>
        public EnvelopePhysicalState AddEnvelopePhysicalState(string details)
        {
            var args = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(ParamId, 0),
                new KeyValuePair<string, object>(ParamPhysicalStateDetails, details)
            };

            var id = RunSingleStoredProcedure(StoredProcSavePhysicalState, args);

            if (id == -1)
            {
                throw new InvalidDataException("A PhysicalState already exists with the same details.");
            }

            return GetEnvelopePhysicalStateById(id);
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
            var args = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(ParamId, id),
                new KeyValuePair<string, object>(ParamPhysicalStateDetails, details)
            };
            
            id = RunSingleStoredProcedure(StoredProcSavePhysicalState, args);

            if (id == -2)
            {
                throw new InvalidDataException("PhysicalState doesn't exist.");
            }

            return GetEnvelopePhysicalStateById(id);
        }

        /// <summary>
        /// Updates the Envelope's physical states.
        /// </summary>
        /// <param name="envelopeId">The Envelope's Id.s</param>
        /// <param name="stateIds">The Ids of the Physical states.</param>
        /// <returns></returns>
        public Envelope UpdateEnvelopesPhysicalStates(int envelopeId, List<int> stateIds)
        {
            TryGetEnvelopeAndThrow(envelopeId);

            var stateTable = new DataTable("IntPK");
            stateTable.Columns.Add("c1", typeof(int));
            stateIds.ForEach(id => stateTable.Rows.Add(id));
            
            
            var args = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(ParamEnvelopeId, envelopeId),
                new KeyValuePair<string, object>(ParamPhysicalStateIds, stateTable)
            };

            RunSingleStoredProcedure(StoredProcUpdatePhysicalStates, args);

            return GetEnvelopeById(envelopeId);
        }

        /// <summary>
        /// Deletes a PhysicalState. If the type is in use, an error is thrown.
        /// </summary>
        /// <param name="id">The Id of the EnvelopePhysicalState to delete.</param>
        /// <returns>Whether the delete succeeded.</returns>
        public bool DeleteEnvelopePhysicalState(int id)
        {
            // check no items have the status
            int count;
            using (var connection = new DatabaseConnection(GlobalVariables.MetabaseConnectionString))
            {
                count = (int)connection.ExecuteScalarSql("SELECT COUNT (" + ColumnNameId + ") FROM [dbo].[EnvelopesPhysicalStates] WHERE " + ColumnNamePhysicalStateId + " = " + id);
            }

            if (count > 0)
            {
                throw new InvalidDataException("You cannot delete this EnvelopePhysicalState as it is in use by an envelope.");
            }

            var args = new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(ParamId, id) };
            var result = RunSingleStoredProcedure(StoredProcDeletePhysicalState, args);
            return result == 0;
        }

        #endregion Envelope Physical State Management

        #region Envelope History Management

        /// <summary>
        /// Gets all the history entries for an envelope.
        /// </summary>
        /// <param name="envelopeId">The Id of the envelope to fetch the history for.</param>
        /// <returns>A list of envelope histories.</returns>
        public IList<EnvelopeHistory> GetEnvelopeHistory(int envelopeId)
        {
            throw new NotImplementedException();
        }

        #endregion Envelope History Management

        #region Utilities

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
            var envelopes = alreadyFetched ?? GetEnvelopesByClaimReferenceNumber(claimReferenceNumber);
            totalCount = envelopes.Count;
            completedCount = envelopes.Count(e => (e.Status == EnvelopeStatus.ConfirmedSent && e.DeclaredLostInPost)
                                               || (e.Status >= EnvelopeStatus.ReceiptsAttached && e.DateReceived.HasValue));
            return totalCount == completedCount;
        }

        #endregion Utilities

        #region Private Helpers

        /// <summary>
        /// Reads the database and builds envelopes for each row in the query result.
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="addWithValueArgs">Any arguments to pass to the SQL.</param>
        /// <returns>A list of envelopes.</returns>
        private static IList<Envelope> ExtractEnvelopesFromSql(string sql, List<KeyValuePair<string, object>> addWithValueArgs = null)
        {
            var envelopes = new List<Envelope>();
            using (var connection = new DatabaseConnection(GlobalVariables.MetabaseConnectionString))
            {
                // add arguments to the connection
                if (addWithValueArgs != null)
                {
                    addWithValueArgs.ForEach(x => connection.AddWithValue(x.Key, x.Value));
                }

                // read the db
                using (IDataReader reader = connection.GetReader(sql))
                {
                    while (reader.Read())
                    {
                        var envelopeId = reader.GetValueOrDefault(ColumnNameEnvelopeId, 0);
                        var envelope = envelopes.FirstOrDefault(x => x.EnvelopeId == envelopeId);
                        
                        if (envelope == null)
                        {
                            // generate an envelope
                            envelope = new Envelope
                            {
                                EnvelopeId = envelopeId,
                                AccountId = reader.GetNullable<int>(ColumnNameEnvelopeAccountId),
                                ClaimId = reader.GetNullable<int>(ColumnNameEnvelopeClaimId),
                                EnvelopeNumber = reader.GetValueOrDefault<string>(ColumnNameEnvelopeNumber, null),
                                ClaimReferenceNumber = reader.GetValueOrDefault<string>(ColumnNameEnvelopeCrn, null),
                                Type = GetEnvelopeTypeFromReader(reader),
                                Status = reader.GetRequiredEnumValue<EnvelopeStatus>(ColumnNameEnvelopeStatus),
                                DateIssuedToClaimant = reader.GetNullable<DateTime>(ColumnNameEnvelopeDateIssued),
                                DateAssignedToClaim = reader.GetNullable<DateTime>(ColumnNameEnvelopeDateAssigned),
                                DateReceived = reader.GetNullable<DateTime>(ColumnNameEnvelopeDateReceived),
                                DateAttachCompleted = reader.GetNullable<DateTime>(ColumnNameEnvelopeDateAttached),
                                DateDestroyed = reader.GetNullable<DateTime>(ColumnNameEnvelopeDateDestroyed),
                                OverpaymentCharge = reader.GetNullable<decimal>(ColumnNameEnvelopeOverpayment),
                                PhysicalState = new List<EnvelopePhysicalState>(),
                                PhysicalStateProofUrl = reader.GetValueOrDefault<string>(ColumnNameEnvelopePhysicalStateUrl, null),
                                DeclaredLostInPost = reader.GetRequiredValue<bool>(ColumnNameEnvelopeDeclaredLostInPost)
                            };
                            
                            envelopes.Add(envelope);
                        }

                        // we are just adding physical states, whether or not we have just created the envelope.
                        if (reader.GetValueOrDefault<string>(ColumnNamePhysicalStateDetails, null) != null)
                        {
                            envelope.PhysicalState.Add(GetEnvelopePhysicalStateFromReader(reader));
                        }
                    }
                }

                connection.ClearParameters();
            }
            return envelopes;
        }

        /// <summary>
        /// Reads the database and builds EnvelopeTypes for each row in the query result.
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="addWithValueArgs">Any arguments to pass to the SQL.</param>
        /// <returns>A list of EnvelopeType.</returns>
        private static IList<EnvelopeType> ExtractEnvelopeTypesFromSql(string sql, List<KeyValuePair<string, object>> addWithValueArgs = null)
        {
            var list = new List<EnvelopeType>();
            using (var connection = new DatabaseConnection(GlobalVariables.MetabaseConnectionString))
            {
                // add arguments to the connection
                if (addWithValueArgs != null)
                {
                    addWithValueArgs.ForEach(x => connection.AddWithValue(x.Key, x.Value));
                }

                // read the db
                using (IDataReader reader = connection.GetReader(sql))
                {
                    while (reader.Read())
                    {
                        // generate an envelope type
                        list.Add(GetEnvelopeTypeFromReader(reader));
                    }
                }

                connection.ClearParameters();
            }
            return list;
        }

        /// <summary>
        /// Reads the database and builds EnvelopePhysicalStates for each row in the query result.
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="addWithValueArgs">Any arguments to pass to the SQL.</param>
        /// <returns>A list of EnvelopeType.</returns>
        private static IList<EnvelopePhysicalState> ExtractEnvelopePhysicalStatesFromSql(string sql, List<KeyValuePair<string, object>> addWithValueArgs = null)
        {
            var list = new List<EnvelopePhysicalState>();
            using (var connection = new DatabaseConnection(GlobalVariables.MetabaseConnectionString))
            {
                // add arguments to the connection
                if (addWithValueArgs != null)
                {
                    addWithValueArgs.ForEach(x => connection.AddWithValue(x.Key, x.Value));
                }

                // read the db
                using (IDataReader reader = connection.GetReader(sql))
                {
                    while (reader.Read())
                    {
                        // generate an envelope physical state
                        list.Add(GetEnvelopePhysicalStateFromReader(reader));
                    }
                }

                connection.ClearParameters();
            }
            return list;
        }

        /// <summary>
        /// Uses the reader to build an <see cref="EnvelopeType"/> object.
        /// </summary>
        /// <param name="reader">The instance of IDataReader.</param>
        /// <returns>An EnvelopeType instance.</returns>
        private static EnvelopeType GetEnvelopeTypeFromReader(IDataReader reader)
        {
            return new EnvelopeType
            {
                EnvelopeTypeId = reader.GetRequiredValue<int>(ColumnNameTypeId),
                Label = reader.GetRequiredValue<string>(ColumnNameTypeLabel)
            };
        }

        /// <summary>
        /// Uses the reader to build an <see cref="EnvelopeType"/> object.
        /// </summary>
        /// <param name="reader">The instance of IDataReader.</param>
        /// <returns>An EnvelopeType instance.</returns>
        private static EnvelopePhysicalState GetEnvelopePhysicalStateFromReader(IDataReader reader)
        {
            return new EnvelopePhysicalState
            {
                EnvelopePhysicalStateId = reader.GetRequiredValue<int>(ColumnNamePhysicalStateId),
                Details = reader.GetRequiredValue<string>(ColumnNamePhysicalStateDetails)
            };
        }

        /// <summary>
        /// Executes a batch stored procedure, updating the Id and EnvelopeNumber if applicable of each envelope passed in.
        /// </summary>
        /// <param name="operation">The name of the stored procedure to perform. Use a constant from the top of this class.</param>
        /// <param name="envelopeList">The list of envelopes on which to execute the stored procedure.</param>
        /// <param name="excludeEnvelopeId">Whether to exclude the envelope Id from the internal data table creation. This should be set true for envelope addtion.</param>
        private static void ExecuteBatchProcedure(string operation, IList<Envelope> envelopeList, bool excludeEnvelopeId = false)
        {
            // input is a User-Defined Table Type called EnvelopeBatch.
            // set up the table and cols
            DataTable inputTable = CreateEnvelopeBatchDataTable(excludeEnvelopeId);

            // add to the row
            envelopeList.ToList().ForEach(env => PopulateRowFromEnvelope(inputTable, env, excludeEnvelopeId));

            // make the connection
            using (var connection = new DatabaseConnection(GlobalVariables.MetabaseConnectionString))
            {
                // add the table
                connection.AddWithValue(ParamEnvelopes, inputTable);
                
                // grab a reader for the results
                // output is a table containing the int Id and string EnvelopeNumber
                using (IDataReader reader = connection.GetReader(operation, CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        // get the two output values for each row (Id and EnvelopeNumber)
                        var id = reader.GetRequiredValue<int>(ColumnNameId);
                        var en = reader.GetRequiredValue<string>(ColumnNameEnvelopeNumber);

                        // populate the PK of the output list by matching the Envelope number.
                        envelopeList.First(e => e.EnvelopeNumber == en).EnvelopeId = id;
                    }
                }

                // clean up
                connection.ClearParameters();
            }
        }

        /// <summary>
        /// Executes a stored procedure on a single item, returning the result.
        /// </summary>
        /// <param name="sql">The name of the stored procedure.</param>
        /// <param name="addWithValueArgs">The arguments to pass to the stored procedure.</param>
        /// <returns>An integer result.</returns>
        private static int RunSingleStoredProcedure(string sql, List<KeyValuePair<string, object>> addWithValueArgs = null)
        {
            const string returnValueKey = "returnvalue";
            int result;

            using (var connection = new DatabaseConnection(GlobalVariables.MetabaseConnectionString))
            {
                if (addWithValueArgs != null)
                {
                    addWithValueArgs.ForEach(x => connection.AddWithValue(x.Key, x.Value));
                }

                connection.AddReturn(returnValueKey, SqlDbType.Int);
                connection.ExecuteProc(sql);
                result = connection.GetReturnValue<int>(returnValueKey);

                if (result < 0)
                {
                    throw new InvalidDataException("Item not saved.");
                }

                connection.ClearParameters();
            }

            return result;
        }

        /// <summary>
        /// Converts all the properties of an envelope to a list of KeyValuePairs, ready to be iterated upon and added as SQL parameters.
        /// </summary>
        /// <param name="envelope">The envelope to convert.</param>
        /// <param name="user">The current user.</param>
        /// <returns></returns>
        private static List<KeyValuePair<string, object>> ConvertEnvelopeToSqlParams(Envelope envelope, ICurrentUserBase user)
        {
            envelope.LastModifiedBy = user.EmployeeID;

            var args = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(ParamEnvelopeId, envelope.EnvelopeId),
                new KeyValuePair<string, object>(ParamEnvelopeAccountId, envelope.AccountId),
                new KeyValuePair<string, object>(ParamEnvelopeClaimId, envelope.ClaimId),
                new KeyValuePair<string, object>(ParamEnvelopeNumber, envelope.EnvelopeNumber),
                new KeyValuePair<string, object>(ParamEnvelopeCrn, envelope.ClaimReferenceNumber),
                new KeyValuePair<string, object>(ParamEnvelopeStatus, envelope.Status),
                new KeyValuePair<string, object>(ParamEnvelopeDateIssued, envelope.DateIssuedToClaimant),
                new KeyValuePair<string, object>(ParamEnvelopeDateAssigned, envelope.DateAssignedToClaim),
                new KeyValuePair<string, object>(ParamEnvelopeDateReceived, envelope.DateReceived),
                new KeyValuePair<string, object>(ParamEnvelopeDateAttached, envelope.DateAttachCompleted),
                new KeyValuePair<string, object>(ParamEnvelopeDateDestroyed, envelope.DateDestroyed),
                new KeyValuePair<string, object>(ParamEnvelopeOverpayment, envelope.OverpaymentCharge),
                new KeyValuePair<string, object>(ParamEnvelopePhysicalStateUrl, envelope.PhysicalStateProofUrl),
                new KeyValuePair<string, object>(ParamEnvelopeLastModifiedBy, envelope.LastModifiedBy),
                new KeyValuePair<string, object>(ParamEnvelopeDeclaredLostInPost, envelope.DeclaredLostInPost)
            };

            args.Add(envelope.Type == null
                ? new KeyValuePair<string, object>(ParamEnvelopeType, DBNull.Value)
                : new KeyValuePair<string, object>(ParamEnvelopeType, envelope.Type.EnvelopeTypeId));

            return args;
        }

        /// <summary>
        /// Creates a data table for the User Defined Table Type "EnvelopeBatch".
        /// </summary>
        /// <param name="excludeEnvelopeId">Whether to exclude EnvelopeId. Default false.</param>
        /// <returns>A new DataTable instance.</returns>
        private static DataTable CreateEnvelopeBatchDataTable(bool excludeEnvelopeId = false)
        {
            var envelopeTable = new DataTable("EnvelopeBatch");
            
            if (!excludeEnvelopeId)
            {
                envelopeTable.Columns.Add(ColumnNameEnvelopeId, typeof(int));
            }

            envelopeTable.Columns.Add(ColumnNameEnvelopeAccountId, typeof(int));
            envelopeTable.Columns.Add(ColumnNameEnvelopeClaimId, typeof(int));
            envelopeTable.Columns.Add(ColumnNameEnvelopeNumber, typeof(string));
            envelopeTable.Columns.Add(ColumnNameEnvelopeCrn, typeof(string));
            envelopeTable.Columns.Add(ColumnNameEnvelopeStatus, typeof(int));
            envelopeTable.Columns.Add(ColumnNameEnvelopeType, typeof(int));
            envelopeTable.Columns.Add(ColumnNameEnvelopeDateIssued, typeof(DateTime));
            envelopeTable.Columns.Add(ColumnNameEnvelopeDateAssigned, typeof(DateTime));
            envelopeTable.Columns.Add(ColumnNameEnvelopeDateReceived, typeof(DateTime));
            envelopeTable.Columns.Add(ColumnNameEnvelopeDateAttached, typeof(DateTime));
            envelopeTable.Columns.Add(ColumnNameEnvelopeDateDestroyed, typeof(DateTime));
            envelopeTable.Columns.Add(ColumnNameEnvelopeOverpayment, typeof(int));
            envelopeTable.Columns.Add(ColumnNameEnvelopePhysicalStateUrl, typeof(string));
            envelopeTable.Columns.Add(ColumnNameEnvelopeLastModifiedBy, typeof(int));
            envelopeTable.Columns.Add(ColumnNameEnvelopeDeclaredLostInPost, typeof(bool));
            return envelopeTable;
        }

        /// <summary>
        /// Creates / Populates a DataRow from the data in an envelope.
        /// </summary>
        /// <param name="inputTable">The DataTable to add a row to.</param>
        /// <param name="env">The envelope from which to take the values.</param>
        /// <param name="excludeEnvelopeId">Whether to exclude EnvelopeId. Default false.</param>
        private static void PopulateRowFromEnvelope(DataTable inputTable, Envelope env, bool excludeEnvelopeId = false)
        {
            // if you change this setup, ensure you change the RemoveAt below...
            var row = new List<object>
            {
                env.EnvelopeId,
                env.AccountId,
                env.ClaimId,
                env.EnvelopeNumber,
                env.ClaimReferenceNumber,
                env.Status,
                env.Type.EnvelopeTypeId,
                env.DateIssuedToClaimant,
                env.DateAssignedToClaim,
                env.DateReceived,
                env.DateAttachCompleted,
                env.DateDestroyed,
                env.OverpaymentCharge,
                env.PhysicalStateProofUrl,
                env.LastModifiedBy,
                env.DeclaredLostInPost
            };

            if (excludeEnvelopeId)
            {
                row.RemoveAt(0);
            }

            inputTable.Rows.Add(row.ToArray());
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
                throw new InvalidDataException("An Envelope with this EnvelopeId was not found.");
            }
            return envelope;
        }

        /// <summary>
        /// Attempts to get an EnvelopeType, throwing an error if it doesn't exist.
        /// </summary>
        /// <param name="envelopeTypeId">The EnvelopeTypeId of the EnvelopeType.</param>
        /// <returns>The envelope type.</returns>
        /// <exception cref="InvalidDataException">If the EnvelopeTypeId doesn't result in an envelope type.</exception>
        private void TryGetEnvelopeTypeAndThrow(int envelopeTypeId)
        {
            var envelopeType = GetEnvelopeTypeById(envelopeTypeId);
            if (envelopeType == null)
            {
                throw EnvelopeErrorTypeDoesntExist;
            }
        }

        /// <summary>
        /// Checks to see if an account exists.
        /// </summary>
        /// <param name="accountId">The AccountId of the account to check.</param>
        /// <exception cref="InvalidDataException">If the AccountId doesn't result in an Account.</exception>
        private void ValidateAccount(int accountId)
        {
            // make sure the account is proper
            if (accountId == 0 || new cAccounts().GetAccountByID(accountId) == null)
            {
                throw EnvelopeErrorAccountDoesntExist;
            }
        }

        /// <summary>
        /// Loops throguh the provided envelopes and checks their status and type are valid.
        /// Throws errors if not.
        /// </summary>
        /// <param name="envelopes">The list of envelopes.</param>
        /// <param name="idShouldBeZero">Whether the method sohuld check envelopeIds for being 0 (when adding - true) or >0 (when editing - false).</param>
        private void ValidateEnvelopeTypes(IEnumerable<Envelope> envelopes, bool idShouldBeZero)
        {
            var typeIds = GetAllEnvelopeTypes().Select(t => t.EnvelopeTypeId);
            var accounts = new cAccounts();
            envelopes.ToList().ForEach(e =>
            {
                if (idShouldBeZero)
                {
                    if (e.EnvelopeId != 0)
                    {
                        throw EnvelopeErrorIdIsNotZero;
                    }
                }
                else
                {
                    if (e.EnvelopeId == 0)
                    {
                        throw EnvelopeErrorIdIsZero;
                    }
                }

                if (e.AccountId.HasValue && accounts.GetAccountByID(e.AccountId.Value) == null)
                {
                    throw EnvelopeErrorAccountDoesntExist;
                }

                if (!typeIds.Contains(e.Type.EnvelopeTypeId))
                {
                    throw EnvelopeErrorTypeDoesntExist;
                }
            });
        }

        #endregion Private Helpers
    }
}
