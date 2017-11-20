CREATE TABLE [dbo].[exports] (
    [exportid]   INT           IDENTITY (1, 1)  NOT NULL,
    [exporttype] TINYINT       NOT NULL,
    [datestamp]  SMALLDATETIME NULL
);

