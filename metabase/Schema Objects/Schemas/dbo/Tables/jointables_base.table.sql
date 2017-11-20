CREATE TABLE [dbo].[jointables_base] (
    [jointableid] INT              IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [description] NVARCHAR (4000)  COLLATE Latin1_General_CI_AS NULL,
    [amendedon]   DATETIME         NULL,
    [tableid]     UNIQUEIDENTIFIER NOT NULL,
    [basetableid] UNIQUEIDENTIFIER NOT NULL
);

