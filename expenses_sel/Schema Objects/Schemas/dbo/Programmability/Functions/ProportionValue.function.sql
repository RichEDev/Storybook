CREATE FUNCTION dbo.ProportionValue (@ACValue FLOAT, @Percentage FLOAT)
RETURNS FLOAT AS 
BEGIN
DECLARE @Share FLOAT

IF(@ACValue = 0)
BEGIN
	SET @Share = 0
END
ELSE
BEGIN
	SET @Share = (@ACValue / 100) * @Percentage
END
RETURN @Share
END
