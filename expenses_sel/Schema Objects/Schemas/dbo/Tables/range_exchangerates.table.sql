CREATE TABLE [dbo].[range_exchangerates] (
    [currencyrangeid] INT   NOT NULL,
    [tocurrencyid]    INT   NOT NULL,
    [exchangerate]    FLOAT NOT NULL,
    [subAccountId]    INT   NULL
);

