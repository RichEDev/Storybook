CREATE TABLE [dbo].[quick_entry_columns] (
    [quickentryid] INT              NOT NULL,
    [columntype]   TINYINT          NOT NULL,
    [order]        TINYINT          NOT NULL,
    [subcatID]     INT              NULL,
    [columnid]     UNIQUEIDENTIFIER NULL
);

