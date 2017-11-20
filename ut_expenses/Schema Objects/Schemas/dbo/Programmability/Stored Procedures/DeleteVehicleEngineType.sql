CREATE PROCEDURE [dbo].[DeleteVehicleEngineType]
	@VehicleEngineTypeId int,
	@CuEmployeeId int,
	@CuDelegateId int
AS

	declare @ElementId int = 187,
			@recordTitle nvarchar(max),
			@deleted int;

	select @recordTitle = Name
	from VehicleEngineTypes
	where VehicleEngineTypeId = @VehicleEngineTypeId;

	delete from
		VehicleEngineTypes
	where
		VehicleEngineTypeId = @VehicleEngineTypeId; 

	set @deleted = @@ROWCOUNT;

	if (@deleted > 0)
		exec addDeleteEntryToAuditLog @CuEmployeeId, @CuDelegateId, @ElementId, @VehicleEngineTypeId, @recordTitle, null;
	
return @deleted
