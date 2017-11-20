CREATE Procedure [dbo].[TorchGetReportSortingConfigurations]

 @MergeProjectId          INT,
 @GroupingConfigurationId INT
 AS

Begin

SELECT dbo.DocumentSortingReports.ReportSortingConfigurationId, dbo.DocumentSortingReports.ReportName
FROM  dbo.DocumentSortingReports INNER JOIN
               dbo.DocumentReportSortingConfigurations ON 
               dbo.DocumentSortingReports.ReportSortingConfigurationId = dbo.DocumentReportSortingConfigurations.ReportSortingConfigurationId
WHERE (dbo.DocumentReportSortingConfigurations.GroupingId = @GroupingConfigurationId) AND (dbo.DocumentReportSortingConfigurations.MergeProjectId = @MergeProjectId)

END