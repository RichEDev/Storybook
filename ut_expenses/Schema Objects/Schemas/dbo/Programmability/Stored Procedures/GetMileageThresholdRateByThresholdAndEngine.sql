CREATE PROCEDURE [dbo].[GetMileageThresholdRateByThresholdAndEngine]
	@MileageThresholdId int,
	@VehicleEngineTypeId int
AS
	select
		MileageThresholdRateId,
		CreatedBy,
		CreatedOn,
		ModifiedBy,
		ModifiedOn,
		MileageThresholdId,
		VehicleEngineTypeId,
		RatePerUnit,
		AmountForVat
	from
		MileageThresholdRates
	where
		MileageThresholdId = @MileageThresholdId
		and VehicleEngineTypeId = @VehicleEngineTypeId
		
RETURN 0
