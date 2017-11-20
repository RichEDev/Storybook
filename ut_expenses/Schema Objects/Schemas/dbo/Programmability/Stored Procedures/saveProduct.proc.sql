CREATE procedure [dbo].[saveProduct]
@productId int,
@subAccountId int,
@productName nvarchar(70),
@description nvarchar(max),
@productCode nvarchar(15),
@productCategoryId int,
@noLicencedCopies int,
@availableVersion nvarchar(10),
@installedVersion nvarchar(10),
@dateInstalled datetime,
@userCode nvarchar(15),
@archived bit,
@userid int
as
begin
	declare @retProductId int
	declare @count int	

	if @productId = 0
	begin
		set @count = (select count([productId]) from productDetails where [productName] = @productName and [subAccountId] = @subAccountId)

		if @count > 0
			return -1;

		insert into productDetails ([subAccountId],[productCode],[productName],[description],[productCategoryId],[availableVersionNumber],[installedVersionNumber],[dateInstalled],[numLicencedCopies],[userCode],[archived],[createdon],[createdby])
		values (@subAccountId, @productCode, @productName, @description, @productCategoryId, @availableVersion, @installedVersion, @dateInstalled, @noLicencedCopies, @userCode, @archived, getdate(), @userid)

		set @retProductId = scope_identity()
	end
	else
	begin
		set @count = (select count([productId]) from productDetails where [productName] = @productName and [subAccountId] = @subAccountId and [productId] <> @productId)

		if @count > 0
			return -1;

		update productDetails set [productCode] = @productCode,[productName] = @productName,[description] = @description,[productCategoryId] = @productCategoryId,
		[availableVersionNumber] = @availableVersion,[installedVersionNumber] = @installedVersion,[dateInstalled] = @dateInstalled,[numLicencedCopies] = @noLicencedCopies,[userCode] =  @userCode,[archived] = @archived, modifiedon = getdate(), modifiedby = @userid
		where [productId] = @productId

		set @retProductId = @productId
	end

	return @retProductId;
end
