CREATE TABLE [dbo].[joinbreakdown_base] (
    [joinbreakdownid_old]	INT              NULL,
    [jointableid_old]		INT              NULL,
    [order]					TINYINT          NOT NULL,
    [joinkey]				UNIQUEIDENTIFIER NOT NULL,
    [amendedon]				DATETIME         NULL,
    [destinationkey]		UNIQUEIDENTIFIER NULL,
    [tableid]				UNIQUEIDENTIFIER NOT NULL,
    [sourcetable]			UNIQUEIDENTIFIER NULL, 
    [joinbreakdownid]		UNIQUEIDENTIFIER NOT NULL, 
    [jointableid]			UNIQUEIDENTIFIER NOT NULL
);

