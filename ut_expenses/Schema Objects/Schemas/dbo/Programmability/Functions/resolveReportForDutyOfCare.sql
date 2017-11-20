CREATE FUNCTION [dbo].[resolveReportForDutyOfCare] (@employeeid int) RETURNS @employees TABLE ([employeeid] INT) AS
BEGIN
	DECLARE @empid int
	DECLARE @documentReviewer NVARCHAR(100)

	SELECT @documentReviewer = stringValue FROM accountProperties WHERE stringKey='dutyOfCareApprover'

	IF (@documentReviewer = 'Team')
	BEGIN
		DECLARE @teamId INT
		SELECT @teamId = CAST(stringValue AS INT) from accountProperties where stringKey='dutyOfCareTeamAsApprover'

		IF EXISTS (SELECT 1 FROM teamemps WHERE teamid = @teamid AND employeeid = @employeeid)
		BEGIN
			INSERT INTO @employees SELECT employeeid FROM employees
		END
	END

	ELSE
	BEGIN
	WITH DirectReports (ManagerID, EmployeeID, Title,  Level)
		AS
		(
		-- Anchor member definition
			SELECT e.linemanager, e.EmployeeID, e.Title,  0 AS Level
			FROM dbo.Employees AS e

			WHERE 
			employeeid = @employeeid
			UNION ALL
		-- Recursive member definition
			SELECT e.linemanager, e.EmployeeID, e.Title,         Level + 1
			FROM dbo.Employees AS e
   
			INNER JOIN DirectReports AS d
				ON e.linemanager = d.EmployeeID
		)
-- Statement that executes the CTE
		 INSERT INTO @employees SELECT EmployeeID FROM DirectReports
	END

	RETURN

END