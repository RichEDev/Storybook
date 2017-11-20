CREATE PROCEDURE GetExpensesBeenAssociatedWithFlagCount 
	@flagID int,
	@expenseid int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT count (savedexpensesFlagAssociatedExpenses.flaggeditemid) from savedexpensesFlagAssociatedExpenses 
		inner join savedExpensesFlags on savedexpensesFlags.flaggedItemId = savedexpensesFlagAssociatedExpenses.flaggedItemId
		where savedexpensesFlagAssociatedExpenses.associatedExpenseId = @expenseid and savedexpensesflags.flagID = @flagID
END
GO