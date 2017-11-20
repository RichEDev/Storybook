CREATE PROCEDURE dbo.SaveVehicleEngineType
	@VehicleEngineTypeId int,
	@CuEmployeeId int,
	@CuDelegateId int,
	@Code nvarchar(50),
	@Name nvarchar(450)
AS

	if exists (select VehicleEngineTypeId from VehicleEngineTypes where Code = @Code and (VehicleEngineTypeId <> @VehicleEngineTypeId or @VehicleEngineTypeId is null))
		return -2 -- Code already exists
		
	if exists (select VehicleEngineTypeId from VehicleEngineTypes where Name = @Name and (VehicleEngineTypeId <> @VehicleEngineTypeId or @VehicleEngineTypeId is null))
		return -1 -- Name already exists
		
	declare @ElementId int = 187;

	if exists (select VehicleEngineTypeId from VehicleEngineTypes where VehicleEngineTypeId = @VehicleEngineTypeId)
	begin
		declare
			@PrevCode nvarchar(50),
			@PrevName nvarchar(450)

		select
			@PrevCode = Code,
			@PrevName = Name
		from
			VehicleEngineTypes
		where
			VehicleEngineTypeId = @VehicleEngineTypeId;

		update
			VehicleEngineTypes
		set
			ModifiedBy = @CuEmployeeId,
			ModifiedOn = GETUTCDATE(),
			Code = @Code,
			Name = @Name
		where
			VehicleEngineTypeId = @VehicleEngineTypeId;

		if NOT EXISTS(SELECT @PrevCode INTERSECT SELECT @Code) -- IF x IS DISTINCT FROM y, see http://stackoverflow.com/questions/10416789/
			exec addUpdateEntryToAuditLog @CuEmployeeId, @CuDelegateId, @ElementId, null, '07623B06-1CD1-4777-91C1-104E459B8382', @PrevCode, @Code, @PrevName, null;
		if NOT EXISTS(SELECT @PrevName INTERSECT SELECT @Name)
			exec addUpdateEntryToAuditLog @CuEmployeeId, @CuDelegateId, @ElementId, null, '0B1E263D-214E-4660-83AD-2F16E8BBA252', @PrevName, @Name, @PrevName, null;

	end
	else
	begin
		insert VehicleEngineTypes
			(CreatedBy, CreatedOn, Code, Name)
		values
			(@CuEmployeeId, GETUTCDATE(), @Code, @Name);
			
		set @VehicleEngineTypeId = @@IDENTITY;

		exec addInsertEntryToAuditLog @CuEmployeeId, @CuDelegateId, @ElementId, null, @Name, null;

	end

return @VehicleEngineTypeId
