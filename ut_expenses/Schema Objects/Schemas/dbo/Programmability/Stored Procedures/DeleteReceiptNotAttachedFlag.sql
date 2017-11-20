CREATE PROCEDURE DeleteReceiptNotAttachedFlag
	@expenseid int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	delete from savedexpensesFlags where flagType = 18 and expenseid in (select splititem from dbo.getExpenseItemIdsFromPrimary(@expenseid))
END
GO