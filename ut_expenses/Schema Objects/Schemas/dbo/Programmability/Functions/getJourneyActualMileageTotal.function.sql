CREATE FUNCTION [dbo].[getJourneyActualMileageTotal](@expenseid INT) 
RETURNS decimal(18,2)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @mileage DECIMAL(18,2);

	-- Add the T-SQL statements to compute the return value here
	SET @mileage = (SELECT SUM(numActualMiles) FROM savedexpenses_journey_steps WHERE expenseid = @expenseid);

	-- Return the result of the function
	RETURN @mileage;

END
