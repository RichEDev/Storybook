




-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[deleteESRElement] (@elementID int, @employeeID INT, @delegateID INT)
	
AS
declare @recordTitle nvarchar(2000);
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @globalElementID int;
	declare @NHSTrustID int;
	select @globalElementID = globalElementID, @NHSTrustID = NHSTrustID from ESRElements where elementID = @elementID;
	set @recordTitle = (select '@NHSTrustID ' + CAST(@NHSTrustID AS nvarchar(20)) + ' - @globalElementID ' + CAST(@globalElementID AS nvarchar(20)));

    -- Insert statements for procedure here
	delete from esrElements where elementID = @elementID;

	exec addDeleteEntryToAuditLog @employeeID, @delegateID, 26, @elementID, @recordTitle, null;
END






