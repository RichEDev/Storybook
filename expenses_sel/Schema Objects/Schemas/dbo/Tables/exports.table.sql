CREATE TABLE [dbo].[exports] (
    [exportid]   INT           IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [exporttype] TINYINT       NOT NULL,
    [datestamp]  SMALLDATETIME NULL
);

