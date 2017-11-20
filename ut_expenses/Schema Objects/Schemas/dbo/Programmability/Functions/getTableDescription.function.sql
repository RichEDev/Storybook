CREATE function dbo.getTableDescription(@tableid uniqueidentifier)
returns nvarchar(250) as
begin
	declare @desc nvarchar(250);

	select @desc = description from dbo.[tables] where userdefined_table = @tableid
	return @desc;
end
