CREATE TABLE [dbo].[customJoinTables] (
    [jointableid_old] INT              NULL,
    [tableid]     UNIQUEIDENTIFIER NOT NULL,
    [basetableid] UNIQUEIDENTIFIER NOT NULL,
    [description] NVARCHAR (4000)  NULL,
    [amendedon]   DATETIME         NULL, 
    [jointableid] UNIQUEIDENTIFIER NOT NULL
);

