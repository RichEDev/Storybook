CREATE PROCEDURE CUActivityHit
@employeeID int
AS
BEGIN
	UPDATE accessManagement SET lastActivity = GETDATE() WHERE employeeID = @employeeID;
END
