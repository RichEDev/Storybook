CREATE TABLE [dbo].[supplierNotes] (
    [noteId]     INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [supplierid] INT            NULL,
    [Code]       INT            NULL,
    [Note]       NVARCHAR (MAX) NULL,
    [Date]       DATETIME       NULL,
    [noteType]   INT            NULL,
    [noteCatId]  INT            NULL,
    [createdBy]  INT            NULL
);

