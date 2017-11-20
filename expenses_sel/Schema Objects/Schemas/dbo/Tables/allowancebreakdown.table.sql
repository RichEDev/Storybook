CREATE TABLE [dbo].[allowancebreakdown] (
    [breakdownid] INT   IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [allowanceid] INT   NOT NULL,
    [hours]       INT   NOT NULL,
    [rate]        MONEY NOT NULL
);

