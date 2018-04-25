CREATE PROCEDURE [dbo].[GetApiLicensing] 
AS SELECT AccountId, TotalCalls, FreeToday, HourLimit, HourRemaining, HourLast, MinuteLimit, MinuteRemaining, MinuteLast FROM [dbo].[ApiLicensing]