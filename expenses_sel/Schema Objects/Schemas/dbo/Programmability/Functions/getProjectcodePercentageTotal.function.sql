-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[getProjectcodePercentageTotal](@expenseid INT, @projectcodeID int) 
RETURNS DECIMAL(18,2)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @percent int;
	DECLARE @total decimal(18,2);
	DECLARE @percentageTotal DECIMAL(18,2);

	-- Add the T-SQL statements to compute the return value here
	SET @percent = (SELECT sum(percentused) FROM savedexpenses_costcodes WHERE expenseid = @expenseid AND projectcodeid = @projectcodeID);
	SET @total = (SELECT total FROM savedexpenses WHERE expenseid = @expenseid)
	SET @percentageTotal = (@total * @percent)/100
	-- Return the result of the function
	RETURN @percentageTotal;

END
