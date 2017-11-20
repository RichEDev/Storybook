CREATE FUNCTION [dbo].[GetNotifyRecipients] (@contractId INT, @locationId INT)  
RETURNS @outTable TABLE ([employeeId] int,[memberName] nvarchar(100), [email] nvarchar(250))
BEGIN 
DECLARE @resTable TABLE ([employeeId] int,[memberName] nvarchar(100), [email] nvarchar(250))
DECLARE @NotifyId INT
DECLARE @Is_A_Team INT
DECLARE loop_cursor CURSOR FOR
SELECT [employeeId],[IsTeam] FROM [contract_notification] WHERE [contractId] = @contractId
OPEN loop_cursor
FETCH NEXT FROM loop_cursor INTO @NotifyId, @Is_A_Team
WHILE @@FETCH_STATUS = 0
	BEGIN
		IF(@Is_A_Team = 0)
		BEGIN
			-- Must be an individual
			INSERT @resTable
			SELECT [employeeid],[firstname] + ' ' + [surname],[Email] FROM employees WHERE [employeeid] = @NotifyId --AND [Location Id] = @locationId
		END
		ELSE
		BEGIN
			-- Must be a team of people
			INSERT @resTable
			SELECT employees.employeeid,employees.[firstname] + ' ' + employees.[surname],employees.email FROM [teamemps] 
			INNER JOIN employees ON employees.employeeid = [teamemps].[employeeId]
			WHERE [teamId] = @NotifyId
		END
	FETCH NEXT FROM loop_cursor INTO @NotifyId, @Is_A_Team
	END
	
	CLOSE loop_cursor
	DEALLOCATE loop_cursor
	INSERT @outTable
	SELECT [employeeId],[memberName],[Email] FROM @resTable
	RETURN
END

