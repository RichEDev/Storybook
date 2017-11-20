CREATE TABLE [dbo].[allowances] (
    [allowanceid] INT             IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [allowance]   NVARCHAR (50)   NOT NULL,
    [description] NVARCHAR (4000) NULL,
    [nighthours]  INT             NOT NULL,
    [nightrate]   MONEY           NOT NULL,
    [currencyid]  INT             NULL,
    [CreatedOn]   DATETIME        NULL,
    [CreatedBy]   INT             NULL,
    [ModifiedOn]  DATETIME        NULL,
    [ModifiedBy]  INT             NULL
);

