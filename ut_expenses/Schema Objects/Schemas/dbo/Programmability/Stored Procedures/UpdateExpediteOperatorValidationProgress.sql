
CREATE PROCEDURE [dbo].UpdateExpediteOperatorValidationProgress
 @expenseId int,
 @expediteValidationProgress int
AS
DECLARE @returnvalue INT
 UPDATE savedexpenses 

 SET ExpediteValidationProgress = @expediteValidationProgress  

 WHERE expenseid = @expenseId;
 SET @returnvalue=@expediteValidationProgress
RETURN @returnvalue