CREATE PROCEDURE [dbo].[CreateSubAccountParams](@subAccId int, @employeeId int) 
AS
DECLARE @param_name NVARCHAR(150)
DECLARE @param_value NVARCHAR(MAX)

DECLARE @saId INT
DECLARE @rec_count INT
DECLARE loop_cursor CURSOR FOR
SELECT [subAccountId] FROM [accountsSubAccounts] WHERE [subAccountId] <> @subAccId
OPEN loop_cursor
FETCH NEXT FROM loop_cursor INTO @saId
WHILE @@FETCH_STATUS = 0
BEGIN

DECLARE param_cursor CURSOR FOR
SELECT stringKey, stringValue FROM accountProperties WHERE subAccountID = @subAccId
OPEN param_cursor
FETCH NEXT FROM param_cursor INTO @param_name,@param_value
WHILE @@FETCH_STATUS = 0
BEGIN
	SET @rec_count = (SELECT COUNT(*) AS [Cnt] FROM accountProperties WHERE [subAccountID] = @saId AND [stringKey] = @param_name)
	IF(@rec_count = 0)
	BEGIN
		INSERT INTO accountProperties (subAccountID, stringKey, stringValue, createdOn, createdBy) VALUES (@saId,@param_name,@param_value,GETUTCDATE(),@employeeId)
	END

	FETCH NEXT FROM param_cursor INTO @param_name,@param_value
END
CLOSE param_cursor
DEALLOCATE param_cursor
FETCH NEXT FROM loop_cursor INTO @saId
END

CLOSE loop_cursor
DEALLOCATE loop_cursor
