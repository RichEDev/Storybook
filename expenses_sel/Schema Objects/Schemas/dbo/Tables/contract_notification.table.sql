CREATE TABLE [dbo].[contract_notification] (
    [notificationId] INT IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [subAccountId]   INT NULL,
    [contractId]     INT NULL,
    [employeeId]     INT NULL,
    [IsTeam]         INT NOT NULL
);

