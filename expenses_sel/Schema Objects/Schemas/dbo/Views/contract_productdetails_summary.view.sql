CREATE VIEW [dbo].[contract_productdetails_summary]
AS
SELECT     dbo.contract_productdetails.contractProductId, dbo.global_currencies.label AS contractProductCurrency, 
                      dbo.codes_salestax.description AS contractProductSalesTax
FROM         dbo.contract_productdetails LEFT OUTER JOIN
                      dbo.currencies ON dbo.currencies.currencyId = dbo.contract_productdetails.currencyId INNER JOIN
                      dbo.global_currencies ON dbo.global_currencies.globalcurrencyid = dbo.currencies.globalcurrencyid LEFT OUTER JOIN
                      dbo.codes_salestax ON dbo.codes_salestax.salesTaxId = dbo.contract_productdetails.salesTaxRate


