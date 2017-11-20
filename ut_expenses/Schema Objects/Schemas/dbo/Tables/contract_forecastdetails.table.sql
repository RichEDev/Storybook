CREATE TABLE [dbo].[contract_forecastdetails] (
    [contractForecastId] INT           IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [contractId]         INT           NULL,
    [paymentDate]        SMALLDATETIME NULL,
    [forecastAmount]     FLOAT (53)    NULL,
    [Comment]            VARCHAR (250) NULL,
    [poNumber]           VARCHAR (30)  NULL,
    [poStartDate]        DATETIME      NULL,
    [poExpiryDate]       DATETIME      NULL,
    [poMaxValue]         FLOAT (53)    CONSTRAINT [DF_Contract_ForecastDetails_POMaxValue] DEFAULT ((0)) NOT NULL,
    [coverPeriodEnd]     DATETIME      NULL,
    [financePeriod]      SMALLINT      NULL,
    CONSTRAINT [PK_Contract_ForecastDetails] PRIMARY KEY CLUSTERED ([contractForecastId] ASC),
    CONSTRAINT [FK_contract_forecastdetails_contract_details] FOREIGN KEY ([contractId]) REFERENCES [dbo].[contract_details] ([contractId]) ON DELETE CASCADE
);



