CREATE TABLE [dbo].[hostnames] (
    [hostnameID] INT            IDENTITY (16, 1) NOT FOR REPLICATION NOT NULL,
    [hostname]   NVARCHAR (300) COLLATE Latin1_General_CI_AS NOT NULL,
    [moduleID]   INT            NOT NULL,
    CONSTRAINT [PK_hostnames] PRIMARY KEY CLUSTERED ([hostnameID] ASC)
);



