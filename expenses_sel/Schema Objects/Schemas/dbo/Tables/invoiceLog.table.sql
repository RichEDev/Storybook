CREATE TABLE [dbo].[invoiceLog] (
    [invoiceLogID]  INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [invoiceID]     INT            NULL,
    [dateChanged]   DATETIME       NULL,
    [userID]        INT            NULL,
    [invoiceStatus] INT            NULL,
    [comment]       NVARCHAR (MAX) NULL
);

