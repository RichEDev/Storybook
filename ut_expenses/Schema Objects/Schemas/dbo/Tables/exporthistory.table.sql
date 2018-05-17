CREATE TABLE [dbo].[exporthistory] (
    [exporthistoryid]   INT      IDENTITY (1, 1)  NOT NULL,
    [exportnum]         INT      NOT NULL,
    [employeeid]        INT      NULL,
    [dateexported]      DATETIME NOT NULL,
    [financialexportid] INT      NULL,
    [exportStatus]      TINYINT  NOT NULL
);

