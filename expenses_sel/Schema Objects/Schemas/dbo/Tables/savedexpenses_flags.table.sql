CREATE TABLE [dbo].[savedexpenses_flags] (
    [flagid]    INT             IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [expenseid] INT             NOT NULL,
    [flagtype]  INT             NOT NULL,
    [comment]   NVARCHAR (2000) NOT NULL
);

