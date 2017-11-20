CREATE PROCEDURE [dbo].[GetMileageThresholdRates]
	@MileageThresholdRateId int = null
AS
	select
		MileageThresholdRates.MileageThresholdRateId,
		MileageThresholdRates.CreatedBy,
		MileageThresholdRates.CreatedOn,
		MileageThresholdRates.ModifiedBy,
		MileageThresholdRates.ModifiedOn,
		MileageThresholdRates.MileageThresholdId,
		MileageThresholdRates.VehicleEngineTypeId,
		MileageThresholdRates.RatePerUnit,
		MileageThresholdRates.AmountForVat
	from
		MileageThresholdRates
			join VehicleEngineTypes on VehicleEngineTypes.VehicleEngineTypeId = MileageThresholdRates.VehicleEngineTypeId
	where
		MileageThresholdRates.MileageThresholdRateId = @MileageThresholdRateId
		or @MileageThresholdRateId = 0
		or @MileageThresholdRateId is null
	order by
		MileageThresholdRates.MileageThresholdId,
		VehicleEngineTypes.Name
		
RETURN 0
