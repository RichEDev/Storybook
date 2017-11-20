CREATE TABLE [dbo].[contract_audience] (
    [audienceId]   INT IDENTITY (1, 1) NOT NULL,
    [contractId]   INT NULL,
    [audienceType] INT NULL,
    [accessId]     INT NULL
);

