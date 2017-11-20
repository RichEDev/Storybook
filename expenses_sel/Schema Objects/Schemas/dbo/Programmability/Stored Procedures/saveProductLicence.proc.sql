CREATE procedure [dbo].[saveProductLicence]
@licenceId int,
@productId int,
@licenceKey nvarchar(250),
@licenceType int,
@location nvarchar(250),
@expiry datetime,
@renewalType int,
@notifyId int,
@notifyType int,
@notifyDays int,
@softCopy bit,
@hardCopy bit,
@unlimited bit,
@numberCopiesHeld int,
@userid int,
@updateCount bit
as
begin
	declare @retLicenceId int
	declare @count int

	if @licenceId = 0
	begin
		set @count = (select count([licenceid]) from productLicences where [licenceKey] = @licenceKey and [productId] = @productId)

		if @count > 0
			return -1;

		insert into productLicences ([ProductId], [LicenceKey], [LicenceType], [Location], [Expiry], [RenewalType], [NotifyId], [NotifyType], [NotifyDays],	[SoftCopy], [HardCopy], [Unlimited], [NumberCopiesHeld], [createdon], [createdby])
		values (@productId, @licenceKey, @licenceType, @location, @expiry, @renewalType, @notifyId, @notifyType, @notifyDays, @softCopy, @hardCopy, @unlimited, @numberCopiesHeld, getdate(), @userid)

		set @retLicenceId = scope_identity()
	end
	else
	begin
		set @count = (select count([licenceId]) from productLicences where [licenceKey] = @licenceKey and [productId] = @productId and [licenceId] <> @licenceId)

		if @count > 0
			return -1;

		update productLicences set [ProductId] = @productId, [LicenceKey] = @licenceKey, [LicenceType] = @licenceType, [Location] = @location, [Expiry] = @expiry, [RenewalType] = @renewalType, [NotifyId] = @notifyId, [NotifyType] = @notifyType, [NotifyDays] = @notifyDays, [SoftCopy] = @softCopy, [HardCopy] = @hardCopy, [Unlimited] = @unlimited, [NumberCopiesHeld] = @numberCopiesHeld, [modifiedon] = getdate(), [modifiedby] = @userid
		where [licenceId] = @licenceId

		set @retLicenceId = @licenceId
	end

	if @updateCount = 1
	begin
		-- update the licence count for parent product record
		update productDetails set [NumLicencedCopies] = (select sum([NumberCopiesHeld]) from productLicences where [ProductId] = @productId)
		where [ProductId] = @productId
	end

	return @retLicenceId
end
