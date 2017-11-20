CREATE PROCEDURE [dbo].[GetJourneyStepsByExpenseId] @expenseId INT
AS
BEGIN
	SELECT *
	FROM savedexpenses_journey_steps
	WHERE expenseid = @expenseid
END