CREATE TABLE [dbo].[contract_forecastproducts] (
    [forecastProductId] INT        IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [forecastId]        INT        NULL,
    [productId]         INT        NULL,
    [productAmount]     FLOAT (53) NULL,
    CONSTRAINT [PK_Contract_ForecastProducts] PRIMARY KEY CLUSTERED ([forecastProductId] ASC),
    CONSTRAINT [FK_contract_forecastproducts_contract_forecastdetails] FOREIGN KEY ([forecastId]) REFERENCES [dbo].[contract_forecastdetails] ([contractForecastId]) ON DELETE CASCADE,
    CONSTRAINT [FK_contract_forecastproducts_product_details] FOREIGN KEY ([productId]) REFERENCES [dbo].[productDetails] ([productId])
);



