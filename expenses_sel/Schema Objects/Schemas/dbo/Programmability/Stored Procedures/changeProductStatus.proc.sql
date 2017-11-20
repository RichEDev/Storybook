CREATE procedure [dbo].[changeProductStatus]
@productId int,
@archived bit,
@userid int
as
begin
	update productDetails set archived = @archived, modifiedon = getdate(), modifiedby = @userid where productId = @productId

	return;
end
