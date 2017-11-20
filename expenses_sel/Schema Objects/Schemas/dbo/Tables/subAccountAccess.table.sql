CREATE TABLE [dbo].[subAccountAccess] (
    [accessId]        INT              IDENTITY (1, 1) NOT NULL,
    [subAccountId]    INT              NULL,
    [employeeId]      INT              NULL,
    [lastContractId]  INT              NULL,
    [lastReport]      VARCHAR (100)    NULL,
    [lastReportType]  INT              NULL,
    [oldlastReportId] INT              NULL,
    [lastReportId]    UNIQUEIDENTIFIER NULL
);

