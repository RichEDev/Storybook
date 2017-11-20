CREATE FUNCTION [dbo].[getSplitTotal](@expenseid INT, @percent int) 
RETURNS DECIMAL(18,2)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @total decimal(18,2);
	DECLARE @percentageTotal DECIMAL(18,2);

	-- Add the T-SQL statements to compute the return value here
	SET @total = (SELECT total FROM savedexpenses WHERE expenseid = @expenseid)
	SET @percentageTotal = (@total * @percent)/100
	-- Return the result of the function
	RETURN @percentageTotal;

END

