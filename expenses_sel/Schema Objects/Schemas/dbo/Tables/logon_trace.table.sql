CREATE TABLE [dbo].[logon_trace] (
    [traceId]     INT      IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [employeeid]  INT      NULL,
    [logonPeriod] DATETIME NOT NULL,
    [count]       INT      NULL,
    [processed]   BIT      NOT NULL
);

