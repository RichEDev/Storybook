CREATE TABLE [dbo].[custom_jointables] (
    [jointableid] INT              NOT NULL,
    [tableid]     UNIQUEIDENTIFIER NOT NULL,
    [basetableid] UNIQUEIDENTIFIER NOT NULL,
    [description] NVARCHAR (4000)  NULL,
    [amendedon]   DATETIME         NULL
);

