CREATE FUNCTION [dbo].[IsInSupport](@RA_Id int, @current_date datetime) 
RETURNS bit
AS
BEGIN
DECLARE @retVal bit
DECLARE @SEDate datetime
DECLARE @SSDate datetime

SET @retVal = 0	
SET @SSDate = (SELECT ISNULL([supportStartDate],CONVERT(datetime,'1970-01-01',120)) FROM [recharge_associations] WHERE [rechargeId] = @RA_Id)
SET @SEDate = (SELECT ISNULL([supportEndDate],CONVERT(datetime,'2100-12-31',120)) FROM [recharge_associations] WHERE [rechargeId] = @RA_Id)

IF @current_date >= @SSDate
BEGIN
	IF @current_date < @SEDate
	BEGIN
		SET @retVal = 1
	END
END

RETURN @retVal
END
