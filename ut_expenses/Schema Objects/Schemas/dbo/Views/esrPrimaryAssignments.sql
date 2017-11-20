CREATE VIEW [dbo].[esrPrimaryAssignments]
AS
SELECT MAX(EffectiveStartDate) AS MaxEffectiveStartDate, AssignmentID, ESROrganisationId
FROM  dbo.esr_assignments
WHERE (PrimaryAssignment = 1) AND (EffectiveEndDate IS NULL) AND (FinalAssignmentEndDate IS NULL)
GROUP BY AssignmentID, ESROrganisationId