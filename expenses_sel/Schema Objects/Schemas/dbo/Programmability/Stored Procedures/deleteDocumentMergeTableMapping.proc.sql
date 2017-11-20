CREATE procedure [dbo].[deleteDocumentMergeTableMapping]
@mergetable_mappingid uniqueidentifier,
@table_mappingid int,
@mappingid int,
@CUemployeeID INT,
@CUdelegateID INT
as
begin
	declare @title1 nvarchar(500);
	select @title1 = project_name from document_mergeprojects where mergeprojectid = (select mergeprojectid from document_mappings where mappingid = @mappingId);

	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'Merge Table Mapping for ' + @title1);

	update document_mappings set isMergePart = 0, modifiedOn = getdate(), modifiedBy = @CUemployeeID where mappingid = @table_mappingid;
	
	delete from document_mappings_mergetable where mergetable_mappingid = @mergetable_mappingid;
	
	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 63, @mappingId, @recordTitle, null;

	return @table_mappingid
end
