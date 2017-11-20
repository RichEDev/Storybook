CREATE TABLE [dbo].[float_allocations] (
    [allocationid] INT   IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [floatid]      INT   NOT NULL,
    [expenseid]    INT   NOT NULL,
    [amount]       MONEY NOT NULL
);

