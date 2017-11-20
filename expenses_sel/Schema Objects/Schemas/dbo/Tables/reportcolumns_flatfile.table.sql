CREATE TABLE [dbo].[reportcolumns_flatfile] (
    [employeeid]        INT              NOT NULL,
    [columnlength]      INT              NOT NULL,
    [reportcolumnid]    UNIQUEIDENTIFIER NOT NULL,
    [oldreportcolumnid] INT              NULL
);

