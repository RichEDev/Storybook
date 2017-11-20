CREATE procedure [dbo].[saveDocumentFieldMapping]
@mappingId int,
@projectId int,
@mergesourceid int,
@merge_fieldkey nvarchar(200),
@reportcolumnid uniqueidentifier,
@CUemployeeID INT,
@CUdelegateID INT
as
begin
	declare @count int
	declare @returnMappingId int

	declare @title1 nvarchar(500);
	declare @recordTitle nvarchar(2000);
	
	if @mappingId = 0
	begin
		set @count = (select count([mappingid]) from document_mappings where mergeprojectid = @projectId and merge_fieldkey = @merge_fieldkey)

		if @count > 0
			return -1;

		insert into document_mappings (mergeprojectid, mergesourceid, merge_fieldkey, merge_fieldtype)
		values (@projectId, @mergesourceid, @merge_fieldkey, 1)

		set @returnMappingId = scope_identity();

		delete from document_mappings_field where mappingid = @returnMappingId

		insert into document_mappings_field (mappingid, reportcolumnid)
		values (@returnMappingId, @reportcolumnid)


		select @title1 = project_name from document_mergeprojects where mergeprojectid = @projectId;
		set @recordTitle = (select 'Field Mappings for ' + @title1);

		exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 63, @returnMappingId, @recordTitle, null;
	end
	else
	begin
		set @count = (select count([mappingid]) from document_mappings where mergeprojectid = @projectId and merge_fieldkey = @merge_fieldkey and mappingid <> @mappingId)
		
		if @count > 0
			return -1;

		update document_mappings set mergeprojectid = @projectId, merge_fieldkey = @merge_fieldkey, merge_fieldtype = 1
		where mappingid = @mappingId

		select @title1 = project_name from document_mergeprojects where mergeprojectid = @projectId;
		set @recordTitle = (select 'Field Mappings for ' + @title1);

		declare @oldreportcolumnid uniqueidentifier;
		select @oldreportcolumnid = reportcolumnid from document_mappings_field where mappingid = @mappingId;

		update document_mappings_field set reportcolumnid = @reportcolumnid where mappingid = @mappingId;

		if @oldreportcolumnid <> @reportcolumnid
			exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 63, @mappingId, '8392df99-dc52-41de-9c53-fe6ccae2a77d', @oldreportcolumnid, @reportcolumnid, @recordTitle, null;

		set @returnMappingId = @mappingId
	end

	return @returnMappingId;
end
