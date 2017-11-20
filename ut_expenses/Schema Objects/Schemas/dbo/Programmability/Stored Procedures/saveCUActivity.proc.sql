CREATE PROCEDURE saveCUActivity
@manageID uniqueidentifier,
@employeeID int
AS
BEGIN
-- Clear out any concurrent users that have a last activity date > 1 hour
DELETE FROM accessManagement WHERE lastActivity <= DATEADD(mi, -60, GETDATE());

IF EXISTS (select manageID from accessManagement where manageID = @manageID)
BEGIN
	UPDATE accessManagement SET lastActivity = GETDATE() WHERE manageID = @manageID;
END
ELSE
BEGIN
	INSERT INTO accessManagement (manageID, employeeID, lastActivity) VALUES (@manageID, @employeeID, GETDATE());
END

RETURN;
END
