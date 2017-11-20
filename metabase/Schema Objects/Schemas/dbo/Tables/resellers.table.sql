CREATE TABLE [dbo].[resellers] (
    [resellerid] INT           IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [name]       NVARCHAR (50) COLLATE Latin1_General_CI_AS NOT NULL
);

