CREATE FUNCTION [dbo].[GetNotifyString] (@contractID INT)  
RETURNS varchar(200) AS  
BEGIN 
DECLARE @IsATeam INT
DECLARE @memberID INT
DECLARE @member VARCHAR(100)
DECLARE @memberList VARCHAR(200)
DECLARE @temp_memberList VARCHAR(200)
DECLARE @scolon VARCHAR(2)
DECLARE @typeTag VARCHAR(2)

SET @scolon = ''
SET @memberList = ''
SET @temp_memberList = ''
SET @typeTag = ''

DECLARE loop_cursor CURSOR FOR
SELECT [employeeId],[IsTeam] FROM [contract_notification] WHERE [contractId] = @contractID
OPEN loop_cursor
FETCH NEXT FROM loop_cursor INTO @memberID,@IsATeam
WHILE @@FETCH_STATUS = 0
BEGIN
	SET @temp_memberList = @memberList
	IF(@IsATeam = 1)
	BEGIN
		SET @member = (SELECT [teamName] FROM [teams] WHERE [teamId] = @memberID)
		SET @typeTag = '**'
	END
	ELSE
	BEGIN
		SET @member = (SELECT firstname + ' ' + surname AS [Full Name] FROM employees WHERE employeeId = @memberID)
		SET @typeTag = ''
	END

	SET @memberList = @temp_memberList + @scolon + @typeTag + RTRIM(LTRIM(@member))
	SET @scolon = '; '

	FETCH NEXT FROM loop_cursor INTO @memberID,@IsATeam
END
CLOSE loop_cursor
DEALLOCATE loop_cursor

IF @memberList = ''
BEGIN
	SET @memberList = NULL;
END

RETURN(@memberList)
END
