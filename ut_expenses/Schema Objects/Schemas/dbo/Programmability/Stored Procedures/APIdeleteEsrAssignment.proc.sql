CREATE PROCEDURE [dbo].[APIdeleteEsrAssignment]
	@esrAssignID int 
AS
-- Update any references for this assignment to null.

UPDATE esr_assignments SET SupervisorEsrAssignId = NULL WHERE SupervisorEsrAssignId = @esrAssignID;
UPDATE esr_assignments SET DepartmentManagerAssignmentId = null WHERE DepartmentManagerAssignmentId = (SELECT AssignmentID FROM esr_assignments WHERE esrAssignID = @esrAssignId);

DECLARE @expensecount INT;
SELECT @expensecount = COUNT(esrAssignID) from savedexpenses WHERE esrAssignID = @esrAssignID;

DECLARE @carcount INT;
SELECT @carcount = COUNT(carid) FROM CarAssignmentNumberAllocations WHERE ESRAssignId = @esrAssignID;

IF @expensecount > 0 OR @carcount > 0
	BEGIN
		UPDATE esr_assignments SET Active = 0 WHERE esrAssignID = @esrAssignID;
	END
ELSE
	BEGIN
		UPDATE ESRVehicles SET ESRAssignId = NULL where ESRAssignId = @esrAssignID;
		-- Now delete the record.
		delete from ESRAssignmentLocations where esrAssignID = @esrAssignID;
		DELETE FROM [dbo].[esr_assignments] WHERE esrAssignID = @esrAssignID;
	END