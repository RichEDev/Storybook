CREATE TABLE [dbo].[supplierContactNotes] (
    [noteId]        INT            IDENTITY (1, 1) NOT NULL,
    [oldaddressid]  INT            NULL,
    [Code]          INT            NULL,
    [Note]          NVARCHAR (MAX) NULL,
    [Date]          DATETIME       NULL,
    [noteType]      INT            NULL,
    [noteCatId]     INT            NULL,
    [old_createdby] NVARCHAR (60)  NULL,
    [createdBy]     INT            NULL,
    [contactid]     INT            NOT NULL
);

