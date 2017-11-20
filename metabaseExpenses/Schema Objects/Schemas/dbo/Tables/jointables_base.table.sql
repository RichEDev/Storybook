CREATE TABLE [dbo].[jointables_base] (
    [jointableid_old] INT              NULL,
    [description] NVARCHAR (4000)  COLLATE Latin1_General_CI_AS NULL,
    [amendedon]   DATETIME         NULL,
    [tableid]     UNIQUEIDENTIFIER NOT NULL,
    [basetableid] UNIQUEIDENTIFIER NOT NULL, 
    [jointableid] UNIQUEIDENTIFIER NOT NULL
);

