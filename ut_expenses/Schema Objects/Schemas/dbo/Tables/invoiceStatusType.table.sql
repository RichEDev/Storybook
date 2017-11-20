CREATE TABLE [dbo].[invoiceStatusType] (
    [invoiceStatusTypeID] INT           IDENTITY (1, 1) NOT NULL,
    [description]         NVARCHAR (50) NULL,
    [createdon]           DATETIME      NULL,
    [createdby]           INT           NULL,
    [modifiedon]          DATETIME      NULL,
    [modifiedby]          INT           NULL,
    [subAccountId]        INT           NULL,
    [oldArchive]          INT           NULL,
    [isArchive]           BIT           NOT NULL,
    [archived]            BIT           NOT NULL
);

