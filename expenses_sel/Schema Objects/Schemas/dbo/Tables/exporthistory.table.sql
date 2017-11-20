CREATE TABLE [dbo].[exporthistory] (
    [exporthistoryid]   INT      IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [exportnum]         INT      NOT NULL,
    [employeeid]        INT      NOT NULL,
    [dateexported]      DATETIME NOT NULL,
    [financialexportid] INT      NULL,
    [exportStatus]      TINYINT  NOT NULL
);

