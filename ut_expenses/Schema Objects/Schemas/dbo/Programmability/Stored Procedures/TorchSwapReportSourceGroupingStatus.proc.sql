CREATE PROCEDURE [dbo].TorchSwapReportSourceGroupingStatus 
 @mergeSourceId int = 0
AS
BEGIN
 SET NOCOUNT ON;
 DECLARE @projectId int = (SELECT mergeprojectId from document_mergesources WHERE mergesourceid = @mergesourceId);
 IF NOT EXISTS (select * from DocumentGroupingConfigurations where MergeProjectId = @projectId)
 BEGIN
	update document_mergesources set groupingsource = ~groupingsource where mergesourceid = @mergeSourceId;
 END
 return 0;
END
