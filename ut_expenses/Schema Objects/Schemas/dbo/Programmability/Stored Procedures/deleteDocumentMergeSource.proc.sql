CREATE procedure [dbo].[deleteDocumentMergeSource]
@mergeSourceId int,
@CUemployeeID INT,
@CUdelegateID INT
as
begin
	declare @title1 nvarchar(500);
	select @title1 = project_name from document_mergeprojects where mergeprojectid = (select mergeprojectid from document_mergesources where mergeSourceId = @mergeSourceId);

	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'Merge Source for ' + @title1);

	delete from document_mergesources where mergesourceid = @mergeSourceId;

	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 63, @mergeSourceId, @recordTitle, null;
end
