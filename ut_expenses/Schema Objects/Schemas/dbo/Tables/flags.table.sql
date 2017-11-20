-- Columns

CREATE TABLE [dbo].[flags]
(
[flagID] [int] NOT NULL IDENTITY(1, 1),
[flagType] [tinyint] NOT NULL,
[action] [tinyint] NOT NULL,
[flagText] [nvarchar] (max) COLLATE Latin1_General_CI_AS NULL,
[amberTolerance] [decimal] (18, 2) NULL,
[frequency] [tinyint] NULL,
[frequencyType] [tinyint] NULL,
[period] [tinyint] NULL,
[periodType] [tinyint] NULL,
[financialYear] [int] NULL,
[limit] [decimal] (18, 2) NULL,
[dateComparisonType] [tinyint] NULL,
[dateToCompare] [datetime] NULL,
[numberOfMonths] [tinyint] NULL,
[createdOn] [datetime] NOT NULL CONSTRAINT [DF_flags_createdOn] DEFAULT (getdate()),
[createdBy] [int] NULL,
[modifiedOn] [datetime] NULL,
[modifiedBy] [int] NULL,
[description] [nvarchar] (max) COLLATE Latin1_General_CI_AS NULL,
[active] [bit] NOT NULL CONSTRAINT [DF_flags_active] DEFAULT ((1)),
[tipLimit] [decimal] (18, 2) NULL,
[claimantJustificationRequired] [bit] NOT NULL CONSTRAINT [DF_flags_claimantJustificationRequired] DEFAULT ((0)),
[displayFlagImmediately] [bit] NOT NULL CONSTRAINT [DF_flags_displayFlagImmediately] DEFAULT ((0)),
[noFlagTolerance] [decimal] (18, 2) NULL,
[approverJustificationRequired] [bit] NOT NULL CONSTRAINT [DF_flags_lineManagerJustificationRequired] DEFAULT ((0)),
[flagLevel] [tinyint] NOT NULL,
[increaseByNumOthers] [bit] NOT NULL CONSTRAINT [DF_flags_increaseByNumOthers] DEFAULT ((0)),
[displayLimit] [bit] NOT NULL CONSTRAINT [DF_flags_displayLimit] DEFAULT ((0)),
[notesForAuthoriser] [nvarchar] (max) COLLATE Latin1_General_CI_AS NULL,
[expenseItemInclusionType] [tinyint] NOT NULL,
[itemRoleInclusionType] [tinyint] NOT NULL,
[passengerLimit] [int] NULL
)


GO
ALTER TABLE [dbo].[flags] ADD CONSTRAINT [FK_flags_FinancialYears] FOREIGN KEY ([financialYear]) REFERENCES [dbo].[FinancialYears] ([FinancialYearID])