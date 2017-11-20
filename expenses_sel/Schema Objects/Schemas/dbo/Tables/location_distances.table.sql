CREATE TABLE [dbo].[location_distances] (
    [distanceid]                       INT             IDENTITY (1, 1) NOT NULL,
    [locationa]                        INT             NOT NULL,
    [locationb]                        INT             NOT NULL,
    [distance]                         DECIMAL (18, 2) NOT NULL,
    [postcodeanywheredistance]         DECIMAL (18, 2) NULL,
    [createdon]                        DATETIME        NULL,
    [createdby]                        INT             NULL,
    [postcodeAnywhereFastestDistance]  DECIMAL (18, 2) NULL,
    [postcodeAnywhereShortestDistance] DECIMAL (18, 2) NULL
);

