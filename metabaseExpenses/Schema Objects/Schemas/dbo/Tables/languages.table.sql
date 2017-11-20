CREATE TABLE [dbo].[languages] (
    [phraseid] INT             IDENTITY (605, 1) NOT FOR REPLICATION NOT NULL,
    [phrase]   NVARCHAR (4000) COLLATE Latin1_General_CI_AS NOT NULL,
    [Dutch]    NVARCHAR (4000) COLLATE Latin1_General_CI_AS NULL,
    CONSTRAINT [PK_languages] PRIMARY KEY CLUSTERED ([phraseid] ASC)
);



