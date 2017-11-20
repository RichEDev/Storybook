CREATE TABLE [dbo].[monthly_exchangerates] (
    [currencymonthid] INT   NOT NULL,
    [tocurrencyid]    INT   NOT NULL,
    [exchangerate]    FLOAT NOT NULL,
    [subAccountId]    INT   NULL
);

