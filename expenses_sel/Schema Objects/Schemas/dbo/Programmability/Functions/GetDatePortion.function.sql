
CREATE FUNCTION [dbo].[GetDatePortion]
(
	@Date datetime
)
RETURNS datetime
AS
BEGIN
	DECLARE @ConvertedDate datetime;
	DECLARE @stringDate nvarchar(20);
	set @stringDate = cast(DATEPART(YEAR, GETDATE()) as nvarchar) + '-' + cast(DATEPART(MONTH, GETDATE()) as nvarchar) + '-' + cast(DATEPART(DAY, GETDATE()) as nvarchar);
	SET @ConvertedDate = Convert(datetime, @stringDate, 120);
	return @ConvertedDate
END
