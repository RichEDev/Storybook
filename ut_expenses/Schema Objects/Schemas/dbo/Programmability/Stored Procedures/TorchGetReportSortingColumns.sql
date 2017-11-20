CREATE PROCEDURE [dbo].[TorchGetReportSortingColumns]
@ReportSortingConfigurationId INT

AS

SELECT dbo.DocumentSortingReportColumns.ColumnName, dbo.DocumentSortingReportColumns.DocumentSortTypeId
FROM  dbo.DocumentSortingReportColumns 
WHERE dbo.DocumentSortingReportColumns.DocumentSortingReportId = @ReportSortingConfigurationId
ORDER BY dbo.DocumentSortingReportColumns.SequenceOrder

