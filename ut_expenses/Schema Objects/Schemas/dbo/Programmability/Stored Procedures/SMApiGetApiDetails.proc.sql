CREATE PROCEDURE [dbo].[SMApiGetApiDetails]
	@apiDetailsId int
AS
BEGIN
IF @apiDetailsId = 0
	BEGIN
		SELECT [ApiDetailsId]
		  ,[EmployeeId]
		  ,[CertificateInfo]
		  ,[GenerationTime]
		  ,[ExpiryTime]
	  FROM [dbo].[ApiDetails]
	END
ELSE
	BEGIN
		SELECT [ApiDetailsId]
		  ,[EmployeeId]
		  ,[CertificateInfo]
		  ,[GenerationTime]
		  ,[ExpiryTime]
	  FROM [dbo].[ApiDetails]
	  WHERE @apiDetailsId = ApiDetailsId
	END
END
