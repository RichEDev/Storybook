CREATE FUNCTION [dbo].[GetVjrThresholdRateAmountForVat]
(
	@mileageThresholdId int,
	@engineName nvarchar(450)
)
RETURNS money AS  
BEGIN 

	declare @amountForVat money = 0;
	select
		@amountForVat = AmountForVat
	from
		MileageThresholdRates
			join VehicleEngineTypes on VehicleEngineTypes.VehicleEngineTypeId = MileageThresholdRates.VehicleEngineTypeId
	where
		MileageThresholdId = @mileageThresholdId
		and Name = @engineName
		
	return @amountForVat

END

GO
