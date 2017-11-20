CREATE FUNCTION [dbo].[getFinancialYear](@date DateTime, @financialYearStart DateTime ) 
RETURNS DateTime
AS
BEGIN

DECLARE @yearEnd DateTime;
 
DECLARE @start INT = DATEPART(yy, @date) - DATEPART(yy, @financialYearStart)
DECLARE @end INT = DATEPART(yy, @date) - DATEPART(yy, @financialYearStart)

 SET @financialYearStart = DATEADD(yy, @start, @financialYearStart)
 SET @yearEnd = DATEADD(yy, @end, @yearEnd)

 	IF @date < @financialYearStart
		RETURN DATEADD(YY, -1, @financialYearStart)

RETURN @financialYearStart

END
