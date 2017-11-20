CREATE FUNCTION [dbo].[getSplitVAT](@expenseid INT, @percent int) 
RETURNS DECIMAL(18,2)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @vat decimal(18,2);
	DECLARE @percentageVAT DECIMAL(18,2);

	-- Add the T-SQL statements to compute the return value here
	SET @vat = (SELECT vat FROM savedexpenses WHERE expenseid = @expenseid)
	SET @percentageVAT = (@vat * @percent)/100
	-- Return the result of the function
	RETURN @percentageVAT;

END
