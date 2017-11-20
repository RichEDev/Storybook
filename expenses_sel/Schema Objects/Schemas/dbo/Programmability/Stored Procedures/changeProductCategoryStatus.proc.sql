CREATE procedure [dbo].[changeProductCategoryStatus]
@categoryId int,
@archive bit,
@userid int
as
begin
	update productcategories set archived = @archive, modifiedon = getdate(), modifiedby = @userid where [categoryId] = @categoryId

	return;
end
