CREATE PROCEDURE [dbo].[GetClaimsForApproverWithCostCodeIncluded](@approverid INT)
AS
BEGIN

SELECT DISTINCT claims.claimid, claims.employeeid AS claimantid
FROM savedexpenses_costcodes 
INNER JOIN savedexpenses ON savedexpenses.expenseid = savedexpenses_costcodes.expenseid 
INNER JOIN claims ON savedexpenses.claimid = claims.claimid
INNER JOIN employees ON employees.employeeid = claims.employeeid
INNER JOIN groups ON groups.groupid = employees.groupid
INNER JOIN signoffs ON signoffs.groupid = groups.groupid
LEFT JOIN costcodes ON costcodes.costcodeid = savedexpenses_costcodes.costcodeid
LEFT JOIN teamemps ON teamemps.teamid = costcodes.OwnerTeamId
LEFT JOIN budgetholders ON budgetholders.budgetholderId = costcodes.OwnerBudgetHolderId
WHERE (OwnerEmployeeId = @approverid OR teamemps.employeeid = @approverid OR budgetholders.employeeid = @approverid)
AND signoffs.signofftype = 8 -- cost code owner

END