-- <Migration ID="a0f5a7dd-89e2-4f3d-9dbf-c726e96a8cf8" />
GO

PRINT N'Creating schemas'
GO
PRINT N'Creating types'
GO
CREATE TYPE [dbo].[IntPK] AS TABLE
(
[c1] [int] NOT NULL,
PRIMARY KEY CLUSTERED  ([c1]) WITH (IGNORE_DUP_KEY=ON)
)
GO
CREATE TYPE [dbo].[EnvelopeBatchEdit] AS TABLE
(
[EnvelopeId] [int] NOT NULL,
[AccountId] [int] NULL,
[ClaimId] [int] NULL,
[EnvelopeNumber] [nvarchar] (10) NULL,
[CRN] [nvarchar] (12) NULL,
[EnvelopeStatus] [tinyint] NULL,
[EnvelopeType] [int] NULL,
[DateIssuedToClaimant] [datetime] NULL,
[DateAssignedToClaim] [datetime] NULL,
[DateReceived] [datetime] NULL,
[DateAttachCompleted] [datetime] NULL,
[DateDestroyed] [datetime] NULL,
[OverpaymentCharge] [decimal] (16, 2) NULL,
[PhysicalStateProofUrl] [nvarchar] (100) NULL,
[LastModifiedBy] [int] NULL,
[DeclaredLostInPost] [bit] NULL
)
GO
CREATE TYPE [dbo].[EnvelopeBatchAdd] AS TABLE
(
[AccountId] [int] NULL,
[ClaimId] [int] NULL,
[EnvelopeNumber] [nvarchar] (10) NULL,
[CRN] [nvarchar] (12) NULL,
[EnvelopeStatus] [tinyint] NULL,
[EnvelopeType] [int] NULL,
[DateIssuedToClaimant] [datetime] NULL,
[DateAssignedToClaim] [datetime] NULL,
[DateReceived] [datetime] NULL,
[DateAttachCompleted] [datetime] NULL,
[DateDestroyed] [datetime] NULL,
[OverpaymentCharge] [decimal] (16, 2) NULL,
[PhysicalStateProofUrl] [nvarchar] (100) NULL,
[LastModifiedBy] [int] NULL,
[DeclaredLostInPost] [bit] NULL
)
GO
PRINT N'Creating [dbo].[card_providers]'
GO
CREATE TABLE [dbo].[card_providers]
(
[cardproviderid] [int] NOT NULL,
[cardprovider] [nvarchar] (50) NOT NULL,
[card_type] [tinyint] NOT NULL,
[creditcard] [bit] NOT NULL CONSTRAINT [DF_card_providers_creditcard] DEFAULT ((0)),
[purchasecard] [bit] NOT NULL CONSTRAINT [DF_card_providers_purchasecard] DEFAULT ((0)),
[createdon] [datetime] NOT NULL CONSTRAINT [DF_card_providers_createdon] DEFAULT (getdate()),
[createdby] [int] NULL,
[modifiedon] [datetime] NULL,
[modifiedby] [int] NULL,
[AutoImport] [bit] NULL
)
GO
PRINT N'Creating primary key [PK_card_providers] on [dbo].[card_providers]'
GO
ALTER TABLE [dbo].[card_providers] ADD CONSTRAINT [PK_card_providers] PRIMARY KEY CLUSTERED  ([cardproviderid])
GO
PRINT N'Creating [dbo].[SupportQuestions]'
GO
CREATE TABLE [dbo].[SupportQuestions]
(
[SupportQuestionId] [int] NOT NULL IDENTITY(1, 1),
[Question] [nvarchar] (250) NOT NULL,
[KnowledgeArticleUrl] [nvarchar] (1000) NULL,
[SupportTicketSel] [bit] NOT NULL CONSTRAINT [DF_SupportQuestions_SupportTicketSel] DEFAULT ((0)),
[SupportTicketInternal] [bit] NOT NULL CONSTRAINT [DF_SupportQuestions_SupportTicketInternal] DEFAULT ((0)),
[SupportQuestionHeadingId] [int] NOT NULL,
[Order] [int] NOT NULL
)
GO
PRINT N'Creating primary key [PK_SupportStatements] on [dbo].[SupportQuestions]'
GO
ALTER TABLE [dbo].[SupportQuestions] ADD CONSTRAINT [PK_SupportStatements] PRIMARY KEY CLUSTERED  ([SupportQuestionId])
GO
PRINT N'Creating [dbo].[registeredusers]'
GO
CREATE TABLE [dbo].[registeredusers]
(
[accountid] [int] NOT NULL IDENTITY(355, 1),
[companyname] [nvarchar] (250) NOT NULL,
[companyid] [nvarchar] (50) NOT NULL,
[contact] [nvarchar] (50) NOT NULL,
[nousers] [int] NOT NULL,
[expiry] [datetime] NOT NULL,
[accounttype] [tinyint] NOT NULL CONSTRAINT [DF_registeredusers_accounttype] DEFAULT ((1)),
[dbserver] [int] NULL,
[dbname] [nvarchar] (100) NULL,
[dbusername] [nvarchar] (100) NOT NULL,
[dbpassword] [nvarchar] (200) NULL,
[resellerid] [int] NULL,
[archived] [bit] NOT NULL CONSTRAINT [DF_registeredusers_archived] DEFAULT ((0)),
[autologEnabled] [bit] NOT NULL CONSTRAINT [DF_registeredusers_autologEnabled] DEFAULT ((1)),
[quickEntryFormsEnabled] [bit] NOT NULL CONSTRAINT [DF_registeredusers_quickEntryFormsEnabled] DEFAULT ((1)),
[employeeSearchEnabled] [bit] NOT NULL CONSTRAINT [DF_registeredusers_employeeSearchEnabled] DEFAULT ((1)),
[hotelReviewsEnabled] [bit] NOT NULL CONSTRAINT [DF_registeredusers_hotelReviewsEnabled] DEFAULT ((1)),
[advancesEnabled] [bit] NOT NULL CONSTRAINT [DF_registeredusers_advancesEnabled] DEFAULT ((1)),
[postcodeAnyWhereEnabled] [bit] NOT NULL CONSTRAINT [DF_registeredusers_postcodeAnyWhereEnabled] DEFAULT ((1)),
[corporateCardsEnabled] [bit] NOT NULL CONSTRAINT [DF_registeredusers_corporateCardsEnabled] DEFAULT ((1)),
[reportDatabaseID] [int] NULL,
[isNHSCustomer] [bit] NOT NULL CONSTRAINT [DF_registeredusers_isNHSCustomer] DEFAULT ((0)),
[contactHelpDeskAllowed] [bit] NOT NULL CONSTRAINT [DF_registeredusers_contactHelpDeskAllowed] DEFAULT ((1)),
[postcodeAnywhereKey] [nvarchar] (200) NULL,
[licencedUsers] [nvarchar] (150) NULL,
[mapsEnabled] [bit] NOT NULL CONSTRAINT [DF_registeredusers_mapsEnabled] DEFAULT ((0)),
[ReceiptServiceEnabled] [bit] NOT NULL CONSTRAINT [DF__registere__Recei__423C0E05] DEFAULT ((0)),
[SingleSignOnEnabled] [bit] NOT NULL CONSTRAINT [DF_registeredusers_singleSignOnEnabled] DEFAULT ((0)),
[licenceType] [tinyint] NULL,
[annualContract] [bit] NOT NULL CONSTRAINT [DF_registeredusers_annualContract] DEFAULT ((0)),
[renewalDate] [nvarchar] (5) NULL,
[contactEmail] [nvarchar] (500) NULL,
[addressLookupProvider] [tinyint] NOT NULL CONSTRAINT [DF_registeredusers_addressLookupProvider] DEFAULT ((0)),
[addressLookupsChargeable] [bit] NOT NULL CONSTRAINT [DF_registeredusers_addressLookupsChargeable] DEFAULT ((0)),
[addressLookupPsmaAgreement] [bit] NOT NULL CONSTRAINT [DF_registeredusers_addressLookupPsmaAgreement] DEFAULT ((0)),
[addressInternationalLookupsAndCoordinates] [bit] NOT NULL CONSTRAINT [DF_registeredusers_addressDistanceLookupEnableCoordinates] DEFAULT ((0)),
[addressLookupsRemaining] [int] NOT NULL CONSTRAINT [DF_registeredusers_addressLookupsRemaining] DEFAULT ((0)),
[addressDistanceLookupsRemaining] [int] NOT NULL CONSTRAINT [DF_registeredusers_addressDistanceLookupsRemaining] DEFAULT ((0)),
[chargedinArrearsForExcess] [tinyint] NOT NULL CONSTRAINT [DF__registere__charg__695D856D] DEFAULT ((0)),
[pricePerExcessClaim] [money] NOT NULL CONSTRAINT [DF__registere__price__6B45CDDF] DEFAULT ((0)),
[providerId] [int] NULL,
[accountManagerId] [int] NULL,
[ValidationServiceEnabled] [bit] NOT NULL CONSTRAINT [DF__registere__Valid__2B3255BF] DEFAULT ((0)),
[DaysToWaitBeforeSentEnvelopeIsMissing] [int] NULL CONSTRAINT [DF__registere__DaysT__2D1A9E31] DEFAULT (NULL),
[PaymentServiceEnabled] [bit] NOT NULL CONSTRAINT [DF__registere__Payme__22680594] DEFAULT ((0)),
[FundLimit] [decimal] (18, 2) NULL CONSTRAINT [DF_registeredusers_FundLimit] DEFAULT ((0)),
[DVLALookUpKey] [nvarchar] (50) NULL,
[PostcodeAnywherePaymentServiceKey] [nvarchar] (200) NULL
)
GO
PRINT N'Creating primary key [PK_registeredusers] on [dbo].[registeredusers]'
GO
ALTER TABLE [dbo].[registeredusers] ADD CONSTRAINT [PK_registeredusers] PRIMARY KEY CLUSTERED  ([accountid])
GO
PRINT N'Adding constraints to [dbo].[registeredusers]'
GO
ALTER TABLE [dbo].[registeredusers] ADD CONSTRAINT [UQ_RegisteredUsers_CompanyId] UNIQUE NONCLUSTERED  ([companyid])
GO
PRINT N'Creating [dbo].[GetAccounts]'
GO

CREATE PROCEDURE [dbo].[GetAccounts]
AS
SELECT
accountid, companyname, companyid, contact, nousers, accounttype, expiry, dbserver, dbname, dbusername, dbpassword, archived,
quickEntryFormsEnabled, employeeSearchEnabled, hotelReviewsEnabled, advancesEnabled, postcodeAnyWhereEnabled, corporateCardsEnabled,
reportDatabaseID, isNHSCustomer, contactHelpDeskAllowed, postcodeAnywhereKey, licencedUsers, mapsEnabled,
ReceiptServiceEnabled, addressLookupProvider, addressLookupsChargeable, addressLookupPsmaAgreement, addressInternationalLookupsAndCoordinates,
addressLookupsRemaining, addressDistanceLookupsRemaining, licenceType, annualContract, renewalDate, contactEmail, ValidationServiceEnabled, DaysToWaitBeforeSentEnvelopeIsMissing
,PaymentServiceEnabled,[DVLALookUpKey], postCodeAnywherePaymentServiceKey, SingleSignOnEnabled, FundLimit FROM
dbo.registeredusers;
GO
PRINT N'Creating [dbo].[ExpenseValidationThresholds]'
GO
CREATE TABLE [dbo].[ExpenseValidationThresholds]
(
[ThresholdId] [int] NOT NULL IDENTITY(1, 1),
[Label] [nvarchar] (100) NOT NULL,
[LowerBound] [decimal] (18, 2) NULL,
[UpperBound] [decimal] (18, 2) NULL
)
GO
PRINT N'Creating primary key [PK_ExpenseValidationThresholds] on [dbo].[ExpenseValidationThresholds]'
GO
ALTER TABLE [dbo].[ExpenseValidationThresholds] ADD CONSTRAINT [PK_ExpenseValidationThresholds] PRIMARY KEY CLUSTERED  ([ThresholdId])
GO
PRINT N'Creating [dbo].[ExpenseValidationReasonResultMapping]'
GO
CREATE TABLE [dbo].[ExpenseValidationReasonResultMapping]
(
[ThresholdId] [int] NOT NULL,
[CriterionId] [int] NOT NULL,
[ReasonId] [int] NOT NULL,
[ResultStatus] [int] NOT NULL
)
GO
PRINT N'Creating primary key [PK__ExpenseV__6E649CAB04D7A2AD] on [dbo].[ExpenseValidationReasonResultMapping]'
GO
ALTER TABLE [dbo].[ExpenseValidationReasonResultMapping] ADD CONSTRAINT [PK__ExpenseV__6E649CAB04D7A2AD] PRIMARY KEY CLUSTERED  ([ThresholdId], [CriterionId], [ReasonId])
GO
PRINT N'Creating [dbo].[DetermineExpenseValidationResult]'
GO

-- SQL Work Item ID: 67560 (Sequence 2442)

CREATE PROCEDURE [dbo].[DetermineExpenseValidationResult]
 @criterionId INT,
 @reasonId INT,
 @isVAT BIT,
 @total DECIMAL(18,2) = NULL
AS
BEGIN
 SET NOCOUNT ON;
 DECLARE @result int;
 DECLARE @thresholdId int;
 
 IF (@isVAT = 1) SELECT @thresholdId = ThresholdId FROM ExpenseValidationThresholds WHERE (LowerBound IS NOT NULL AND UpperBound IS NOT NULL) AND (@total >= LowerBound AND @total <= UpperBound)
 ELSE SET @thresholdId = 1
 
 print(@thresholdId)
 
 SELECT @result = ResultStatus FROM ExpenseValidationReasonResultMapping WHERE ThresholdId = @thresholdId AND CriterionId = @criterionId AND ReasonId = @reasonId
 
 RETURN @result;
END


GO
PRINT N'Creating [dbo].[EnvelopeHistory]'
GO
CREATE TABLE [dbo].[EnvelopeHistory]
(
[EnvelopeHistoryId] [int] NOT NULL IDENTITY(1, 1),
[EnvelopeId] [int] NULL,
[EnvelopeStatus] [int] NOT NULL,
[Data] [nvarchar] (500) NULL,
[ModifiedOn] [datetime] NOT NULL,
[ModifiedBy] [int] NOT NULL
)
GO
PRINT N'Creating primary key [PK_EnvelopeHistory] on [dbo].[EnvelopeHistory]'
GO
ALTER TABLE [dbo].[EnvelopeHistory] ADD CONSTRAINT [PK_EnvelopeHistory] PRIMARY KEY CLUSTERED  ([EnvelopeHistoryId])
GO
PRINT N'Creating [dbo].[Envelopes]'
GO
CREATE TABLE [dbo].[Envelopes]
(
[EnvelopeId] [int] NOT NULL IDENTITY(1, 1),
[AccountId] [int] NULL,
[ClaimId] [int] NULL,
[EnvelopeNumber] [nvarchar] (10) NOT NULL,
[CRN] [nvarchar] (12) NULL,
[EnvelopeStatus] [tinyint] NOT NULL,
[EnvelopeType] [int] NULL,
[DateIssuedToClaimant] [datetime] NULL,
[DateAssignedToClaim] [datetime] NULL,
[DateReceived] [datetime] NULL,
[DateAttachCompleted] [datetime] NULL,
[DateDestroyed] [datetime] NULL,
[OverpaymentCharge] [decimal] (16, 2) NULL,
[PhysicalStateProofUrl] [nvarchar] (100) NULL,
[LastModifiedBy] [int] NOT NULL,
[DeclaredLostInPost] [bit] NOT NULL CONSTRAINT [DF__Envelopes__Decla__2E0EC26A] DEFAULT ((0))
)
GO
PRINT N'Creating primary key [PK_Envelopes] on [dbo].[Envelopes]'
GO
ALTER TABLE [dbo].[Envelopes] ADD CONSTRAINT [PK_Envelopes] PRIMARY KEY CLUSTERED  ([EnvelopeId])
GO
PRINT N'Creating index [Index_Envelopes_EnvelopeCRN_FreeText] on [dbo].[Envelopes]'
GO
CREATE NONCLUSTERED INDEX [Index_Envelopes_EnvelopeCRN_FreeText] ON [dbo].[Envelopes] ([CRN])
GO
PRINT N'Creating index [Index_Envelopes_EnvelopeNumber] on [dbo].[Envelopes]'
GO
CREATE NONCLUSTERED INDEX [Index_Envelopes_EnvelopeNumber] ON [dbo].[Envelopes] ([EnvelopeNumber])
GO
PRINT N'Creating trigger [dbo].[EnvelopeInsert] on [dbo].[Envelopes]'
GO

CREATE TRIGGER [dbo].[EnvelopeInsert] ON [dbo].[Envelopes] FOR INSERT
AS
SET CONCAT_NULL_YIELDS_NULL OFF;
 insert into EnvelopeHistory (EnvelopeId, EnvelopeStatus, Data, ModifiedBy, ModifiedOn)
 select i.EnvelopeId, 1, ('Generated: ' + i.EnvelopeNumber + ', Type: ' + convert(varchar, i.EnvelopeType) + ', AccountId: ' + convert(varchar, i.AccountId)), i.LastModifiedBy, GETUTCDATE()
 from inserted i;
SET CONCAT_NULL_YIELDS_NULL ON; 
GO
PRINT N'Creating [dbo].[EnvelopesPhysicalStates]'
GO
CREATE TABLE [dbo].[EnvelopesPhysicalStates]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[EnvelopeId] [int] NOT NULL,
[EnvelopePhysicalStateId] [int] NOT NULL
)
GO
PRINT N'Creating primary key [PK_EnvelopesPhysicalStates] on [dbo].[EnvelopesPhysicalStates]'
GO
ALTER TABLE [dbo].[EnvelopesPhysicalStates] ADD CONSTRAINT [PK_EnvelopesPhysicalStates] PRIMARY KEY CLUSTERED  ([Id])
GO
PRINT N'Creating [dbo].[EnvelopePhysicalState]'
GO
CREATE TABLE [dbo].[EnvelopePhysicalState]
(
[EnvelopePhysicalStateId] [int] NOT NULL IDENTITY(1, 1),
[Details] [nvarchar] (100) NOT NULL
)
GO
PRINT N'Creating primary key [PK_EnvelopePhysicalState] on [dbo].[EnvelopePhysicalState]'
GO
ALTER TABLE [dbo].[EnvelopePhysicalState] ADD CONSTRAINT [PK_EnvelopePhysicalState] PRIMARY KEY CLUSTERED  ([EnvelopePhysicalStateId])
GO
PRINT N'Creating [dbo].[UpdateEnvelopesPhysicalStates]'
GO


CREATE PROCEDURE [dbo].[UpdateEnvelopesPhysicalStates]
 @envelopeId int,
 @physicalStateIds IntPK readonly
AS
BEGIN
 DELETE FROM EnvelopesPhysicalStates WHERE EnvelopeId = @envelopeId AND EnvelopePhysicalStateId NOT IN (SELECT c1 FROM @physicalStateIds)
 INSERT INTO EnvelopesPhysicalStates (EnvelopeId, EnvelopePhysicalStateId)
 SELECT @envelopeId, c1 FROM @physicalStateIds 
  WHERE (c1 IN (SELECT EnvelopePhysicalStateId FROM EnvelopePhysicalState) 
    AND c1 NOT IN (SELECT EnvelopePhysicalStateId FROM EnvelopesPhysicalStates WHERE EnvelopeId = @envelopeId))
 return @envelopeId
END

GO
PRINT N'Creating [dbo].[fields_base]'
GO
CREATE TABLE [dbo].[fields_base]
(
[fieldid] [uniqueidentifier] NOT NULL CONSTRAINT [DF_fields_base_fieldid_new_1] DEFAULT (newid()),
[field] [nvarchar] (250) NOT NULL,
[fieldtype] [nvarchar] (2) NOT NULL,
[description] [nvarchar] (1000) NULL,
[comment] [nvarchar] (4000) NULL,
[normalview] [bit] NOT NULL CONSTRAINT [DF_fields_normalview] DEFAULT ((1)),
[idfield] [bit] NOT NULL CONSTRAINT [DF_fields_idfield] DEFAULT ((0)),
[genlist] [bit] NOT NULL CONSTRAINT [DF_fields_genlist] DEFAULT ((0)),
[width] [int] NOT NULL CONSTRAINT [DF_fields_width] DEFAULT ((0)),
[cantotal] [bit] NOT NULL CONSTRAINT [DF_fields_cantotal] DEFAULT ((0)),
[printout] [bit] NOT NULL CONSTRAINT [DF_fields_printout] DEFAULT ((0)),
[valuelist] [bit] NOT NULL CONSTRAINT [DF_fields_valuelist] DEFAULT ((0)),
[allowimport] [bit] NOT NULL CONSTRAINT [DF_fields_allowimport] DEFAULT ((0)),
[mandatory] [bit] NOT NULL CONSTRAINT [DF_fields_mandatory] DEFAULT ((0)),
[amendedon] [datetime] NOT NULL CONSTRAINT [DF_fields_amendedon] DEFAULT (getdate()),
[lookuptable] [uniqueidentifier] NULL,
[lookupfield] [uniqueidentifier] NULL,
[useforlookup] [bit] NOT NULL CONSTRAINT [DF_fields_useforlookup] DEFAULT ((0)),
[workflowUpdate] [bit] NOT NULL CONSTRAINT [DF_fields_base_workflowUpdate] DEFAULT ((0)),
[workflowSearch] [bit] NOT NULL CONSTRAINT [DF_fields_base_workflowSearch] DEFAULT ((0)),
[length] [int] NOT NULL CONSTRAINT [DF_fields_base_length] DEFAULT ((0)),
[tableid] [uniqueidentifier] NOT NULL,
[viewgroupid] [uniqueidentifier] NULL,
[relabel] [bit] NOT NULL CONSTRAINT [DF__fields_ba__relab__5A8C5FCB] DEFAULT ((0)),
[relabel_param] [nvarchar] (150) NULL,
[allowDuplicateChecking] [bit] NOT NULL CONSTRAINT [DF_fields_base_allowDuplicateChecking] DEFAULT ((0)),
[classPropertyName] [nvarchar] (500) NULL,
[isForeignKey] [bit] NOT NULL CONSTRAINT [DF_fields_base_isForeignKey] DEFAULT ((0)),
[relatedTable] [uniqueidentifier] NULL,
[associatedFieldForDuplicateChecking] [uniqueidentifier] NULL,
[DuplicateCheckingSource] [tinyint] NULL,
[DuplicateCheckingCalculation] [tinyint] NULL,
[friendlyNameFrom] [nvarchar] (100) NULL,
[friendlyNameTo] [nvarchar] (100) NULL,
[TreeGroup] [uniqueidentifier] NULL
)
GO
PRINT N'Creating primary key [PK_fields_base] on [dbo].[fields_base]'
GO
ALTER TABLE [dbo].[fields_base] ADD CONSTRAINT [PK_fields_base] PRIMARY KEY NONCLUSTERED  ([fieldid])
GO
PRINT N'Creating index [IX_fields_base] on [dbo].[fields_base]'
GO
CREATE NONCLUSTERED INDEX [IX_fields_base] ON [dbo].[fields_base] ([tableid], [field])
GO
PRINT N'Creating [dbo].[ExpenseValidationCriteria]'
GO
CREATE TABLE [dbo].[ExpenseValidationCriteria]
(
[CriterionId] [int] NOT NULL IDENTITY(1, 1),
[AccountId] [int] NULL,
[FieldId] [uniqueidentifier] NULL,
[Requirements] [nvarchar] (400) NOT NULL,
[SubcatId] [int] NULL,
[Enabled] [bit] NOT NULL CONSTRAINT [DF__ExpenseVa__Enabl__67473FC6] DEFAULT ((1)),
[FraudulentIfFailsVAT] [bit] NOT NULL CONSTRAINT [DF__ExpenseVa__Fraud__0012ED90] DEFAULT ((0)),
[FriendlyMessageFoundAndMatched] [nvarchar] (200) NOT NULL CONSTRAINT [DF__ExpenseVa__Frien__0B84A03C] DEFAULT ('Message'),
[FriendlyMessageFoundNotMatched] [nvarchar] (200) NOT NULL CONSTRAINT [DF__ExpenseVa__Frien__0C78C475] DEFAULT ('Message'),
[FriendlyMessageFoundNotReadable] [nvarchar] (200) NOT NULL CONSTRAINT [DF__ExpenseVa__Frien__0D6CE8AE] DEFAULT ('Message'),
[FriendlyMessageNotFound] [nvarchar] (200) NOT NULL CONSTRAINT [DF__ExpenseVa__Frien__0E610CE7] DEFAULT ('Message')
)
GO
PRINT N'Creating primary key [PK_ExpenseValidationCriteria] on [dbo].[ExpenseValidationCriteria]'
GO
ALTER TABLE [dbo].[ExpenseValidationCriteria] ADD CONSTRAINT [PK_ExpenseValidationCriteria] PRIMARY KEY CLUSTERED  ([CriterionId])
GO
PRINT N'Creating [dbo].[SaveExpenseValidationCriterion]'
GO


CREATE PROCEDURE [dbo].[SaveExpenseValidationCriterion]
 @id INT = 0,
 @fieldId UNIQUEIDENTIFIER = null,
 @accountId INT = null,
 @subcatId INT = null,
 @enabled bit = 1,
 @requirements NVARCHAR(4000),
 @fraudulentIfFailsVAT bit,
 @friendlyMessageFoundAndMatched NVARCHAR(200) = '',
 @friendlyMessageFoundNotMatched NVARCHAR(200) = '',
 @friendlyMessageFoundNotReadable NVARCHAR(200) = '',
 @friendlyMessageNotFound NVARCHAR(200) = ''
AS
BEGIN
 SET NOCOUNT ON;
 IF (@accountId IS NOT NULL AND (SELECT COUNT(accountid) FROM registeredusers WHERE accountid = @accountId) = 0) RETURN -2;
 IF (@fieldId IS NOT NULL AND (SELECT COUNT(fieldid) FROM fields_base WHERE fieldid = @fieldId) = 0) RETURN -3;
 IF (@id = 0)
  BEGIN
   INSERT INTO [dbo].[ExpenseValidationCriteria] ( FieldId, Requirements, AccountId, SubcatId, Enabled, FraudulentIfFailsVAT, FriendlyMessageFoundAndMatched, FriendlyMessageFoundNotMatched, FriendlyMessageFoundNotReadable, FriendlyMessageNotFound)
   VALUES ( @fieldId, @requirements, @accountId, @subcatId, @enabled, @fraudulentIfFailsVAT, @friendlyMessageFoundAndMatched, @friendlyMessageFoundNotMatched, @friendlyMessageFoundNotReadable, @friendlyMessageNotFound );
   RETURN SCOPE_IDENTITY();
  END
 ELSE
  BEGIN
   IF NOT EXISTS (SELECT CriterionId FROM [dbo].[ExpenseValidationCriteria] WHERE CriterionId = @id) RETURN -1;
   UPDATE [dbo].[ExpenseValidationCriteria]
   SET 
    FieldId = @fieldId,
    Requirements = @requirements,
    AccountId = @accountId,
    SubcatId = @subcatId,
    Enabled = @enabled,
    FraudulentIfFailsVAT = @fraudulentIfFailsVAT,
    FriendlyMessageFoundAndMatched = @friendlyMessageFoundAndMatched,
    FriendlyMessageFoundNotMatched = @friendlyMessageFoundNotMatched,
    FriendlyMessageFoundNotReadable = @friendlyMessageFoundNotReadable,
    FriendlyMessageNotFound = @friendlyMessageNotFound
   WHERE CriterionId = @id;
   RETURN @id;
  END
END
GO
PRINT N'Creating [dbo].[databases]'
GO
CREATE TABLE [dbo].[databases]
(
[databaseID] [int] NOT NULL IDENTITY(11, 1),
[hostname] [nvarchar] (50) NOT NULL,
[receiptpath] [nvarchar] (150) NULL,
[cardtemplatepath] [nvarchar] (150) NULL,
[offlineupdatepath] [nvarchar] (150) NULL,
[policyfilepath] [nvarchar] (150) NULL,
[cardocumentpath] [nvarchar] (150) NULL,
[logopath] [nvarchar] (150) NULL,
[attachmentspath] [nvarchar] (150) NULL
)
GO
PRINT N'Creating primary key [PK_databases] on [dbo].[databases]'
GO
ALTER TABLE [dbo].[databases] ADD CONSTRAINT [PK_databases] PRIMARY KEY CLUSTERED  ([databaseID])
GO
PRINT N'Creating [dbo].[authenticateForKnowledgeBase]'
GO




-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[authenticateForKnowledgeBase] 
	@companyID nvarchar(50),
	@username nvarchar(50),
	@password nvarchar(250)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	
	
	declare @count int
	
    -- Insert statements for procedure here
	set @count = (select count(*) from registeredusers where companyid = @companyID)
	if (@count = 0)
		return -1
		
	declare @hostname nvarchar(50)
	declare @dbname nvarchar(100)
	select @hostname = hostname, @dbname = dbname from registeredusers inner join databases on databases.databaseid = registeredusers.dbserver where companyid = @companyid
	declare @sql nvarchar(max)
	DECLARE @ParmDefinition nvarchar(500);
	if @hostname = '192.168.101.127' --must be changed to the hostname of this server
				set @hostname = ''
			else
				set @hostname = '[' + @hostname + '].';
		print @dbname
		
	set @parmdefinition = '@inUsername nvarchar(50), @inPassword nvarchar(250), @countOut int output'
	set @sql = 'select @countOut = count(*) from ' + @hostname + '[' + @dbname + '].dbo.employees where username = @inUsername and password = @inPassword '

	execute sp_executesql @sql, @parmdefinition, @inUsername = @username, @inPassword = @password, @countOut = @count output

	return @count
END




GO
PRINT N'Creating [dbo].[DebugLog]'
GO
CREATE TABLE [dbo].[DebugLog]
(
[DateTime] [datetime] NOT NULL,
[Source] [nvarchar] (100) NOT NULL,
[Message] [nvarchar] (max) NULL,
[AccountId] [int] NULL,
[UserEmployeeId] [int] NULL,
[DeligateEmployeeId] [int] NULL,
[Uri] [nvarchar] (max) NULL,
[Body] [nvarchar] (max) NULL,
[Headers] [nvarchar] (max) NULL,
[PostData] [nvarchar] (max) NULL,
[Cookies] [nvarchar] (max) NULL,
[ServerVariables] [nvarchar] (max) NULL,
[AdditionalData] [nvarchar] (max) NULL,
[StackTrace] [nvarchar] (max) NOT NULL
)
GO
PRINT N'Creating index [IX_DebugLog] on [dbo].[DebugLog]'
GO
CREATE CLUSTERED INDEX [IX_DebugLog] ON [dbo].[DebugLog] ([DateTime] DESC, [Source])
GO
PRINT N'Creating [dbo].[AddDebugLog]'
GO

create procedure [dbo].[AddDebugLog]
(
	@Source				nvarchar(100),
	@Message			nvarchar(MAX),
	@AccountId			int,
	@UserEmployeeId		int,
	@DeligateEmployeeId	int,
	@Uri				nvarchar(MAX),
	@Body				nvarchar(MAX),
	@Headers			nvarchar(MAX),
	@PostData			nvarchar(MAX),
	@Cookies			nvarchar(MAX),
	@ServerVariables	nvarchar(MAX),
	@AdditionalData		nvarchar(MAX),
	@StackTrace			nvarchar(MAX)
)
as
	insert into DebugLog
	values
	(
		getutcdate(),
		@Source,
		@Message,
		@AccountId,
		@UserEmployeeId,
		@DeligateEmployeeId,
		@Uri,
		@Body,
		@Headers,
		@PostData,
		@Cookies,
		@ServerVariables,
		@AdditionalData,
		@StackTrace
	)
GO
PRINT N'Creating [dbo].[accountManagers]'
GO
CREATE TABLE [dbo].[accountManagers]
(
[accountManagerId] [int] NOT NULL IDENTITY(1, 1),
[name] [nvarchar] (100) NOT NULL,
[email] [nvarchar] (500) NOT NULL
)
GO
PRINT N'Creating primary key [PK_accountManagers] on [dbo].[accountManagers]'
GO
ALTER TABLE [dbo].[accountManagers] ADD CONSTRAINT [PK_accountManagers] PRIMARY KEY CLUSTERED  ([accountManagerId])
GO
PRINT N'Creating [dbo].[GetAccountSummary]'
GO



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetAccountSummary] 
	@accountId int
AS
BEGIN
declare @companyname nvarchar(250)
	declare @quantity int
	declare @licencetype nvarchar(10)
	declare @annualcontract nvarchar(10)
	declare @usedquantity int
	declare @hostname nvarchar(50)
	declare @dbname nvarchar(100)
	declare @percentageused decimal(18,2)
	declare @accountManagerName nvarchar(100)
	declare @clientContact nvarchar(50)
	declare @clientEmail nvarchar(500)
	declare @renewalDate nvarchar(5)
	declare @chargedInArrearsForExcess bit
	declare @pricePerExcessClaim money
	

	CREATE TABLE #billingStats (
	accountid int,
	companyname nvarchar(100),
	quantity int,
	licencetype nvarchar(10),
	annualcontract nvarchar(10),
	cumulativeQuantity int,
	averageYearlyQuantity int,
	averagePercentUsed decimal(18,2),
	accountManagerName nvarchar(100),
	clientContact nvarchar(50),
	clientEmail nvarchar(500),
	renewalDate nvarchar(5),
	chargedInArrearsForExcess bit,
	pricePerExcessClaim money
	
)

select @companyname = companyname, @quantity = nousers,@licencetype = (case licenceType when 1 then 'Claims' when 2 then 'Claimants' end), @annualcontract = (case annualContract when 0 then 'No' else 'Yes' end), @hostname = databases.hostname, @dbname = dbname, @accountManagerName = accountManagers.name, @clientcontact = contact, @clientemail = contactemail, @renewaldate = renewalDate, @chargedInArrearsForExcess = chargedinArrearsForExcess, @pricePerExcessClaim = pricePerExcessClaim
	from registeredusers inner join databases on databases.databaseid = registeredusers.dbserver
	left join accountManagers on accountManagers.accountManagerId = registeredusers.accountManagerId
	where accountid = @accountId
	--get the quantity from client database
			declare @sql nvarchar(max)
			if @hostname = '172.24.16.209'
						set @hostname = ''
					else
						set @hostname = '[' + @hostname + '].';
			declare @delimiterpos int
			declare @day nvarchar(2)
			declare @month nvarchar(2)
			

			set @delimiterpos = charindex('/',@renewaldate)

			set @day = substring(@renewaldate, 1, @delimiterpos-1)
			set @month = substring(@renewaldate, @delimiterpos+1, len(@renewaldate))

		
			declare @startDate datetime
			set @startDate = convert(varchar,year(GETDATE())) + '/' + @month + '/' + @day
			if @startDate > getdate()
				set @startDate = dateadd(yyyy,-1,@startDate)
			
			
			declare @parmdef nvarchar(max)
			
	
			set @parmdef = '@startDateIn datetime, @cumulativeQuantity int output'
			if @licencetype = 'Claims'
				set @sql = 'select @cumulativeQuantity = count(claimid) from ' + @hostname + '[' + @dbname + '].dbo.claims where datesubmitted >= @startDateIn'
			else if @licencetype = 'Claimants'
				set @sql = 'select @cumulativeQuantity = count(distinct employeeid) from ' + @hostname + '[' + @dbname + '].dbo.claims where datesubmitted >= @startDateIn'	
			declare @count int
			declare @cumulativeQuantity int
			
			exec sp_executesql @sql, @parmdef, @startDateIn = @startDate, @cumulativeQuantity = @count output
			if @licencetype = 'Claims'
				set @sql = 'select @yearlyAverageOut = avg(monthlyCount) from (select count(claimid) as monthlyCount from ' + @hostname + '[' + @dbname + '].dbo.claims where datesubmitted >= @startDateIn group by month(datesubmitted)) b'
			else if @licencetype = 'Claimants'
				set @sql = 'select @yearlyAverageOut = avg(monthlyCount) from (select count(distinct employeeid) as monthlyCount from ' + @hostname + '[' + @dbname + '].dbo.claims where datesubmitted >= @startDateIn group by month(datesubmitted)) b'
			
			declare @yearlyAverage int
			declare @yearlyAverageOut int
			set @parmdef = '@startDateIn datetime, @yearlyAverageOut int output'
			exec sp_executesql @sql, @parmdef, @startDateIn = @startDate, @yearlyAverageOut = @yearlyAverage output			
			insert into #billingStats (accountid, companyname, quantity, licencetype, annualcontract, cumulativequantity, averageYearlyQuantity, averagePercentUsed, accountManagerName, clientContact, clientEmail, renewaldate, chargedinarrearsforexcess, priceperexcessclaim)
				values (@accountid, @companyname, @quantity, @licencetype, @annualcontract, @count, @yearlyAverage,0,@accountmanagername, @clientcontact, @clientemail, @renewaldate, @chargedinarrearsforexcess, @priceperexcessclaim)
				
			select * from #billingstats
	
END



GO
PRINT N'Creating [dbo].[moduleElementBase]'
GO
CREATE TABLE [dbo].[moduleElementBase]
(
[moduleID] [int] NOT NULL CONSTRAINT [DF_module_element_base_moduleID] DEFAULT ((2)),
[elementID] [int] NOT NULL
)
GO
PRINT N'Creating primary key [PK_moduleElementBase] on [dbo].[moduleElementBase]'
GO
ALTER TABLE [dbo].[moduleElementBase] ADD CONSTRAINT [PK_moduleElementBase] PRIMARY KEY CLUSTERED  ([moduleID], [elementID])
GO
PRINT N'Creating [dbo].[accountsLicencedElements]'
GO
CREATE TABLE [dbo].[accountsLicencedElements]
(
[accountID] [int] NOT NULL,
[elementID] [int] NOT NULL
)
GO
PRINT N'Creating primary key [PK_accountsLicencedElements] on [dbo].[accountsLicencedElements]'
GO
ALTER TABLE [dbo].[accountsLicencedElements] ADD CONSTRAINT [PK_accountsLicencedElements] PRIMARY KEY CLUSTERED  ([accountID], [elementID])
GO
PRINT N'Creating [dbo].[GetLicencedElementsByAccountId]'
GO

CREATE PROCEDURE [dbo].[GetLicencedElementsByAccountId]
	@accountID int
AS
SELECT
	moduleID, moduleElementBase.elementID
FROM
	moduleElementBase
		join accountsLicencedElements on moduleElementBase.elementID = accountsLicencedElements.elementID
									and accountsLicencedElements.accountID = @accountID
ORDER by
	moduleID;
	
RETURN 0

GO
PRINT N'Creating [dbo].[GetFundLimit]'
GO
Create PROCEDURE [dbo].[GetFundLimit]
@AccountId int
AS
begin
select fundlimit from registeredusers where accountid=@AccountId
end
GO
PRINT N'Creating [dbo].[getSchedules]'
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[getSchedules] 
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    --declare table to hold results

if OBJECT_ID('tempdb..#schedules','u') IS NOT NULL
	drop table #schedules


create table #schedules
(
	dbname nvarchar(100),
	scheduletype nvarchar(50),
	runday int,
	scheduleid int,
	reportname nvarchar(max),
	runtime datetime
)



declare @sql nvarchar(max)


declare @hostname nvarchar(50)
declare @dbname nvarchar(100)

declare employees_cursor cursor for select hostname, dbname from registeredusers inner join databases on databases.databaseid = registeredusers.dbserver where registeredusers.archived = 0

open employees_cursor

fetch next from employees_cursor into @hostname, @dbname
	while @@fetch_status = 0
		BEGIN
			if @hostname = '172.24.16.209'
				set @hostname = ''
			else
				set @hostname = '[' + @hostname + '].';


--monthly reports
set @sql = N'declare @scheduleid int ' +
	' declare @reportname nvarchar(max) ' +
	' declare @calendardays nvarchar(max) ' +
	' declare @startdate datetime ' + 
	' declare @enddate datetime ' +
	' declare @starttime datetime ' +
	' declare @month int ' +
	'declare @pos int ' +
	'declare @day nvarchar(10) ' +
	'set @pos = 0 ' + 
	'set @startdate = cast(datepart(yyyy,getdate()) as nvarchar(5))  + ''/'' + cast(datepart(MM,getdate()) as nvarchar(5)) + ''/01'' ' +
	'set @enddate = dateadd(MM,1,@startdate) ' +
	'set @enddate = dateadd(dd,-1,@enddate) ' +
	'set @month = datepart(MM,@startdate) ' +
	



	
	'declare calendar_cursor cursor for select scheduled_reports.scheduleid, reports.reportname, scheduled_reports.calendar_days, scheduled_reports.starttime from ' + @hostname + '[' + @dbname + '].dbo.scheduled_reports inner join  ' + @hostname + '[' + @dbname + '].dbo.reports on reports.reportid = scheduled_reports.reportid inner join  ' + @hostname + '[' + @dbname + '].dbo.scheduled_months on scheduled_reports.scheduleid = scheduled_months.scheduleid where scheduletype = 3 and scheduled_months.[month] = @month and calendar_days is not null and calendar_days <> '''' ' +

	'open calendar_cursor ' +

	'fetch next from calendar_cursor into @scheduleid, @reportname, @calendardays, @starttime ' +
	'while @@fetch_status = 0 ' +
		'BEGIN ' +
			' set @pos = 0; ' +
			'while @pos < len(@calendardays) ' +
				'begin ' +
					'if charindex('','',@calendardays) > 0 ' +
						'begin ' +
							'insert into #schedules (dbname, scheduletype, runday, scheduleid, reportname, runtime) values (''' + @dbname + ''', ''Monthly by calendar'', 0, @scheduleid, ''has multiple days'' + @calendardays + @reportname, @starttime) ' +
						'end ' +
					'else ' +
						'begin ' +
							'if charindex(''-'',@calendardays) > 0 ' +
								'begin ' +
									'insert into #schedules (dbname, scheduletype, runday, scheduleid, reportname, runtime) values (''' + @dbname + ''', ''Monthly by calendar'', 0, @scheduleid, ''has range days'' + @calendardays + @reportname, @starttime) ' +
								'end ' +
							'else ' +
								'begin ' +
									'insert into #schedules (dbname, scheduletype, runday, scheduleid, reportname, runtime) values (''' + @dbname + ''', ''Monthly by calendar'', @calendardays, @scheduleid, @reportname, @starttime) ' +
									'break; ' +
								'end ' +
						'end ' +
				'end ' +
			'fetch next from calendar_cursor into @scheduleid, @reportname, @calendardays, @starttime ' +
		'end ' +
	'close calendar_cursor ' +
'deallocate calendar_cursor ' + 

--weeks of the month
'declare @weekday int ' +
'declare @week int ' +
'declare @startday int ' +
'declare @loopStartDate datetime ' +
'set @startday = datepart(dw,@startdate) ' +
	
'declare calendar_cursor cursor for select scheduled_reports.scheduleid, reports.reportname, scheduled_reports.[week], scheduled_days.[day], scheduled_reports.starttime from  ' + @hostname + '[' + @dbname + '].dbo.scheduled_reports inner join   ' + @hostname + '[' + @dbname + '].dbo.reports on reports.reportid = scheduled_reports.reportid inner join   ' + @hostname + '[' + @dbname + '].dbo.scheduled_months on scheduled_reports.scheduleid = scheduled_months.scheduleid inner join  ' + @hostname + '[' + @dbname + '].dbo.scheduled_days on scheduled_days.scheduleid = scheduled_reports.scheduleid where scheduletype = 3 and scheduled_months.[month] = @month and (calendar_days is null or (calendar_days is not null and calendar_days = '''')) ' +
'open calendar_cursor ' +

'	fetch next from calendar_cursor into @scheduleid, @reportname, @week, @weekday, @starttime ' +
'	while @@fetch_status = 0 ' +
'		BEGIN ' +
'		set @weekday = @weekday + 1 ' +

'			set @loopStartDate = @startdate ' +
'			set @loopstartdate = dateadd(dd, ((7 * @week) - 7), @startdate) ' +
'			while datepart(dw,@loopstartdate) <> @weekday ' +
'				set @loopstartdate = dateadd(dd,1,@loopstartdate) ' +		
			'insert into #schedules (dbname, scheduletype, runday, scheduleid, reportname, runtime) values (''' + @dbname + ''', ''Monthly by week'', datepart(dd,@loopstartdate), @scheduleid, @reportname, @starttime) ' +
'			fetch next from calendar_cursor into @scheduleid, @reportname, @week, @weekday, @starttime ' +
'		end ' +
'	close calendar_cursor ' +
'	deallocate calendar_cursor '



exec (@sql)

--daily non repeating
set @sql = N'declare @scheduleid int ' +
	' declare @reportname nvarchar(max) ' +
	' declare @calendardays nvarchar(max) ' +
	' declare @startdate datetime ' + 
	' declare @enddate datetime ' +
	' declare @starttime datetime ' +
	' declare @month int ' +
	'declare @pos int ' +
	'declare @day nvarchar(10) ' +
	'set @pos = 0 ' + 
	'set @startdate = cast(datepart(yyyy,getdate()) as nvarchar(5))  + ''/'' + cast(datepart(MM,getdate()) as nvarchar(5)) + ''/01'' ' +
	'set @enddate = dateadd(MM,1,@startdate) ' +
	'set @enddate = dateadd(dd,-1,@enddate) ' +
	'set @month = datepart(MM,@startdate) ' +
'declare @weekday int ' +
'declare @week int ' +
'declare @startday int ' +
'declare @loopStartDate datetime ' +
'set @startday = datepart(dw,@startdate) ' +
'	declare calendar_cursor cursor for  select scheduled_reports.scheduleid, reports.reportname, scheduled_days.[day], scheduled_reports.starttime from  ' + @hostname + '[' + @dbname + '].dbo.scheduled_reports inner join   ' + @hostname + '[' + @dbname + '].dbo.reports on reports.reportid = scheduled_reports.reportid inner join   ' + @hostname + '[' + @dbname + '].dbo.scheduled_days on scheduled_days.scheduleid = scheduled_reports.scheduleid where scheduletype = 1 and (repeat_frequency is null or repeat_frequency = 0) ' +
	
'	open calendar_cursor ' +

'	fetch next from calendar_cursor into @scheduleid, @reportname, @weekday, @starttime ' +
'	while @@fetch_status = 0 ' +
'		BEGIN ' +
'		set @weekday = @weekday + 1 ' +
'		set @loopStartDate = @startdate ' +
'		print @loopStartDate ' +
'			while datepart(dw,@loopstartdate) <> @weekday ' +
'				Begin ' +
'				set @loopstartdate = dateadd(dd,1,@loopstartdate) ' +
'				print ''loop start now '' + cast(@loopstartdate as nvarchar(20)) ' +
'				end ' +
'		print datepart(MM,@loopstartdate)' +
'		print datepart(MM,@startdate)' +
'			while datepart(MM,@loopstartdate) = datepart(MM,@startdate) ' +
'				BEGIN	' +
'					insert into #schedules (dbname, scheduletype, runday, scheduleid, reportname, runtime) values (''' + @dbname + ''', ''Daily'', datepart(dd,@loopstartdate), @scheduleid, @reportname, @starttime) ' +
'					set @loopstartdate = dateadd(dd,7,@loopstartdate) ' +
'				END ' +
'		fetch next from calendar_cursor into @scheduleid, @reportname, @weekday, @starttime ' +
'		end ' +
'	close calendar_cursor ' +
'deallocate calendar_cursor '
exec (@sql)
fetch next from employees_cursor into @hostname, @dbname
		end
	close employees_cursor
deallocate employees_cursor


select * from #schedules
END


GO
PRINT N'Creating [dbo].[UpdateFundLimit]'
GO
CREATE PROCEDURE [dbo].[UpdateFundLimit] 
@accountID int,
@amount decimal(18,2)

AS
BEGIN
IF EXISTS (SELECT * FROM dbo.registeredusers WHERE accountid = @accountID)
BEGIN
UPDATE registeredusers SET fundlimit=@amount WHERE accountid = @accountID
SELECT fundlimit FROM registeredusers WHERE accountid=@accountID
END
END
GO
PRINT N'Creating [dbo].[GetCustomerFundDetailsForExpediteEmail]'
GO


CREATE PROC [dbo].[GetCustomerFundDetailsForExpediteEmail]
@accountId int

AS

BEGIN

DECLARE @dbName varchar(50)
DECLARE @sqlScript varchar(MAX)
DECLARE @template VARCHAR(MAX)
DECLARE @fundLimit decimal(18,2)
DECLARE @fundInVarchar VARCHAR(MAX)
DECLARE @accountIdInVarchar varchar(10)

SELECT @dbName = dbname, @fundLimit= FundLimit FROM registeredusers WHERE accountid = @accountId
SELECT @fundInVarchar = CONVERT(varchar(Max), @fundLimit)
SELECT @accountIdInVarchar = CONVERT(varchar(10),@accountId)
SET @template = 'DECLARE @AvailableFund decimal(18,2) DECLARE @AdminId int DECLARE @EmailServerAddress varchar(100) SELECT @EmailServerAddress = stringValue from {DATABASE}.dbo.accountProperties where stringKey =''emailServerAddress'' SELECT @AdminId = stringValue FROM {DATABASE}.dbo.accountProperties WHERE stringKey=''mainAdministrator'' SELECT @AvailableFund= AvailableFund FROM {DATABASE}.dbo.FundTransaction SELECT '+ @fundInVarchar + 'as FundLimit,' + @fundInVarchar + '- @AvailableFund as MinTopUpRequired , email as Email,firstname as FirstName,surname as SurName, @EmailServerAddress as EmailServerAddress,'+ @accountIdInVarchar + 'as accountId FROM {DATABASE}.dbo.employees WHERE employeeid = @AdminId ' 
SET @sqlScript = REPLACE(@template, '{DATABASE}', @dbName)

EXECUTE (@sqlScript)

END

-- BEGIN TRANSACTION CHECK
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
PRINT N'Creating [dbo].[getMonthlyBillingStatistics]'
GO





-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[getMonthlyBillingStatistics]
	@month int,
	@year int,
	@clientFilter tinyint,
	@provider int,
	@excessChargesApply bit
AS
BEGIN
	

	declare @companyname nvarchar(250)
	declare @quantity int
	declare @licencetype nvarchar(10)
	declare @annualcontract nvarchar(10)
	declare @usedquantity int
	declare @hostname nvarchar(50)
	declare @dbname nvarchar(100)
	declare @percentageused decimal(18,2)
	declare @accountManagerName nvarchar(100)
	declare @clientContact nvarchar(50)
	declare @clientEmail nvarchar(500)
	declare @renewalDate nvarchar(5)
	declare @chargedInArrearsForExcess bit
	declare @pricePerExcessClaim money
	declare @accountid int
	
	
CREATE TABLE #billingStats (
	accountid int,
	companyname nvarchar(100),
	quantity int,
	licencetype nvarchar(10),
	annualcontract nvarchar(10),
	usedquantity int,
	percentageused decimal(18,2),
	accountManagerName nvarchar(100),
	clientContact nvarchar(50),
	clientEmail nvarchar(500),
	renewalDate nvarchar(5),
	chargedInArrearsForExcess bit,
	pricePerExcessClaim money
	
)

    
declare employees_cursor cursor for select accountid, companyname, nousers as Quantity, case licenceType when 1 then 'Claims' when 2 then 'Claimants' end as [Licenced Unit], case annualContract when 0 then 'No' else 'Yes' end as [Annualised Contract], databases.hostname, dbname, accountManagers.name, contact, contactemail, renewalDate, chargedinArrearsForExcess, pricePerExcessClaim
	from registeredusers inner join databases on databases.databaseid = registeredusers.dbserver
	left join accountManagers on accountManagers.accountManagerId = registeredusers.accountManagerId
	
	where licenceType is not null and licenceType <> 0 and (@provider = 0 or (@provider <> 0 and registeredusers.providerid = @provider))
open employees_cursor

fetch next from employees_cursor into @accountid, @companyname, @quantity, @licencetype, @annualcontract, @hostname, @dbname, @accountManagerName, @clientContact, @clientEmail, @renewalDate, @chargedInArrearsForExcess, @pricePerExcessClaim
	while @@fetch_status = 0
		BEGIN
			--get the quantity from client database
			declare @sql nvarchar(max)
			if @hostname = '172.24.16.209'
						set @hostname = ''
					else
						set @hostname = '[' + @hostname + '].';
			declare @count int
			declare @parmdef nvarchar(max)
			
			set @parmdef = '@monthIn int, @yearIn int, @countOut int output'
			if @licencetype = 'Claims'
				set @sql = 'select @countOut = count(claimid) from ' + @hostname + '[' + @dbname + '].dbo.claims where month(datesubmitted) = @monthIn and year(datesubmitted) = @yearIn'
			else if @licencetype = 'Claimants'
				set @sql = 'select @countOut = count(distinct employeeid) from ' + @hostname + '[' + @dbname + '].dbo.claims where month(datesubmitted) = @monthIn and year(datesubmitted) = @yearIn'	
			
			declare @countOut int
			
			exec sp_executesql @sql, @parmdef, @monthIn = @month, @yearIn = @year, @countOut = @count output
			
			set @percentageused = (cast(@count as decimal(18,2)) / cast(@quantity as decimal(18,2))) * 100
			insert into #billingStats (companyname, quantity, licencetype, annualcontract,usedquantity, percentageused, accountid, accountManagerName, clientContact, clientEmail, renewalDate, chargedInArrearsForExcess,pricePerExcessClaim) values (@companyname, @quantity, @licencetype, @annualcontract, @count, @percentageused, @accountid, @accountManagerName, @clientContact, @clientEmail, @renewalDate, @chargedInArrearsForExcess, @pricePerExcessClaim)
	
			fetch next from employees_cursor into @accountid, @companyname, @quantity, @licencetype, @annualcontract, @hostname, @dbname, @accountManagerName, @clientContact, @clientEmail, @renewalDate, @chargedInArrearsForExcess, @pricePerExcessClaim
		end
	close employees_cursor
deallocate employees_cursor

	if @clientFilter = 2
		select * from #billingStats where usedquantity > quantity  order by companyname
	else if @clientFilter = 3
		select * from #billingStats where percentageused <= 50 order by companyname
	else
		select * from #billingStats order by companyname
END





GO
PRINT N'Creating [dbo].[getReportsRan]'
GO





-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[getReportsRan] 
	@startDate datetime,
	@endDate datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    --declare table to hold results

if OBJECT_ID('tempdb..#sreportsran','u') IS NOT NULL
	drop table #reportsran


create table #reportsran
(
	dbname nvarchar(100),
	reportId uniqueidentifier,
	employeeid int,
	reportname nvarchar(150),
	basetableid uniqueidentifier,
	limit smallint,
	subaccountid int,
	forclaimants bit,
	requestnumber int,
	startedon datetime,
	isfinancialexport bit,
	sqlran nvarchar(max)
)



declare @sql nvarchar(max)
declare @parmdefinition nvarchar(max);

declare @hostname nvarchar(50)
declare @dbname nvarchar(100)

set @parmdefinition = '@aStartDate datetime, @aEndDate datetime'
declare employees_cursor cursor for select hostname, dbname from registeredusers inner join databases on databases.databaseid = registeredusers.dbserver where registeredusers.archived = 0

open employees_cursor

fetch next from employees_cursor into @hostname, @dbname
	while @@fetch_status = 0
		BEGIN
			if @hostname = '172.24.16.209'
				set @hostname = ''
			else
				set @hostname = '[' + @hostname + '].';

		set @sql = 'select ''' + @dbname + ''' as dbname, reportid, employeeid, reportname, basetableid, [limit], subaccountid, forclaimants, requestnumber, startedon, isfinancialexport, [sql] from ' + @hostname + '[' + @dbname + '].dbo.reportslog where (startedon between @astartDate and @aendDate)'
			--set @sql = 'select ''' + @dbname + ''' as dbname, date, createdon from [' + @hostname + '].[' + @dbname + '].dbo.savedexpenses'
			insert into #reportsran exec sp_executesql @sql, @parmdefinition, @astartDate = @startDate, @aendDate = @endDate
		


fetch next from employees_cursor into @hostname, @dbname
		end
	close employees_cursor
deallocate employees_cursor


select * from #reportsran
END





GO
PRINT N'Creating [dbo].[getauditloglogons]'
GO












-- =============================================
-- Author:		Lynne Hunt
-- Create date: 21/06/2013
-- Description:	All auditlog entries in the specified daate range
-- =============================================
CREATE PROCEDURE [dbo].[getauditloglogons] 
	@startDate datetime,
	@endDate datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    --declare table to hold results

if OBJECT_ID('tempdb..#auditlog','u') IS NOT NULL
	drop table #auditlog


create table #auditlog
(
	dbname nvarchar(100),
	logid int,
	datestamp datetime,
	elementId int,
	elementFriendlyName nvarchar(100),
	employeeid int,
	username nvarchar(50),
	delegate nvarchar(50),
	[action] tinyint,
	[description] nvarchar(1000),
	oldvalue nvarchar(400),
	newvalue nvarchar(400),
	recordtitle nvarchar(2000)
	
)



declare @sql nvarchar(max)
declare @parmdefinition nvarchar(max);

declare @hostname nvarchar(50)
declare @dbname nvarchar(100)

set @parmdefinition = '@aStartDate datetime, @aEndDate datetime'
declare employees_cursor cursor for select hostname, dbname from registeredusers inner join databases on databases.databaseid = registeredusers.dbserver where registeredusers.archived = 0

open employees_cursor

fetch next from employees_cursor into @hostname, @dbname
	while @@fetch_status = 0
		BEGIN
			if @hostname = '172.24.16.209'
				set @hostname = ''
			else
				set @hostname = '[' + @hostname + '].';

		set @sql = 'select ''' + @dbname + ''' as dbname,* from ' + @hostname + '[' + @dbname + '].dbo.auditlogview	where elementid=25 and action=4 and (datestamp between @astartDate and @aendDate)'
			
			--set @sql = 'select ''' + @dbname + ''' as dbname, date, createdon from [' + @hostname + '].[' + @dbname + '].dbo.savedexpenses'
			insert into #auditlog exec sp_executesql @sql, @parmdefinition, @astartDate = @startDate, @aendDate = @endDate
		

fetch next from employees_cursor into @hostname, @dbname
		end
	close employees_cursor
deallocate employees_cursor

select * from #auditlog order by datestamp

END






GO
PRINT N'Creating [dbo].[reportcolumns]'
GO
CREATE TABLE [dbo].[reportcolumns]
(
[groupby] [bit] NOT NULL CONSTRAINT [DF_reportcolumns_groupby] DEFAULT ((0)),
[sort] [tinyint] NOT NULL CONSTRAINT [DF_reportcolumns_sort] DEFAULT ((0)),
[order] [int] NOT NULL CONSTRAINT [DF_reportcolumns_order] DEFAULT ((1)),
[aggfunction] [tinyint] NULL,
[funcsum] [bit] NOT NULL CONSTRAINT [DF_reportcolumns_funcsum] DEFAULT ((0)),
[funcmax] [bit] NOT NULL CONSTRAINT [DF_reportcolumns_funcmax] DEFAULT ((0)),
[funcmin] [bit] NOT NULL CONSTRAINT [DF_reportcolumns_funcmin] DEFAULT ((0)),
[funcavg] [bit] NOT NULL CONSTRAINT [DF_reportcolumns_funcavg] DEFAULT ((0)),
[funccount] [bit] NOT NULL CONSTRAINT [DF_reportcolumns_funccount] DEFAULT ((0)),
[isLiteral] [bit] NOT NULL CONSTRAINT [DF_reportcolumns_isLiteral] DEFAULT ((0)),
[literalname] [nvarchar] (50) NULL,
[literalvalue] [nvarchar] (max) NULL,
[length] [int] NOT NULL CONSTRAINT [DF_reportcolumns_length] DEFAULT ((0)),
[format] [nvarchar] (50) NULL,
[removedecimals] [bit] NOT NULL CONSTRAINT [DF_reportcolumns_removedecimals] DEFAULT ((0)),
[pivottype] [tinyint] NOT NULL CONSTRAINT [DF_reportcolumns_pivottype] DEFAULT ((0)),
[pivotorder] [int] NOT NULL CONSTRAINT [DF_reportcolumns_pivotorder] DEFAULT ((0)),
[runtime] [bit] NOT NULL CONSTRAINT [DF_reportcolumns_runtime] DEFAULT ((0)),
[columntype] [tinyint] NOT NULL,
[hidden] [bit] NOT NULL CONSTRAINT [DF_reportcolumns_hidden] DEFAULT ((0)),
[reportcolumnid] [uniqueidentifier] NOT NULL CONSTRAINT [DF_reportcolumns_reportcolumnid] DEFAULT (newid()),
[reportID] [uniqueidentifier] NOT NULL CONSTRAINT [DF_reportcolumns_reportID] DEFAULT (newid()),
[fieldID] [uniqueidentifier] NULL
)
GO
PRINT N'Creating primary key [PK_reportcolumns_1] on [dbo].[reportcolumns]'
GO
ALTER TABLE [dbo].[reportcolumns] ADD CONSTRAINT [PK_reportcolumns_1] PRIMARY KEY NONCLUSTERED  ([reportcolumnid])
GO
PRINT N'Creating [dbo].[getReportType]'
GO

CREATE FUNCTION [dbo].[getReportType]
(
 @reportID uniqueidentifier
)
RETURNS tinyint
AS
BEGIN
 declare @count int
 set @count = (select count(*) from reportcolumns where reportid = @reportID and (funcsum = 1 or funcmax = 1 or funcmin = 1 or funcavg = 1 or funccount = 1))
 if @count = 0
  return 1
 else
  return 2

 return 0
END

GO
PRINT N'Creating [dbo].[moduleBase]'
GO
CREATE TABLE [dbo].[moduleBase]
(
[moduleID] [int] NOT NULL,
[moduleName] [nvarchar] (100) NOT NULL,
[description] [nvarchar] (4000) NULL,
[brandName] [nvarchar] (250) NOT NULL CONSTRAINT [DF_moduleBase_brandName] DEFAULT (N'Brand Name'),
[brandNameHTML] [nvarchar] (max) NOT NULL CONSTRAINT [DF_moduleBase_brandNameHTML] DEFAULT (N'<strong>Brand Name</strong>'),
[ThemeId] [int] NOT NULL
)
GO
PRINT N'Creating primary key [PK_module_base] on [dbo].[moduleBase]'
GO
ALTER TABLE [dbo].[moduleBase] ADD CONSTRAINT [PK_module_base] PRIMARY KEY CLUSTERED  ([moduleID])
GO
PRINT N'Creating [dbo].[MessageModuleBase]'
GO
CREATE TABLE [dbo].[MessageModuleBase]
(
[ModuleId] [int] NOT NULL,
[MessageId] [int] NOT NULL
)
GO
PRINT N'Creating primary key [PK_MessageModuleBase] on [dbo].[MessageModuleBase]'
GO
ALTER TABLE [dbo].[MessageModuleBase] ADD CONSTRAINT [PK_MessageModuleBase] PRIMARY KEY CLUSTERED  ([ModuleId], [MessageId])
GO
PRINT N'Creating [dbo].[LogonMessages]'
GO
CREATE TABLE [dbo].[LogonMessages]
(
[MessageId] [int] NOT NULL IDENTITY(1, 1),
[CategoryTitle] [nvarchar] (40) NULL,
[CategoryTitleColourCode] [nvarchar] (7) NOT NULL,
[HeaderText] [nvarchar] (50) NOT NULL,
[HeaderTextColourCode] [nvarchar] (6) NULL,
[BodyText] [nvarchar] (max) NULL,
[BodyTextColourCode] [nvarchar] (6) NULL,
[BackgroundImage] [nvarchar] (max) NOT NULL,
[Icon] [nvarchar] (max) NULL,
[ButtonText] [nvarchar] (20) NULL,
[ButtonLink] [nvarchar] (2000) NULL,
[ButtonForeColour] [nvarchar] (6) NULL,
[ButtonBackGroundColour] [nvarchar] (6) NULL,
[Archived] [bit] NOT NULL CONSTRAINT [DF_LogonMessages_Archived] DEFAULT ((1)),
[CreatedBy] [int] NULL,
[CreatedOn] [datetime] NULL,
[ModifiedBy] [int] NULL,
[ModifiedOn] [datetime] NULL
)
GO
PRINT N'Creating primary key [PK_LogonMessages] on [dbo].[LogonMessages]'
GO
ALTER TABLE [dbo].[LogonMessages] ADD CONSTRAINT [PK_LogonMessages] PRIMARY KEY CLUSTERED  ([MessageId])
GO
PRINT N'Creating [dbo].[CheckMaxMessageCountFoModules]'
GO

CREATE PROCEDURE [dbo].[CheckMaxMessageCountFoModules]  
@messageId INT  
AS  
  BEGIN
SELECT    Cast(m.brandName AS VARCHAR)   as modules
    FROM MessageModuleBase mmb 
	inner join moduleBase m on m.moduleID=mmb.ModuleId
	WHERE mmb.moduleid in(  
SELECT mm.moduleid FROM LogonMessages  
LM INNER JOIN MessageModuleBase MM ON MM.MessageId=LM.MessageId 
where  LM.Archived=0
GROUP BY mm.moduleid  
HAVING COUNT(mm.messageid)>2)  and messageid=@messageId

END
GO
PRINT N'Creating [dbo].[DeleteLogonMessages]'
GO
CREATE PROCEDURE [dbo].[DeleteLogonMessages]    
@messageId INT    
AS    
    
BEGIN    
    
 SET NOCOUNT ON;    
 DECLARE @returnCode INT;    
 SET @returnCode = 0;    
     
 DECLARE @categoryTitle varchar(max)    
 IF @returnCode = 0    
 BEGIN    
    DELETE FROM MessageModuleBase WHERE MessageId = @messageId;  
    DELETE FROM LogonMessages WHERE MessageId = @messageId;    
 END    
 END    


GO
PRINT N'Creating [dbo].[ChangeLogonMessagesStatus]'
GO
CREATE PROCEDURE [dbo].[ChangeLogonMessagesStatus]
@messageId INT,
@archive INT
AS
UPDATE LogonMessages SET archived = @archive WHERE MessageId = @messageId;
GO
PRINT N'Creating [dbo].[GetAllLogonMessages]'
GO
CREATE PROCEDURE [dbo].[GetAllLogonMessages] 
AS

SELECT [MessageId]
      ,[CategoryTitle]
      ,[CategoryTitleColourCode]
      ,[HeaderText]
      ,[HeaderTextColourCode]
      ,[BodyText]
      ,[BodyTextColourCode]
      ,[BackgroundImage]
      ,[Icon]
      ,[ButtonText]
      ,[ButtonLink]
      ,[ButtonForeColour]
      ,[ButtonBackGroundColour]
      ,[Archived]
      ,[CreatedBy]
      ,[CreatedOn]
      ,[ModifiedBy]
      ,[ModifiedOn]
  FROM [dbo].[LogonMessages]
GO
PRINT N'Creating [dbo].[GetMessagesModules]'
GO
CREATE PROCEDURE [dbo].[GetMessagesModules]
AS
SELECT DISTINCT moduleId
	,messageId FROM MessageModuleBase
GO
PRINT N'Creating [dbo].[OrphanedReceipts]'
GO
CREATE TABLE [dbo].[OrphanedReceipts]
(
[ReceiptId] [int] NOT NULL IDENTITY(1, 1),
[FileExtension] [nvarchar] (6) NOT NULL,
[CreationMethod] [tinyint] NOT NULL,
[CreatedOn] [datetime] NOT NULL CONSTRAINT [DF__OrphanedR__Creat__4650604F] DEFAULT (getdate())
)
GO
PRINT N'Creating primary key [PK_OrphanedReceipts] on [dbo].[OrphanedReceipts]'
GO
ALTER TABLE [dbo].[OrphanedReceipts] ADD CONSTRAINT [PK_OrphanedReceipts] PRIMARY KEY CLUSTERED  ([ReceiptId])
GO
PRINT N'Creating [dbo].[AddOrphanedReceipt]'
GO

CREATE PROCEDURE [dbo].[AddOrphanedReceipt]
 @fileExtension nvarchar(6),
 @creationMethod tinyint
AS
BEGIN
 SET NOCOUNT ON;
 INSERT INTO OrphanedReceipts (FileExtension, CreationMethod) 
 VALUES (@fileExtension, @creationMethod); 
 RETURN SCOPE_IDENTITY();
END
GO
PRINT N'Creating [dbo].[DeleteOrphanedReceipt]'
GO

-- Add DeleteReceipt (doesn't actually delete)
CREATE PROCEDURE [dbo].[DeleteOrphanedReceipt]
 @receiptId INT
AS
BEGIN
 SET NOCOUNT ON;
 DELETE FROM OrphanedReceipts
 WHERE ReceiptId = @receiptId;
END
GO
PRINT N'Creating [dbo].[tables_base]'
GO
CREATE TABLE [dbo].[tables_base]
(
[tablename] [nvarchar] (50) NOT NULL,
[jointype] [tinyint] NULL,
[allowreporton] [bit] NOT NULL CONSTRAINT [DF_tables_allowreporton] DEFAULT ((0)),
[description] [nvarchar] (50) NULL,
[primarykey] [uniqueidentifier] NULL,
[keyfield] [uniqueidentifier] NULL,
[allowimport] [bit] NOT NULL CONSTRAINT [DF_tables_allowimport] DEFAULT ((0)),
[amendedon] [datetime] NULL CONSTRAINT [DF_tables_amendedon] DEFAULT (getdate()),
[allowworkflow] [bit] NOT NULL CONSTRAINT [DF_tables_allowworkflow] DEFAULT ((0)),
[allowentityrelationship] [bit] NOT NULL CONSTRAINT [DF_tables_base_allowentityrelationsip] DEFAULT ((0)),
[tableid] [uniqueidentifier] NOT NULL CONSTRAINT [DF_tables_base_tableid_new] DEFAULT (newid()),
[hasUserDefinedFields] [bit] NOT NULL CONSTRAINT [DF_tables_base_hasUserDefinedFields] DEFAULT ((0)),
[userdefined_table] [uniqueidentifier] NULL,
[elementID] [int] NULL,
[subAccountIDField] [int] NULL,
[tableFrom] [int] NOT NULL CONSTRAINT [DF__tables_ba__table__105805DF] DEFAULT ((0)),
[relabel_param] [nvarchar] (150) NULL,
[linkingTable] [bit] NOT NULL CONSTRAINT [DF_tables_base_linkingTable] DEFAULT ((0))
)
GO
PRINT N'Creating primary key [PK_tables] on [dbo].[tables_base]'
GO
ALTER TABLE [dbo].[tables_base] ADD CONSTRAINT [PK_tables] PRIMARY KEY NONCLUSTERED  ([tableid])
GO
PRINT N'Adding constraints to [dbo].[tables_base]'
GO
ALTER TABLE [dbo].[tables_base] ADD CONSTRAINT [IX_tables_base] UNIQUE NONCLUSTERED  ([tablename])
GO
PRINT N'Creating [dbo].[jointables_base]'
GO
CREATE TABLE [dbo].[jointables_base]
(
[description] [nvarchar] (4000) NULL,
[amendedon] [datetime] NULL CONSTRAINT [DF_jointables_amendedon] DEFAULT (getdate()),
[tableid] [uniqueidentifier] NOT NULL,
[basetableid] [uniqueidentifier] NOT NULL,
[jointableid] [uniqueidentifier] NOT NULL CONSTRAINT [DF_jointables_base_jointableid] DEFAULT (newid())
)
GO
PRINT N'Creating primary key [PK_jointables] on [dbo].[jointables_base]'
GO
ALTER TABLE [dbo].[jointables_base] ADD CONSTRAINT [PK_jointables] PRIMARY KEY NONCLUSTERED  ([jointableid])
GO
PRINT N'Creating index [Base_to_Table] on [dbo].[jointables_base]'
GO
CREATE UNIQUE NONCLUSTERED INDEX [Base_to_Table] ON [dbo].[jointables_base] ([tableid], [basetableid])
GO
PRINT N'Creating [dbo].[joinbreakdown_base]'
GO
CREATE TABLE [dbo].[joinbreakdown_base]
(
[order] [tinyint] NOT NULL,
[joinkey] [uniqueidentifier] NOT NULL,
[amendedon] [datetime] NULL CONSTRAINT [DF_joinbreakdown_amendedon] DEFAULT (getdate()),
[destinationkey] [uniqueidentifier] NULL,
[tableid] [uniqueidentifier] NOT NULL,
[sourcetable] [uniqueidentifier] NULL,
[joinbreakdownid] [uniqueidentifier] NOT NULL CONSTRAINT [DF_joinbreakdown_base_joinbreakdownid] DEFAULT (newid()),
[jointableid] [uniqueidentifier] NULL
)
GO
PRINT N'Creating primary key [PK_joinbreakdown] on [dbo].[joinbreakdown_base]'
GO
ALTER TABLE [dbo].[joinbreakdown_base] ADD CONSTRAINT [PK_joinbreakdown] PRIMARY KEY NONCLUSTERED  ([joinbreakdownid])
GO
PRINT N'Creating index [JoinTableId_Order] on [dbo].[joinbreakdown_base]'
GO
CREATE UNIQUE NONCLUSTERED INDEX [JoinTableId_Order] ON [dbo].[joinbreakdown_base] ([jointableid], [order])
GO
PRINT N'Creating [dbo].[availablejoins]'
GO
CREATE VIEW [dbo].[availablejoins]
AS
SELECT     dbo.jointables_base.description, dbo.tables_base.tablename, dbo.fields_base.field, sourcetables.tablename AS sourcetable, dbo.tables_base.jointype, 
                      dbo.joinbreakdown_base.[order], sourcetables.tableid AS sourcetableid, dbo.tables_base.tableid, dbo.jointables_base.basetableid, dbo.joinbreakdown_base.jointableid, dbo.[joinbreakdown_base].destinationkey, destinationfields.field AS destinationfield
FROM         dbo.jointables_base INNER JOIN
                      dbo.joinbreakdown_base ON dbo.jointables_base.jointableid = dbo.joinbreakdown_base.jointableid INNER JOIN
                      dbo.tables_base ON dbo.joinbreakdown_base.tableid = dbo.tables_base.tableid LEFT OUTER JOIN
                      dbo.tables_base sourcetables ON sourcetables.tableid = dbo.joinbreakdown_base.sourcetable INNER JOIN
                      dbo.fields_base ON dbo.fields_base.fieldid = dbo.joinbreakdown_base.joinkey
					LEFT JOIN dbo.fields_base AS destinationfields ON destinationfields.fieldid = joinbreakdown_base.destinationkey

GO
PRINT N'Creating [dbo].[UnlinkEnvelopeFromClaim]'
GO

CREATE PROCEDURE [dbo].[UnlinkEnvelopeFromClaim]
 @envelopeId int,
 @lastModifiedBy int
AS
BEGIN
 SET CONCAT_NULL_YIELDS_NULL OFF;
 UPDATE [dbo].[Envelopes]
 SET  
  ClaimId = NULL, 
  CRN = null,
  EnvelopeStatus = 20, 
  DateAssignedToClaim = NULL, 
  LastModifiedBy = @lastModifiedBy
 WHERE EnvelopeId = @envelopeId;
 SET CONCAT_NULL_YIELDS_NULL ON;
END
GO
PRINT N'Creating [dbo].[moduleLicencesBase]'
GO
CREATE TABLE [dbo].[moduleLicencesBase]
(
[moduleID] [int] NOT NULL,
[accountID] [int] NOT NULL,
[expiryDate] [datetime] NULL,
[maxUsers] [int] NOT NULL CONSTRAINT [DF_module_licences_maxUers] DEFAULT ((0))
)
GO
PRINT N'Creating primary key [PK_moduleLicencesBase] on [dbo].[moduleLicencesBase]'
GO
ALTER TABLE [dbo].[moduleLicencesBase] ADD CONSTRAINT [PK_moduleLicencesBase] PRIMARY KEY CLUSTERED  ([moduleID], [accountID])
GO
PRINT N'Creating [dbo].[GetAvailableActiveModules]'
GO
	CREATE PROCEDURE [dbo].[GetAvailableActiveModules]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
select brandName, m.moduleID from modulebase m inner join moduleLicencesBase l on l.moduleID = m.moduleID group by brandName, m.moduleID
END

GO
PRINT N'Creating [dbo].[CheckMaxMessageCountForModules]'
GO

CREATE PROCEDURE [dbo].[CheckMaxMessageCountForModules]
@messageId INT
AS  
  BEGIN
  IF @messageId=0
  BEGIN
  SELECT    Cast(m.brandName AS VARCHAR)   AS modules
    FROM MessageModuleBase mmb 
	INNER JOIN moduleBase m on m.moduleID=mmb.ModuleId
	WHERE mmb.moduleid in(  
SELECT mm.moduleid FROM LogonMessages  
LM INNER JOIN MessageModuleBase MM ON MM.MessageId=LM.MessageId 
WHERE  LM.Archived=0
GROUP BY mm.moduleid  
HAVING COUNT(mm.messageid)>2)  GROUP BY m.brandName 
  END
  ELSE
  BEGIN
  SELECT    Cast(m.brandName AS VARCHAR)   AS modules
    FROM MessageModuleBase mmb 
	INNER JOIN moduleBase m on m.moduleID=mmb.ModuleId
	WHERE mmb.moduleid in(  
SELECT mm.moduleid FROM LogonMessages  
LM INNER JOIN MessageModuleBase MM ON MM.MessageId=LM.MessageId 
WHERE  LM.Archived=0
GROUP BY mm.moduleid  
HAVING COUNT(mm.messageid)>2)  and messageid=@messageId
  END
  END

GO
PRINT N'Creating [dbo].[mobileDeviceTypes]'
GO
CREATE TABLE [dbo].[mobileDeviceTypes]
(
[mobileDeviceTypeID] [int] NOT NULL IDENTITY(12, 1),
[model] [nvarchar] (50) NOT NULL,
[mobileDeviceOSType] [int] NULL
)
GO
PRINT N'Creating primary key [PK_mobileDeviceTypes] on [dbo].[mobileDeviceTypes]'
GO
ALTER TABLE [dbo].[mobileDeviceTypes] ADD CONSTRAINT [PK_mobileDeviceTypes] PRIMARY KEY CLUSTERED  ([mobileDeviceTypeID])
GO
PRINT N'Creating [dbo].[MobileDeviceTypeReader]'
GO
-- work item ID: 46404
-- sequence: 4
CREATE PROCEDURE [dbo].[MobileDeviceTypeReader] 
AS
BEGIN
      select [dbo].[mobileDeviceTypes].[mobileDeviceTypeID],[dbo].[mobileDeviceTypes].[model], [dbo].[mobileDeviceTypes].mobileDeviceOSType FROM mobileDeviceTypes
END
GO
PRINT N'Creating [dbo].[hotels]'
GO
CREATE TABLE [dbo].[hotels]
(
[hotelid] [int] NOT NULL IDENTITY(15052, 1),
[hotelname] [nvarchar] (250) NOT NULL,
[address1] [nvarchar] (100) NULL,
[address2] [nvarchar] (100) NULL,
[city] [nvarchar] (100) NULL,
[county] [nvarchar] (100) NULL,
[postcode] [nvarchar] (100) NULL,
[country] [nvarchar] (100) NULL,
[rating] [tinyint] NOT NULL CONSTRAINT [DF_hotels_rating] DEFAULT ((0)),
[telno] [nvarchar] (50) NULL,
[email] [nvarchar] (500) NULL,
[CreatedOn] [datetime] NULL,
[CreatedBy] [int] NULL,
[ModifiedOn] [datetime] NULL,
[ModifiedBy] [int] NULL
)
GO
PRINT N'Creating primary key [PK_hotels] on [dbo].[hotels]'
GO
ALTER TABLE [dbo].[hotels] ADD CONSTRAINT [PK_hotels] PRIMARY KEY CLUSTERED  ([hotelid])
GO
PRINT N'Creating [dbo].[GetHotelsByName]'
GO

CREATE PROCEDURE [dbo].[GetHotelsByName] @hotelName NVARCHAR(50)
AS
SELECT top 10 hotelid
	,hotelname
	,address1
	,address2
	,city
	,county
	,postcode
	,country
	,rating
	,telno
	,email
	,CreatedOn
	,CreatedBy
FROM hotels
WHERE hotelname LIKE @hotelName + '%'

GO
PRINT N'Creating [dbo].[mobileDeviceOSTypes]'
GO
CREATE TABLE [dbo].[mobileDeviceOSTypes]
(
[mobileDeviceOSTypeId] [int] NOT NULL,
[mobileInstallFrom] [nvarchar] (100) NOT NULL,
[mobileImage] [nvarchar] (100) NOT NULL
)
GO
PRINT N'Creating primary key [PK__mobileDe__83B5B159481280E7] on [dbo].[mobileDeviceOSTypes]'
GO
ALTER TABLE [dbo].[mobileDeviceOSTypes] ADD CONSTRAINT [PK__mobileDe__83B5B159481280E7] PRIMARY KEY CLUSTERED  ([mobileDeviceOSTypeId])
GO
PRINT N'Creating [dbo].[mobileDeviceTypeOsReader]'
GO
-- work item ID: 46406
-- sequence: 6
CREATE PROCEDURE [dbo].[mobileDeviceTypeOsReader]
AS
BEGIN
      SELECT mobileDeviceOSTypeId, mobileInstallFrom, mobileImage FROM mobileDeviceOSTypes
END
GO
PRINT N'Creating [dbo].[GetElementLicencedModuleIDs]'
GO


CREATE PROCEDURE [dbo].[GetElementLicencedModuleIDs]
	@moduleID tinyint,
	@accountID int
AS
BEGIN
	SELECT moduleElementBase.elementID FROM accountsLicencedElements 
		INNER JOIN moduleElementBase ON accountsLicencedElements.elementID = moduleElementBase.elementID 
		WHERE moduleElementBase.moduleID = @moduleID AND accountsLicencedElements.accountID = @accountID
END

GO
PRINT N'Creating [dbo].[AddOrUpdateLogonMessages]'
GO

CREATE PROCEDURE [dbo].[AddOrUpdateLogonMessages] @messageid INT
	,@CategoryTitle NVARCHAR(40)
	,@CategoryTitleColourCode NVARCHAR(6)
	,@HeaderText NVARCHAR(80)
	,@HeaderTextColourCode NVARCHAR(200)
	,@BodyText NVARCHAR(500)
	,@BodyTextColourCode NVARCHAR(6)
	,@BackgroundImage NVARCHAR(80)
	,@Icon NVARCHAR(80)=null
	,@ButtonText NVARCHAR(40)
	,@ButtonLink NVARCHAR(2000)
	,@ButtonForeColour NVARCHAR(6)
	,@ButtonBackGroundColour NVARCHAR(6)
	,@Archived INT
	,@moduleIds varchar(50)=null
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @pos INT
	DECLARE @len INT
	DECLARE @module INT

	IF @messageid = 0
	BEGIN
		INSERT INTO LogonMessages (
			CategoryTitle
			,CategoryTitleColourCode
			,HeaderText
			,HeaderTextColourCode
			,BodyText
			,BodyTextColourCode
			,BackgroundImage
			,Icon
			,ButtonText
			,ButtonLink
			,ButtonForeColour
			,ButtonBackGroundColour
			,Archived
			)
		VALUES (
			@CategoryTitle
			,@CategoryTitleColourCode
			,@HeaderText
			,@HeaderTextColourCode
			,@BodyText
			,@BodyTextColourCode
			,@BackgroundImage
			,@Icon
			,@ButtonText
			,@ButtonLink
			,@ButtonForeColour
			,@ButtonBackGroundColour
			,@Archived
			)

		SET @messageid = SCOPE_IDENTITY();
	END
	ELSE
	BEGIN
		UPDATE LogonMessages
		SET CategoryTitle = @CategoryTitle
			,CategoryTitleColourCode = @CategoryTitleColourCode
			,HeaderText = @HeaderText
			,HeaderTextColourCode = @HeaderTextColourCode
			,BodyText = @BodyText
			,BodyTextColourCode = @BodyTextColourCode
			,BackgroundImage = @BackgroundImage
			,Icon = @Icon
			,ButtonText = @ButtonText
			,ButtonForeColour = @ButtonForeColour
			,ButtonBackGroundColour = @ButtonBackGroundColour
			,ButtonLink=@ButtonLink
		WHERE MessageId = @messageid
		END
		DELETE
		FROM MessageModuleBase
		WHERE MessageId = @messageid

		SET @pos = 0
		SET @len = 0

		WHILE CHARINDEX(',', @moduleIds, @pos + 1) > 0
		BEGIN
			SET @len = CHARINDEX(',', @moduleIds, @pos + 1) - @pos
			SET @module = SUBSTRING(@moduleIds, @pos, @len)

			INSERT INTO MessageModuleBase (
				ModuleId
				,MessageId
				)
			VALUES (
				@module
				,@messageid
				)

			SET @pos = CHARINDEX(',', @moduleIds, @pos + @len) + 1
		END

		RETURN @messageid	
END

;
GO
PRINT N'Creating [dbo].[information_messages]'
GO
CREATE TABLE [dbo].[information_messages]
(
[informationID] [int] NOT NULL IDENTITY(87, 1),
[title] [nvarchar] (22) NULL,
[message] [nvarchar] (1000) NULL,
[administratorID] [int] NULL,
[dateAdded] [datetime] NULL,
[displayOrder] [int] NULL,
[deleted] [bit] NOT NULL CONSTRAINT [DF_information_messages_deleted] DEFAULT ((0)),
[MobileInformationMessage] [nvarchar] (400) NULL CONSTRAINT [DF__informati__Mobil__5AAC5EB7] DEFAULT (NULL)
)
GO
PRINT N'Creating primary key [PK_information_messages] on [dbo].[information_messages]'
GO
ALTER TABLE [dbo].[information_messages] ADD CONSTRAINT [PK_information_messages] PRIMARY KEY CLUSTERED  ([informationID])
GO
PRINT N'Creating [dbo].[GetMobileInformationMessages]'
GO

CREATE PROCEDURE [dbo].[GetMobileInformationMessages]
AS
  BEGIN
      SELECT informationid,
             title,
             mobileInformationMessage
      FROM   information_messages
      WHERE  ( deleted = 0
               AND mobileInformationMessage IS NOT NULL )
      ORDER  BY displayorder
  END

;
GO
PRINT N'Creating [dbo].[ValidateCustomerBankIdentifier]'
GO


CREATE PROCEDURE [dbo].[ValidateCustomerBankIdentifier]
	@cardProviderId int,
	@FileIdentifier nvarchar(max)
AS

DECLARE @accountid INT;
DECLARE @dbname NVARCHAR(MAX);
DECLARE @sql NVARCHAR(MAX);
DECLARE AccountCursor CURSOR FOR SELECT accountid, dbname FROM registeredusers WHERE archived = 0;
DECLARE @result int= -1;
declare @cardProviderCount int;

OPEN AccountCursor;

FETCH NEXT FROM AccountCursor INTO
@accountid, @dbname

WHILE @@FETCH_STATUS = 0
BEGIN
	SET @sql = 'SELECT @cardProviderCountOUT =  COUNT(cardproviderid) FROM '+@dbname+'.dbo.corporate_cards WHERE cardproviderid = '+CAST(@cardProviderId as nvarchar(max))+' AND FileIdentifier = '''+@FileIdentifier+''''
	declare @ParmDefinition nvarchar(max) = N'@cardproviderCountOUT int OUTPUT'; 
	EXEC sp_executesql @sql, @ParmDefinition, @cardProviderCountOUT=@cardProviderCount OUTPUT;
	IF @cardProviderCount = 1
	BEGIN
		IF @result = -1
		BEGIN
			SET @result = @accountid;
		END
		ELSE
		BEGIN
			select -100; -- duplicate
		END
	END
	ELSE
	BEGIN
		IF @cardProviderCount > 1
		BEGIN
			select -100; -- Duplicate
		END
	END

	FETCH NEXT FROM AccountCursor INTO
	@accountid, @dbname
	
END
CLOSE AccountCursor
DEALLOCATE AccountCursor

select @result;

;
GO
PRINT N'Creating [dbo].[GetProductUsageForAllAccounts]'
GO






-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetProductUsageForAllAccounts]
	@numMonths int
AS
BEGIN
	
	
	CREATE TABLE #usages (
	[companyId] nvarchar(250),
	[usage] decimal(18,2)
	)
	
	declare @parmdef nvarchar(max)
	set @parmdef = '@startDateIn datetime, @endDateIn datetime, @licenceQuantityIn int, @numMonthsIn int, @usageOut decimal(18,2) output '
	
	declare @startDate datetime
	declare @endDate datetime
	declare @usage decimal(18,2)
	declare @sql nvarchar(max)
	
	set @endDate = convert(varchar,year(GETDATE())) + '/' + convert(varchar,month(GETDATE())) + '/01'
	set @endDate = DATEADD(dd,-1,@enddate)		
	
	set @startDate = convert(varchar,year(GETDATE())) + '/' + convert(varchar,month(GETDATE())) + '/01'		
	set @startDate = DATEADD(MM,(@numMonths) / -1,@startdate)
	
declare @hostname nvarchar(50)
	declare @dbname nvarchar(100)
	declare @licencequantity int
	declare @licencetype tinyint
	declare @companyId nvarchar(250)
    -- Insert statements for procedure here
	declare employees_cursor cursor for select companyid, nousers, licencetype, hostname, dbname from registeredusers inner join databases on databases.databaseID = registeredusers.dbserver where archived = 0
open employees_cursor

fetch next from employees_cursor into @companyId, @licencequantity, @licencetype, @hostname, @dbname
	while @@fetch_status = 0
		BEGIN
			if @hostname = '172.24.16.209'
						set @hostname = ''
					else
						set @hostname = '[' + @hostname + '].';

    -- Insert statements for procedure here
    if @licencetype = 1
		set @sql = 'select @usageOut = (count(claimid) / cast((@licenceQuantityIn * @numMonthsIn) as decimal(18,2))) * 100 from ' + @hostname + '[' + @dbname + '].dbo.claims where (datesubmitted between @startDateIn and @endDateIn)'
	else if @licencetype = 2
		set @sql = 'select @usageOut = AVG(claimantAverages) from (select (count(distinct employeeid) / cast((@licenceQuantityIn) as decimal(18,2)) * 100 ) as claimantAverages from ' + @hostname + '[' + @dbname + '].dbo.claims where (datesubmitted between @startDateIn and @endDateIn) group by YEAR(datesubmitted), month(datesubmitted)) as averages'
	exec sp_executesql @sql, @parmdef, @startDateIn = @startDate, @endDateIn = @endDate, @licenceQuantityIn = @licenceQuantity, @numMonthsIn = @numMonths, @usageOut = @usage output
	
	insert into #usages (companyId, usage) values (@companyId, @usage)
			fetch next from employees_cursor into  @companyId, @licencequantity, @licencetype, @hostname, @dbname
		end
		
	close employees_cursor
deallocate employees_cursor

	select * from #usages
END






GO
PRINT N'Creating [dbo].[getSchedulesRan]'
GO






-- =============================================
-- Author:		Lynne Hunt
-- Create date: 21/06/2013
-- Description:	All schedules ran in the specified daate range
-- =============================================
CREATE PROCEDURE [dbo].[getSchedulesRan] 
	@startDate datetime,
	@endDate datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    --declare table to hold results

if OBJECT_ID('tempdb..#sschedulesran','u') IS NOT NULL
	drop table #schedulesran


create table #schedulessran
(
	dbname nvarchar(100),
	reportId uniqueidentifier,
	scheduleId int,
	employeeid int,
	reportname nvarchar(150),
	subaccountid int,
	financialexportID int,
	scheduletype tinyint,
	deliverymethod tinyint,
	
)



declare @sql nvarchar(max)
declare @parmdefinition nvarchar(max);

declare @hostname nvarchar(50)
declare @dbname nvarchar(100)

set @parmdefinition = '@aStartDate datetime, @aEndDate datetime'
declare employees_cursor cursor for select hostname, dbname from registeredusers inner join databases on databases.databaseid = registeredusers.dbserver where registeredusers.archived = 0

open employees_cursor

fetch next from employees_cursor into @hostname, @dbname
	while @@fetch_status = 0
		BEGIN
			if @hostname = '172.24.16.209'
				set @hostname = ''
			else
				set @hostname = '[' + @hostname + '].';

		set @sql = 'select ''' + @dbname + ''' as dbname,scheduled_reports.reportid,scheduled_reports_log.scheduleid, scheduled_reports.employeeid, reportname, subaccountid, financialexportid, scheduletype,deliverymethod from ' + @hostname + '[' + @dbname + '].dbo.scheduled_reports_log	
						inner join '+ @hostname + '[' + @dbname + '].dbo.scheduled_reports on scheduled_reports.scheduleid=scheduled_reports_log.scheduleid
						inner join '+ @hostname + '[' + @dbname + '].dbo.reports on reports.reportID=scheduled_reports.reportID where (datestamp between @astartDate and @aendDate)'
			
			--set @sql = 'select ''' + @dbname + ''' as dbname, date, createdon from [' + @hostname + '].[' + @dbname + '].dbo.savedexpenses'
			insert into #schedulessran exec sp_executesql @sql, @parmdefinition, @astartDate = @startDate, @aendDate = @endDate
		

fetch next from employees_cursor into @hostname, @dbname
		end
	close employees_cursor
deallocate employees_cursor


select * from #schedulessran
END
GO
PRINT N'Creating [dbo].[AddEnvelopeHistoryEntry]'
GO




CREATE PROCEDURE [dbo].[AddEnvelopeHistoryEntry]
 @envelopeId int,
 @envelopeStatus int,
 @data nvarchar(500),
 @lastModifiedBy int
AS
BEGIN
 INSERT INTO [dbo].[EnvelopeHistory]
 (
  EnvelopeId, 
  EnvelopeStatus,
  Data,
  ModifiedBy,
  ModifiedOn   
 ) VALUES (
  @envelopeId,
  @envelopeStatus,
  @data,
  @lastModifiedBy,
  GETUTCDATE()
 )
END
GO
PRINT N'Creating [dbo].[providers]'
GO
CREATE TABLE [dbo].[providers]
(
[providerId] [int] NOT NULL IDENTITY(1, 1),
[provider] [nvarchar] (100) NOT NULL
)
GO
PRINT N'Creating primary key [PK_providers] on [dbo].[providers]'
GO
ALTER TABLE [dbo].[providers] ADD CONSTRAINT [PK_providers] PRIMARY KEY CLUSTERED  ([providerId])
GO
PRINT N'Creating [dbo].[getProviders]'
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[getProviders] 
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT providerid, provider from providers order by provider
END
GO
PRINT N'Creating [dbo].[DeleteExpenseValidationCriterion]'
GO

CREATE PROCEDURE [dbo].[DeleteExpenseValidationCriterion]
 @id int
AS
BEGIN
 SET NOCOUNT ON;

 -- check it even exists
 IF NOT EXISTS (SELECT CriterionId FROM [dbo].[ExpenseValidationCriteria] WHERE CriterionId = @id) RETURN -1;

 -- loop through each client db and check for anything that may be using the reason with this Id. 
 DECLARE @sql NVARCHAR(MAX);
 DECLARE @accountId INT;
 DECLARE @hostname NVARCHAR(100);
 DECLARE @dbName NVARCHAR(100);
 DECLARE @inUse bit = 0;
 DECLARE @checkOUT int;
 
 DECLARE accounts_cursor CURSOR FAST_FORWARD FOR 
  SELECT r.accountID, d.hostName, r.dbname
  FROM [dbo].[registeredusers] AS r 
  INNER JOIN [dbo].[databases] AS d ON d.databaseID = r.dbserver 
  WHERE r.archived = 0;

 OPEN accounts_cursor;

 FETCH NEXT FROM accounts_cursor INTO @accountId, @hostname, @dbName;
 WHILE @@fetch_status = 0
  BEGIN    
   SET @sql = 'SELECT @checkOUT = COUNT(CriterionId) FROM [' + @dbName + '].[dbo].[ExpenseValidationResults] WHERE CriterionId = ' + CAST(@id as NVARCHAR);
   EXEC sp_executesql @sql, N'@checkOUT int OUTPUT', @checkOUT OUTPUT;
   IF (@checkOUT > 0) SET @inUse = 1;

   FETCH NEXT FROM accounts_cursor INTO @accountId, @hostname, @dbName;
  END;
 CLOSE accounts_cursor;
 DEALLOCATE accounts_cursor;

 -- dip out if it is in use
 if (@inUse = 1) RETURN -2;

 -- delete from metabase
 DELETE FROM [dbo].[ExpenseValidationCriteria]
 WHERE CriterionId = @id;
 RETURN 0;
END
GO
PRINT N'Creating [dbo].[getClientUsageForLast12Months]'
GO






-- ============================================= 
-- Author:	<Author,,Name> 
-- Create date: <Create Date,,> 
-- Description:	<Description,,> 
-- ============================================= 
CREATE PROCEDURE [dbo].[getClientUsageForLast12Months] 
@accountId int 
AS 
BEGIN 

CREATE TABLE #stats ( 
year int, 
month nvarchar(100), 
recordCount int 
) 

declare @sql nvarchar(max) 
declare @hostname nvarchar(50) 
declare @dbname nvarchar(100) 
declare @licencetype tinyint 
declare @startdate datetime 
declare @enddate datetime 


set @startdate = convert(varchar,year(GETDATE()-365)) + '/' + CONVERT(VARCHAR, month(getdate())) + '/01' 
set @enddate = dateadd(day,-1,convert(varchar,year(GETDATE())) + '/' + CONVERT(VARCHAR, month(getdate())) + '/01') 

select @licencetype = licencetype, @dbname = dbname, @hostname = hostname from registeredusers inner join databases on databases.databaseid = registeredusers.dbserver where accountid = @accountId 

if @hostname = '172.24.16.209' 
set @hostname = '' 
else 
set @hostname = '[' + @hostname + '].'; 

if @licencetype = 1 
set @sql = 'select year(datesubmitted), case month(datesubmitted) when 1 then ''January'' when 2 then ''February'' when 3 then ''March'' when 4 then ''April'' when 5 then ''May'' when 6 then ''June'' when 7 then ''July'' when 8 then ''August'' when 9 then ''September'' when 10 then ''October'' when 11 then ''November'' when 12 then ''December'' end, count(claimid) from ' + @hostname + '[' + @dbname + '].dbo. claims where (datesubmitted between @startdatein and @enddatein) group by year(datesubmitted), month(datesubmitted) order by YEAR(datesubmitted), month(datesubmitted)' 
else if @licencetype = 2 
set @sql = 'select year(datesubmitted), case month(datesubmitted) when 1 then ''January'' when 2 then ''February'' when 3 then ''March'' when 4 then ''April'' when 5 then ''May'' when 6 then ''June'' when 7 then ''July'' when 8 then ''August'' when 9 then ''September'' when 10 then ''October'' when 11 then ''November'' when 12 then ''December'' end, count(distinct employeeid) from ' + @hostname + '[' + @dbname + '].dbo. claims where (datesubmitted between @startdatein and @enddatein) group by year(datesubmitted), month(datesubmitted) order by YEAR(datesubmitted), month(datesubmitted)' 

declare @parmdef nvarchar(max) 
set @parmdef = '@startDateIn datetime, @endDateIn datetime' 
insert into #stats exec sp_executesql @sql, @parmdef, @startDateIn = @startdate, @endDateIn = @enddate 

select * from #stats 
END 






GO
PRINT N'Creating [dbo].[getauditlog]'
GO










-- =============================================
-- Author:		Lynne Hunt
-- Create date: 21/06/2013
-- Description:	All auditlog entries in the specified daate range
-- =============================================
CREATE PROCEDURE [dbo].[getauditlog] 
	@startDate datetime,
	@endDate datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    --declare table to hold results

if OBJECT_ID('tempdb..#auditlog','u') IS NOT NULL
	drop table #auditlog


create table #auditlog
(
	dbname nvarchar(100),
	logid int,
	datestamp datetime,
	elementId int,
	elementFriendlyName nvarchar(100),
	employeeid int,
	username nvarchar(50),
	delegate nvarchar(50),
	[action] tinyint,
	[description] nvarchar(1000),
	oldvalue nvarchar(400),
	newvalue nvarchar(400),
	recordtitle nvarchar(2000)
	
)



declare @sql nvarchar(max)
declare @parmdefinition nvarchar(max);

declare @hostname nvarchar(50)
declare @dbname nvarchar(100)

set @parmdefinition = '@aStartDate datetime, @aEndDate datetime'
declare employees_cursor cursor for select hostname, dbname from registeredusers inner join databases on databases.databaseid = registeredusers.dbserver where registeredusers.archived = 0

open employees_cursor

fetch next from employees_cursor into @hostname, @dbname
	while @@fetch_status = 0
		BEGIN
			if @hostname = '172.24.16.209'
				set @hostname = ''
			else
				set @hostname = '[' + @hostname + '].';

		set @sql = 'select ''' + @dbname + ''' as dbname,* from ' + @hostname + '[' + @dbname + '].dbo.auditlogview	where (datestamp between @astartDate and @aendDate)'
			
			--set @sql = 'select ''' + @dbname + ''' as dbname, date, createdon from [' + @hostname + '].[' + @dbname + '].dbo.savedexpenses'
			insert into #auditlog exec sp_executesql @sql, @parmdefinition, @astartDate = @startDate, @aendDate = @endDate
		

fetch next from employees_cursor into @hostname, @dbname
		end
	close employees_cursor
deallocate employees_cursor


select * from #auditlog order by datestamp
END




GO
PRINT N'Creating [dbo].[DeleteEnvelope]'
GO


-- Create DeleteEnvelope SPROC
CREATE PROCEDURE [dbo].[DeleteEnvelope]
 @evenlopeId int,
 @lastModifiedBy int
AS
BEGIN
 DELETE FROM [dbo].[Envelopes]
 WHERE EnvelopeId = @evenlopeId;
END
GO
PRINT N'Creating [dbo].[EnvelopeTypes]'
GO
CREATE TABLE [dbo].[EnvelopeTypes]
(
[EnvelopeTypeId] [int] NOT NULL IDENTITY(1, 1),
[Label] [nvarchar] (50) NOT NULL
)
GO
PRINT N'Creating primary key [PK_EnvelopeTypes] on [dbo].[EnvelopeTypes]'
GO
ALTER TABLE [dbo].[EnvelopeTypes] ADD CONSTRAINT [PK_EnvelopeTypes] PRIMARY KEY CLUSTERED  ([EnvelopeTypeId])
GO
PRINT N'Creating [dbo].[AddEnvelopeType]'
GO


-- add envelope Type
CREATE PROCEDURE [dbo].[AddEnvelopeType]
 @label nvarchar(60)
AS
BEGIN
 INSERT INTO [dbo].[EnvelopeTypes] (
 Label
) VALUES (
 @label
 )
 RETURN SCOPE_IDENTITY()
END
GO
PRINT N'Creating [dbo].[EditEnvelopeType]'
GO

-- edit envelope status
CREATE PROCEDURE [dbo].[EditEnvelopeType]
 @id int,
 @label nvarchar(50)
AS
BEGIN
 UPDATE [dbo].[EnvelopeTypes]
 SET Label = @label
 WHERE EnvelopeTypeId = @id;
END
GO
PRINT N'Creating [dbo].[SaveEnvelopePhysicalState]'
GO


CREATE PROCEDURE [dbo].[SaveEnvelopePhysicalState]
 @id int,
 @details nvarchar(100)
AS
BEGIN
 IF (@id = 0)
 BEGIN
  IF ((SELECT COUNT (EnvelopePhysicalState.Details) FROM EnvelopePhysicalState WHERE Details = @details) > 0) RETURN -1

  INSERT INTO [dbo].[EnvelopePhysicalState] (Details)
  VALUES (@details)
  RETURN SCOPE_IDENTITY()
 END
 ELSE 
 BEGIN
  IF ((SELECT COUNT (EnvelopePhysicalState.EnvelopePhysicalStateId) FROM EnvelopePhysicalState WHERE EnvelopePhysicalStateId = @id) = 0) RETURN -2
  UPDATE [dbo].[EnvelopePhysicalState]
  SET Details = @details
  WHERE EnvelopePhysicalStateId = @id
  RETURN @id
 END 
END
GO
PRINT N'Creating [dbo].[DeleteEnvelopeType]'
GO

-- delete envelope status
CREATE PROCEDURE [dbo].[DeleteEnvelopeType]
 @id int
AS
BEGIN
 DELETE FROM [dbo].[EnvelopeTypes]
 WHERE EnvelopeTypeId = @id;
END
GO
PRINT N'Creating [dbo].[DeleteEnvelopePhysicalState]'
GO


CREATE PROCEDURE [dbo].[DeleteEnvelopePhysicalState]
 @id int 
AS
BEGIN
 IF ((SELECT COUNT (EnvelopesPhysicalStates.EnvelopePhysicalStateId) FROM EnvelopesPhysicalStates WHERE EnvelopePhysicalStateId = @id) > 0) RETURN -1
 DELETE FROM EnvelopePhysicalState WHERE EnvelopePhysicalStateId = @id
END
GO
PRINT N'Creating [dbo].[reports_common_columns]'
GO
CREATE TABLE [dbo].[reports_common_columns]
(
[tableid] [uniqueidentifier] NOT NULL,
[fieldid] [uniqueidentifier] NOT NULL,
[joinPath] [nvarchar] (max) NULL
)
GO
PRINT N'Creating primary key [PK_reports_common_columns] on [dbo].[reports_common_columns]'
GO
ALTER TABLE [dbo].[reports_common_columns] ADD CONSTRAINT [PK_reports_common_columns] PRIMARY KEY NONCLUSTERED  ([tableid], [fieldid])
GO
PRINT N'Creating [dbo].[GetCommonColumns]'
GO

CREATE PROCEDURE [dbo].[GetCommonColumns] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT tableid, fieldid FROM reports_common_columns
END
GO
PRINT N'Creating [dbo].[GetProductUsageByAccount]'
GO












-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetProductUsageByAccount] 
	@companyId nvarchar(250),
	@numMonths int,
	@usage decimal(18,2) out
AS

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	
	declare @hostname nvarchar(50)
	declare @dbname nvarchar(100)
	declare @licencequantity int
	declare @licencetype tinyint
	declare @annualContract bit
	declare @sql nvarchar(max)
	
	
	select @licencequantity = nousers, @licencetype = licencetype, @hostname = hostname, @dbname = dbname, @annualcontract = annualContract from registeredusers inner join databases on databases.databaseID = registeredusers.dbserver where companyid = @companyId
			if @hostname = '172.24.16.209'
						set @hostname = ''
					else
						set @hostname = '[' + @hostname + '].';
	
	
	
	
	declare @startDate datetime
	declare @endDate datetime
	
	
	
	set @endDate = convert(varchar,year(GETDATE())) + '/' + convert(varchar,month(GETDATE())) + '/01 23:59:59'
	set @endDate = DATEADD(dd,-1,@enddate)		
	
	set @startDate = convert(varchar,year(GETDATE())) + '/' + convert(varchar,month(GETDATE())) + '/01 00:00:00'		
	set @startDate = DATEADD(MM,(@numMonths) / -1,@startdate)
	
	
	declare @parmdef nvarchar(max)
			
	if @annualContract = 1
		set @licencequantity = @licencequantity / 12
	
	
	set @parmdef = '@startDateIn datetime, @endDateIn datetime, @licenceQuantityIn int, @numMonthsIn int, @usageOut decimal(18,2) output '
    -- Insert statements for procedure here
    if @licencetype = 1
		set @sql = 'select @usageOut = (count(claimid) / cast((@licenceQuantityIn * @numMonthsIn) as decimal(18,2))) * 100 from ' + @hostname + '[' + @dbname + '].dbo.claims where (datesubmitted between @startDateIn and @endDateIn)'
	else if @licencetype = 2
		set @sql = 'select @usageOut = AVG(claimantAverages) from (select (count(distinct employeeid) / cast((@licenceQuantityIn) as decimal(18,2)) * 100 ) as claimantAverages from ' + @hostname + '[' + @dbname + '].dbo.claims where (datesubmitted between @startDateIn and @endDateIn) group by YEAR(datesubmitted), month(datesubmitted)) as averages'
	exec sp_executesql @sql, @parmdef, @startDateIn = @startDate, @endDateIn = @endDate, @licenceQuantityIn = @licenceQuantity, @numMonthsIn = @numMonths, @usageOut = @usage output
	
	
	
END










GO
PRINT N'Creating [dbo].[mime_headers]'
GO
CREATE TABLE [dbo].[mime_headers]
(
[fileExtension] [nvarchar] (50) NOT NULL,
[mimeHeader] [nvarchar] (150) NOT NULL,
[mimeID] [uniqueidentifier] NOT NULL CONSTRAINT [DF_mime_headers_mimeID] DEFAULT (newid()),
[description] [nvarchar] (500) NULL
)
GO
PRINT N'Creating primary key [PK_mime_headers] on [dbo].[mime_headers]'
GO
ALTER TABLE [dbo].[mime_headers] ADD CONSTRAINT [PK_mime_headers] PRIMARY KEY CLUSTERED  ([mimeID])
GO
PRINT N'Creating [dbo].[GetMimeTypes]'
GO

CREATE PROCEDURE [dbo].[GetMimeTypes] 

AS
BEGIN
	select mimeID, fileExtension, mimeHeader, [description] from dbo.mime_headers
END
GO
PRINT N'Creating [dbo].[registeredUsersHostnames]'
GO
CREATE TABLE [dbo].[registeredUsersHostnames]
(
[accountid] [int] NOT NULL,
[hostnameID] [int] NOT NULL
)
GO
PRINT N'Creating primary key [PK_registeredUsersHostnames] on [dbo].[registeredUsersHostnames]'
GO
ALTER TABLE [dbo].[registeredUsersHostnames] ADD CONSTRAINT [PK_registeredUsersHostnames] PRIMARY KEY CLUSTERED  ([accountid], [hostnameID])
GO
PRINT N'Creating [dbo].[hostnames]'
GO
CREATE TABLE [dbo].[hostnames]
(
[hostnameID] [int] NOT NULL IDENTITY(16, 1),
[hostname] [nvarchar] (300) NOT NULL,
[moduleID] [int] NOT NULL
)
GO
PRINT N'Creating primary key [PK_hostnames] on [dbo].[hostnames]'
GO
ALTER TABLE [dbo].[hostnames] ADD CONSTRAINT [PK_hostnames] PRIMARY KEY CLUSTERED  ([hostnameID])
GO
PRINT N'Creating [dbo].[GetAccountIdsBySingleSignOnIssuerUri]'
GO

CREATE PROCEDURE [dbo].[GetAccountIdsBySingleSignOnIssuerUri]
	@Hostname nvarchar(300),
	@IssuerUri nvarchar(1000)
AS

DECLARE @accounts IntPK;
DECLARE @accountID int;
DECLARE @dbname NVARCHAR(MAX);
DECLARE @UriMatchQuery NVARCHAR(MAX);
DECLARE @UriMatch int;

DECLARE AccountCursor CURSOR FOR SELECT DISTINCT registeredusers.accountid, dbname FROM registeredusers 
JOIN accountsLicencedElements on accountsLicencedElements.accountID = registeredusers.accountid
JOIN registeredUsersHostnames on registeredUsersHostnames.accountid = registeredusers.accountid
JOIN hostnames on hostnames.hostnameID = registeredUsersHostnames.hostnameID
WHERE archived = 0 and accountsLicencedElements.elementID = 185 and hostnames.hostname = @hostname

OPEN AccountCursor;
FETCH NEXT FROM AccountCursor INTO
@accountID, @dbname

WHILE @@FETCH_STATUS = 0
BEGIN
set @UriMatchQuery = 'select @UriMatch = count(issueruri) from ' + @dbname + '.dbo.SingleSignOn where IssuerUri = ''' + @issueruri + ''''
exec sp_executesql @UriMatchQuery, N'@UriMatch int out', @UriMatch out

IF (@UriMatch > 0)
	INSERT INTO @accounts VALUES (@accountId)
FETCH NEXT FROM AccountCursor INTO @accountID, @dbname
END
CLOSE AccountCursor
DEALLOCATE AccountCursor

select * from @accounts
GO
PRINT N'Creating [dbo].[ApiLicensing]'
GO
CREATE TABLE [dbo].[ApiLicensing]
(
[AccountId] [int] NOT NULL,
[TotalCalls] [int] NOT NULL,
[FreeToday] [int] NOT NULL,
[HourLimit] [int] NOT NULL,
[HourRemaining] [int] NOT NULL,
[HourLast] [datetime] NOT NULL,
[MinuteLimit] [int] NOT NULL,
[MinuteRemaining] [int] NOT NULL,
[MinuteLast] [datetime] NOT NULL
)
GO
PRINT N'Creating primary key [PK_ApiLicensing] on [dbo].[ApiLicensing]'
GO
ALTER TABLE [dbo].[ApiLicensing] ADD CONSTRAINT [PK_ApiLicensing] PRIMARY KEY CLUSTERED  ([AccountId])
GO
PRINT N'Creating [dbo].[UpdateApiAccountLicenses]'
GO
CREATE PROCEDURE [dbo].[UpdateApiAccountLicenses] 
( 
 @accountId [int],
 @totalCalls [int],
 @freeToday [int],
 @hourLimit  [int],
 @hourRemaining  [int],
 @hourLast [datetime],
 @minuteLimit  [int],
 @minuteRemaining [int],
 @minuteLast [datetime]
)
AS
BEGIN
 SET NOCOUNT ON;
 UPDATE [dbo].[ApiLicensing]
 SET 
 TotalCalls = @totalCalls,
 FreeToday = @freeToday,
 HourLimit = @hourLimit,
 HourRemaining = @hourRemaining,
 HourLast = @hourLast,
 MinuteLimit = @minuteLimit,
 MinuteRemaining = @minuteRemaining,
 MinuteLast = @minuteLast 
 WHERE AccountId = @accountId;
END
GO
PRINT N'Creating [dbo].[GetHostnames]'
GO
 
CREATE PROCEDURE [dbo].[GetHostnames] 
AS
BEGIN
	SET NOCOUNT ON;
	 
	SELECT hostname, moduleID FROM hostnames;
END
GO
PRINT N'Creating [dbo].[GetApiLicensing]'
GO
CREATE PROCEDURE [dbo].[GetApiLicensing] 
AS SELECT * FROM [dbo].[ApiLicensing]
GO
PRINT N'Creating trigger [dbo].[EnvelopeUpdate] on [dbo].[Envelopes]'
GO


-- create trigger
CREATE TRIGGER [dbo].[EnvelopeUpdate] ON [dbo].[Envelopes] FOR UPDATE
AS
 declare @envelopeId int;
 declare @accountId int;
 declare @claimId int;
 declare @envelopeNumber nvarchar(10);
 declare @crn nvarchar(12); 
 declare @envelopeStatus tinyint;
 declare @envelopeType int;
 declare @issued datetime;
 declare @assigned datetime;
 declare @received datetime;
 declare @attached datetime;
 declare @destroyed datetime;
 declare @overpayment decimal;
 declare @physicalStateUrl nvarchar(100);
 declare @lastModifiedBy int;
 declare @declaredLostInPost bit;
 declare @output nvarchar(500);
 DECLARE @cursor CURSOR
 
 SET CONCAT_NULL_YIELDS_NULL OFF;
 
 SET @cursor = CURSOR FAST_FORWARD
 FOR SELECT i.EnvelopeId, 
    i.AccountId,
    i.ClaimId,
    i.EnvelopeNumber,
    i.CRN,
    i.EnvelopeStatus,
    i.EnvelopeType,
    i.DateIssuedToClaimant,
    i.DateAssignedToClaim,
    i.DateReceived,
    i.DateAttachCompleted,
    i.DateDestroyed,
    i.OverpaymentCharge,
    i.PhysicalStateProofUrl,
    i.LastModifiedBy,
    i.DeclaredLostInPost
  FROM inserted i;
 
 OPEN @cursor;
 
 FETCH NEXT FROM @cursor into
  @envelopeId,
  @accountId,
  @claimId,
  @envelopeNumber,
  @crn,
  @envelopeStatus,
  @envelopeType,
  @issued,
  @assigned,
  @received,
  @attached,
  @destroyed,
  @overpayment,
  @physicalStateUrl,
  @lastModifiedBy,
  @declaredLostInPost
  
 WHILE @@FETCH_STATUS = 0
 BEGIN
  
  if update(AccountId)  set @output = 'AccountId: ' + ISNULL(convert(nvarchar(10), @accountId), 'null')  + ', '
  if update(ClaimId)  set @output = @output + 'ClaimId: ' + ISNULL(convert(nvarchar(10), @claimId), 'null') + ', '
  if update(EnvelopeNumber)  set @output = @output + 'EnvelopeNumber: ' + @envelopeNumber + ', '
  if update(EnvelopeStatus)  set @output = @output + 'EnvelopeStatus: ' + ISNULL(convert(nvarchar(3), @envelopeStatus), 'null') + ', '
  if update(EnvelopeType)  set @output = @output + 'EnvelopeType: ' + ISNULL(convert(nvarchar(10), @envelopeType), 'null') + ', '
  if update(DateIssuedToClaimant)  set @output = @output + 'Issued: ' + ISNULL(convert(nvarchar(20), @issued), 'null') + ', '
  if update(DateAssignedToClaim)  set @output = @output + 'Assigned: ' + ISNULL(convert(nvarchar(20), @assigned), 'null') + ', '
  if update(DateReceived)  set @output = @output + 'Received: ' + ISNULL(convert(nvarchar(20), @received), 'null') + ', '
  if update(DateAttachCompleted)  set @output = @output + 'Attached: ' + ISNULL(convert(nvarchar(20), @attached), 'null') + ', '
  if update(DateDestroyed)  set @output = @output + 'Destroyed: ' + ISNULL(convert(nvarchar(20), @destroyed), 'null') + ', '
  if update(OverpaymentCharge)  set @output = @output + 'Overpayment: ' + ISNULL(convert(nvarchar(10), @overpayment), 'null') + ', '
  if update(PhysicalStateProofUrl) set @output = @output + 'PhysicalStateProof: ' + @physicalStateUrl + ', ' 
  if update(DeclaredLostInPost) set @output = @output + 'DeclaredLostInPost: ' + ISNULL(convert(nvarchar(1), @declaredLostInPost), 'null')
   
  insert into EnvelopeHistory (
   EnvelopeId, EnvelopeStatus, Data, ModifiedBy, ModifiedOn
  ) values (
   @envelopeId, @envelopeStatus, @output, @lastModifiedBy, getutcdate()
  ) 
  
 FETCH NEXT FROM @cursor into
  @envelopeId,
  @accountId,
  @claimId,
  @envelopeNumber,
  @crn,
  @envelopeStatus,
  @envelopeType,
  @issued,
  @assigned,
  @received,
  @attached,
  @destroyed,
  @overpayment,
  @physicalStateUrl,
  @lastModifiedBy,
  @declaredLostInPost
 END
 
 SET CONCAT_NULL_YIELDS_NULL ON;
GO
PRINT N'Creating [dbo].[ResetDailyFreeCalls]'
GO
CREATE PROCEDURE [dbo].[ResetDailyFreeCalls] 
AS
 UPDATE [dbo].[ApiLicensing]
 SET FreeToday =
  CASE 
   WHEN nousers < 300
   THEN 50
   WHEN nousers >= 300 AND nousers < 600
   THEN 100
   WHEN nousers >= 600 AND nousers < 1000
   THEN 500
   WHEN nousers >= 1000 AND nousers < 6000
   THEN 1000
   WHEN nousers >= 6000
   THEN 2500 
  END
 FROM [dbo].[ApiLicensing]
 INNER JOIN [dbo].[registeredusers] ON [dbo].[ApiLicensing].AccountId = [dbo].[registeredusers].accountid
 WHERE archived = 0
 
GO
PRINT N'Creating [dbo].[AddEnvelope]'
GO


CREATE PROCEDURE [dbo].[AddEnvelope]
 @accountId int = null,
 @claimId int = null,
 @envelopeNumber nvarchar(10),
 @crn nvarchar(12) = null,
 @envelopeStatus tinyint,
 @envelopeType int,
 @dateIssuedToClaimant DateTime = null,
 @dateAssignedToClaim DateTime = null,
 @dateReceived DateTime = null,
 @dateAttachCompleted DateTime = null,
 @dateDestroyed DateTime = null,
 @overpaymentCharge decimal(16,2) = null,
 @physicalStateProofUrl nvarchar(100) = null,
 @lastModifiedBy int,
 @declaredLostInPost bit
AS
BEGIN
 INSERT INTO [dbo].[Envelopes] (
 AccountId,
 ClaimId, 
 EnvelopeNumber,
 CRN,
 EnvelopeStatus,
 EnvelopeType,
 DateIssuedToClaimant,
 DateAssignedToClaim,
 DateReceived,
 DateAttachCompleted,
 DateDestroyed,
 OverpaymentCharge,
 PhysicalStateProofUrl,
 LastModifiedBy,
 DeclaredLostInPost
) VALUES (
 @accountId,
 @claimId,
 @envelopeNumber,
 @crn,
 @envelopeStatus,
 @envelopeType,
 @dateIssuedToClaimant,
 @dateAssignedToClaim,
 @dateReceived,
 @dateAttachCompleted,
 @dateDestroyed,
 @overpaymentCharge,
 @physicalStateProofUrl,
 @lastModifiedBy,
 @declaredLostInPost
 )
 RETURN SCOPE_IDENTITY()
END
GO
PRINT N'Creating [dbo].[CalculateExcessCharges]'
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[CalculateExcessCharges] 
	@accountId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	CREATE TABLE #excessCharges (
	[year] int,
	[month] int,
	excessQuantity int,
	excessCharge money
	)
    -- Insert statements for procedure here
	declare @pricePerExcessClaim money
	declare @licencequantity int
	declare @licencetype tinyint
	declare @hostname nvarchar(50)
	declare @dbname nvarchar(100)
	declare @startdate datetime
	
	select @licencequantity = nousers, @licencetype = licencetype, @pricePerExcessClaim = priceperexcessclaim, @hostname = hostname, @dbname = dbname from registeredusers inner join databases on databases.databaseID = registeredusers.dbserver where accountid = @accountId
	
	declare @sql nvarchar(max)
			if @hostname = '192.168.101.127'
						set @hostname = ''
					else
						set @hostname = '[' + @hostname + '].';
	
	set @startDate = convert(varchar,year(GETDATE())) + '/' + convert(varchar,month(GETDATE())) + '/01'
	set @startdate = DATEADD(yyyy,-1,@startdate)
	declare @parmdef nvarchar(max)
			
	
	set @parmdef = '@startDateIn datetime, @licenceQuantityIn int, @pricePerExcessClaimIn money '					
	if @licencetype = 1
		set @sql = 'select year(datesubmitted) as [year], month(datesubmitted) as [month], case when (count(claimid) < @licencequantityIn) then 0 else (count(claimid) - @licencequantityIn) end as excessQuantity, case when count(claimid) > @licencequantityIn then (@pricePerExcessClaimIn * (count(claimid) - @licencequantityIn)) else 0 end as excessCharge from ' + @hostname + '[' + @dbname + '].dbo.claims where datesubmitted >= @startdateIn group by year(datesubmitted), month(datesubmitted) order by year(datesubmitted), month(datesubmitted)'
	else if @licencetype = 2
		set @sql = 'select year(datesubmitted) as [year], month(datesubmitted) as [month], case when (count(distinct employeeid) < @licencequantityIn) then 0 else (count(distinct employeeid) - @licencequantityIn) end as excessQuantity, case when count(distinct employeeid) > @licencequantityIn then (@pricePerExcessClaimIn * (count(distinct employeeid) - @licencequantityIn)) else 0 end as excessCharge from ' + @hostname + '[' + @dbname + '].dbo.claims where datesubmitted >= @startdateIn group by year(datesubmitted), month(datesubmitted) order by year(datesubmitted), month(datesubmitted)'
	insert into #excessCharges exec sp_executesql @sql, @parmdef, @startDateIn = @startDate, @licencequantityIn = @licencequantity, @pricePerExcessClaimIn = @pricePerExcessClaim
	
	select * from #excessCharges
END


GO
PRINT N'Creating [dbo].[EditEnvelope]'
GO


-- this method only updates the fields that you pass without overwriting everything will null.
CREATE PROCEDURE [dbo].[EditEnvelope]
 @envelopeId int,
 @accountId int = null,
 @claimId int = null,
 @envelopeNumber nvarchar(10) = null,
 @crn nvarchar(12) = null,
 @envelopeStatus tinyint,
 @envelopeType int,
 @dateIssuedToClaimant DateTime = null,
 @dateAssignedToClaim DateTime = null,
 @dateReceived DateTime = null,
 @dateAttachCompleted DateTime = null,
 @dateDestroyed DateTime = null,
 @overpaymentCharge decimal(16,2) = null,
 @physicalStateProofUrl nvarchar(100) = null,
 @lastModifiedBy int,
 @declaredLostInPost bit
AS
BEGIN
 SET CONCAT_NULL_YIELDS_NULL OFF;
 UPDATE [dbo].[Envelopes]
 SET 
  AccountId = (CASE WHEN @accountId IS NOT NULL THEN @accountId ELSE AccountId END),
  ClaimId = (CASE WHEN @claimId IS NOT NULL THEN @claimId ELSE ClaimId END), 
  EnvelopeNumber = (CASE WHEN @envelopeNumber IS NOT NULL THEN @envelopeNumber ELSE EnvelopeNumber END), 
  CRN = (CASE WHEN @crn IS NOT NULL THEN @crn ELSE CRN END), 
  EnvelopeStatus = (CASE WHEN @envelopeStatus IS NOT NULL THEN @envelopeStatus ELSE EnvelopeStatus END), 
  EnvelopeType = (CASE WHEN @envelopeType IS NOT NULL THEN @envelopeType ELSE EnvelopeType END), 
  DateIssuedToClaimant = (CASE WHEN @dateIssuedToClaimant IS NOT NULL THEN @dateIssuedToClaimant ELSE DateIssuedToClaimant END), 
  DateAssignedToClaim = (CASE WHEN @dateAssignedToClaim IS NOT NULL THEN @dateAssignedToClaim ELSE DateAssignedToClaim END), 
  DateReceived = (CASE WHEN @dateReceived IS NOT NULL THEN @dateReceived ELSE DateReceived END), 
  DateAttachCompleted = (CASE WHEN @dateAttachCompleted IS NOT NULL THEN @dateAttachCompleted ELSE DateAttachCompleted END),
  DateDestroyed = (CASE WHEN @dateDestroyed IS NOT NULL THEN @dateDestroyed ELSE DateDestroyed END),
  OverpaymentCharge = (CASE WHEN @overpaymentCharge IS NOT NULL THEN @overpaymentCharge ELSE OverpaymentCharge END), 
  PhysicalStateProofUrl = (CASE WHEN @physicalStateProofUrl IS NOT NULL THEN @physicalStateProofUrl ELSE PhysicalStateProofUrl END), 
  LastModifiedBy = (CASE WHEN @lastModifiedBy IS NOT NULL THEN @lastModifiedBy ELSE LastModifiedBy END),
  DeclaredLostInPost = (CASE WHEN @declaredLostInPost IS NOT NULL THEN @declaredLostInPost ELSE DeclaredLostInPost END)
 WHERE EnvelopeId = @envelopeId;
 SET CONCAT_NULL_YIELDS_NULL ON;
END
GO
PRINT N'Creating [dbo].[AddEnvelopeBatch]'
GO



CREATE PROCEDURE [dbo].[AddEnvelopeBatch]
 @envelopes [EnvelopeBatchAdd] readonly
AS
BEGIN

-- create temp table
DECLARE @output TABLE
(
 Id int,
 EnvelopeNumber varchar(10)
)

INSERT INTO [dbo].[Envelopes] (
 AccountId,
 ClaimId, 
 EnvelopeNumber,
 CRN,
 EnvelopeStatus,
 EnvelopeType,
 DateIssuedToClaimant,
 DateAssignedToClaim,
 DateReceived,
 DateAttachCompleted,
 DateDestroyed,
 OverpaymentCharge,
 PhysicalStateProofUrl,
 LastModifiedBy,
 DeclaredLostInPost
) 
OUTPUT SCOPE_IDENTITY(), inserted.EnvelopeNumber
INTO @output
SELECT 
 AccountId,
 ClaimId, 
 EnvelopeNumber,
 CRN,
 EnvelopeStatus,
 EnvelopeType,
 DateIssuedToClaimant,
 DateAssignedToClaim,
 DateReceived,
 DateAttachCompleted,
 DateDestroyed,
 OverpaymentCharge,
 PhysicalStateProofUrl,
 LastModifiedBy,
 DeclaredLostInPost
FROM @envelopes;

RETURN SELECT * FROM @output;
END
GO
PRINT N'Creating [dbo].[EditEnvelopeBatch]'
GO


CREATE PROCEDURE [dbo].[EditEnvelopeBatch]
 @envelopes EnvelopeBatchEdit readonly
AS
BEGIN
 SET CONCAT_NULL_YIELDS_NULL OFF;
 UPDATE t
 SET 
  t.AccountId = (CASE WHEN e.AccountId IS NOT NULL THEN e.AccountId ELSE t.AccountId END),
  t.ClaimId = (CASE WHEN e.ClaimId IS NOT NULL THEN e.ClaimId ELSE t.ClaimId END), 
  t.EnvelopeNumber = (CASE WHEN t.EnvelopeNumber IS NOT NULL THEN e.EnvelopeNumber ELSE t.EnvelopeNumber END), 
  t.CRN = (CASE WHEN e.CRN IS NOT NULL THEN e.CRN ELSE t.CRN END), 
  t.EnvelopeStatus = (CASE WHEN e.EnvelopeStatus IS NOT NULL THEN e.EnvelopeStatus ELSE t.EnvelopeStatus END), 
  t.EnvelopeType = (CASE WHEN e.EnvelopeType IS NOT NULL THEN e.EnvelopeType ELSE t.EnvelopeType END), 
  t.DateIssuedToClaimant = (CASE WHEN e.DateIssuedToClaimant IS NOT NULL THEN e.DateIssuedToClaimant ELSE t.DateIssuedToClaimant END), 
  t.DateAssignedToClaim = (CASE WHEN e.DateAssignedToClaim IS NOT NULL THEN e.DateAssignedToClaim ELSE t.DateAssignedToClaim END), 
  t.DateReceived = (CASE WHEN e.DateReceived IS NOT NULL THEN e.DateReceived ELSE t.DateReceived END),
  t.DateAttachCompleted = (CASE WHEN e.dateAttachCompleted IS NOT NULL THEN e.dateAttachCompleted ELSE t.DateAttachCompleted END),
  t.DateDestroyed = (CASE WHEN e.dateDestroyed IS NOT NULL THEN e.dateDestroyed ELSE t.DateDestroyed END),
  t.OverpaymentCharge = (CASE WHEN e.OverpaymentCharge IS NOT NULL THEN e.OverpaymentCharge ELSE t.OverpaymentCharge END), 
  t.PhysicalStateProofUrl = (CASE WHEN e.PhysicalStateProofUrl IS NOT NULL THEN e.PhysicalStateProofUrl ELSE t.PhysicalStateProofUrl END), 
  t.LastModifiedBy = (CASE WHEN e.LastModifiedBy IS NOT NULL THEN e.LastModifiedBy ELSE t.LastModifiedBy END),
  t.DeclaredLostInPost = (CASE WHEN e.DeclaredLostInPost IS NOT NULL THEN e.DeclaredLostInPost ELSE t.DeclaredLostInPost END)
 FROM @envelopes e
 INNER JOIN [dbo].[Envelopes] t
 ON t.EnvelopeId = e.EnvelopeId;
 SET CONCAT_NULL_YIELDS_NULL ON;
END
GO
PRINT N'Creating [dbo].[locales]'
GO
CREATE TABLE [dbo].[locales]
(
[localeID] [int] NOT NULL IDENTITY(153, 1),
[localeName] [nvarchar] (250) NOT NULL,
[localeCode] [nvarchar] (50) NOT NULL,
[active] [bit] NOT NULL CONSTRAINT [DF_locales_active] DEFAULT ((0))
)
GO
PRINT N'Creating primary key [PK_locales] on [dbo].[locales]'
GO
ALTER TABLE [dbo].[locales] ADD CONSTRAINT [PK_locales] PRIMARY KEY CLUSTERED  ([localeID])
GO
PRINT N'Adding constraints to [dbo].[locales]'
GO
ALTER TABLE [dbo].[locales] ADD CONSTRAINT [IX_locales] UNIQUE NONCLUSTERED  ([localeName])
GO
PRINT N'Creating [dbo].[report_folders]'
GO
CREATE TABLE [dbo].[report_folders]
(
[foldername] [nvarchar] (100) NOT NULL,
[CreatedOn] [datetime] NULL,
[CreatedBy] [int] NULL,
[ModifiedOn] [datetime] NULL,
[ModifiedBy] [int] NULL,
[folderid] [uniqueidentifier] NOT NULL CONSTRAINT [DF_report_folders_folderid] DEFAULT (newid())
)
GO
PRINT N'Creating primary key [PK_report_folders_1] on [dbo].[report_folders]'
GO
ALTER TABLE [dbo].[report_folders] ADD CONSTRAINT [PK_report_folders_1] PRIMARY KEY CLUSTERED  ([folderid])
GO
PRINT N'Creating [dbo].[viewgroups_base]'
GO
CREATE TABLE [dbo].[viewgroups_base]
(
[groupname] [nvarchar] (50) NOT NULL,
[level] [int] NOT NULL CONSTRAINT [DF_viewgroups_level] DEFAULT ((1)),
[amendedon] [datetime] NULL CONSTRAINT [DF_viewgroups_amendedon] DEFAULT (getdate()),
[viewgroupid] [uniqueidentifier] NOT NULL CONSTRAINT [DF_viewgroups_viewgroupid_new] DEFAULT (newid()),
[parentid] [uniqueidentifier] NULL,
[alias] [nvarchar] (150) NULL,
[relabel_param] [nvarchar] (150) NULL
)
GO
PRINT N'Creating primary key [PK_viewgroups] on [dbo].[viewgroups_base]'
GO
ALTER TABLE [dbo].[viewgroups_base] ADD CONSTRAINT [PK_viewgroups] PRIMARY KEY NONCLUSTERED  ([viewgroupid])
GO
PRINT N'Adding constraints to [dbo].[viewgroups_base]'
GO
ALTER TABLE [dbo].[viewgroups_base] ADD CONSTRAINT [IX_viewgroups_base] UNIQUE NONCLUSTERED  ([parentid], [groupname])
GO
PRINT N'Creating [dbo].[reportcriteria]'
GO
CREATE TABLE [dbo].[reportcriteria]
(
[condition] [tinyint] NOT NULL,
[value1] [nvarchar] (1000) NULL,
[value2] [nvarchar] (1000) NULL,
[andor] [tinyint] NOT NULL CONSTRAINT [DF_reportcriteria_andor] DEFAULT ((0)),
[order] [int] NOT NULL CONSTRAINT [DF_reportcriteria_order] DEFAULT ((1)),
[runtime] [bit] NOT NULL CONSTRAINT [DF_reportcriteria_runtime] DEFAULT ((0)),
[groupnumber] [tinyint] NULL,
[criteriaid] [uniqueidentifier] NOT NULL CONSTRAINT [DF_reportcriteria_criteriaid] DEFAULT (newid()),
[reportid] [uniqueidentifier] NOT NULL CONSTRAINT [DF_reportcriteria_reportid] DEFAULT (newid()),
[fieldID] [uniqueidentifier] NULL
)
GO
PRINT N'Creating primary key [PK_reportcriteria_1] on [dbo].[reportcriteria]'
GO
ALTER TABLE [dbo].[reportcriteria] ADD CONSTRAINT [PK_reportcriteria_1] PRIMARY KEY NONCLUSTERED  ([criteriaid])
GO
PRINT N'Creating [dbo].[elementsBase]'
GO
CREATE TABLE [dbo].[elementsBase]
(
[elementID] [int] NOT NULL,
[categoryID] [int] NOT NULL,
[elementName] [nvarchar] (100) NOT NULL,
[elementFriendlyName] [nvarchar] (100) NULL,
[description] [nvarchar] (4000) NULL,
[accessRolesCanEdit] [bit] NULL CONSTRAINT [DF_elements_base_accessRolesCanEdit] DEFAULT ((1)),
[accessRolesCanAdd] [bit] NULL CONSTRAINT [DF_elements_base_accessRolesCanAdd] DEFAULT ((1)),
[accessRolesCanDelete] [bit] NULL CONSTRAINT [DF_elements_base_accessRolesCanDelete] DEFAULT ((1)),
[accessRolesApplicable] [bit] NOT NULL CONSTRAINT [DF_elementsBase_accessRolesApplicable] DEFAULT ((1))
)
GO
PRINT N'Creating primary key [PK_elements_base] on [dbo].[elementsBase]'
GO
ALTER TABLE [dbo].[elementsBase] ADD CONSTRAINT [PK_elements_base] PRIMARY KEY CLUSTERED  ([elementID])
GO
PRINT N'Adding constraints to [dbo].[elementsBase]'
GO
ALTER TABLE [dbo].[elementsBase] ADD CONSTRAINT [IX_elementsBase_1] UNIQUE NONCLUSTERED  ([elementFriendlyName])
GO
PRINT N'Adding constraints to [dbo].[elementsBase]'
GO
ALTER TABLE [dbo].[elementsBase] ADD CONSTRAINT [IX_elementsBase] UNIQUE NONCLUSTERED  ([elementName])
GO
PRINT N'Creating [dbo].[reports]'
GO
CREATE TABLE [dbo].[reports]
(
[reportname] [nvarchar] (150) NOT NULL,
[description] [nvarchar] (2000) NULL,
[curexportnum] [int] NOT NULL CONSTRAINT [DF_reports_curexportnum] DEFAULT ((1)),
[lastexportdate] [datetime] NULL,
[footerreport] [bit] NOT NULL CONSTRAINT [DF_reports_footerreport] DEFAULT ((0)),
[readonly] [bit] NOT NULL CONSTRAINT [DF_reports_readonly] DEFAULT ((0)),
[forclaimants] [bit] NOT NULL CONSTRAINT [DF_reports_forclaimants] DEFAULT ((0)),
[allowexport] [bit] NOT NULL CONSTRAINT [DF_reports_allowexport] DEFAULT ((0)),
[exporttype] [tinyint] NOT NULL CONSTRAINT [DF_reports_exporttype] DEFAULT ((3)),
[CreatedOn] [datetime] NULL,
[CreatedBy] [int] NULL,
[ModifiedOn] [datetime] NULL,
[ModifiedBy] [int] NULL,
[staticReportSQL] [nvarchar] (max) NULL,
[limit] [smallint] NOT NULL CONSTRAINT [DF_reports_limit] DEFAULT ((0)),
[basetable] [uniqueidentifier] NOT NULL,
[reportid] [uniqueidentifier] NOT NULL CONSTRAINT [DF_reports_reportid] DEFAULT (newid()),
[folderid] [uniqueidentifier] NULL,
[footerreportid] [uniqueidentifier] NULL,
[module] [tinyint] NULL
)
GO
PRINT N'Creating primary key [PK_reports] on [dbo].[reports]'
GO
ALTER TABLE [dbo].[reports] ADD CONSTRAINT [PK_reports] PRIMARY KEY NONCLUSTERED  ([reportid])
GO
PRINT N'Creating [dbo].[help_text]'
GO
CREATE TABLE [dbo].[help_text]
(
[page] [nvarchar] (50) NULL,
[description] [nvarchar] (200) NOT NULL,
[helptext] [nvarchar] (4000) NULL,
[tooltipID] [uniqueidentifier] NOT NULL CONSTRAINT [DF_help_text_tooltipID] DEFAULT (newid()),
[tooltipArea] [nvarchar] (3) NOT NULL,
[moduleID] [int] NULL
)
GO
PRINT N'Creating primary key [PK_help_text] on [dbo].[help_text]'
GO
ALTER TABLE [dbo].[help_text] ADD CONSTRAINT [PK_help_text] PRIMARY KEY NONCLUSTERED  ([tooltipID])
GO
PRINT N'Creating [dbo].[emailNotifications]'
GO
CREATE TABLE [dbo].[emailNotifications]
(
[emailNotificationID] [int] NOT NULL IDENTITY(13, 1),
[name] [nvarchar] (100) NOT NULL,
[description] [nvarchar] (4000) NULL,
[emailTemplateID] [int] NOT NULL,
[enabled] [bit] NOT NULL CONSTRAINT [DF_emailNotifications_enabled] DEFAULT ((0)),
[customerType] [int] NOT NULL CONSTRAINT [DF_emailNotifications_customerType] DEFAULT ((1)),
[emailNotificationType] [int] NOT NULL
)
GO
PRINT N'Creating primary key [PK_emailNotifications] on [dbo].[emailNotifications]'
GO
ALTER TABLE [dbo].[emailNotifications] ADD CONSTRAINT [PK_emailNotifications] PRIMARY KEY CLUSTERED  ([emailNotificationID])
GO
PRINT N'Creating [dbo].[global_countries]'
GO
CREATE TABLE [dbo].[global_countries]
(
[globalcountryid] [int] NOT NULL IDENTITY(262, 1),
[country] [nvarchar] (100) NOT NULL,
[countrycode] [nvarchar] (2) NOT NULL,
[createdon] [datetime] NULL,
[modifiedon] [datetime] NULL,
[postcodeRegexFormat] [nvarchar] (max) NULL,
[alpha3CountryCode] [nvarchar] (3) NULL,
[numeric3Code] [int] NOT NULL CONSTRAINT [DF_global_countries_numeric3Code] DEFAULT ((0)),
[postcodeAnywhereEnabled] [bit] NOT NULL CONSTRAINT [DF_global_countries_postcodeAnywhereEnabled] DEFAULT ((0))
)
GO
PRINT N'Creating primary key [PK_global_countries] on [dbo].[global_countries]'
GO
ALTER TABLE [dbo].[global_countries] ADD CONSTRAINT [PK_global_countries] PRIMARY KEY CLUSTERED  ([globalcountryid])
GO
PRINT N'Creating [dbo].[global_faqs]'
GO
CREATE TABLE [dbo].[global_faqs]
(
[faqid] [int] NOT NULL IDENTITY(59, 1),
[question] [nvarchar] (4000) NOT NULL,
[answer] [nvarchar] (4000) NOT NULL,
[tip] [nvarchar] (200) NULL,
[datecreated] [datetime] NOT NULL CONSTRAINT [DF_faqs_datecreated] DEFAULT (getdate()),
[faqcategoryid] [int] NOT NULL,
[CreatedOn] [datetime] NULL,
[CreatedBy] [int] NULL,
[ModifiedOn] [datetime] NULL,
[ModifiedBy] [int] NULL
)
GO
PRINT N'Creating primary key [PK_faqs] on [dbo].[global_faqs]'
GO
ALTER TABLE [dbo].[global_faqs] ADD CONSTRAINT [PK_faqs] PRIMARY KEY CLUSTERED  ([faqid])
GO
PRINT N'Creating [dbo].[globalESRElementFields]'
GO
CREATE TABLE [dbo].[globalESRElementFields]
(
[globalESRElementFieldID] [int] NOT NULL IDENTITY(387, 1),
[globalESRElementID] [int] NOT NULL,
[ESRElementFieldName] [nvarchar] (250) NOT NULL,
[isMandatory] [bit] NOT NULL CONSTRAINT [DF_globalESRElementFields_isMandatory] DEFAULT ((0)),
[isControlColumn] [bit] NOT NULL CONSTRAINT [DF_globalESRElementFields_isControlColumn] DEFAULT ((0)),
[isSummaryColumn] [bit] NOT NULL CONSTRAINT [DF_globalESRElementFields_isSummaryColumn] DEFAULT ((0)),
[isRounded] [bit] NOT NULL CONSTRAINT [DF_globalESRElementFields_isRounded] DEFAULT ((0))
)
GO
PRINT N'Creating primary key [PK_globalESRElementFields] on [dbo].[globalESRElementFields]'
GO
ALTER TABLE [dbo].[globalESRElementFields] ADD CONSTRAINT [PK_globalESRElementFields] PRIMARY KEY CLUSTERED  ([globalESRElementFieldID])
GO
PRINT N'Creating index [IX_globalESRElementFields] on [dbo].[globalESRElementFields]'
GO
CREATE NONCLUSTERED INDEX [IX_globalESRElementFields] ON [dbo].[globalESRElementFields] ([globalESRElementID])
GO
PRINT N'Creating [dbo].[administrators]'
GO
CREATE TABLE [dbo].[administrators]
(
[administratorid] [int] NOT NULL IDENTITY(1, 1),
[username] [nvarchar] (50) NOT NULL,
[password] [nvarchar] (100) NOT NULL,
[email] [nvarchar] (150) NOT NULL,
[title] [nvarchar] (20) NOT NULL,
[firstname] [nvarchar] (50) NOT NULL,
[surname] [nvarchar] (50) NOT NULL,
[resellerid] [int] NULL,
[archived] [bit] NOT NULL CONSTRAINT [DF_administrators_archived] DEFAULT ((0))
)
GO
PRINT N'Creating primary key [PK_administrators] on [dbo].[administrators]'
GO
ALTER TABLE [dbo].[administrators] ADD CONSTRAINT [PK_administrators] PRIMARY KEY CLUSTERED  ([administratorid])
GO
PRINT N'Creating [dbo].[hotel_reviews]'
GO
CREATE TABLE [dbo].[hotel_reviews]
(
[reviewid] [int] NOT NULL IDENTITY(79, 1),
[hotelid] [int] NOT NULL,
[rating] [tinyint] NOT NULL,
[review] [nvarchar] (4000) NULL,
[employeeid] [int] NULL,
[displaytype] [tinyint] NOT NULL,
[amountpaid] [money] NULL,
[reviewdate] [datetime] NOT NULL CONSTRAINT [DF_hotel_review_reviewdate] DEFAULT (getdate()),
[standardrooms] [tinyint] NULL,
[hotelfacilities] [tinyint] NULL,
[valuemoney] [tinyint] NULL,
[performancestaff] [tinyint] NULL,
[location] [tinyint] NULL
)
GO
PRINT N'Creating primary key [PK_hotel_review] on [dbo].[hotel_reviews]'
GO
ALTER TABLE [dbo].[hotel_reviews] ADD CONSTRAINT [PK_hotel_review] PRIMARY KEY CLUSTERED  ([reviewid])
GO
PRINT N'Creating [dbo].[mobileAPITypes]'
GO
CREATE TABLE [dbo].[mobileAPITypes]
(
[API_TypeId] [uniqueidentifier] NOT NULL CONSTRAINT [DF_mobileAPITypes_API_TypeId] DEFAULT (newid()),
[typeKey] [nvarchar] (50) NOT NULL,
[typeDescription] [nvarchar] (250) NULL,
[modifiedOn] [datetime] NULL
)
GO
PRINT N'Creating primary key [PK_mobileAPITypes] on [dbo].[mobileAPITypes]'
GO
ALTER TABLE [dbo].[mobileAPITypes] ADD CONSTRAINT [PK_mobileAPITypes] PRIMARY KEY CLUSTERED  ([API_TypeId])
GO
PRINT N'Creating [dbo].[mobileAPIVersion]'
GO
CREATE TABLE [dbo].[mobileAPIVersion]
(
[VersionID] [uniqueidentifier] NOT NULL,
[mobileAPITypeID] [uniqueidentifier] NOT NULL,
[versionNumber] [nvarchar] (20) NULL,
[disableAppUsage] [bit] NOT NULL CONSTRAINT [DF_mobileAPIVersion_disableAppUsage] DEFAULT ((0)),
[notifyMessage] [nvarchar] (300) NULL,
[title] [nvarchar] (100) NULL,
[syncMessage] [nvarchar] (300) NULL,
[appStoreURL] [nvarchar] (300) NULL,
[modifiedOn] [datetime] NULL
)
GO
PRINT N'Creating primary key [PK_mobileAPIVersion] on [dbo].[mobileAPIVersion]'
GO
ALTER TABLE [dbo].[mobileAPIVersion] ADD CONSTRAINT [PK_mobileAPIVersion] PRIMARY KEY CLUSTERED  ([VersionID])
GO
PRINT N'Creating [dbo].[elementCategoryBase]'
GO
CREATE TABLE [dbo].[elementCategoryBase]
(
[categoryName] [nvarchar] (100) NOT NULL,
[description] [nvarchar] (4000) NULL,
[moduleID] [int] NOT NULL,
[categoryID] [int] NOT NULL
)
GO
PRINT N'Creating primary key [PK_module_category_base] on [dbo].[elementCategoryBase]'
GO
ALTER TABLE [dbo].[elementCategoryBase] ADD CONSTRAINT [PK_module_category_base] PRIMARY KEY CLUSTERED  ([categoryID])
GO
PRINT N'Creating [dbo].[excessCharges]'
GO
CREATE TABLE [dbo].[excessCharges]
(
[accountId] [int] NOT NULL,
[quantity] [int] NOT NULL,
[pricePerExcess] [money] NOT NULL
)
GO
PRINT N'Creating primary key [PK_excessCharges] on [dbo].[excessCharges]'
GO
ALTER TABLE [dbo].[excessCharges] ADD CONSTRAINT [PK_excessCharges] PRIMARY KEY CLUSTERED  ([accountId], [quantity])
GO
PRINT N'Creating [dbo].[global_faqcategories]'
GO
CREATE TABLE [dbo].[global_faqcategories]
(
[faqcategoryid] [int] NOT NULL IDENTITY(11, 1),
[category] [nvarchar] (50) NOT NULL,
[CreatedOn] [datetime] NULL,
[CreatedBy] [int] NULL,
[ModifiedOn] [datetime] NULL,
[ModifiedBy] [int] NULL
)
GO
PRINT N'Creating primary key [PK_faqcategories] on [dbo].[global_faqcategories]'
GO
ALTER TABLE [dbo].[global_faqcategories] ADD CONSTRAINT [PK_faqcategories] PRIMARY KEY CLUSTERED  ([faqcategoryid])
GO
PRINT N'Creating [dbo].[TreeGroup]'
GO
CREATE TABLE [dbo].[TreeGroup]
(
[Id] [uniqueidentifier] NOT NULL,
[Name] [nvarchar] (50) NOT NULL
)
GO
PRINT N'Creating primary key [PK_TreeGroup] on [dbo].[TreeGroup]'
GO
ALTER TABLE [dbo].[TreeGroup] ADD CONSTRAINT [PK_TreeGroup] PRIMARY KEY NONCLUSTERED  ([Id])
GO
PRINT N'Creating [dbo].[globalESRElements]'
GO
CREATE TABLE [dbo].[globalESRElements]
(
[globalESRElementID] [int] NOT NULL IDENTITY(55, 1),
[ESRElementName] [nvarchar] (250) NOT NULL
)
GO
PRINT N'Creating primary key [PK_globalESRElements] on [dbo].[globalESRElements]'
GO
ALTER TABLE [dbo].[globalESRElements] ADD CONSTRAINT [PK_globalESRElements] PRIMARY KEY CLUSTERED  ([globalESRElementID])
GO
PRINT N'Creating [dbo].[Theme]'
GO
CREATE TABLE [dbo].[Theme]
(
[ThemeId] [int] NOT NULL IDENTITY(1, 1),
[Theme] [nvarchar] (50) NOT NULL
)
GO
PRINT N'Creating primary key [PK_Theme_1] on [dbo].[Theme]'
GO
ALTER TABLE [dbo].[Theme] ADD CONSTRAINT [PK_Theme_1] PRIMARY KEY CLUSTERED  ([ThemeId])
GO
PRINT N'Creating [dbo].[reports_allowedtables_base]'
GO
CREATE TABLE [dbo].[reports_allowedtables_base]
(
[basetableid] [uniqueidentifier] NOT NULL,
[tableid] [uniqueidentifier] NOT NULL
)
GO
PRINT N'Creating primary key [PK_reports_allowedtables_base] on [dbo].[reports_allowedtables_base]'
GO
ALTER TABLE [dbo].[reports_allowedtables_base] ADD CONSTRAINT [PK_reports_allowedtables_base] PRIMARY KEY NONCLUSTERED  ([basetableid], [tableid])
GO
PRINT N'Creating [dbo].[rptlistitems]'
GO
CREATE TABLE [dbo].[rptlistitems]
(
[listitem] [nvarchar] (100) NOT NULL,
[fieldid] [uniqueidentifier] NOT NULL,
[listvalue] [nvarchar] (50) NOT NULL,
[valuetype] [nvarchar] (50) NULL
)
GO
PRINT N'Creating primary key [PK_rptlistitems_1] on [dbo].[rptlistitems]'
GO
ALTER TABLE [dbo].[rptlistitems] ADD CONSTRAINT [PK_rptlistitems_1] PRIMARY KEY NONCLUSTERED  ([listitem], [fieldid])
GO
PRINT N'Creating [dbo].[SupportQuestionHeadings]'
GO
CREATE TABLE [dbo].[SupportQuestionHeadings]
(
[SupportQuestionHeadingId] [int] NOT NULL IDENTITY(1, 1),
[Heading] [nvarchar] (250) NOT NULL,
[ModuleId] [int] NOT NULL,
[Order] [int] NOT NULL
)
GO
PRINT N'Creating primary key [PK_SupportStatementHeadings] on [dbo].[SupportQuestionHeadings]'
GO
ALTER TABLE [dbo].[SupportQuestionHeadings] ADD CONSTRAINT [PK_SupportStatementHeadings] PRIMARY KEY CLUSTERED  ([SupportQuestionHeadingId])
GO
PRINT N'Creating [dbo].[fn_diagramobjects]'
GO
CREATE FUNCTION [dbo].[fn_diagramobjects]() 
	RETURNS int
	WITH EXECUTE AS N'dbo'
	AS
	BEGIN
		declare @id_upgraddiagrams		int
		declare @id_sysdiagrams			int
		declare @id_helpdiagrams		int
		declare @id_helpdiagramdefinition	int
		declare @id_creatediagram	int
		declare @id_renamediagram	int
		declare @id_alterdiagram 	int 
		declare @id_dropdiagram		int
		declare @InstalledObjects	int

		select @InstalledObjects = 0

		select 	@id_upgraddiagrams = object_id(N'dbo.sp_upgraddiagrams'),
			@id_sysdiagrams = object_id(N'dbo.sysdiagrams'),
			@id_helpdiagrams = object_id(N'dbo.sp_helpdiagrams'),
			@id_helpdiagramdefinition = object_id(N'dbo.sp_helpdiagramdefinition'),
			@id_creatediagram = object_id(N'dbo.sp_creatediagram'),
			@id_renamediagram = object_id(N'dbo.sp_renamediagram'),
			@id_alterdiagram = object_id(N'dbo.sp_alterdiagram'), 
			@id_dropdiagram = object_id(N'dbo.sp_dropdiagram')

		if @id_upgraddiagrams is not null
			select @InstalledObjects = @InstalledObjects + 1
		if @id_sysdiagrams is not null
			select @InstalledObjects = @InstalledObjects + 2
		if @id_helpdiagrams is not null
			select @InstalledObjects = @InstalledObjects + 4
		if @id_helpdiagramdefinition is not null
			select @InstalledObjects = @InstalledObjects + 8
		if @id_creatediagram is not null
			select @InstalledObjects = @InstalledObjects + 16
		if @id_renamediagram is not null
			select @InstalledObjects = @InstalledObjects + 32
		if @id_alterdiagram  is not null
			select @InstalledObjects = @InstalledObjects + 64
		if @id_dropdiagram is not null
			select @InstalledObjects = @InstalledObjects + 128
		
		return @InstalledObjects 
	END
	
GO
PRINT N'Creating [dbo].[getClientUsageForMonth]'
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[getClientUsageForMonth] 
(
	@month int,
	@year int,
	@hostname nvarchar(50),
	@licencetype tinyint,
	@dbname nvarchar(100)
)
RETURNS int
AS
BEGIN
	declare @sql nvarchar(max)
	if @hostname = '192.168.101.127'
				set @hostname = ''
			else
				set @hostname = '[' + @hostname + '].';
	declare @count int
	declare @parmdef nvarchar(max)
	
	set @parmdef = '@monthIn int, @yearIn int, @countIn int output'
	if @licencetype = 1
		set @sql = 'select count(claimid) from ' + @hostname + '[' + @dbname + '].dbo.claims where month(createdon) = @monthIn and year(createdon) = @yearIn'
		
	
	
	
	exec sp_executesql @sql, @parmdef, @monthIn = @month, @yearIn = @year, @countIn = @count
	
	return @count
END
GO
PRINT N'Creating [dbo].[global_currencies]'
GO
CREATE TABLE [dbo].[global_currencies]
(
[globalcurrencyid] [int] NOT NULL IDENTITY(171, 1),
[label] [nvarchar] (500) NOT NULL,
[alphacode] [nvarchar] (50) NOT NULL,
[numericcode] [nvarchar] (50) NOT NULL,
[symbol] [nvarchar] (50) NULL,
[createdon] [datetime] NULL,
[modifiedon] [datetime] NULL
)
GO
PRINT N'Creating primary key [PK_global_currencies] on [dbo].[global_currencies]'
GO
ALTER TABLE [dbo].[global_currencies] ADD CONSTRAINT [PK_global_currencies] PRIMARY KEY CLUSTERED  ([globalcurrencyid])
GO
PRINT N'Creating [dbo].[help_text_new]'
GO
CREATE TABLE [dbo].[help_text_new]
(
[helpid] [float] NOT NULL,
[page] [nvarchar] (255) NULL,
[description] [nvarchar] (255) NULL,
[helptext] [nvarchar] (max) NULL
)
GO
PRINT N'Creating primary key [PK_help_text_new] on [dbo].[help_text_new]'
GO
ALTER TABLE [dbo].[help_text_new] ADD CONSTRAINT [PK_help_text_new] PRIMARY KEY CLUSTERED  ([helpid])
GO
PRINT N'Creating [dbo].[languages]'
GO
CREATE TABLE [dbo].[languages]
(
[phraseid] [int] NOT NULL IDENTITY(605, 1),
[phrase] [nvarchar] (4000) NOT NULL,
[Dutch] [nvarchar] (4000) NULL
)
GO
PRINT N'Creating primary key [PK_languages] on [dbo].[languages]'
GO
ALTER TABLE [dbo].[languages] ADD CONSTRAINT [PK_languages] PRIMARY KEY CLUSTERED  ([phraseid])
GO
PRINT N'Creating [dbo].[logErrorReasons]'
GO
CREATE TABLE [dbo].[logErrorReasons]
(
[logReasonID] [int] NOT NULL IDENTITY(13, 1),
[reasonType] [tinyint] NOT NULL,
[reason] [nvarchar] (4000) NOT NULL,
[createdon] [datetime] NULL,
[modifiedon] [datetime] NULL
)
GO
PRINT N'Creating primary key [PK_logErrorReasons] on [dbo].[logErrorReasons]'
GO
ALTER TABLE [dbo].[logErrorReasons] ADD CONSTRAINT [PK_logErrorReasons] PRIMARY KEY CLUSTERED  ([logReasonID])
GO
PRINT N'Creating [dbo].[menu_structure_base]'
GO
CREATE TABLE [dbo].[menu_structure_base]
(
[menuid] [int] NOT NULL IDENTITY(8, 1),
[menu_name] [nvarchar] (100) NOT NULL,
[parentid] [int] NULL
)
GO
PRINT N'Creating primary key [PK_menu_structure] on [dbo].[menu_structure_base]'
GO
ALTER TABLE [dbo].[menu_structure_base] ADD CONSTRAINT [PK_menu_structure] PRIMARY KEY CLUSTERED  ([menuid])
GO
PRINT N'Creating [dbo].[resellers]'
GO
CREATE TABLE [dbo].[resellers]
(
[resellerid] [int] NOT NULL IDENTITY(1, 1),
[name] [nvarchar] (50) NOT NULL
)
GO
PRINT N'Creating primary key [PK_resellers] on [dbo].[resellers]'
GO
ALTER TABLE [dbo].[resellers] ADD CONSTRAINT [PK_resellers] PRIMARY KEY CLUSTERED  ([resellerid])
GO
PRINT N'Creating [dbo].[sysdiagrams]'
GO
CREATE TABLE [dbo].[sysdiagrams]
(
[name] [sys].[sysname] NOT NULL,
[principal_id] [int] NOT NULL,
[diagram_id] [int] NOT NULL IDENTITY(1, 1),
[version] [int] NULL,
[definition] [varbinary] (max) NULL
)
GO
PRINT N'Creating primary key [PK__sysdiagrams__17C81630] on [dbo].[sysdiagrams]'
GO
ALTER TABLE [dbo].[sysdiagrams] ADD CONSTRAINT [PK__sysdiagrams__17C81630] PRIMARY KEY CLUSTERED  ([diagram_id])
GO
PRINT N'Adding constraints to [dbo].[sysdiagrams]'
GO
ALTER TABLE [dbo].[sysdiagrams] ADD CONSTRAINT [UK_principal_name] UNIQUE NONCLUSTERED  ([principal_id], [name])
GO
PRINT N'Creating [dbo].[APIsaveCompany]'
GO
CREATE PROCEDURE [dbo].[APIsaveCompany]
			@companyid int out
		   ,@company nvarchar(250)
           ,@archived bit
           ,@comment nvarchar(4000)
           ,@companycode nvarchar(60)
           ,@showfrom bit
           ,@showto bit
           ,@CreatedOn datetime
           ,@CreatedBy int
           ,@ModifiedOn datetime
           ,@ModifiedBy int
           ,@address1 nvarchar(250)
           ,@address2 nvarchar(250)
           ,@city nvarchar(250)
           ,@county nvarchar(250)
           ,@postcode nvarchar(250)
           ,@country int
           ,@parentcompanyid int
           ,@iscompany bit
           ,@CacheExpiry datetime
           ,@addressCreationMethod tinyint
           ,@isPrivateAddress bit
           ,@addressLookupDate datetime
           ,@subAccountID int
           ,@ESRLocationId bigint
           ,@address3 nvarchar(250)
           ,@ESRAddressId bigint
AS
BEGIN
	IF @companyid = 0 AND EXISTS (SELECT companyid FROM COMPANIES WHERE company = @company AND ESRLocationId IS NULL AND ESRAddressId IS NULL)
	BEGIN
		SELECT @companyid = companyid FROM COMPANIES WHERE company = @company AND ESRLocationId IS NULL AND ESRAddressId IS NULL;
	END

	IF @companyid = 0 AND EXISTS (SELECT companyid FROM COMPANIES WHERE address1 = @address1 AND postcode = @postcode AND ESRLocationId IS NULL AND ESRAddressId IS NULL)
	BEGIN
		SELECT @companyid = companyid, @company = company  FROM COMPANIES WHERE address1 = @address1 AND postcode = @postcode AND ESRLocationId IS NULL AND ESRAddressId IS NULL;
	END

	IF @companyid = 0
	BEGIN
		IF EXISTS (SELECT companyid FROM companies WHERE [company] = @company AND (ESRLocationId = @ESRLocationId OR ESRAddressId = @ESRAddressId))
		BEGIN
			RETURN -1;
		END
		
		INSERT INTO [dbo].[companies]
           ([company]
           ,[archived]
           ,[comment]
           ,[companycode]
           ,[showfrom]
           ,[showto]
           ,[CreatedOn]
           ,[CreatedBy]
           ,[ModifiedOn]
           ,[ModifiedBy]
           ,[address1]
           ,[address2]
           ,[city]
           ,[county]
           ,[postcode]
           ,[country]
           ,[parentcompanyid]
           ,[iscompany]
           ,[CacheExpiry]
           ,[addressCreationMethod]
           ,[isPrivateAddress]
           ,[addressLookupDate]
           ,[subAccountID]
           ,[ESRLocationId]
           ,[address3]
           ,[ESRAddressId])
     VALUES
           (@company 
           ,@archived
           ,@comment 
           ,@companycode 
           ,@showfrom 
           ,@showto 
           ,@CreatedOn
           ,@CreatedBy 
           ,@ModifiedOn 
           ,@ModifiedBy 
           ,@address1 
           ,@address2 
           ,@city 
           ,@county 
           ,@postcode 
           ,@country 
           ,@parentcompanyid 
           ,@iscompany 
           ,@CacheExpiry 
           ,@addressCreationMethod 
           ,@isPrivateAddress 
           ,@addressLookupDate 
           ,@subAccountID 
           ,@ESRLocationId 
           ,@address3 
           ,@ESRAddressId )
		   	set @companyid = scope_identity();
		END
	ELSE
		BEGIN
		UPDATE [dbo].[companies]
		   SET [company] = @company 
			  ,[archived] = @archived 
			  ,[comment] = @comment 
			  ,[companycode] = @companycode 
			  ,[showfrom] = @showfrom 
			  ,[showto] = @showto 
			  ,[CreatedOn] = @CreatedOn 
			  ,[CreatedBy] = @CreatedBy 
			  ,[ModifiedOn] = @ModifiedOn 
			  ,[ModifiedBy] = @ModifiedBy 
			  ,[address1] = @address1 
			  ,[address2] = @address2 
			  ,[city] = @city 
			  ,[county] = @county 
			  ,[postcode] = @postcode 
			  ,[country] = @country 
			  ,[parentcompanyid] = @parentcompanyid 
			  ,[iscompany] = @iscompany 
			  ,[CacheExpiry] = @CacheExpiry 
			  ,[addressCreationMethod] = @addressCreationMethod 
			  ,[isPrivateAddress] = @isPrivateAddress 
			  ,[addressLookupDate] = @addressLookupDate 
			  ,[subAccountID] = @subAccountID 
			  ,[ESRLocationId] = @ESRLocationId 
			  ,[address3] = @address3 
			  ,[ESRAddressId] = @ESRAddressId 
		 WHERE [companyid] = @companyid
		 
		END
END
GO
PRINT N'Creating [dbo].[SqlQueryNotificationStoredProcedure-2aa8b047-eb3b-4362-a20c-17885f650ec0]'
GO
CREATE PROCEDURE [dbo].[SqlQueryNotificationStoredProcedure-2aa8b047-eb3b-4362-a20c-17885f650ec0] AS BEGIN BEGIN TRANSACTION; RECEIVE TOP(0) conversation_handle FROM [SqlQueryNotificationService-2aa8b047-eb3b-4362-a20c-17885f650ec0]; IF (SELECT COUNT(*) FROM [SqlQueryNotificationService-2aa8b047-eb3b-4362-a20c-17885f650ec0] WHERE message_type_name = 'http://schemas.microsoft.com/SQL/ServiceBroker/DialogTimer') > 0 BEGIN if ((SELECT COUNT(*) FROM sys.services WHERE name = 'SqlQueryNotificationService-2aa8b047-eb3b-4362-a20c-17885f650ec0') > 0)   DROP SERVICE [SqlQueryNotificationService-2aa8b047-eb3b-4362-a20c-17885f650ec0]; if (OBJECT_ID('SqlQueryNotificationService-2aa8b047-eb3b-4362-a20c-17885f650ec0', 'SQ') IS NOT NULL)   DROP QUEUE [SqlQueryNotificationService-2aa8b047-eb3b-4362-a20c-17885f650ec0]; DROP PROCEDURE [SqlQueryNotificationStoredProcedure-2aa8b047-eb3b-4362-a20c-17885f650ec0]; END COMMIT TRANSACTION; END
GO
PRINT N'Creating [dbo].[SqlQueryNotificationStoredProcedure-5e2be5a5-85a6-41ea-a5d7-7e348f666b66]'
GO
CREATE PROCEDURE [dbo].[SqlQueryNotificationStoredProcedure-5e2be5a5-85a6-41ea-a5d7-7e348f666b66] AS BEGIN BEGIN TRANSACTION; RECEIVE TOP(0) conversation_handle FROM [SqlQueryNotificationService-5e2be5a5-85a6-41ea-a5d7-7e348f666b66]; IF (SELECT COUNT(*) FROM [SqlQueryNotificationService-5e2be5a5-85a6-41ea-a5d7-7e348f666b66] WHERE message_type_name = 'http://schemas.microsoft.com/SQL/ServiceBroker/DialogTimer') > 0 BEGIN if ((SELECT COUNT(*) FROM sys.services WHERE name = 'SqlQueryNotificationService-5e2be5a5-85a6-41ea-a5d7-7e348f666b66') > 0)   DROP SERVICE [SqlQueryNotificationService-5e2be5a5-85a6-41ea-a5d7-7e348f666b66]; if (OBJECT_ID('SqlQueryNotificationService-5e2be5a5-85a6-41ea-a5d7-7e348f666b66', 'SQ') IS NOT NULL)   DROP QUEUE [SqlQueryNotificationService-5e2be5a5-85a6-41ea-a5d7-7e348f666b66]; DROP PROCEDURE [SqlQueryNotificationStoredProcedure-5e2be5a5-85a6-41ea-a5d7-7e348f666b66]; END COMMIT TRANSACTION; END
GO
PRINT N'Creating [dbo].[SqlQueryNotificationStoredProcedure-785957b8-a831-4bcd-9f91-38a240e691f2]'
GO
CREATE PROCEDURE [dbo].[SqlQueryNotificationStoredProcedure-785957b8-a831-4bcd-9f91-38a240e691f2] AS BEGIN BEGIN TRANSACTION; RECEIVE TOP(0) conversation_handle FROM [SqlQueryNotificationService-785957b8-a831-4bcd-9f91-38a240e691f2]; IF (SELECT COUNT(*) FROM [SqlQueryNotificationService-785957b8-a831-4bcd-9f91-38a240e691f2] WHERE message_type_name = 'http://schemas.microsoft.com/SQL/ServiceBroker/DialogTimer') > 0 BEGIN if ((SELECT COUNT(*) FROM sys.services WHERE name = 'SqlQueryNotificationService-785957b8-a831-4bcd-9f91-38a240e691f2') > 0)   DROP SERVICE [SqlQueryNotificationService-785957b8-a831-4bcd-9f91-38a240e691f2]; if (OBJECT_ID('SqlQueryNotificationService-785957b8-a831-4bcd-9f91-38a240e691f2', 'SQ') IS NOT NULL)   DROP QUEUE [SqlQueryNotificationService-785957b8-a831-4bcd-9f91-38a240e691f2]; DROP PROCEDURE [SqlQueryNotificationStoredProcedure-785957b8-a831-4bcd-9f91-38a240e691f2]; END COMMIT TRANSACTION; END
GO
PRINT N'Creating [dbo].[SqlQueryNotificationStoredProcedure-78ec6ec0-f304-449e-a807-424aa0c49afb]'
GO
CREATE PROCEDURE [dbo].[SqlQueryNotificationStoredProcedure-78ec6ec0-f304-449e-a807-424aa0c49afb] AS BEGIN BEGIN TRANSACTION; RECEIVE TOP(0) conversation_handle FROM [SqlQueryNotificationService-78ec6ec0-f304-449e-a807-424aa0c49afb]; IF (SELECT COUNT(*) FROM [SqlQueryNotificationService-78ec6ec0-f304-449e-a807-424aa0c49afb] WHERE message_type_name = 'http://schemas.microsoft.com/SQL/ServiceBroker/DialogTimer') > 0 BEGIN if ((SELECT COUNT(*) FROM sys.services WHERE name = 'SqlQueryNotificationService-78ec6ec0-f304-449e-a807-424aa0c49afb') > 0)   DROP SERVICE [SqlQueryNotificationService-78ec6ec0-f304-449e-a807-424aa0c49afb]; if (OBJECT_ID('SqlQueryNotificationService-78ec6ec0-f304-449e-a807-424aa0c49afb', 'SQ') IS NOT NULL)   DROP QUEUE [SqlQueryNotificationService-78ec6ec0-f304-449e-a807-424aa0c49afb]; DROP PROCEDURE [SqlQueryNotificationStoredProcedure-78ec6ec0-f304-449e-a807-424aa0c49afb]; END COMMIT TRANSACTION; END
GO
PRINT N'Creating [dbo].[SqlQueryNotificationStoredProcedure-894fafc9-4334-498c-8fea-9e192b1e67fc]'
GO
CREATE PROCEDURE [dbo].[SqlQueryNotificationStoredProcedure-894fafc9-4334-498c-8fea-9e192b1e67fc] AS BEGIN BEGIN TRANSACTION; RECEIVE TOP(0) conversation_handle FROM [SqlQueryNotificationService-894fafc9-4334-498c-8fea-9e192b1e67fc]; IF (SELECT COUNT(*) FROM [SqlQueryNotificationService-894fafc9-4334-498c-8fea-9e192b1e67fc] WHERE message_type_name = 'http://schemas.microsoft.com/SQL/ServiceBroker/DialogTimer') > 0 BEGIN if ((SELECT COUNT(*) FROM sys.services WHERE name = 'SqlQueryNotificationService-894fafc9-4334-498c-8fea-9e192b1e67fc') > 0)   DROP SERVICE [SqlQueryNotificationService-894fafc9-4334-498c-8fea-9e192b1e67fc]; if (OBJECT_ID('SqlQueryNotificationService-894fafc9-4334-498c-8fea-9e192b1e67fc', 'SQ') IS NOT NULL)   DROP QUEUE [SqlQueryNotificationService-894fafc9-4334-498c-8fea-9e192b1e67fc]; DROP PROCEDURE [SqlQueryNotificationStoredProcedure-894fafc9-4334-498c-8fea-9e192b1e67fc]; END COMMIT TRANSACTION; END
GO
PRINT N'Creating [dbo].[SqlQueryNotificationStoredProcedure-ee476783-707c-4c38-bd27-62b98d966967]'
GO
CREATE PROCEDURE [dbo].[SqlQueryNotificationStoredProcedure-ee476783-707c-4c38-bd27-62b98d966967] AS BEGIN BEGIN TRANSACTION; RECEIVE TOP(0) conversation_handle FROM [SqlQueryNotificationService-ee476783-707c-4c38-bd27-62b98d966967]; IF (SELECT COUNT(*) FROM [SqlQueryNotificationService-ee476783-707c-4c38-bd27-62b98d966967] WHERE message_type_name = 'http://schemas.microsoft.com/SQL/ServiceBroker/DialogTimer') > 0 BEGIN if ((SELECT COUNT(*) FROM sys.services WHERE name = 'SqlQueryNotificationService-ee476783-707c-4c38-bd27-62b98d966967') > 0)   DROP SERVICE [SqlQueryNotificationService-ee476783-707c-4c38-bd27-62b98d966967]; if (OBJECT_ID('SqlQueryNotificationService-ee476783-707c-4c38-bd27-62b98d966967', 'SQ') IS NOT NULL)   DROP QUEUE [SqlQueryNotificationService-ee476783-707c-4c38-bd27-62b98d966967]; DROP PROCEDURE [SqlQueryNotificationStoredProcedure-ee476783-707c-4c38-bd27-62b98d966967]; END COMMIT TRANSACTION; END
GO
PRINT N'Creating [dbo].[restoreDefaultTooltip]'
GO

CREATE PROCEDURE [dbo].[restoreDefaultTooltip] 
(
@tooltipID uniqueidentifier,
@CUemployeeID INT,
@CUdelegateID INT
)
 
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

 DECLARE @page NVARCHAR(50);
 DECLARE @description NVARCHAR(200);
 DECLARE @auditInfo NVARCHAR(300);

 SELECT @page = page, @description = [description] FROM customised_help_text WHERE tooltipID = @tooltipID;

 SET @auditInfo = 'Page: "' + @page + '", Area: "' + @description + '"';

    -- Insert statements for procedure here
 DELETE FROM customised_help_text WHERE tooltipID = @tooltipID;

 exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 86, 0, @auditInfo, null;
END
GO
PRINT N'Adding constraints to [dbo].[card_providers]'
GO
ALTER TABLE [dbo].[card_providers] ADD CONSTRAINT [CK_card_providers] CHECK (([card_type]=(1) OR [card_type]=(2)))
GO
PRINT N'Adding constraints to [dbo].[SupportQuestions]'
GO
ALTER TABLE [dbo].[SupportQuestions] ADD CONSTRAINT [CK_SupportQuestion] CHECK (([SupportTicketSel]=(1) AND [SupportTicketInternal]=(0) AND [KnowledgeArticleUrl] IS NULL OR [SupportTicketSel]=(0) AND [SupportTicketInternal]=(1) AND [KnowledgeArticleUrl] IS NULL OR [SupportTicketSel]=(0) AND [SupportTicketInternal]=(0) AND [KnowledgeArticleUrl] IS NOT NULL))
GO
PRINT N'Adding constraints to [dbo].[registeredusers]'
GO
ALTER TABLE [dbo].[registeredusers] ADD CONSTRAINT [CK_registeredusers_licenceType] CHECK (([licenceType]=(2) OR [licenceType]=(1)))
GO
ALTER TABLE [dbo].[registeredusers] ADD CONSTRAINT [CK_registeredusers_addressLookupProvider] CHECK (([addressLookupProvider]=(2) OR [addressLookupProvider]=(1) OR [addressLookupProvider]=(0)))
GO
ALTER TABLE [dbo].[registeredusers] ADD CONSTRAINT [CK_registeredusers_1] CHECK (([chargedinArrearsForExcess]>=(0) AND [chargedinArrearsForExcess]<=(2)))
GO
PRINT N'Adding foreign keys to [dbo].[accountsLicencedElements]'
GO
ALTER TABLE [dbo].[accountsLicencedElements] ADD CONSTRAINT [FK_registeredusers_licenced_elements_elements_base] FOREIGN KEY ([elementID]) REFERENCES [dbo].[elementsBase] ([elementID])
GO
PRINT N'Adding foreign keys to [dbo].[elementCategoryBase]'
GO
ALTER TABLE [dbo].[elementCategoryBase] ADD CONSTRAINT [FK_elementCategoryBase_moduleBase] FOREIGN KEY ([moduleID]) REFERENCES [dbo].[moduleBase] ([moduleID])
GO
PRINT N'Adding foreign keys to [dbo].[moduleElementBase]'
GO
ALTER TABLE [dbo].[moduleElementBase] ADD CONSTRAINT [FK_module_element_base_elements_base] FOREIGN KEY ([elementID]) REFERENCES [dbo].[elementsBase] ([elementID])
GO
ALTER TABLE [dbo].[moduleElementBase] ADD CONSTRAINT [FK_module_element_base_module_base] FOREIGN KEY ([moduleID]) REFERENCES [dbo].[moduleBase] ([moduleID])
GO
PRINT N'Adding foreign keys to [dbo].[globalESRElementFields]'
GO
ALTER TABLE [dbo].[globalESRElementFields] ADD CONSTRAINT [FK_globalESRElementFields_globalESRElements] FOREIGN KEY ([globalESRElementID]) REFERENCES [dbo].[globalESRElements] ([globalESRElementID]) ON DELETE CASCADE ON UPDATE CASCADE
GO
PRINT N'Adding foreign keys to [dbo].[global_faqs]'
GO
ALTER TABLE [dbo].[global_faqs] ADD CONSTRAINT [FK_faqs_global_faqcategories] FOREIGN KEY ([faqcategoryid]) REFERENCES [dbo].[global_faqcategories] ([faqcategoryid])
GO
PRINT N'Adding foreign keys to [dbo].[hotel_reviews]'
GO
ALTER TABLE [dbo].[hotel_reviews] ADD CONSTRAINT [FK_hotel_reviews_hotels] FOREIGN KEY ([hotelid]) REFERENCES [dbo].[hotels] ([hotelid]) ON DELETE CASCADE ON UPDATE CASCADE
GO
PRINT N'Adding foreign keys to [dbo].[menu_structure_base]'
GO
ALTER TABLE [dbo].[menu_structure_base] ADD CONSTRAINT [FK_menu_structure_menu_structure] FOREIGN KEY ([parentid]) REFERENCES [dbo].[menu_structure_base] ([menuid])
GO
PRINT N'Adding foreign keys to [dbo].[moduleLicencesBase]'
GO
ALTER TABLE [dbo].[moduleLicencesBase] ADD CONSTRAINT [FK_module_licences_base_module_base] FOREIGN KEY ([moduleID]) REFERENCES [dbo].[moduleBase] ([moduleID]) ON DELETE CASCADE ON UPDATE CASCADE
GO
PRINT N'Adding foreign keys to [dbo].[ApiLicensing]'
GO
ALTER TABLE [dbo].[ApiLicensing] ADD CONSTRAINT [FK__ApiLicensing__registeredusers] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[registeredusers] ([accountid]) ON DELETE CASCADE ON UPDATE CASCADE
GO
PRINT N'Adding foreign keys to [dbo].[EnvelopeHistory]'
GO
ALTER TABLE [dbo].[EnvelopeHistory] ADD CONSTRAINT [FK_EnvelopeHistory_Envelopes] FOREIGN KEY ([EnvelopeId]) REFERENCES [dbo].[Envelopes] ([EnvelopeId])
GO
PRINT N'Adding foreign keys to [dbo].[EnvelopesPhysicalStates]'
GO
ALTER TABLE [dbo].[EnvelopesPhysicalStates] ADD CONSTRAINT [FK_EnvelopePhysicalStates_EnvelopePhysicalState] FOREIGN KEY ([EnvelopePhysicalStateId]) REFERENCES [dbo].[EnvelopePhysicalState] ([EnvelopePhysicalStateId])
GO
ALTER TABLE [dbo].[EnvelopesPhysicalStates] ADD CONSTRAINT [FK_EnvelopePhysicalStates_Envelopes] FOREIGN KEY ([EnvelopeId]) REFERENCES [dbo].[Envelopes] ([EnvelopeId])
GO
PRINT N'Adding foreign keys to [dbo].[Envelopes]'
GO
ALTER TABLE [dbo].[Envelopes] ADD CONSTRAINT [FK_Envelopes_EnvelopeTypes] FOREIGN KEY ([EnvelopeType]) REFERENCES [dbo].[EnvelopeTypes] ([EnvelopeTypeId])
GO
ALTER TABLE [dbo].[Envelopes] ADD CONSTRAINT [FK_Envelopes_RegisteredUsers] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[registeredusers] ([accountid])
GO
PRINT N'Adding foreign keys to [dbo].[ExpenseValidationReasonResultMapping]'
GO
ALTER TABLE [dbo].[ExpenseValidationReasonResultMapping] ADD CONSTRAINT [FK_ExpenseValidationReasonResultMapping_ExpenseValidationCriteria] FOREIGN KEY ([CriterionId]) REFERENCES [dbo].[ExpenseValidationCriteria] ([CriterionId])
GO
ALTER TABLE [dbo].[ExpenseValidationReasonResultMapping] ADD CONSTRAINT [FK_ExpenseValidationReasonResultMapping_ExpenseValidationThresholds] FOREIGN KEY ([ThresholdId]) REFERENCES [dbo].[ExpenseValidationThresholds] ([ThresholdId])
GO
PRINT N'Adding foreign keys to [dbo].[ExpenseValidationCriteria]'
GO
ALTER TABLE [dbo].[ExpenseValidationCriteria] ADD CONSTRAINT [FK_ExpenseValidationCriteria_RegisteredUsers] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[registeredusers] ([accountid])
GO
ALTER TABLE [dbo].[ExpenseValidationCriteria] ADD CONSTRAINT [FK_ExpenseValidationCriteria_FieldsBase] FOREIGN KEY ([FieldId]) REFERENCES [dbo].[fields_base] ([fieldid])
GO
PRINT N'Adding foreign keys to [dbo].[MessageModuleBase]'
GO
ALTER TABLE [dbo].[MessageModuleBase] ADD CONSTRAINT [FK_MessageModuleBase_LogonMessages] FOREIGN KEY ([MessageId]) REFERENCES [dbo].[LogonMessages] ([MessageId])
GO
ALTER TABLE [dbo].[MessageModuleBase] ADD CONSTRAINT [FK_MessageModuleBase_moduleBase] FOREIGN KEY ([ModuleId]) REFERENCES [dbo].[moduleBase] ([moduleID])
GO
PRINT N'Adding foreign keys to [dbo].[SupportQuestions]'
GO
ALTER TABLE [dbo].[SupportQuestions] ADD CONSTRAINT [FK_SupportStatements_SupportStatementHeadings] FOREIGN KEY ([SupportQuestionHeadingId]) REFERENCES [dbo].[SupportQuestionHeadings] ([SupportQuestionHeadingId])
GO
PRINT N'Adding foreign keys to [dbo].[SupportQuestionHeadings]'
GO
ALTER TABLE [dbo].[SupportQuestionHeadings] ADD CONSTRAINT [FK_SupportQuestionHeadings_moduleBase] FOREIGN KEY ([ModuleId]) REFERENCES [dbo].[moduleBase] ([moduleID])
GO
PRINT N'Adding foreign keys to [dbo].[moduleBase]'
GO
ALTER TABLE [dbo].[moduleBase] ADD CONSTRAINT [FK_moduleBase_Theme] FOREIGN KEY ([ThemeId]) REFERENCES [dbo].[Theme] ([ThemeId])
GO
PRINT N'Adding foreign keys to [dbo].[fields_base]'
GO
ALTER TABLE [dbo].[fields_base] ADD CONSTRAINT [FK_fields_base_treeGroup] FOREIGN KEY ([TreeGroup]) REFERENCES [dbo].[TreeGroup] ([Id])
GO
ALTER TABLE [dbo].[fields_base] ADD CONSTRAINT [FK_fields_base_fields_base] FOREIGN KEY ([lookupfield]) REFERENCES [dbo].[fields_base] ([fieldid])
GO
ALTER TABLE [dbo].[fields_base] ADD CONSTRAINT [FK_fields_base_fields_base1] FOREIGN KEY ([associatedFieldForDuplicateChecking]) REFERENCES [dbo].[fields_base] ([fieldid])
GO
ALTER TABLE [dbo].[fields_base] ADD CONSTRAINT [FK_fields_base_tables_base1] FOREIGN KEY ([lookuptable]) REFERENCES [dbo].[tables_base] ([tableid])
GO
ALTER TABLE [dbo].[fields_base] ADD CONSTRAINT [FK_fields_base_tables_base] FOREIGN KEY ([tableid]) REFERENCES [dbo].[tables_base] ([tableid]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[fields_base] ADD CONSTRAINT [FK_fields_base_viewgroups] FOREIGN KEY ([viewgroupid]) REFERENCES [dbo].[viewgroups_base] ([viewgroupid])
GO
ALTER TABLE [dbo].[fields_base] ADD CONSTRAINT [FK_fields_base_relatedTable_tables_base] FOREIGN KEY ([relatedTable]) REFERENCES [dbo].[tables_base] ([tableid])
GO
PRINT N'Adding foreign keys to [dbo].[registeredusers]'
GO
ALTER TABLE [dbo].[registeredusers] ADD CONSTRAINT [FK_registeredusers_accountManagers] FOREIGN KEY ([accountManagerId]) REFERENCES [dbo].[accountManagers] ([accountManagerId]) ON DELETE SET NULL
GO
ALTER TABLE [dbo].[registeredusers] ADD CONSTRAINT [FK_registeredUsers_dbserver] FOREIGN KEY ([dbserver]) REFERENCES [dbo].[databases] ([databaseID])
GO
ALTER TABLE [dbo].[registeredusers] ADD CONSTRAINT [FK_registeredusers_providers] FOREIGN KEY ([providerId]) REFERENCES [dbo].[providers] ([providerId]) ON DELETE SET NULL
GO
PRINT N'Adding foreign keys to [dbo].[accountsLicencedElements]'
GO
ALTER TABLE [dbo].[accountsLicencedElements] ADD CONSTRAINT [FK_registeredusers_licenced_elements_registeredusers] FOREIGN KEY ([accountID]) REFERENCES [dbo].[registeredusers] ([accountid]) ON DELETE CASCADE
GO
PRINT N'Adding foreign keys to [dbo].[elementsBase]'
GO
ALTER TABLE [dbo].[elementsBase] ADD CONSTRAINT [FK_elements_base_elements_base] FOREIGN KEY ([categoryID]) REFERENCES [dbo].[elementCategoryBase] ([categoryID])
GO
PRINT N'Adding foreign keys to [dbo].[tables_base]'
GO
ALTER TABLE [dbo].[tables_base] ADD CONSTRAINT [FK_tables_base_elementsBase] FOREIGN KEY ([elementID]) REFERENCES [dbo].[elementsBase] ([elementID])
GO
ALTER TABLE [dbo].[tables_base] ADD CONSTRAINT [FK_tables_base_fields_base] FOREIGN KEY ([primarykey]) REFERENCES [dbo].[fields_base] ([fieldid])
GO
ALTER TABLE [dbo].[tables_base] ADD CONSTRAINT [FK_tables_base_fields_base1] FOREIGN KEY ([keyfield]) REFERENCES [dbo].[fields_base] ([fieldid])
GO
ALTER TABLE [dbo].[tables_base] ADD CONSTRAINT [FK_tables_base_tables_base] FOREIGN KEY ([userdefined_table]) REFERENCES [dbo].[tables_base] ([tableid])
GO
PRINT N'Adding foreign keys to [dbo].[excessCharges]'
GO
ALTER TABLE [dbo].[excessCharges] ADD CONSTRAINT [FK_excessCharges_registeredusers] FOREIGN KEY ([accountId]) REFERENCES [dbo].[registeredusers] ([accountid]) ON DELETE CASCADE
GO
PRINT N'Adding foreign keys to [dbo].[joinbreakdown_base]'
GO
ALTER TABLE [dbo].[joinbreakdown_base] ADD CONSTRAINT [FK_joinbreakdown_base_fields_base] FOREIGN KEY ([destinationkey]) REFERENCES [dbo].[fields_base] ([fieldid])
GO
ALTER TABLE [dbo].[joinbreakdown_base] ADD CONSTRAINT [FK_joinbreakdown_base_fields_base1] FOREIGN KEY ([joinkey]) REFERENCES [dbo].[fields_base] ([fieldid])
GO
ALTER TABLE [dbo].[joinbreakdown_base] ADD CONSTRAINT [FK_joinbreakdown_base_tables_base] FOREIGN KEY ([tableid]) REFERENCES [dbo].[tables_base] ([tableid])
GO
ALTER TABLE [dbo].[joinbreakdown_base] ADD CONSTRAINT [FK_joinbreakdown_base_tables_base1] FOREIGN KEY ([sourcetable]) REFERENCES [dbo].[tables_base] ([tableid])
GO
ALTER TABLE [dbo].[joinbreakdown_base] ADD CONSTRAINT [FK_joinbreakdown_base_jointables_base] FOREIGN KEY ([jointableid]) REFERENCES [dbo].[jointables_base] ([jointableid]) ON DELETE CASCADE
GO
PRINT N'Adding foreign keys to [dbo].[reportcolumns]'
GO
ALTER TABLE [dbo].[reportcolumns] ADD CONSTRAINT [FK_reportcolumns_fields_base] FOREIGN KEY ([fieldID]) REFERENCES [dbo].[fields_base] ([fieldid]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[reportcolumns] ADD CONSTRAINT [FK_reportcolumns_reports] FOREIGN KEY ([reportID]) REFERENCES [dbo].[reports] ([reportid]) ON DELETE CASCADE
GO
PRINT N'Adding foreign keys to [dbo].[reportcriteria]'
GO
ALTER TABLE [dbo].[reportcriteria] ADD CONSTRAINT [FK_reportcriteria_fields_base] FOREIGN KEY ([fieldID]) REFERENCES [dbo].[fields_base] ([fieldid]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[reportcriteria] ADD CONSTRAINT [FK_reportcriteria_reports] FOREIGN KEY ([reportid]) REFERENCES [dbo].[reports] ([reportid]) ON DELETE CASCADE
GO
PRINT N'Adding foreign keys to [dbo].[reports_common_columns]'
GO
ALTER TABLE [dbo].[reports_common_columns] ADD CONSTRAINT [FK_reports_common_columns_fields_base] FOREIGN KEY ([fieldid]) REFERENCES [dbo].[fields_base] ([fieldid])
GO
ALTER TABLE [dbo].[reports_common_columns] ADD CONSTRAINT [FK_reports_common_columns_tables_base] FOREIGN KEY ([tableid]) REFERENCES [dbo].[tables_base] ([tableid])
GO
PRINT N'Adding foreign keys to [dbo].[rptlistitems]'
GO
ALTER TABLE [dbo].[rptlistitems] ADD CONSTRAINT [FK_rptlistitems_fields_base] FOREIGN KEY ([fieldid]) REFERENCES [dbo].[fields_base] ([fieldid]) ON DELETE CASCADE
GO
PRINT N'Adding foreign keys to [dbo].[registeredUsersHostnames]'
GO
ALTER TABLE [dbo].[registeredUsersHostnames] ADD CONSTRAINT [FK_registeredUsersHostnames_hostnameid] FOREIGN KEY ([hostnameID]) REFERENCES [dbo].[hostnames] ([hostnameID])
GO
ALTER TABLE [dbo].[registeredUsersHostnames] ADD CONSTRAINT [FK_registeredUsersHostnames_accountid] FOREIGN KEY ([accountid]) REFERENCES [dbo].[registeredusers] ([accountid])
GO
PRINT N'Adding foreign keys to [dbo].[hostnames]'
GO
ALTER TABLE [dbo].[hostnames] ADD CONSTRAINT [FK_hostnames_moduleBase] FOREIGN KEY ([moduleID]) REFERENCES [dbo].[moduleBase] ([moduleID])
GO
PRINT N'Adding foreign keys to [dbo].[jointables_base]'
GO
ALTER TABLE [dbo].[jointables_base] ADD CONSTRAINT [FK_jointables_base_tables_base] FOREIGN KEY ([tableid]) REFERENCES [dbo].[tables_base] ([tableid])
GO
ALTER TABLE [dbo].[jointables_base] ADD CONSTRAINT [FK_jointables_base_tables_base1] FOREIGN KEY ([basetableid]) REFERENCES [dbo].[tables_base] ([tableid])
GO
PRINT N'Adding foreign keys to [dbo].[mobileAPIVersion]'
GO
ALTER TABLE [dbo].[mobileAPIVersion] ADD CONSTRAINT [FK_mobileAPIVersion_mobileAPITypes] FOREIGN KEY ([mobileAPITypeID]) REFERENCES [dbo].[mobileAPITypes] ([API_TypeId]) ON DELETE CASCADE
GO
PRINT N'Adding foreign keys to [dbo].[mobileDeviceTypes]'
GO
ALTER TABLE [dbo].[mobileDeviceTypes] ADD CONSTRAINT [FK_mobileDeviceTypes_mobileDeviceOSTypes] FOREIGN KEY ([mobileDeviceOSType]) REFERENCES [dbo].[mobileDeviceOSTypes] ([mobileDeviceOSTypeId])
GO
PRINT N'Adding foreign keys to [dbo].[moduleLicencesBase]'
GO
ALTER TABLE [dbo].[moduleLicencesBase] ADD CONSTRAINT [FK_module_licences_base_registeredusers] FOREIGN KEY ([accountID]) REFERENCES [dbo].[registeredusers] ([accountid]) ON DELETE CASCADE ON UPDATE CASCADE
GO
PRINT N'Adding foreign keys to [dbo].[reports]'
GO
ALTER TABLE [dbo].[reports] ADD CONSTRAINT [FK_reports_report_folders] FOREIGN KEY ([folderid]) REFERENCES [dbo].[report_folders] ([folderid])
GO
ALTER TABLE [dbo].[reports] ADD CONSTRAINT [FK_reports_tables_base] FOREIGN KEY ([basetable]) REFERENCES [dbo].[tables_base] ([tableid])
GO
ALTER TABLE [dbo].[reports] ADD CONSTRAINT [FK_reports_reports] FOREIGN KEY ([footerreportid]) REFERENCES [dbo].[reports] ([reportid])
GO
PRINT N'Adding foreign keys to [dbo].[reports_allowedtables_base]'
GO
ALTER TABLE [dbo].[reports_allowedtables_base] ADD CONSTRAINT [FK_reports_allowedtables_base_tables_base] FOREIGN KEY ([basetableid]) REFERENCES [dbo].[tables_base] ([tableid])
GO
ALTER TABLE [dbo].[reports_allowedtables_base] ADD CONSTRAINT [FK_reports_allowedtables_base_tables_base1] FOREIGN KEY ([tableid]) REFERENCES [dbo].[tables_base] ([tableid])
GO
PRINT N'Adding foreign keys to [dbo].[viewgroups_base]'
GO
ALTER TABLE [dbo].[viewgroups_base] ADD CONSTRAINT [FK_viewgroups_base_viewgroups_base] FOREIGN KEY ([parentid]) REFERENCES [dbo].[viewgroups_base] ([viewgroupid])
GO
PRINT N'Creating extended properties'
GO
EXEC sp_addextendedproperty N'MS_Description', '0 = Teleatlas, 1 = PAF, 2 = AddressBase', 'SCHEMA', N'dbo', 'TABLE', N'registeredusers', 'COLUMN', N'addressLookupProvider'
GO
EXEC sp_addextendedproperty N'MS_Description', N'1=claims, 2=claimants', 'SCHEMA', N'dbo', 'TABLE', N'registeredusers', 'COLUMN', N'licenceType'
GO
