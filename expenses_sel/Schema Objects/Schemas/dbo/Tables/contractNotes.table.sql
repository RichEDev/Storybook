CREATE TABLE [dbo].[contractNotes] (
    [noteId]     INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [contractID] INT            NULL,
    [Code]       INT            NULL,
    [Note]       NVARCHAR (MAX) NULL,
    [Date]       DATETIME       NULL,
    [noteType]   INT            NULL,
    [noteCatId]  INT            NULL,
    [createdBy]  INT            NULL,
    [modifiedBy] INT            NULL,
    [modifiedOn] DATETIME       NULL
);

