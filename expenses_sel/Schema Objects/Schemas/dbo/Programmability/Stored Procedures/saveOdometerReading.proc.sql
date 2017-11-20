

CREATE PROCEDURE [dbo].[saveOdometerReading]
	
@odometerID int,
@carid int,
@dateStamp datetime,
@oldReading int,
@newReading int,
@businessMileage bit,
@userID int,
@CUemployeeID INT,
@CUdelegateID INT

AS

BEGIN
	declare @recordTitle nvarchar(2000);
	declare @title1 nvarchar(500);
	select @title1 = registration from cars where carid = @carid;
	set @recordTitle = (select 'Odometer reading for ' + @title1);

	if @odometerID = 0
		BEGIN
			INSERT INTO odometer_readings(carid, datestamp, oldreading, newreading, businessmileage, createdon, createdby) VALUES (@carid, @dateStamp, @oldReading, @newReading, @businessMileage, getutcdate(), @userid);
			set @odometerID = scope_identity();

			exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @odometerID, @recordTitle, null;
		END
	else
		BEGIN
			declare @oldcarid int;
			declare @olddateStamp datetime;
			declare @oldoldReading int;
			declare @oldnewReading int;
			declare @oldbusinessMileage bit;
			select @oldcarid = carid, @olddateStamp = datestamp, @oldoldReading = oldreading, @oldnewReading = newreading, @oldbusinessMileage = businessmileage from odometer_readings WHERE odometerid = @odometerID;

			UPDATE odometer_readings SET carid = @carid, datestamp = @dateStamp, oldreading = @oldReading, newreading = @newReading, businessmileage = @businessMileage, modifiedon = getutcDate(), modifiedby = @userid WHERE odometerid = @odometerID;

			if @oldcarid <> @carid
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @odometerID, '193f25e6-adb2-4e6a-8ff4-f0ac6f38263b', @oldcarid, @carid, @recordtitle, null;
			if @olddateStamp <> @dateStamp
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @odometerID, '0b9c5500-3716-4c88-87d0-6d087d8657a4', @olddateStamp, @dateStamp, @recordtitle, null;
			if @oldoldReading <> @oldReading
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @odometerID, 'fd75e8f0-0317-4689-a059-77bc66c8a98a', @oldoldReading, @oldReading, @recordtitle, null;
			if @oldnewReading <> @newReading
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @odometerID, 'c063831a-af34-4b0a-af76-0966a750c8c6', @oldnewReading, @newReading, @recordtitle, null;
			if @oldbusinessMileage <> @businessMileage
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 13, @odometerID, '39556306-c07c-415f-9e67-6826c8165ae2', @oldbusinessMileage, @businessMileage, @recordtitle, null;

		END
	return @odometerID;
END


