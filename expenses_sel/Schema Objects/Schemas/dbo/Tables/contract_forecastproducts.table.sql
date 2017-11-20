CREATE TABLE [dbo].[contract_forecastproducts] (
    [forecastProductId] INT   IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [forecastId]        INT   NULL,
    [productId]         INT   NULL,
    [productAmount]     FLOAT NULL
);

