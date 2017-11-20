CREATE PROCEDURE [dbo].[DeleteFinancialExportHistory] (@exporthistoryid int, @exportType int) 
AS
BEGIN
    -- Insert statements for procedure here
	IF @exportType = 1
	BEGIN
		delete from exporteditems where exporthistoryid = @exporthistoryid;
	END
	ELSE IF @exportType = 2
	BEGIN
		delete from customEntityExportedItems where exportHistoryID = @exporthistoryID
	END
	
	delete from exporthistory where exporthistoryid = @exporthistoryid;
END
