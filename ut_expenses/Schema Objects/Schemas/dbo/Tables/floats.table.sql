CREATE TABLE [dbo].[floats] (
    [floatid]       INT             IDENTITY (1, 1) NOT NULL,
    [employeeid]    INT             CONSTRAINT [DF_floats_employeeid] DEFAULT (0) NOT NULL,
    [currencyid]    INT             CONSTRAINT [DF_floats_currencyid] DEFAULT (0) NULL,
    [float]         MONEY           CONSTRAINT [DF_floats_float] DEFAULT (0) NOT NULL,
    [name]          NVARCHAR (50)   NOT NULL,
    [reason]        NVARCHAR (4000) NULL,
    [requiredby]    DATETIME        NULL,
    [approved]      BIT             CONSTRAINT [DF_floats_approved] DEFAULT (0) NOT NULL,
    [approver]      INT             CONSTRAINT [DF_floats_approver] DEFAULT (0) NOT NULL,
    [exchangerate]  FLOAT (53)      CONSTRAINT [DF_floats_exchangerate] DEFAULT (0) NOT NULL,
    [stage]         TINYINT         CONSTRAINT [DF_floats_stage] DEFAULT (0) NOT NULL,
    [rejected]      BIT             CONSTRAINT [DF_floats_rejected] DEFAULT (0) NOT NULL,
    [rejectreason]  NVARCHAR (4000) NULL,
    [disputed]      BIT             CONSTRAINT [DF_floats_disputed] DEFAULT (0) NOT NULL,
    [dispute]       NVARCHAR (4000) NULL,
    [paid]          BIT             CONSTRAINT [DF_floats_paid] DEFAULT (0) NOT NULL,
    [datepaid]      DATETIME        NULL,
    [issuenum]      INT             NULL,
    [basecurrency]  INT             NULL,
    [settled]       BIT             CONSTRAINT [DF_floats_settled] DEFAULT (0) NOT NULL,
    [CreatedOn]     DATETIME        NULL,
    [CreatedBy]     INT             NULL,
    [ModifiedOn]    DATETIME        NULL,
    [ModifiedBy]    INT             NULL,
    [foreignAmount] MONEY           NOT NULL,
    CONSTRAINT [PK_floats] PRIMARY KEY NONCLUSTERED ([floatid] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_floats_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid])
);



