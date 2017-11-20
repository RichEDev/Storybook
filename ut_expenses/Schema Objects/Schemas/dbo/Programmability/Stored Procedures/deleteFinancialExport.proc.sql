

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[deleteFinancialExport] (@financialexportid int, @employeeID INT, @delegateID INT) 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	delete from financial_exports where financialexportid = @financialexportid;

	exec addDeleteEntryToAuditLog @employeeID, @delegateID, 32, @financialexportid, null, null;
END


