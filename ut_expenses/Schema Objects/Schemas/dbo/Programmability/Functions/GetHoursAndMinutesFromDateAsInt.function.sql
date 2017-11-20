CREATE FUNCTION [dbo].[GetHoursAndMinutesFromDateAsInt]
(
	@thedate datetime
)
RETURNS int
AS

BEGIN
	DECLARE @hoursandmins int;
	SET @hoursandmins = (select cast(cast(DATEPART(hh, @thedate) as nvarchar) + cast(DATEPART(mi, @thedate) as nvarchar) as int));
	RETURN @hoursandmins;
END