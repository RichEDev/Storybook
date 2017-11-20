CREATE TABLE [dbo].[report_folders] (
    [oldfolderid] INT              IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [foldername]  NVARCHAR (100)   COLLATE Latin1_General_CI_AS NOT NULL,
    [CreatedOn]   DATETIME         NULL,
    [CreatedBy]   INT              NULL,
    [ModifiedOn]  DATETIME         NULL,
    [ModifiedBy]  INT              NULL,
    [folderid]    UNIQUEIDENTIFIER NOT NULL
);

