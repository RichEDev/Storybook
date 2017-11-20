CREATE FUNCTION [dbo].[FormatDateRangeTitle] 
(
	@dateRangeId int,
	@thresholdId int
)
RETURNS nvarchar(1000)
AS
BEGIN

DECLARE @dateRangeType nvarchar(100);
DECLARE @dateRangeTypeText nvarchar(1000);
DECLARE @dateValue1 nvarchar(10);
DECLARE @dateValue2 nvarchar(10);

DECLARE @rangeType nvarchar(100) = '';
DECLARE @rangeTypeText nvarchar(1000);
DECLARE @value1 nvarchar(10);
DECLARE @value2 nvarchar(10);

select @dateRangeType =  (case  
WHEN daterangetype = 0  THEN 'Before' 
when daterangetype= 1 THEN 'After Or Equal To' 
when daterangetype= 2 THEN 'Between' 
when daterangetype= 3 THEN 'Any' 
END) ,
@dateValue1 =  convert(varchar(10),datevalue1,103) ,
@dateValue2 =  convert(varchar(10),datevalue2,103) 
from mileage_dateranges
WHERE mileagedateid = @dateRangeId

select  @rangeType =  (case WHEN rangetype = 0  THEN 'Greater than or Equal To' 
when rangetype= 1 THEN 'Between' 
when rangetype= 2 THEN 'Less than' 
when rangetype= 3 THEN 'Any' 
END),
@value1 =  rangevalue1 ,
@value2 = rangevalue2
from mileage_thresholds
where mileagethresholdid = @thresholdId

SET @rangeTypetext = ''

IF @dateRangeType = 'Any'
BEGIN
	SET @dateRangeTypetext =  @dateRangeType
END
ELSE
BEGIN
	IF @dateRangeType = 'Between'
	BEGIN
		SET @dateRangeTypetext =  @dateRangeType + ' ' + @dateValue1 + ' and ' + @dateValue2
	END
	ELSE
	BEGIN
		SET @dateRangeTypetext =  @dateRangeType + ' ' + @dateValue1 
	END
END

IF @rangeType <> ''
BEGIN
	IF @rangeType = 'Any'
	BEGIN
		SET @rangeTypeText = ' Threshold ' + @rangeType
	END
	ELSE
	BEGIN
		IF @rangeType = 'Between'
		BEGIN
			SET @rangeTypeText = ' Threshold ' + @rangeType + ' ' + @value1 + ' and ' + @value2
		END
		ELSE
		BEGIN
			SET @rangeTypeText = ' Threshold ' + @rangeType + ' ' + @value1 
		END
	END
END

SET @dateRangeTypeText = @dateRangeTypetext + ' ' + @rangeTypeText;

return @dateRangeTypeText
END