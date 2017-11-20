CREATE TABLE [dbo].[hostnames] (
    [hostnameID] INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [hostname]   NVARCHAR (300) COLLATE Latin1_General_CI_AS NOT NULL
);

