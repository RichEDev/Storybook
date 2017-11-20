
create procedure dbo.deleteStaticDefinition
@staticId int
as
begin
	declare @count int

	set @count = (select count([static_textid]) from document_mappings_static where static_textid = @staticId)
	if @count > 0
		return -1;

	delete from document_mappings_text where static_textid = @staticId

	return @staticId;
end
