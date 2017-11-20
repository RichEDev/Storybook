
CREATE PROCEDURE [dbo].[APIupdateCostCodes]
AS
IF EXISTS (
		SELECT stringkey
		FROM dbo.accountproperties
		WHERE stringkey = 'updateCostCodesWithPrimaryAssignment'
			AND stringValue = 1
		)
BEGIN
	DECLARE @cpId INT
	DECLARE @costcodeCentre NVARCHAR(100)
	DECLARE @AssignmentCostcodeCentre NVARCHAR(100)
	DECLARE @ESROrgCostcodeCentre NVARCHAR(100)

	DECLARE assignmentCostingloop_cursor CURSOR FAST_FORWARD
	FOR
	SELECT dbo.ESRAssignmentCostings.ESRPersonId
		,dbo.ESROrganisations.DefaultCostCentre
		,dbo.ESRAssignmentCostings.CostCentre
	FROM dbo.ESRAssignmentCostings
	INNER JOIN dbo.esrPrimaryAssignments ON dbo.ESRAssignmentCostings.ESRAssignmentId = dbo.esrPrimaryAssignments.AssignmentID
	INNER JOIN dbo.ESROrganisations ON dbo.esrPrimaryAssignments.ESROrganisationId = dbo.ESROrganisations.ESROrganisationId

	OPEN assignmentCostingloop_cursor

	FETCH NEXT
	FROM assignmentCostingloop_cursor
	INTO @cpId
		,@ESROrgCostcodeCentre
		,@AssignmentCostcodeCentre

	WHILE @@FETCH_STATUS = 0
	BEGIN
	
		DECLARE @empId INT;

		SELECT @empId = employeeId
		FROM employees
		WHERE ESRPersonId = @cpId

		IF LTRIM(RTRIM(@AssignmentCostcodeCentre)) = ''
			SET @costcodeCentre = @ESROrgCostcodeCentre
		ELSE
			SET @costcodeCentre = @AssignmentCostcodeCentre
		
	    DECLARE @costcodeId INT = NULL;
				
		SELECT @costcodeId = costcodeId
		FROM costcodes
		WHERE costcode = @costcodeCentre
		GROUP BY costcodeid

		IF (@costcodeId is NULL)
			-- no costcode record exists for costcode centre, so create one 
		BEGIN
			INSERT INTO [costcodes] (
				costcode
				,[description]
				,archived
				,CreatedOn
				)
			VALUES (
				@costcodeCentre
				,@costcodeCentre
				,0
				,GETDATE()
				);

			SET @costcodeId = SCOPE_IDENTITY();
		END

		DECLARE @employeeCount INT;

		SELECT @employeeCount = COUNT(employeeId)
		FROM employee_costcodes
		WHERE employeeid = @empId

		IF @employeeCount = 1
			--employee has costcode set, so update 
			UPDATE employee_costcodes
			SET costcodeid = @costcodeId
				,percentused = 100
			WHERE employeeid = @empId
		ELSE
			--employee has no costcode set, so insert.
			INSERT INTO employee_costcodes
			VALUES (
				@empId
				,NULL
				,@costcodeId
				,NULL
				,100
				)

		FETCH NEXT
		FROM assignmentCostingloop_cursor
		INTO @cpId
			,@ESROrgCostcodeCentre
			,@AssignmentCostcodeCentre
	END

	CLOSE assignmentCostingloop_cursor

	DEALLOCATE assignmentCostingloop_cursor
END