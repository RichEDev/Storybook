CREATE PROCEDURE [dbo].[SaveExpenseValidationResult]
	@id INT = 0,
	@expenseId INT,
	@criterionId INT,
	@validationStatusBusiness INT,
	@validationStatusVAT INT,
	@possibleFraud BIT,
	@validationCompleted DATETIME,
	@comments NVARCHAR(400) = null,
	@data XML = null,
	@matchingResult INT
AS
BEGIN
	SET NOCOUNT ON;
	IF ((SELECT COUNT(expenseid) FROM savedexpenses WHERE expenseid = @expenseId) = 0) RETURN -2;
	IF ((SELECT COUNT(CriterionId) FROM ExpenseValidationCriteria WHERE CriterionId = @criterionId) = 0) RETURN -3;
	IF (@id = 0)
		BEGIN
			INSERT INTO [dbo].[ExpenseValidationResults] ( ExpenseId, CriterionId, ValidationStatusBusiness, ValidationStatusVAT, PossiblyFraudulent, ValidationCompleted, Comments, Data, MatchingResult )
			VALUES (@expenseId, @criterionId, @validationStatusBusiness, @validationStatusVAT, @possibleFraud, @validationCompleted, @comments, @data, @matchingResult);
			RETURN SCOPE_IDENTITY();
		END
	ELSE
		BEGIN
			IF NOT EXISTS (SELECT ResultId FROM [dbo].[ExpenseValidationResults] WHERE ResultId = @id) RETURN -1;
			UPDATE [dbo].[ExpenseValidationResults]
			SET 
				ExpenseId = @expenseId,
				CriterionId = @criterionId,
				ValidationStatusBusiness = @validationStatusBusiness, 
				ValidationStatusVAT = @validationStatusVAT, 
				PossiblyFraudulent = @possibleFraud,
				ValidationCompleted = @validationCompleted,
				Comments = ISNULL(@comments, Comments),
				Data = @data,
				MatchingResult = @matchingResult
			WHERE ResultId = @id;
			RETURN @id;
		END
END