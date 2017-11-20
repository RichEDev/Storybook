CREATE VIEW dbo.global_currencies
AS
SELECT     globalcurrencyid, label, alphacode, numericcode, symbol, dbo.getCurrencySymbol(symbol) AS currencySymbol, createdon, modifiedon
FROM         [$(targetMetabase)].dbo.global_currencies AS global_currencies_1

