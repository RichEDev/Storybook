CREATE PROCEDURE [dbo].[APIgetCarVehicleJourneyRatesSpecial]
	@reference NVARCHAR(10)
	
AS
	SELECT carid, mileageid FROM car_mileagecats WHERE carid = CAST(@reference AS INT);
RETURN 0
