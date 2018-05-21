CREATE PROCEDURE [dbo].[SaveFinancialExportHistory] @financialExportID int, @exportNum int, @employeeID int = NULL, @dateExported datetime, @exportType INT, @historyIDs IntPK READONLY
AS
BEGIN
	DECLARE @exportHistoryID INT;
	SET @exportHistoryID = 0;
	
	-- Create the main history record
    INSERT INTO exporthistory (financialexportid, exportnum, employeeid, dateexported)
		VALUES (@financialExportID, @exportNum, @employeeID, @dateExported);
    SELECT @exportHistoryID = @@identity;
    
    -- Add the history items
	IF @exportHistoryID > 0
	BEGIN
		IF @exportType = 1
		BEGIN
			INSERT INTO exporteditems (exporthistoryid, expenseid) (SELECT @exportHistoryID, tmp.c1 FROM @historyIDs AS tmp);
		END
		ELSE IF @exportType = 2
		BEGIN
			INSERT INTO customEntityExportedItems (exportHistoryID, customEntityRecordID) (SELECT @exporthistoryID, tmp.c1 FROM @historyIDs AS tmp);
		END
	END

	RETURN @exportHistoryID;
END
