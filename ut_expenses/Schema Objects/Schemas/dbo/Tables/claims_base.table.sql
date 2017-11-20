CREATE TABLE [dbo].[claims_base] (
    [claimid]            INT             IDENTITY (1, 1) NOT NULL,
    [claimno]            INT             NOT NULL,
    [employeeid]         INT             CONSTRAINT [DF_claims_employeeid] DEFAULT ((0)) NOT NULL,
    [approved]           BIT             CONSTRAINT [DF_claims_approved] DEFAULT ((0)) NOT NULL,
    [paid]               BIT             CONSTRAINT [DF_claims_paid] DEFAULT ((0)) NOT NULL,
    [datesubmitted]      DATETIME        NULL,
    [datepaid]           DATETIME        NULL,
    [description]        NVARCHAR (2000) NULL,
    [status]             TINYINT         CONSTRAINT [DF_claims_status] DEFAULT ((0)) NOT NULL,
    [teamid]             INT             NULL,
    [checkerid]          INT             CONSTRAINT [DF__claims__checkeri__0C50D423] DEFAULT ((0)) NULL,
    [stage]              INT             CONSTRAINT [DF__claims__stage__0D44F85C] DEFAULT ((0)) NOT NULL,
    [submitted]          BIT             CONSTRAINT [DF_claims_submitted] DEFAULT ((0)) NULL,
    [name]               NVARCHAR (50)   NOT NULL,
    [currencyid]         INT             NULL,
    [CreatedOn]          DATETIME        NULL,
    [CreatedBy]          INT             NULL,
    [ModifiedOn]         DATETIME        NULL,
    [ModifiedBy]         INT             NULL,
    [CacheExpiry]        DATETIME        NULL,
    [ReferenceNumber]    NVARCHAR (11)   NULL,
    [splitApprovalStage] BIT             CONSTRAINT [DF_claims_base_splitApprovalStage] DEFAULT ((0)) NOT NULL,
    [PayBeforeValidate]  BIT             CONSTRAINT [DF_claims_base_PayBeforeValidate] DEFAULT (NULL) NULL,
    CONSTRAINT [PK_claims] PRIMARY KEY CLUSTERED ([claimid] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_claims_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE,
    CONSTRAINT [FK_claims_teams] FOREIGN KEY ([teamid]) REFERENCES [dbo].[teams] ([teamid])
);



