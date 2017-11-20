CREATE PROC [dbo].[GetExpediteCustomers]
AS
BEGIN

SELECT accountid FROM registeredusers WHERE PaymentServiceEnabled =1

END
GO