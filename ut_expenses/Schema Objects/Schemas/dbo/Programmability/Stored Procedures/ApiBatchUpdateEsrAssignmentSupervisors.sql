CREATE PROCEDURE [dbo].[ApiBatchUpdateEsrAssignmentSupervisors] 
AS
BEGIN
UPDATE ea
SET ea.SupervisorEsrAssignID = sa.esrAssignId
FROM dbo.esr_assignments ea
INNER JOIN dbo.esr_assignments sa ON ea.SupervisorAssignmentId = sa.AssignmentID
WHERE (ea.SupervisorEsrAssignId IS NULL)
	AND (ea.SupervisorAssignmentId IS NOT NULL)
	RETURN 0;
END
