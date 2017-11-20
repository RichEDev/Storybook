CREATE TABLE [dbo].[invoiceNotes] (
    [noteID]     INT            IDENTITY (1, 1)  NOT NULL,
    [invoiceID]  INT            NULL,
    [code]       INT            NULL,
    [note]       NVARCHAR (MAX) NULL,
    [date]       DATETIME       NULL,
    [noteType]   INT            NULL,
    [noteCatID]  INT            NULL,
    [createdBy]  INT            NULL,
    [modifiedBy] INT            NULL,
    [modifiedOn] DATETIME       NULL
);

