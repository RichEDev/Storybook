CREATE TABLE [dbo].[invoiceHistory] (
    [invoiceID]        INT            NOT NULL,
    [comment]          NVARCHAR (MAX) NOT NULL,
    [createdByString]  NVARCHAR (150) NOT NULL,
    [createdOn]        DATETIME       NOT NULL,
    [createdBy]        INT            NOT NULL,
    [invoiceHistoryID] INT            IDENTITY (1, 1) NOT NULL
);

