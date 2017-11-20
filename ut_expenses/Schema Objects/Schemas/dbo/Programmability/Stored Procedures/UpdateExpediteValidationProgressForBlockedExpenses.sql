CREATE PROCEDURE UpdateExpediteValidationProgressForBlockedExpenses
AS
BEGIN
DECLARE @isUpdated int
 
IF EXISTS(SELECT TOP 1 * FROM savedexpenses WHERE ExpediteValidationProgress=1)
BEGIN
UPDATE savedexpenses
SET ExpediteValidationProgress=0
WHERE ExpediteValidationProgress=1
SET @isUpdated=1
END
ELSE
SET @isUpdated=0
 
RETURN @isUpdated
 
END