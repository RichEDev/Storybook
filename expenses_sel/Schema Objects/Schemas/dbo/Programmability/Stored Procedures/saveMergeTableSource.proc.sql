CREATE PROCEDURE [dbo].[saveMergeTableSource]
@mergeProjectId INT,
@mappingId INT,
@noDataMessage nvarchar(max),
@description nvarchar(100),
@CUemployeeID INT,
@CUdelegateID INT
as
begin
	declare @returnId int;
	declare @title1 nvarchar(500);
	declare @merge_fieldkey nvarchar(200);
	
	set @merge_fieldkey = '<@%MT_MERGETABLES_' + CAST(@mergeProjectId as nvarchar) + '_' + CAST(@mappingId as nvarchar) + '%@>';
	select @title1 = project_name from document_mergeprojects where mergeprojectid = @mergeProjectId;

	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'Merge Table Mappings for ' + @title1);
	
	if(@mappingId = 0)
	begin
		insert into document_mappings (mergeprojectid, merge_fieldtype, merge_fieldkey, noDataMessage, [description], createdOn, createdBy) values (@mergeProjectId, 4, @merge_fieldkey, @noDataMessage, @description, GETDATE(), @CUemployeeID);

		set @returnId = scope_identity();
		set @merge_fieldkey = replace(@merge_fieldkey, '_0%@>','_' + CAST(@returnId as nvarchar) + '%@>');
		
		update document_mappings set merge_fieldkey = @merge_fieldkey where mappingid = @returnId;
	end
	else
	begin
		update document_mappings set noDataMessage = @noDataMessage, [description] = @description where mappingid = @mappingId;
		set @returnId = @mappingId;
	end

	return @returnId;
end
