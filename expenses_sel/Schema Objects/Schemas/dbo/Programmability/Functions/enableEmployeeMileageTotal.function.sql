
CREATE FUNCTION [dbo].[enableEmployeeMileageTotal] 
(
	@employeeid int
)
RETURNS INT
AS
BEGIN
	DECLARE @retVal INT;
	SET @retVal = 0;
	
	IF ((SELECT COUNT(expenseid) FROM savedexpenses 
	INNER JOIN claims ON savedexpenses.claimid = claims.claimid 
	INNER JOIN subcats ON savedexpenses.subcatid = subcats.subcatid
	WHERE employeeid = @employeeid AND subcats.mileageapp = 1) > 0)
	BEGIN
		SET @retVal = 1;
	END
	
	RETURN @retVal;
END
