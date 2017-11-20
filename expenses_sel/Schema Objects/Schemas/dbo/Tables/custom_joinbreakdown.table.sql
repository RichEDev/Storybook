CREATE TABLE [dbo].[custom_joinbreakdown] (
    [joinbreakdownid] INT              NOT NULL,
    [jointableid]     INT              NOT NULL,
    [order]           TINYINT          NOT NULL,
    [tableid]         UNIQUEIDENTIFIER NOT NULL,
    [sourcetable]     UNIQUEIDENTIFIER NOT NULL,
    [joinkey]         UNIQUEIDENTIFIER NOT NULL,
    [destinationkey]  UNIQUEIDENTIFIER NOT NULL,
    [amendedon]       DATETIME         NULL
);

