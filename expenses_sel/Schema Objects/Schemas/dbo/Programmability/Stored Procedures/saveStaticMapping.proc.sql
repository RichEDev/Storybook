



CREATE PROCEDURE [dbo].[saveStaticMapping]
@staticId int,
@projectId int,
@mergeFieldKey nvarchar(200),
@CUemployeeID INT,
@CUdelegateID INT
as
begin
	declare @returnID int
	declare @count int
	declare @newMappingId int

	set @count = (select count(document_mappings_static.mappingid) from document_mappings_static inner join document_mappings on document_mappings_static.mappingid = document_mappings.mappingid where static_textid = @staticId and document_mappings.mergeprojectid = @projectId)
	if @count > 0
		return -1;

	declare @title1 nvarchar(500);
	select @title1 = project_name from document_mergeprojects where mergeprojectid = @projectId;

	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'Static Mappings for ' + @title1);


	insert into document_mappings (mergeprojectid, merge_fieldtype, merge_fieldkey) values (@projectId, 3, @mergeFieldKey)

	set @newMappingId = scope_identity();

	insert into document_mappings_static (mappingid, static_textid) values (@newMappingId, @staticId)

	exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 63, @staticId, @recordTitle, null;

	return @newMappingId;
end



