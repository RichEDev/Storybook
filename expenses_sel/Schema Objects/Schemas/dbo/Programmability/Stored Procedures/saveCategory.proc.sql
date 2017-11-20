CREATE PROCEDURE [dbo].[saveCategory]
@categoryid int,
@category nvarchar(50),
@description nvarchar(4000),
@date DateTime,
@userid int,
@id int out
AS

if (@categoryid = 0)
begin
	insert into categories (category, [description], createdon, createdby) values (@category, @description, @date, @userid)
	set @id = scope_identity()
end
else
begin
	update categories set category = @category, [description] = @description, modifiedon = @date, modifiedby = @userid where categoryid = @categoryid
	set @id = @categoryid
end

return @id