CREATE procedure [dbo].[deleteProductLicence]
@licenceId int,
@userid int,
@updateCount bit
as
begin
	declare @productId int
	set @productId = (select [productId] from productLicences where [licenceid] = @licenceId)

	if @productId > 0
	begin
		delete from productlicences where [licenceid] = @licenceId

		if @updateCount = 1
		begin
			-- update the licence count for parent product record
			update productDetails set [NumLicencedCopies] = (select sum([NumberCopiesHeld]) from productLicences where [ProductId] = @productId)
			where [ProductId] = @productId
		end
	end

	return;
end
