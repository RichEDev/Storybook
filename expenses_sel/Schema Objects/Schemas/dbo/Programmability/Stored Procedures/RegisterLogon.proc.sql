CREATE PROCEDURE [dbo].[RegisterLogon]
@employeeId INT
AS
	IF NOT @employeeId IS NULL
	BEGIN
		DECLARE @datestamp datetime
		DECLARE @datestr nvarchar(25)
		SET @datestr = CAST(DATEPART(year,getdate()) AS nvarchar) + '-' + CAST(DATEPART(month,getdate()) AS nvarchar) + '-01'
		SET @datestamp = CONVERT(datetime,@datestr,120)

		IF EXISTS(SELECT [traceId] FROM logon_trace WHERE [logonPeriod] = @datestamp AND [employeeId] = @employeeId)
		BEGIN
			UPDATE logon_trace SET [count] = (SELECT [count]+1 FROM logon_trace WHERE [logonPeriod] = @datestamp AND [employeeId] = @employeeId) WHERE [logonPeriod] = @datestamp AND [employeeId] = @employeeId
		END
		ELSE
		BEGIN
			INSERT INTO logon_trace([employeeId],[logonPeriod],[count]) VALUES (@employeeId,@datestamp,1)
		END
	END
