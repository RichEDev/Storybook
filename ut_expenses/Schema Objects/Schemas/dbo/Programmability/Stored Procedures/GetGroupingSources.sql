CREATE PROCEDURE [dbo].[GetGroupingSources]
	@mergeprojectid int
AS
select mergesourceid from dbo.document_mergesources where mergeprojectid = @mergeprojectid and groupingSource = 1	

