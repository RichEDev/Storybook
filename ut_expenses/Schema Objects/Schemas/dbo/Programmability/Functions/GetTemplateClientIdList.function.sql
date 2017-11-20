CREATE FUNCTION dbo.GetTemplateClientIdList(@CPId int)
RETURNS nvarchar(500)
AS
BEGIN
DECLARE @retCSV nvarchar(500)
DECLARE @tmpCSV nvarchar(500)
DECLARE @comma nvarchar(1)
DECLARE @reId int
SET @comma = ''
SET @retCSV = ''
SET @tmpCSV = ''

DECLARE loop_cursor CURSOR FOR
SELECT [RechargeEntityId] FROM recharge_associations WHERE [ContractProductId] = @CPId
OPEN loop_cursor
FETCH NEXT FROM loop_cursor INTO @reId
WHILE @@FETCH_STATUS = 0
BEGIN
	SET @tmpCSV = @retCSV + @comma + CAST(@reId AS nvarchar)
	SET @comma = ','
	SET @retCSV = @tmpCSV
FETCH NEXT FROM loop_cursor INTO @reId
END
CLOSE loop_cursor
DEALLOCATE loop_cursor

RETURN @retCSV
END
