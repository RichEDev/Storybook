
CREATE PROCEDURE [dbo].[TorchGetSortingReports]

 @MergeProjectId          INT,
 @GroupingConfigurationId INT
 
AS

BEGIN

SELECT dbo.DocumentSortingReports.DocumentSortingReportId, dbo.DocumentSortingReports.ReportName
FROM  dbo.DocumentSortingReports INNER JOIN
               dbo.DocumentReportSortingConfigurations ON 
               dbo.DocumentSortingReports.ReportSortingConfigurationId = dbo.DocumentReportSortingConfigurations.ReportSortingConfigurationId
WHERE (dbo.DocumentReportSortingConfigurations.GroupingId = @GroupingConfigurationId) AND (dbo.DocumentReportSortingConfigurations.MergeProjectId = @MergeProjectId)

END