CREATE procedure [dbo].[deleteDocumentFieldMapping]
@mappingId int,
@CUemployeeID INT,
@CUdelegateID INT
as
begin

	declare @title1 nvarchar(500);
	select @title1 = project_name from document_mergeprojects where mergeprojectid = (select mergeprojectid from document_mappings where mappingid = @mappingId);

	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'Field Mappings for ' + @title1);

	delete from document_mappings_field where mappingid = @mappingId;
	delete from document_mappings where mappingid = @mappingId;

	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 63, @mappingId, @recordTitle, null;
end
