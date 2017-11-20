
create FUNCTION [dbo].[getHeavybulkyEquipmentMileageTotal](@expenseid INT) 
RETURNS decimal(18,2)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @mileage DECIMAL(18,2);
	-- Add the T-SQL statements to compute the return value here
	SET @mileage = (SELECT SUM((num_miles * HeavyBulkyEquipment)) FROM savedexpenses_journey_steps WHERE expenseid = @expenseid AND heavybulkyequipment = 1);
	
	RETURN @mileage;

END