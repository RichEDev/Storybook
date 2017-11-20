CREATE PROCEDURE [dbo].[APIdeleteCarVehicleJourneyRate]
	@carid int
	
AS
	DELETE FROM car_mileagecats WHERE carid = @carid;
RETURN 0
