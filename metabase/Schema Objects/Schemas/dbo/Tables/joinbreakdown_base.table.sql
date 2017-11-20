CREATE TABLE [dbo].[joinbreakdown_base] (
    [joinbreakdownid] INT              IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [jointableid]     INT              NOT NULL,
    [order]           TINYINT          NOT NULL,
    [joinkey]         UNIQUEIDENTIFIER NOT NULL,
    [amendedon]       DATETIME         NULL,
    [destinationkey]  UNIQUEIDENTIFIER NULL,
    [tableid]         UNIQUEIDENTIFIER NOT NULL,
    [sourcetable]     UNIQUEIDENTIFIER NULL
);

