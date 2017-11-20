CREATE PROCEDURE GetEmployeeIdByNotificationType
@emailNotificationType INT
AS
BEGIN
SELECT enl.employeeID FROM [TestingMetabaseExpenses].dbo.emailnotifications en 
INNER JOIN emailNotificationLink enl ON (en.emailNotificationID = enl.emailNotificationID)
WHERE en.emailNotificationType = @emailNotificationType
END