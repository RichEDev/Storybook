
CREATE FUNCTION [dbo].[CalcNYM](@CPId int, @InflatorCalcType int, @InflatorType int, @MIXtype int, @MIXpct float, @MIXextrapct float, @MIYtype int, @MIYpct float, @MIYextrapct float, @years int)
RETURNS float
AS
BEGIN
DECLARE @resultLP float
DECLARE @curLP float
DECLARE @curMV float
DECLARE @pctLP float
DECLARE @resultX float
DECLARE @resultY float
DECLARE @XYresult float
DECLARE @NYMresult float

SET @curLP = (SELECT [productValue] FROM contract_productdetails WHERE [contractProductId] = @CPId)
SET @pctLP = (SELECT ISNULL([maintenancePercent],0) FROM contract_productdetails WHERE [contractProductId] = @CPId)
SET @curMV = (SELECT ISNULL([maintenanceValue],0) FROM contract_productdetails WHERE [contractProductId] = @CPId)

SET @XYresult = @curMV

WHILE @years >= 0
BEGIN
	IF(@curLP > 0 AND @pctLP > 0)
	BEGIN
		SET @resultLP = (@curLP*(@pctLP/100))
	END
	ELSE
	BEGIN
		SET @resultLP = 0
	END
	
	SET @resultX = @XYresult + (@XYresult * ((@MIXpct + @MIXextrapct) / 100))
	SET @resultY = @XYresult + (@XYresult * ((@MIYpct + @MIYextrapct) / 100))
	
	IF @InflatorType = 0 -- Single Inflator (x only)
	BEGIN
		SET @XYresult = @resultX
	END
	ELSE IF @InflatorType = 1 -- Greater of X and Y
	BEGIN
		IF(@resultX >= @resultY)
		BEGIN
			SET @XYresult = @resultX
		END
		ELSE
		BEGIN
			SET @XYresult = @resultY
		END
	END
	ELSE -- Lesser of X and Y
	BEGIN
		IF(@resultX <= @resultY)
		BEGIN
			SET @XYresult = @resultX
		END
		ELSE
		BEGIN
			SET @XYresult = @resultY
		END
	END

	SET @years = @years-1
END

IF @InflatorCalcType = 0 -- Prod £ v Inflator
BEGIN	
	IF((@resultLP <= @XYresult) AND (@resultLP <> 0))
	BEGIN
		SET @NYMresult = @resultLP
	END
	ELSE
	BEGIN
		SET @NYMresult = @XYresult
	END
END
ELSE IF	@InflatorCalcType = 1 -- Inflator only
BEGIN
	SET @NYMresult = @XYresult
END
ELSE
BEGIN
	SET @NYMresult = 0
END
RETURN @NYMresult
END
