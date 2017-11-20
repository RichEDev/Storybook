CREATE TABLE [dbo].[exporteditems] (
    [exportid]        INT IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [expenseid]       INT NOT NULL,
    [exporthistoryid] INT NOT NULL
);

