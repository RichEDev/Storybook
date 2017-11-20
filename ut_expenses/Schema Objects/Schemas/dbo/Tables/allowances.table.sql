CREATE TABLE [dbo].[allowances] (
    [allowanceid] INT             IDENTITY (1, 1) NOT NULL,
    [allowance]   NVARCHAR (50)   NOT NULL,
    [description] NVARCHAR (4000) NULL,
    [nighthours]  INT             NOT NULL,
    [nightrate]   MONEY           NOT NULL,
    [currencyid]  INT             NULL,
    [CreatedOn]   DATETIME        NULL,
    [CreatedBy]   INT             NULL,
    [ModifiedOn]  DATETIME        NULL,
    [ModifiedBy]  INT             NULL,
    CONSTRAINT [PK_allowances] PRIMARY KEY CLUSTERED ([allowanceid] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [IX_allowances] UNIQUE NONCLUSTERED ([allowance] ASC)
);



