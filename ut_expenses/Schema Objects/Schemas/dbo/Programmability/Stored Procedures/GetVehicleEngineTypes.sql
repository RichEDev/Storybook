CREATE PROCEDURE [dbo].[GetVehicleEngineTypes]
	@VehicleEngineTypeId int = null
AS
	select
		VehicleEngineTypeId,
		CreatedBy,
		CreatedOn,
		ModifiedBy,
		ModifiedOn,
		Code,
		Name
	from
		VehicleEngineTypes
	where
		VehicleEngineTypeId = @VehicleEngineTypeId
		or @VehicleEngineTypeId = 0
		or @VehicleEngineTypeId is null
	order by
		Name
		
RETURN 0
