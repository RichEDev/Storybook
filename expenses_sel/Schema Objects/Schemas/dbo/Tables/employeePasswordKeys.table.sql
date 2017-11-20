CREATE TABLE [dbo].[employeePasswordKeys] (
    [passwordKeyID] INT           IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [employeeID]    INT           NOT NULL,
    [uniqueKey]     NVARCHAR (50) NOT NULL,
    [createdOn]     DATETIME      NOT NULL,
    [sendOnDate]    DATETIME      NULL,
    [sendType]      SMALLINT      NOT NULL
);

