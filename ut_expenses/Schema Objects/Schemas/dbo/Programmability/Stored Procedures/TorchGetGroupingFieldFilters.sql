CREATE PROCEDURE [dbo].[TorchGetGroupingFieldFilters]
 @mergeProjectId INT = 0,
 @groupingConfigurationId INT = 0

AS

SELECT dbo.DocumentFiltersConfigurationColumns.FilterColumn, dbo.DocumentFiltersConfigurationColumns.SequenceOrder, 
               dbo.DocumentFiltersConfigurationColumns.Condition, dbo.DocumentFiltersConfigurationColumns.ValueOne, 
               dbo.DocumentFiltersConfigurationColumns.ValueTwo, dbo.DocumentFiltersConfigurationColumns.TypeText, 
               dbo.DocumentFiltersConfigurationColumns.FieldType
FROM  dbo.DocumentFiltersConfigurationColumns 
WHERE  (dbo.DocumentFiltersConfigurationColumns.GroupingId = @groupingConfigurationId) AND
       (dbo.DocumentFiltersConfigurationColumns.MergeProjectId = mergeProjectId) 
ORDER BY DocumentFiltersConfigurationColumns.SequenceOrder