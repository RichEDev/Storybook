CREATE PROCEDURE [dbo].[DeleteExpenseValidationResult]
	@id INT
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS (SELECT * FROM [dbo].[ExpenseValidationResults] WHERE ResultId = @id) RETURN -1;

	DELETE FROM [dbo].[ExpenseValidationReasonsForResults] WHERE ResultId = @id;
	DELETE FROM [dbo].[ExpenseValidationResults] WHERE ResultId = @id;	
	
	RETURN 0;
END