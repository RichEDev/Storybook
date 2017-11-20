CREATE PROCEDURE [dbo].[PopulateDateRanges]
AS
-- Delete future date ranges
	DELETE FROM range_exchangerates WHERE currencyrangeid IN (select currencyrangeid from currencyranges where startdate > GETDATE())
	DELETE FROM currencyranges where startdate > GETDATE()
-- update future end date(s) to today
	UPDATE currencyranges SET enddate = CAST(GETDATE() as date) where enddate >= CAST(GETDATE() as date)
RETURN 0
