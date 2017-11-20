CREATE TABLE [dbo].[notes] (
    [employeeid] INT      NOT NULL,
    [datestamp]  DATETIME NOT NULL,
    [note]       NTEXT    NOT NULL,
    [noteid]     INT      IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [read]       BIT      NOT NULL
);

