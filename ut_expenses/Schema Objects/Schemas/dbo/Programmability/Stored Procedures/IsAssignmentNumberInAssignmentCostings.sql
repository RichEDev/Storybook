CREATE PROCEDURE [dbo].[IsAssignmentNumberInAssignmentCostings] 
@assignmentNumber NVARCHAR(20)
,@costcode NVARCHAR(20)
AS
DECLARE @count INT

SELECT @count = count(dbo.ESRAssignmentCostings.CostCentre)
FROM dbo.esr_assignments
RIGHT OUTER JOIN dbo.ESRAssignmentCostings ON dbo.esr_assignments.AssignmentID = dbo.ESRAssignmentCostings.ESRAssignmentId
WHERE esr_assignments.AssignmentNumber = @assignmentNumber
	AND ESRAssignmentCostings.CostCentre = @costcode
GROUP BY dbo.ESRAssignmentCostings.CostCentre
	,dbo.ESRAssignmentCostings.ESRAssignmentId

IF @count > 0
	RETURN 1
ELSE
	RETURN 0