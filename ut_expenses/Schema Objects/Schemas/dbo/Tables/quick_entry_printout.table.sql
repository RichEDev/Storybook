CREATE TABLE [dbo].[quick_entry_printout] (
    [quickentryid] INT              NOT NULL,
    [freefield]    NVARCHAR (1000)  NULL,
    [position]     TINYINT          NOT NULL,
    [order]        INT              NOT NULL,
    [fieldID]      UNIQUEIDENTIFIER NULL
);

