CREATE PROCEDURE [dbo].[SaveVehicleLookupLog]
	@employeeid		int,
	@delegateid		int,
	@registration	nvarchar(100),
	@code			nvarchar(100),
	@message		nvarchar(100)
	
AS
	INSERT INTO VehicleLookupLog (EmployeeId, DelegateId, Registration, Code, [Message], DateTimeStamp) 
	VALUES						 (@employeeid, @delegateid, @registration, @code, @message, SYSUTCDATETIME())
RETURN 0
