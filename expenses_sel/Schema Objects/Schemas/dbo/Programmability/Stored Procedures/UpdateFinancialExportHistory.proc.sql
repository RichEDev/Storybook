CREATE PROCEDURE [dbo].[UpdateFinancialExportHistory] @exportHistoryID int, @exportStatus tinyint
AS
BEGIN
    UPDATE exporthistory SET exportStatus = @exportStatus WHERE exporthistoryid = @exportHistoryID;
END
