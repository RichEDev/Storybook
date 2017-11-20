CREATE TABLE [dbo].[purchaseOrderHistory] (
    [purchaseOrderID]        INT            NOT NULL,
    [comment]                NVARCHAR (MAX) NOT NULL,
    [createdByString]        NVARCHAR (150) NOT NULL,
    [createdOn]              DATETIME       NOT NULL,
    [createdBy]              INT            NOT NULL,
    [purchaseOrderHistoryID] INT            IDENTITY (1, 1) NOT NULL
);

