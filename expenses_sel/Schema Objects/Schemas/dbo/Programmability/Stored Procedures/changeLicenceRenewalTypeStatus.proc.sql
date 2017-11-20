CREATE procedure [dbo].[changeLicenceRenewalTypeStatus]
@typeId int,
@archive bit,
@userid int
as
begin
	update licenceRenewalTypes set archived = @archive, modifiedon = getdate(), modifiedby = @userid where [renewalType] = @typeId

	return;
end
