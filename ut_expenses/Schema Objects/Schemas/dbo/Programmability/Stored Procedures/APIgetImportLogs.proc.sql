CREATE PROCEDURE [dbo].[APIgetImportLogs]
	@logID int
	
AS
IF @logID = 0
	BEGIN
		SELECT logID, logType, logName, successfulLines, failedLines, warningLines, expectedLines, processedLines, createdOn, modifiedOn 
		FROM logNames
	END
ELSE
	BEGIN
		SELECT logID, logType, logName, successfulLines, failedLines, warningLines, expectedLines, processedLines, createdOn, modifiedOn 
		FROM logNames
		WHERE logID = @logID
	END
RETURN 0