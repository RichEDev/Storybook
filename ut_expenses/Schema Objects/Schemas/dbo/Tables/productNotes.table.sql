CREATE TABLE [dbo].[productNotes] (
    [noteId]        INT            IDENTITY (1, 1) NOT NULL,
    [productID]     INT            NULL,
    [Code]          INT            NULL,
    [Note]          NVARCHAR (MAX) NULL,
    [Date]          DATETIME       NULL,
    [noteType]      INT            NULL,
    [noteCatId]     INT            NULL,
    [old_createdby] VARCHAR (60)   NULL,
    [createdBy]     INT            NULL,
    [modifiedBy]    INT            NULL,
    [modifiedOn]    DATETIME       NULL
);

