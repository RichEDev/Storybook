CREATE VIEW [dbo].[contract_productdetails_view]
AS 
	SELECT contract_productdetails.contractProductId AS [conProdID], contract_productdetails.contractId, contract_productdetails.productId,
	contract_productdetails.productValue, contract_productdetails.currencyId, contract_productdetails.salesTaxRate,
	contract_productdetails.maintenanceValue, contract_productdetails.maintenancePercent, contract_productdetails.unitCost,
	contract_productdetails.Quantity, contract_productdetails.unitId, contract_productdetails.projectedSaving,
	contract_productdetails.pricePaid, contract_productdetails.archiveStatus, contract_productdetails.archiveDate,
	contract_productdetails.createdDate, contract_productdetails.lastModified,
	userdefinedContractProductDetails.*,
	productdetails.[productName],
	global_currencies.[label] AS [ContractProductCurrency],
	codes_salestax.[Description] AS [ContractProductSalesTax],
	codes_units.[Description] AS [ContractProductUnitDesc]
	FROM contract_productdetails
	INNER JOIN userdefinedContractProductDetails ON userdefinedContractProductDetails.contractproductid = contract_productdetails.contractProductId
	INNER JOIN productdetails ON contract_productdetails.[ProductId] = productdetails.[ProductId]
	LEFT JOIN currencies ON contract_productdetails.[CurrencyId] = currencies.[currencyId]
	INNER JOIN global_currencies ON global_currencies.globalcurrencyid = currencies.globalcurrencyid
	LEFT JOIN codes_salestax ON contract_productdetails.[SalesTaxRate] = codes_salestax.[SalesTaxId]
	LEFT JOIN codes_units ON contract_productdetails.[UnitId] = codes_units.[UnitId]
