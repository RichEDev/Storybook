CREATE TABLE [dbo].[moduleBase] (
    [moduleID]      INT             IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [moduleName]    NVARCHAR (100)  COLLATE Latin1_General_CI_AS NOT NULL,
    [description]   NVARCHAR (4000) COLLATE Latin1_General_CI_AS NULL,
    [brandName]     NVARCHAR (250)  NOT NULL,
    [brandNameHTML] NVARCHAR (MAX)  NOT NULL
);

