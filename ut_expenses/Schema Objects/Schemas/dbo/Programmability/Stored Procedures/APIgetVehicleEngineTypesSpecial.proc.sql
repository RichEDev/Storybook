CREATE PROCEDURE [dbo].[APIgetVehicleEngineTypesSpecial]
	@reference nvarchar(50)
AS
	select
		VehicleEngineTypeId,
		CreatedOn,
		CreatedBy,
		ModifiedOn,
		ModifiedBy,
		Code,
		Name
	from
		VehicleEngineTypes
	where
		Code = @reference

RETURN 0
