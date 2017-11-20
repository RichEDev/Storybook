
CREATE VIEW dbo.currencyView
	AS
	SELECT 
	currencies.currencyId,
	currencies.globalcurrencyid,
	global_currencies.currencySymbol,
	global_currencies.label AS currencyName,
	global_currencies.numericcode,
	global_currencies.symbol,
	global_currencies.alphacode,
	currencies.archived,
	currencies.positiveFormat,
	currencies.negativeFormat,
	currencies.createdOn,
	dbo.getEmployeeFullName(currencies.createdBy) AS createdBy,
	currencies.modifiedOn,
	dbo.getEmployeeFullName(currencies.modifiedBy) AS modifiedBy
	FROM currencies
	INNER JOIN global_currencies ON global_currencies.globalcurrencyid = currencies.globalcurrencyid
