CREATE PROCEDURE [dbo].[deleteCategory]
@categoryid int,
@id int out
AS

declare @count int

set @count = (select count(*) from subcats where categoryid = @categoryid)

if (@count = 0)
begin
	delete from categories where categoryid = @categoryid
	set @id = 0
end
else
	set @id = 1

return @id