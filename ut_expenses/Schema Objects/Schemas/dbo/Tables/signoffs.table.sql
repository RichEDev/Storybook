CREATE TABLE [dbo].[signoffs] (
    [signoffid]           INT      IDENTITY (1, 1) NOT NULL,
    [groupid]             INT      CONSTRAINT [DF_signoffs_groupid] DEFAULT (0) NOT NULL,
    [signofftype]         TINYINT  CONSTRAINT [DF_signoffs_signofftype] DEFAULT (0) NOT NULL,
    [relid]               INT      CONSTRAINT [DF_signoffs_relid] DEFAULT (0) NOT NULL,
    [include]             INT      CONSTRAINT [DF_signoffs_include] DEFAULT (0) NOT NULL,
    [amount]              MONEY    CONSTRAINT [DF_signoffs_amount] DEFAULT (0) NOT NULL,
    [notify]              INT      CONSTRAINT [DF_signoffs_notify] DEFAULT (0) NOT NULL,
    [stage]               TINYINT  CONSTRAINT [DF_signoffs_stage] DEFAULT (0) NOT NULL,
    [holidaytype]         INT      CONSTRAINT [DF_signoffs_holidaytype] DEFAULT (0) NOT NULL,
    [holidayid]           INT      CONSTRAINT [DF_signoffs_holidayid] DEFAULT (0) NOT NULL,
    [onholiday]           TINYINT  CONSTRAINT [DF_signoffs_onholiday] DEFAULT (0) NOT NULL,
    [includeid]           INT      NULL,
    [claimantmail]        BIT      CONSTRAINT [DF_signoffs_claimantmail] DEFAULT (0) NOT NULL,
    [singlesignoff]       BIT      CONSTRAINT [DF_signoffs_singlesignoff] DEFAULT (0) NOT NULL,
    [sendmail]            BIT      CONSTRAINT [DF_signoffs_sendmail] DEFAULT (0) NOT NULL,
    [displaydeclaration]  BIT      CONSTRAINT [DF_signoffs_displaydeclaration] DEFAULT ((0)) NOT NULL,
    [CreatedOn]           DATETIME NULL,
    [CreatedBy]           INT      NULL,
    [ModifiedOn]          DATETIME NULL,
    [ModifiedBy]          INT      NULL,
    [extraApprovalLevels] INT      CONSTRAINT [DF_signoffs_extraApprovalLevels] DEFAULT ((0)) NOT NULL,
	[approveHigherLevelsOnly] BIT  NOT NULL,
    [NhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner] BIT NOT NULL DEFAULT 0, 
	[AllocateForPayment]			BIT CONSTRAINT [DF_signoffs_AllocateForPayment] DEFAULT (0) NOT NULL,
	[IsPostValidationCleanupStage]	BIT CONSTRAINT [DF_signoffs_IsPostValidationCleanupStage] DEFAULT (0) NOT NULL,
	[ValidationCorrectionThreshold]	INT NULL,
	[approverJustificationsRequired] [bit] NOT NULL,
	[ClaimPercentageToValidate] DECIMAL(18, 2) NULL 
	CONSTRAINT [DF_signoffs_approverJustificationsRequired] DEFAULT ((0))	
	CONSTRAINT [PK__signoffs__282DF8C2] PRIMARY KEY CLUSTERED ([signoffid] ASC) WITH (FILLFACTOR = 90),    
    CONSTRAINT [FK_signoffs_groups] FOREIGN KEY ([groupid]) REFERENCES [dbo].[groups] ([groupid]) ON DELETE CASCADE ON UPDATE CASCADE
);



