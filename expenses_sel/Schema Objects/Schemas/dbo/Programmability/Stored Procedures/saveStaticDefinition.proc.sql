CREATE PROCEDURE [dbo].[saveStaticDefinition]
@staticId int,
@mergeProjectId int,
@staticText nvarchar(max),
@CUemployeeID INT,
@CUdelegateID INT
as
begin
	declare @returnID int
	declare @count int
	declare @title1 nvarchar(500);
	declare @recordTitle nvarchar(2000);

	if @staticId = 0
	begin
		SET @count = (SELECT COUNT([static_textid]) FROM document_mappings_text WHERE static_text = @staticText AND mergeprojectid = @mergeProjectId);
		IF @count > 0
			RETURN -1;

		insert into document_mappings_text (mergeprojectid, static_text) values (@mergeProjectId, @staticText);
		set @returnID = scope_identity();

		set @recordTitle = (select 'Static Mapping Text - ' + cast(@returnID as nvarchar(20)));
		exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 63, @returnID, @recordTitle, null;

	end
	else
	begin
		set @count = (SELECT COUNT([static_textid]) FROM document_mappings_text WHERE static_text = @staticText and static_textid <> @staticId AND mergeprojectid = @mergeProjectId)
		if @count > 0
			return -1;

		select @title1 = project_name from document_mergeprojects where mergeprojectid = (select mergeprojectid from document_mappings where mappingid = (select mappingid from document_mappings_static where static_textid = @staticId));
		set @recordTitle = (select 'Static Mapping Text for ' + @title1);

		declare @oldstaticText nvarchar(max);
		select @oldstaticText = static_text from document_mappings_text where static_textid = @staticId;

		update document_mappings_text set static_text = @staticText where static_textid = @staticId and mergeprojectid = @mergeProjectId;

		if @oldstaticText <> @staticText
			exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 63, @staticId, '8ea17dc8-9762-4c8b-a789-6b11562e0da2', @oldstaticText, @staticText, @recordTitle, null;

		set @returnID = @staticId
	end

	return @returnID		
end
