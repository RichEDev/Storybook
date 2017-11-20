CREATE FUNCTION [dbo].[getSplitNet](@expenseid INT, @percent int) 
RETURNS DECIMAL(18,2)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @net decimal(18,2);
	DECLARE @percentageNet DECIMAL(18,2);

	-- Add the T-SQL statements to compute the return value here
	SET @net = (SELECT net FROM savedexpenses WHERE expenseid = @expenseid)
	SET @percentagenet = (@net * @percent)/100
	-- Return the result of the function
	RETURN @percentageNet;

END
