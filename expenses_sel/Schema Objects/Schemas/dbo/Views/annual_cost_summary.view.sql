CREATE VIEW [dbo].[annual_cost_summary]
AS 
SELECT 
contract_details.[ContractId],
[ContractProductId],
ISNULL([ProductValue],0) AS [ProductValue],
ISNULL([MaintenanceValue],0) AS [MaintenanceValue],
ISNULL([MaintenancePercent],0) AS [MaintenancePercent],
ISNULL(contract_details.[ForecastTypeId],0) AS [ForecastTypeId],
ISNULL(contract_details.[MaintenanceType],0) AS [MaintenanceType],
ISNULL([MaintenanceInflatorX],0) AS [MaintenanceInflatorX],
ISNULL(contract_details.[MaintenancePercentX],0) AS [MaintenancePercentX],
ISNULL([X].[Percentage],0) AS [PercentageX],
ISNULL([MaintenanceInflatorY],0) AS [MaintenanceInflatorY],
ISNULL([MaintenancePercentY],0) AS [MaintenancePercentY],
ISNULL([Y].[Percentage],0) AS [PercentageY],
dbo.CalcNYM([ContractProductId],ISNULL([ForecastTypeId],0),ISNULL([MaintenanceType],0),ISNULL([MaintenanceInflatorX],0),ISNULL([X].[Percentage],0),ISNULL([MaintenancePercentX],0),ISNULL([MaintenanceInflatorY],0),ISNULL(Y.[Percentage],0),ISNULL([MaintenancePercentY],0),0) AS [NextYearMaintenance],
dbo.CalcNYM([ContractProductId],ISNULL([ForecastTypeId],0),ISNULL([MaintenanceType],0),ISNULL([MaintenanceInflatorX],0),ISNULL([X].[Percentage],0),ISNULL([MaintenancePercentX],0),ISNULL([MaintenanceInflatorY],0),ISNULL([Y].[Percentage],0),ISNULL([MaintenancePercentY],0),1) AS [NextYearMaintenance+1],
dbo.CalcNYM([ContractProductId],ISNULL([ForecastTypeId],0),ISNULL([MaintenanceType],0),ISNULL([MaintenanceInflatorX],0),ISNULL([X].[Percentage],0),ISNULL([MaintenancePercentX],0),ISNULL([MaintenanceInflatorY],0),ISNULL([Y].[Percentage],0),ISNULL([MaintenancePercentY],0),2) AS [NextYearMaintenance+2],
dbo.CalcNYM([ContractProductId],ISNULL([ForecastTypeId],0),ISNULL([MaintenanceType],0),ISNULL([MaintenanceInflatorX],0),ISNULL([X].[Percentage],0),ISNULL([MaintenancePercentX],0),ISNULL([MaintenanceInflatorY],0),ISNULL([Y].[Percentage],0),ISNULL([MaintenancePercentY],0),3) AS [NextYearMaintenance+3],
dbo.CalcNYM([ContractProductId],ISNULL([ForecastTypeId],0),ISNULL([MaintenanceType],0),ISNULL([MaintenanceInflatorX],0),ISNULL([X].[Percentage],0),ISNULL([MaintenancePercentX],0),ISNULL([MaintenanceInflatorY],0),ISNULL([Y].[Percentage],0),ISNULL([MaintenancePercentY],0),4) AS [NextYearMaintenance+4],
dbo.CalcNYM([ContractProductId],ISNULL([ForecastTypeId],0),ISNULL([MaintenanceType],0),ISNULL([MaintenanceInflatorX],0),ISNULL([X].[Percentage],0),ISNULL([MaintenancePercentX],0),ISNULL([MaintenanceInflatorY],0),ISNULL([Y].[Percentage],0),ISNULL([MaintenancePercentY],0),5) AS [NextYearMaintenance+5]
FROM contract_productdetails
INNER JOIN contract_details ON contract_productdetails.[ContractId] = contract_details.[ContractId]
LEFT OUTER JOIN [codes_inflatormetrics] [X] ON [contract_details].[MaintenanceInflatorX] = [X].[MetricId]
LEFT OUTER JOIN [codes_inflatormetrics] [Y] ON [contract_details].[MaintenanceInflatorY] = [Y].[MetricId]
