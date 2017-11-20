CREATE PROCEDURE [dbo].[deleteOdometerReading] 
	
@odometerID int,
@CUemployeeID INT,
@CUdelegateID INT

AS
BEGIN	
	declare @title1 nvarchar(500);
	select @title1 = registration from cars where carid = (select carid from odometer_readings where odometerID = @odometerID);
	declare @username nvarchar(50);
	select @username = username from employees inner join cars on cars.employeeid = employees.employeeid where carid = (select carid from odometer_readings where odometerID = @odometerID);
	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'Odometer reading for ' + @title1 + '. Employee - ' + @username);

	DELETE FROM odometer_readings WHERE odometerid = @odometerID;

	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @odometerID, @recordTitle, null;
END