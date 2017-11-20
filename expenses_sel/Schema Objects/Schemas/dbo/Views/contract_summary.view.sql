CREATE VIEW [dbo].[contract_summary]
AS
SELECT     
dbo.contract_details.contractId, 
dbo.contract_details.subAccountId, 
dbo.GetContractValue(dbo.contract_details.contractId, 1) AS ContractValueinclVariations, 
dbo.GetContractValue(dbo.contract_details.contractId, 0) AS ContractValueexclVariations, 
dbo.AttachmentCount(0, dbo.contract_details.contractId) AS ContractAttachmentCount, 
dbo.VariationCount(dbo.contract_details.contractId) AS ContractVariationsCount, 
dbo.GetNotifyString(dbo.contract_details.contractId) AS ContractNotifyList, 
dbo.IsVariation(dbo.contract_details.contractId) AS IsVariation, 
dbo.global_currencies.label AS ContractCurrencyName, 
inflatorX.name AS InflatorX, 
inflatorX.percentage AS InflatorXPercentage, 
inflatorY.name AS InflatorY, 
inflatorY.percentage AS InflatorYPercentage, 
dbo.getEmployeeFullName(dbo.contract_details.contractOwner) AS ContractOwnerName, 
dbo.contract_details.lastChangedBy AS ContractLastChangedBy, 
dbo.getEmployeeFullName(dbo.contract_details.createdBy) AS ContractCreatedBy
FROM         dbo.contract_details LEFT OUTER JOIN
                      dbo.currencies ON dbo.currencies.currencyid = dbo.contract_details.contractCurrency LEFT OUTER JOIN
                      dbo.codes_inflatormetrics AS inflatorX ON inflatorX.metricId = dbo.contract_details.maintenanceInflatorX LEFT OUTER JOIN
                      dbo.codes_inflatormetrics AS inflatorY ON inflatorY.metricId = dbo.contract_details.maintenanceInflatorY INNER JOIN
                      dbo.global_currencies ON dbo.global_currencies.globalcurrencyid = dbo.currencies.globalcurrencyid
