CREATE PROCEDURE [dbo].[DetermineExpenseValidationResult]
	@criterionId INT,
	@reasonId INT,
	@isVAT BIT,
	@total DECIMAL = NULL
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @thresholdId int;
	
	IF (@isVAT = 1) SELECT @thresholdId = ThresholdId FROM ExpenseValidationThresholds WHERE (LowerBound IS NOT NULL AND UpperBound IS NOT NULL) AND (@total >= LowerBound AND @total < UpperBound)
	ELSE SET @thresholdId = 1
	
	RETURN SELECT ResultStatus FROM ExpenseValidationReasonResultMapping WHERE ThresholdId = 1 AND CriterionId = @criterionId AND ReasonId = @reasonId	
END
