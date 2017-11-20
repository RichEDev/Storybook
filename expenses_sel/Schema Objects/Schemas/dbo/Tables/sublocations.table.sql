CREATE TABLE [dbo].[sublocations] (
    [Sublocation Id]   INT           IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [Location Id]      INT           NULL,
    [Sublocation Desc] NVARCHAR (50) NULL
);

