CREATE TABLE [dbo].[codes_notecategory] (
    [noteCatId]       INT           IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [noteType]        INT           NULL,
    [Description]     NVARCHAR (30) NULL,
    [fullDescription] NVARCHAR (50) NULL,
    CONSTRAINT [PK_Codes_NoteCategory] PRIMARY KEY CLUSTERED ([noteCatId] ASC)
);



