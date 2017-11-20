CREATE PROCEDURE [dbo].[TorchGetCommonGroupingColumns] 
@mergeProjectId INT = 0
AS
BEGIN
 DECLARE @numGroupingSources INT = 0;

 SET @numGroupingSources = (
   SELECT count(*)
   FROM document_mergesources
   WHERE groupingsource = 1 AND
   mergeprojectid = @mergeProjectId

   )

 SELECT dbo.fields.[description]
 FROM dbo.reportcolumns
 INNER JOIN dbo.document_mergesources ON dbo.document_mergesources.reportid = dbo.reportcolumns.reportID
 INNER JOIN dbo.fields ON dbo.reportcolumns.fieldID = dbo.fields.fieldid
 WHERE (dbo.document_mergesources.mergeprojectid = @mergeProjectId)
  AND (dbo.document_mergesources.groupingsource = 1)
 GROUP BY dbo.fields.[description]
 HAVING (COUNT(dbo.fields.[description]) >= @numGroupingSources)
 ORDER BY dbo.fields.[description]
END