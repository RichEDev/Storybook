CREATE procedure [dbo].[deleteDocumentTableMapping]
@mappingid int,
@CUemployeeID INT,
@CUdelegateID INT
as
begin
	declare @title1 nvarchar(500);
	select @title1 = project_name from document_mergeprojects where mergeprojectid = (select mergeprojectid from document_mappings where mappingid = @mappingId);

	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'Table Mappings for ' + @title1);

	delete from document_mappings_table where mappingid = @mappingid
	delete from document_mappings where mappingid = @mappingid

	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 63, @mappingId, @recordTitle, null;

	return @mappingid
end
