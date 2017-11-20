CREATE VIEW [dbo].[contractdetails_summary_view]
AS
SELECT contract_details.*,
[X].[Name] AS [Inflator X],
[Y].[Name] AS [Inflator Y]
FROM contract_details
LEFT JOIN codes_inflatormetrics AS [X] ON [X].[MetricId] = contract_details.[MaintenanceInflatorX]
LEFT JOIN codes_inflatormetrics AS [Y] ON [Y].[MetricId] = contract_details.[MaintenanceInflatorY]
