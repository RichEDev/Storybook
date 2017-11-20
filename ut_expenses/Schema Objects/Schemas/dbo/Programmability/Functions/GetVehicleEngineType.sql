CREATE FUNCTION GetVehicleEngineType
(
	@EngineTypeId int
)
RETURNS nvarchar(50)
AS
BEGIN
	DECLARE @EngineType nvarchar(50);
	SELECT @engineType = Name FROM VehicleEngineTypes WHERE VehicleEngineTypeId = @EngineTypeId
	RETURN @EngineType;

END