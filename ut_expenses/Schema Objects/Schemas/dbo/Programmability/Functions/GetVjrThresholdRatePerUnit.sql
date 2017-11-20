CREATE FUNCTION [dbo].[GetVjrThresholdRatePerUnit]
(
	@mileageThresholdId int,
	@engineName nvarchar(450)
)
RETURNS money AS  
BEGIN 

	declare @ratePerUnit money = 0;
	select
		@ratePerUnit = RatePerUnit
	from
		MileageThresholdRates
			join VehicleEngineTypes on VehicleEngineTypes.VehicleEngineTypeId = MileageThresholdRates.VehicleEngineTypeId
	where
		MileageThresholdId = @mileageThresholdId
		and Name = @engineName
		
	return @ratePerUnit

END

GO
