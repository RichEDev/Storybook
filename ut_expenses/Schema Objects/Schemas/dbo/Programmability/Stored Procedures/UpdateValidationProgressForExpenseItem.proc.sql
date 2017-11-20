CREATE PROCEDURE [dbo].[UpdateValidationProgressForExpenseItem]
	@expenseId int,
	@validationProgress int
AS
	UPDATE savedexpenses SET ValidationProgress = @validationProgress WHERE expenseid = @expenseId;

RETURN @validationProgress
