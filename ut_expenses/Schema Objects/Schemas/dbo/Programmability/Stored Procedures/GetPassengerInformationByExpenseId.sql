CREATE PROCEDURE [dbo].[GetPassengerInformationByExpenseId] @expenseId INT
AS
BEGIN
	SELECT 
		employeeid, 
		CASE 
			WHEN employeeid IS NULL
				THEN NAME
			ELSE dbo.GetEmployeeFirstnameSurnameUsernameById(employeeid)
		END NAME,
	   savedexpenses_journey_steps_passengers.step_number
	FROM savedexpenses_journey_steps_passengers
	WHERE expenseid = @expenseId
	ORDER BY id
END