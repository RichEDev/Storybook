CREATE PROCEDURE [dbo].[TorchGetFieldIdFromCommonColumn]
	@mergeProjectId INT,
	@commonColumnName NVARCHAR(100)

AS
DECLARE @numGroupingSources INT = 0;

 SET @numGroupingSources = (
   SELECT count(*)
   FROM document_mergesources
   WHERE groupingsource = 1 AND mergeprojectid = @mergeProjectId
   )

 SELECT top 1 dbo.fields.fieldid, dbo.fields.fieldtype
 FROM dbo.reportcolumns
 INNER JOIN dbo.document_mergesources ON dbo.document_mergesources.reportid = dbo.reportcolumns.reportID
 INNER JOIN dbo.fields ON dbo.reportcolumns.fieldID = dbo.fields.fieldid
 WHERE (dbo.document_mergesources.mergeprojectid = @mergeProjectId)
  AND (dbo.document_mergesources.groupingsource = 1)
 AND  dbo.fields.description = @commonColumnName
 ORDER BY dbo.fields.[description]
 
