CREATE TABLE [dbo].[customJoinBreakdown] (
    [joinbreakdownid_old] INT              NULL,
    [jointableid_old]     INT              NULL,
    [order]           TINYINT          NOT NULL,
    [tableid]         UNIQUEIDENTIFIER NOT NULL,
    [sourcetable]     UNIQUEIDENTIFIER NOT NULL,
    [joinkey]         UNIQUEIDENTIFIER NOT NULL,
    [destinationkey]  UNIQUEIDENTIFIER NOT NULL,
    [amendedon]       DATETIME         NULL, 
    [joinbreakdownid] UNIQUEIDENTIFIER NOT NULL, 
    [jointableid] UNIQUEIDENTIFIER NOT NULL
);

