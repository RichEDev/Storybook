CREATE TABLE [dbo].[document_mappings_text] (
    [static_textid]  INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [static_text]    NVARCHAR (MAX) NOT NULL,
    [mergeprojectid] INT            NOT NULL
);

