CREATE PROCEDURE [dbo].[GetCostCodeHierarchy] @employeeid INT
	,@subAccountId INT
AS

DECLARE @hierarchy TABLE (
	employeeId INT NOT NULL
	,checked BIT DEFAULT(0)
	,PRIMARY KEY CLUSTERED ([employeeid] ASC) WITH (ignore_dup_key = ON)
	)

-- employees should be able to see their own records
INSERT INTO @hierarchy (employeeid)
VALUES (@employeeid)

--Get the employees that have a cost code where your employee is the owner
INSERT INTO @hierarchy (employeeid)
SELECT employeeid
FROM employee_costcodes
INNER JOIN costcodes ON employee_costcodes.costcodeid = costcodes.costcodeid
WHERE OwnerEmployeeId = @employeeid AND archived = 0

DECLARE @costcodeids TABLE (
	costcodeid INT
	,PRIMARY KEY CLUSTERED ([costcodeid] ASC) WITH (ignore_dup_key = ON)
	);

-- Get the teams where your employee is a member and then find the cost codes with your team as the owner
DECLARE @teamIds TABLE (teamId INT NOT NULL)

INSERT INTO @teamIds
SELECT teamid
FROM teamemps
WHERE employeeid = @employeeid

INSERT INTO @costcodeids
SELECT costcodeid
FROM costcodes
WHERE ownerteamid IN (
		SELECT teamid
		FROM @teamIds
		)
		AND archived = 0

-- Get the budget holder ids where your employee is the budget holder and then find the cost codes with your budget holder as the owner
DECLARE @budgetHolderIds TABLE (budgetHolderId INT NOT NULL)

INSERT INTO @budgetHolderIds
SELECT budgetHolderId
FROM budgetholders
WHERE employeeid = @employeeid

INSERT INTO @costcodeids
SELECT costcodeid
FROM costcodes
WHERE ownerBudgetHolderId IN (
		SELECT budgetHolderId
		FROM @budgetHolderIds
		)
		AND archived = 0

-- Get the employees that have a cost code where your employee is part of the team or is the budget holder that owns the cost code
INSERT INTO @hierarchy (employeeid)
SELECT employeeid
FROM employee_costcodes
WHERE costcodeid IN (
		SELECT costcodeid
		FROM @costcodeids
		)

-- Check if the employee is also the default cost code owner and get all the cost codes that don''t have an owner or are archived 
-- and all the employees that don''t have a cost code owner assigned to them
DECLARE @DefaultCostCodeOwnerId INT
DECLARE @DefaultCostCodeOwnerType INT

EXEC GetDefaultCostCodeOwner @subAccountId
	,@DefaultCostCodeOwnerType OUTPUT
	,@DefaultCostCodeOwnerId OUTPUT

IF (
		@DefaultCostCodeOwnerType = 25
		AND @DefaultCostCodeOwnerId = @employeeid
		)
	OR (
		@DefaultCostCodeOwnerType = 49
		AND EXISTS (
			SELECT teamId
			FROM @teamIds
			WHERE teamId = @DefaultCostCodeOwnerId
			)
		)
	OR (
		@DefaultCostCodeOwnerType = 11
		AND EXISTS (
			SELECT budgetHolderId
			FROM @budgetHolderIds
			WHERE budgetHolderId = @DefaultCostCodeOwnerId
			)
		)
BEGIN
	INSERT INTO @hierarchy (employeeid)
	SELECT employeeid
	FROM employee_costcodes
	INNER JOIN costcodes ON employee_costcodes.costcodeid = costcodes.costcodeid
	WHERE OwnerEmployeeId IS NULL
		AND OwnerTeamId IS NULL
		AND OwnerBudgetHolderId IS NULL
		OR archived = 1

	INSERT INTO @hierarchy (employeeid)
	SELECT employeeid
	FROM employees
	WHERE employeeid NOT IN (
			SELECT employeeid
			FROM employee_costcodes
			)

	INSERT INTO @hierarchy (employeeid)
	SELECT employeeid
	FROM employee_costcodes
	WHERE costcodeid IS NULL

END

SELECT *
FROM @hierarchy