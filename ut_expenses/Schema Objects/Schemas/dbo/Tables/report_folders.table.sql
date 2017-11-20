CREATE TABLE [dbo].[report_folders] (
    [foldername]  NVARCHAR (100)   NOT NULL,
    [employeeid]  INT              NOT NULL,
    [personal]    BIT              NOT NULL,
    [CreatedOn]   DATETIME         NULL,
    [CreatedBy]   INT              NULL,
    [ModifiedOn]  DATETIME         NULL,
    [ModifiedBy]  INT              NULL,
    [folderID]    UNIQUEIDENTIFIER NOT NULL,
    [oldFolderID] INT              IDENTITY (1, 1) NOT NULL
);

