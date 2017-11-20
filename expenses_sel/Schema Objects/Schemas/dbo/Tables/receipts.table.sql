CREATE TABLE [dbo].[receipts] (
    [receiptid]  INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [expenseid]  INT            NOT NULL,
    [filename]   NVARCHAR (500) NOT NULL,
    [CreatedOn]  DATETIME       NULL,
    [CreatedBy]  INT            NULL,
    [ModifiedOn] DATETIME       NULL,
    [ModifiedBy] INT            NULL
);

