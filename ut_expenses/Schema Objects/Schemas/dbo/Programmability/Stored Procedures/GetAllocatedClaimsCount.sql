CREATE PROCEDURE [dbo].[GetAllocatedClaimsCount] @claimId INT
 ,@employeeId INT
AS
BEGIN
 SELECT count(*)
 FROM unallocatedclaims
 WHERE claimid = @claimId
  AND (
		(
			(
				teamemployeeid = @employeeId
				AND costcodeteamemployeeid IS NULL
				AND itemCheckerTeamEmployeeId IS NULL
				)
			OR (
				teamemployeeid IS NULL
				AND costcodeteamemployeeid = @employeeId
				AND itemcheckerid IS NULL
				)
			OR (
				teamemployeeid IS NULL
				AND itemCheckerTeamEmployeeId = @employeeId
				AND itemcheckerid IS NULL
				)
			)
		OR (
			teamemployeeid IS NULL
			AND itemcheckerid IS NULL
			AND teamemployeeid IS NULL
			AND costcodeteamemployeeid IS NULL
			AND itemCheckerTeamEmployeeId IS NULL
			)
		)
  
END