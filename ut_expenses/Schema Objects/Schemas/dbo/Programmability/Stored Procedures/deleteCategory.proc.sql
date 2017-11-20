CREATE PROCEDURE [dbo].[deleteCategory]
@categoryid int,
@id int out
AS

declare @count int

declare @tableId uniqueidentifier = (select tableid from tables where tablename = 'categories');
exec @count = dbo.checkReferencedBy @tableId, @categoryid;
if @count = -10
	return -10;

set @count = (select count(*) from subcats where categoryid = @categoryid)

if (@count = 0)
begin
	delete from categories where categoryid = @categoryid
	set @id = 0
end
else
	set @id = 1

return @id