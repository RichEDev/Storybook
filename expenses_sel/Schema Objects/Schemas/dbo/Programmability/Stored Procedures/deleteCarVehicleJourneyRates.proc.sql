CREATE PROCEDURE [dbo].[deleteCarVehicleJourneyRates]
	@carID int,
	@CUemployeeID INT,
	@CUdelegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @carName nvarchar(MAX);
	declare @recordTitle nvarchar(MAX);
	
	set @carName = '';
	set @recordTitle = '';
	
	set @carName = (select make + ' ' + model + ', ' + registration from cars where carid = @carID);
	set @recordTitle = (select 'Car (' + @carName + ')''s Vehicle Journey Rates have been cleared');

	delete from car_mileagecats where carid = @carID;
            
	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @carID, @recordTitle, NULL;

	return 0;
END
