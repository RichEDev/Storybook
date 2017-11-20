
CREATE PROCEDURE [dbo].[TorchGetSortingColumnsById]
@groupingConfigurationId INT = 0,
@MergeProjectId INT = 0
AS
  
SELECT dbo.DocumentSortingConfigurationColumns.SortingColumn, dbo.DocumentSortType.DocumentSortType
FROM   dbo.DocumentSortingConfigurationColumns INNER JOIN
       dbo.DocumentSortType ON dbo.DocumentSortingConfigurationColumns.DocumentSortTypeId = dbo.DocumentSortType.DocumentSortTypeId
WHERE (dbo.DocumentSortingConfigurationColumns.GroupingId = @groupingConfigurationId)
       AND (DocumentSortingConfigurationColumns.MergeProjectId = @MergeProjectId)
ORDER BY dbo.DocumentSortingConfigurationColumns.SequenceOrder