CREATE PROCEDURE [dbo].[DeleteConsentDetailsAfterRevokingTheConsent]
@employeeId nvarchar(20)
AS
BEGIN
UPDATE employees set AgreeToProvideConsent = null where employeeid = @employeeId
END
