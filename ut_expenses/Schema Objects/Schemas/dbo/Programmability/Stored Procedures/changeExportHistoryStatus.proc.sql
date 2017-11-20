


-- =============================================
-- Author:		Paul Lancashire
-- Create date: 02/07/2009
-- Description:	Update a financial exports export history status
-- =============================================
CREATE PROCEDURE [dbo].[changeExportHistoryStatus] (@exporthistoryid int, @exportStatus tinyint, @employeeID INT, @delegateID INT) 	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @oldexportStatus tinyint;
	select @oldexportStatus = exportStatus from exporthistory where exporthistoryid = @exporthistoryid;

    -- Insert statements for procedure here
	update exporthistory set exportStatus = @exportStatus where exporthistoryid = @exporthistoryid;
	if @employeeid > 0
	BEGIN
	if @oldexportStatus <> @exportStatus
		begin
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 32, @exporthistoryid, 'e410d710-9d6b-4c27-b355-3d760e72b525', @oldexportStatus, @exportStatus, null, null;
		end
	END
END
