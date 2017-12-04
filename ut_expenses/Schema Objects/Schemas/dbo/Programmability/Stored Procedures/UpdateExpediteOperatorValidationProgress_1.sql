CREATE PROCEDURE [dbo].[UpdateExpediteOperatorValidationProgress]
	@expenseId int,
	@expediteValidationProgress int
AS
DECLARE @returnvalue INT

IF EXISTS(SELECT expenseid FROM savedexpenses WHERE ExpediteValidationProgress = @expediteValidationProgress AND expenseid = @expenseId)
BEGIN
SET @returnvalue=-1 -- Cannot update as it is already this value.
END
ELSE
BEGIN
 UPDATE savedexpenses 
 SET ExpediteValidationProgress = @expediteValidationProgress  
 WHERE expenseid = @expenseId;
 SET @returnvalue=@expediteValidationProgress
 END
RETURN @returnvalue
