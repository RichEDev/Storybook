CREATE PROCEDURE [dbo].[deleteStaticMapping]
@mappingId int,
@CUemployeeID INT,
@CUdelegateID INT
as
begin
	declare @title1 nvarchar(500);
	select @title1 = project_name from document_mergeprojects where mergeprojectid = (select mergeprojectid from document_mappings where mappingid = @mappingId);

	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'Static Mappings for ' + @title1);

	DECLARE @static_textid int;
	SET @static_textid = (SELECT static_textid from document_mappings_static WHERE mappingid = @mappingId)
	DELETE FROM document_mappings_text WHERE static_textid = @static_textid
	delete from document_mappings_static where mappingid = @mappingId
	delete from document_mappings where mappingid = @mappingId

	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 63, @mappingId, @recordTitle, null;

	return @mappingId
end
