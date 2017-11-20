

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[deleteAllowanceBreakdown]
	@breakdownid int,
	@employeeID int,
	@delegateID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @recordTitle nvarchar(2000);
	select @recordTitle = allowance from allowances where allowanceid = (select allowanceid from allowancebreakdown where breakdownid = @breakdownid);

    -- Insert statements for procedure here
	delete from allowancebreakdown where breakdownid = @breakdownid;
	exec addDeleteEntryToAuditLog @employeeID, @delegateID, 8, @breakdownid, @recordTitle, null;
END
