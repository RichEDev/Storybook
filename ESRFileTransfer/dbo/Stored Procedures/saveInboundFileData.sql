CREATE PROCEDURE [dbo].[saveInboundFileData] 

@FileData varbinary(MAX),
@FileName nvarchar(250),
@NHSTrustID int,
@FinancialExportID int,
@Status tinyint,
@ExportHistoryID int    
AS


BEGIN
      INSERT INTO InboundFileData (FileData, [FileName], NHSTrustID, FinancialExportID, Status, ExportHistoryID) VALUES (@FileData, @FileName, @NHSTrustID, @FinancialExportID, @Status, @ExportHistoryID)
END
