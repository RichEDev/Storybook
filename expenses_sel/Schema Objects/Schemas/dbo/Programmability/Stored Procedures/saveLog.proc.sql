


--Create saveLog sp
CREATE PROCEDURE [dbo].[saveLog] (@logID int, @logType tinyint, @logName nvarchar (500), @successfulLines int, @failedLines int, @warningLines int, @expectedLines int, @processedLines int, @date DateTime)
AS
if @logID = 0
BEGIN
	insert into logNames (logType, logName, successfulLines, failedLines, warningLines, expectedLines, processedLines, createdOn) values (@logType, @logName,  @successfulLines, @failedLines, @warningLines, @expectedLines, @processedLines, @date)
	SET @logID = scope_identity()
END
else
BEGIN
	UPDATE logNames SET logType = @logType, logName = @logName, successfulLines = @successfulLines, failedLines = @failedLines, warningLines = @warningLines, expectedLines = @expectedLines, processedLines = @processedLines, modifiedOn = @date WHERE logID = @logID
END

return @logID;






