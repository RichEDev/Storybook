CREATE FUNCTION [dbo].[CalcRechargeValue](@currentRechargeDate datetime, @RA_Id int)
RETURNS float
BEGIN
--incoming params
DECLARE @SEDate datetime
DECLARE @SSDate datetime
DECLARE @WEDate datetime
DECLARE @curDate datetime

DECLARE @endofmonthdate datetime
DECLARE @startofmonthdate datetime
DECLARE @numDaysInMonth int
DECLARE @hasSSD tinyint
DECLARE @hasSED tinyint
DECLARE @hasWED tinyint
DECLARE @curValue float
DECLARE @tmpStr nvarchar(100)

SET @SSDate = (SELECT ISNULL([supportStartDate],CONVERT(datetime,'1970-01-01',120)) FROM [recharge_associations] WHERE [rechargeId] = @RA_Id)
SET @SEDate = (SELECT ISNULL([supportEndDate],CONVERT(datetime,'2100-12-31',120)) FROM [recharge_associations] WHERE [rechargeId] = @RA_Id)
SET @WEDate = (SELECT ISNULL([warrantyEndDate],CONVERT(datetime,'2100-12-31',120)) FROM [recharge_associations] WHERE [rechargeId] = @RA_Id)

IF(CONVERT(datetime,@SEDate,120) < CONVERT(datetime,'2100-12-31',120))
BEGIN
	SET @hasSED = 1
END

IF(CONVERT(datetime,@SSDate,120) > CONVERT(datetime,'1970-01-01',120))
BEGIN
	SET @hasSSD = 1
END

IF(CONVERT(datetime,@WEDate,120) < CONVERT(datetime,'2100-12-31',120))
BEGIN
	SET @hasWED = 1
END

SET @numDaysInMonth = 
	CASE DATEPART(month,@currentRechargeDate)
	WHEN 2 THEN 
		CASE DATEPART(year,@currentRechargeDate) % 4
		WHEN 0 THEN 29
		ELSE 28
		END
	WHEN 4 THEN 30
	WHEN 6 THEN 30
	WHEN 9 THEN 30
	WHEN 11 THEN 30
	ELSE 31
	END

SET @tmpStr = CAST(DATEPART(year,@currentRechargeDate) as nvarchar) + '-' + CAST(DATEPART(month,@currentRechargeDate) as nvarchar) + '-' + CAST(@numDaysInMonth AS nvarchar)
SET @endofmonthdate = CONVERT(datetime, @tmpStr, 120)
SET @tmpStr = CAST(DATEPART(year,@currentRechargeDate) AS nvarchar) + '-' + CAST(DATEPART(month,@currentRechargeDate) AS nvarchar) + '-01'
SET @startofmonthdate = CONVERT(datetime, @tmpStr, 120)

IF @hasSSD = 1 AND (DATEPART(month,@SSDate) = DATEPART(month,@currentRechargeDate) AND DATEPART(year,@SSDate) = DATEPART(year,@currentRechargeDate))
BEGIN
	SET @startofmonthdate = @SSDate
END

IF @hasSED = 1 AND (DATEPART(month,@SEDate) = DATEPART(month,@currentRechargeDate) AND DATEPART(year,@SEDate) = DATEPART(year,@currentRechargeDate))
BEGIN
	SET @endofmonthdate = @SEDate
END

DECLARE @day int
SET @day = DATEPART(day,@startofmonthdate)
DECLARE @numStandardDays int
DECLARE @numPWDays int
SET @numStandardDays = 0
SET @numPWDays = 0

IF((@currentRechargeDate > @SEDate) OR (@currentRechargeDate < @SSDate))
BEGIN
    -- current date doesn't fall within the suppORt periods
	SET @curValue = 0
END
ELSE
BEGIN
	WHILE @day <= @numDaysInMonth
	BEGIN
		SET @tmpStr = (CAST(DATEPART(year,@currentRechargeDate) AS nvarchar) + '-' + CAST(DATEPART(month,@currentRechargeDate) AS nvarchar) + '-' + CAST(@day AS nvarchar))
		SET @curDate = CONVERT(datetime,@tmpStr,120)

		IF @curDate <= @SEDate
		BEGIN
			IF dbo.RechargeItemIsOffline(@RA_Id, @curDate) = 0
			BEGIN
				IF @curDate > @WEDate
				BEGIN
					-- Is after WED
					IF @curDate <= @endofmonthdate
					BEGIN
						-- Is before SED but after WED
						SET @numPWDays = (@numPWDays + 1)
					END
				END
				ELSE
				BEGIN
					-- Is before WED & SED
					IF @curDate >= @startofmonthdate
					BEGIN
						-- Is after SSD
						SET @numStandardDays = (@numStandardDays + 1)
					END
				END
			END
		END
		SET @day = (@day + 1)
	END
END

DECLARE @StandardCharge float
DECLARE @PWCharge float

SET @StandardCharge = dbo.GetBasicCharge(@RA_Id, @currentRechargeDate, @numStandardDays)
SET @PWCharge = dbo.GetPostWarrantyCharge(@RA_Id, @currentRechargeDate, @numPWDays)

SET @curValue = (@StandardCharge + @PWCharge)

RETURN @curValue
END
