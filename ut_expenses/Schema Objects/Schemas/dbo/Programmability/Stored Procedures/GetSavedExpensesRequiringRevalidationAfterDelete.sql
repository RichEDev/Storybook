CREATE PROCEDURE GetSavedExpensesRequiringRevalidationAfterDelete 
	@expenseid int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select distinct savedexpensesFlags.expenseid from savedexpensesFlags inner join savedexpensesFlagAssociatedExpenses on savedexpensesFlagAssociatedExpenses.flaggedItemId = savedexpensesFlags.flaggedItemId
	where savedexpensesFlagAssociatedExpenses.associatedExpenseId = @expenseid
END
GO