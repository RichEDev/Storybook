CREATE PROCEDURE [dbo].[SMApiGetApiDetailsByEmployeeId]
	@employeeId int
AS
BEGIN
	SELECT [ApiDetailsId]
		,[EmployeeId]
		,[CertificateInfo]
		,[GenerationTime]
		,[ExpiryTime]
	FROM [dbo].[ApiDetails]
	WHERE @employeeId = EmployeeId
END
