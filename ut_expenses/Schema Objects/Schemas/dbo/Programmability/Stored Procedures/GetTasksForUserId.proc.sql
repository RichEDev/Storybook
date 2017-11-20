CREATE PROCEDURE [dbo].[GetTasksForUserId] 
	-- Add the parameters for the stored procedure here
	@taskOwnerType1 tinyint, 
	@taskOwnerType2 tinyint,
	@employeeID int,
	@subAccountID int = -1,
	@regardingArea int
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	
	IF @subAccountID > -1
		BEGIN
			SELECT taskID FROM tasks 
			WHERE
			(
				(
					taskOwnerType = @taskOwnerType1 
					AND 
					taskOwnerId IN 
					(
						SELECT teams.teamid FROM teams 
						INNER JOIN teamemps ON teamemps.[teamid] = teams.teamid 
						WHERE employeeid = @employeeID
					)
				) 
				or 
				(
					taskOwnerType = @taskOwnerType2 
					and
					taskOwnerID = @employeeID
				)
			)
			AND
			(
				(
					subAccountId IS NULL 
					AND 
					regardingArea = @regardingArea
				) 
				OR 
				subAccountId = @subAccountID
			)
			order by dueDate
		END
	ELSE
		BEGIN
			SELECT taskID FROM tasks 
			WHERE
			(
				(
					taskOwnerType = @taskOwnerType1 
					AND 
					taskOwnerId IN 
					(
						SELECT teams.teamid FROM teams 
						INNER JOIN teamemps ON teamemps.[teamid] = teams.teamid 
						WHERE employeeid = @employeeID
					)
				) 
				or 
				(
					taskOwnerType = @taskOwnerType2 
					and
					taskOwnerID = @employeeID
				)
			)
			order by dueDate
		END
	
END
