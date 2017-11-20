CREATE PROCEDURE [dbo].[APIgetImportHistorys]
	@historyid int 
	
AS

IF @historyid = 0
	BEGIN
		SELECT historyId, importId, logId, importedDate, importStatus, applicationType, dataId, createdOn, modifiedOn  
		FROM importHistory
	END
ELSE
	BEGIN
		SELECT historyId, importId, logId, importedDate, importStatus, applicationType, dataId, createdOn, modifiedOn  
		FROM importHistory
		WHERE historyId = @historyid
	END
RETURN 0
