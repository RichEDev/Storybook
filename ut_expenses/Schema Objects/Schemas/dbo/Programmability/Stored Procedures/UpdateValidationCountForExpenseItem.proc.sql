CREATE PROCEDURE [dbo].[UpdateValidationCountForExpenseItem](
	@expenseId int,
	@validationCount int)
AS
BEGIN
	UPDATE savedexpenses SET ValidationCount = @validationCount WHERE expenseid = @expenseId;
	RETURN @validationCount
END
