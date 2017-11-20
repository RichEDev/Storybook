CREATE PROCEDURE [dbo].[CheckExpenseItemValidationCount]
	@claimid int
AS
DECLARE @MaxValidation int;
	SELECT @MaxValidation = MAX(validationcount) FROM savedexpenses WHERE claimid = @claimid and ValidationProgress <> 22
RETURN @MaxValidation;
