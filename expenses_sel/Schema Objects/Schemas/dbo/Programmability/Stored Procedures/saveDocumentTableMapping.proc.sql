CREATE PROCEDURE [dbo].[saveDocumentTableMapping]
@projectid int,
@mappingid int,
@mergesourceid int,
@merge_fieldkey nvarchar(200),
@includeHeaders bit,
@headerFont nvarchar(50),
@headerFontSize decimal,
@bodyFont nvarchar(50),
@bodyFontSize decimal,
@userid int,
@noDataMessage nvarchar(max),
@subAccountID int,
@CUemployeeID INT,
@CUdelegateID INT
as
begin
	declare @returnId int
	declare @count int

	declare @title1 nvarchar(500);
	declare @recordTitle nvarchar(2000);

	if @mappingid = 0
	begin	
		insert into document_mappings (mergeprojectid, mergesourceid, merge_fieldtype, merge_fieldkey, includeHeaders, headerFont, headerFontSize, bodyFont, bodyFontSize, noDataMessage) values (@projectid, @mergesourceid, 2, '', @includeHeaders, @headerFont, @headerFontSize, @bodyFont, @bodyFontSize, @noDataMessage)

		set @returnId = scope_identity();
		set @merge_fieldkey = replace(@merge_fieldkey, '_0%@>','_' + CAST(@returnId as nvarchar) + '%@>')

		update document_mappings set merge_fieldkey = @merge_fieldkey where mappingid = @returnId;
		
		select @title1 = project_name from document_mergeprojects where mergeprojectid = @projectid;
		set @recordTitle = (select 'Table Mapping for ' + @title1);
		exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 63, @returnId, @recordTitle, @subAccountID;
	end
	else
	begin
		set @count = (select count(mappingid) from document_mappings where mergeprojectid = @projectid and merge_fieldkey = @merge_fieldkey and mappingid <> @mappingid)

		if @count > 0
			return -1;

		declare @oldprojectid int;
		declare @oldmerge_fieldkey nvarchar(200);
		declare @oldnodatamessage nvarchar(max);
		declare @oldreportsourceid int;
		
		select @oldprojectid = mergeprojectid, @oldreportsourceid = mergesourceid, @oldmerge_fieldkey = merge_fieldkey, @oldnodatamessage = noDataMessage from document_mappings where mappingid = @mappingid;

		update document_mappings set mergeprojectid = @projectid, merge_fieldtype = 2, mergesourceid = @mergesourceid, merge_fieldkey = @merge_fieldkey, includeHeaders = @includeHeaders, headerFont = @headerFont, headerFontSize = @headerFontSize, bodyFont = @bodyFont, bodyFontSize = @bodyFontSize, noDataMessage = @noDataMessage where mappingid = @mappingid;

		select @title1 = project_name from document_mergeprojects where mergeprojectid = @projectid;
		set @recordTitle = (select 'Table Mapping for ' + @title1);

		if @oldprojectid <> @projectid
			exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 63, @mappingid, '4252b899-1d44-4d9a-a0bb-418c73cff200', @oldprojectid, @projectid, @recordtitle, @subAccountID;
		if @oldmerge_fieldkey <> @merge_fieldkey
			exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 63, @mappingid, '3cdf279b-b5c3-4735-8932-ef6af82b3dc8', @oldmerge_fieldkey, @merge_fieldkey, @recordtitle, @subAccountID;
		if @oldreportsourceid <> @mergesourceid
			exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 63, @mappingid, 'F7802F4C-9091-449C-B77F-EA9F35C59236', @oldreportsourceid, @mergesourceid, @recordtitle, @subAccountID;			
		if @oldnodatamessage <> @noDataMessage
			exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 63, @mappingid, 'F9DED916-EE69-4499-A70D-42623821FE5C', @oldnodatamessage, @noDataMessage, @recordtitle, @subAccountID;
		
		set @returnId = @mappingid;
	end

	return @returnId;
end
