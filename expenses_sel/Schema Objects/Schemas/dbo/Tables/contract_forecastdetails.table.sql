CREATE TABLE [dbo].[contract_forecastdetails] (
    [contractForecastId] INT           IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [contractId]         INT           NULL,
    [paymentDate]        SMALLDATETIME NULL,
    [forecastAmount]     FLOAT         NULL,
    [Comment]            VARCHAR (250) NULL,
    [poNumber]           VARCHAR (30)  NULL,
    [poStartDate]        DATETIME      NULL,
    [poExpiryDate]       DATETIME      NULL,
    [poMaxValue]         FLOAT         NOT NULL,
    [coverPeriodEnd]     DATETIME      NULL,
    [financePeriod]      SMALLINT      NULL
);

