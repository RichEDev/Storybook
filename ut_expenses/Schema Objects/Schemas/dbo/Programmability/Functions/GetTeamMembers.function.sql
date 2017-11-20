CREATE FUNCTION [dbo].[GetTeamMembers] (@teamID INT)  
RETURNS varchar(200) AS  
BEGIN 
DECLARE @member VARCHAR(100)
DECLARE @memberList VARCHAR(200)
DECLARE @temp_memberList VARCHAR(200)
DECLARE @scolon VARCHAR(2)
SET @scolon = ''
SET @memberList = ''
SET @temp_memberList = ''

DECLARE loop_cursor CURSOR FOR
SELECT firstname + ' ' + surname FROM employees WHERE employeeid IN (SELECT [employeeid] FROM [teamemps] WHERE [teamId] = @teamID)
OPEN loop_cursor
FETCH NEXT FROM loop_cursor INTO @member
WHILE @@FETCH_STATUS = 0
BEGIN
	SET @temp_memberList = @memberList
	SET @memberList = @temp_memberList + @scolon + RTRIM(LTRIM(@member))
	SET @scolon = '; '
	FETCH NEXT FROM loop_cursor INTO @member
END
CLOSE loop_cursor
DEALLOCATE loop_cursor

RETURN(@memberList)
END
