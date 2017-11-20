CREATE VIEW [dbo].[TorchGroupingSources]
AS
SELECT dbo.reportsview.description AS reportname, dbo.DocumentGroupingConfigurations.GroupingId, dbo.DocumentGroupingConfigurations.MergeProjectId
FROM  dbo.DocumentGroupingConfigurations INNER JOIN
               dbo.document_mergeprojects ON dbo.DocumentGroupingConfigurations.MergeProjectId = dbo.document_mergeprojects.mergeprojectid INNER JOIN
               dbo.document_mergesources ON dbo.document_mergeprojects.mergeprojectid = dbo.document_mergesources.mergeprojectid INNER JOIN
               dbo.reportsview ON dbo.document_mergesources.reportid = dbo.reportsview.reportID
WHERE (dbo.document_mergesources.groupingsource = 1)