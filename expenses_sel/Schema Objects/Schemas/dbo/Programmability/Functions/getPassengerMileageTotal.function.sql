create FUNCTION [dbo].[getPassengerMileageTotal](@expenseid INT) 
RETURNS decimal(18,2)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @mileage DECIMAL(18,2);
	-- Add the T-SQL statements to compute the return value here
	SET @mileage = (SELECT SUM((num_miles * num_passengers)) FROM savedexpenses_journey_steps WHERE expenseid = @expenseid AND num_passengers > 0);
	
	RETURN @mileage;

END