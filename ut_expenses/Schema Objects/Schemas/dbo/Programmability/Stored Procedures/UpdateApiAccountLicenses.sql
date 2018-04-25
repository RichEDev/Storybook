CREATE PROCEDURE [dbo].[UpdateApiAccountLicenses] 
( 
 @accountId [int],
 @totalCalls [int],
 @freeToday [int],
 @hourLimit  [int],
 @hourRemaining  [int],
 @hourLast [datetime],
 @minuteLimit  [int],
 @minuteRemaining [int],
 @minuteLast [datetime]
)
AS
BEGIN
 SET NOCOUNT ON;
 UPDATE [dbo].[ApiLicensing]
 SET 
 TotalCalls = @totalCalls,
 FreeToday = @freeToday,
 HourLimit = @hourLimit,
 HourRemaining = @hourRemaining,
 HourLast = @hourLast,
 MinuteLimit = @minuteLimit,
 MinuteRemaining = @minuteRemaining,
 MinuteLast = @minuteLast 
 WHERE AccountId = @accountId;
END