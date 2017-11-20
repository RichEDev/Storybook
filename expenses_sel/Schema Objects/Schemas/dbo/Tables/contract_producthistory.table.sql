CREATE TABLE [dbo].[contract_producthistory] (
    [productHistoryId]  INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [dateChanged]       SMALLDATETIME  NULL,
    [employeeId]        INT            NULL,
    [contractProductId] INT            NULL,
    [productValue]      FLOAT          NULL,
    [currentMaint]      FLOAT          NULL,
    [nextYearMaint]     FLOAT          NULL,
    [Comment]           NVARCHAR (255) NULL
);

