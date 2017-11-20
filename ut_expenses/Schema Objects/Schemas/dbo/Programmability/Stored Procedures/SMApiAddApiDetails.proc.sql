CREATE PROCEDURE [dbo].[SMApiAddApiDetails]
	@employeeId int = 0,
	@certificateInfo nvarchar(max),
	@generationTime bigint,
	@expiryTime datetime
AS
BEGIN
	INSERT INTO [dbo].[ApiDetails] (EmployeeId, CertificateInfo, GenerationTime, ExpiryTime)
	VALUES (@employeeId, @certificateInfo, @generationTime, @expiryTime)
	RETURN SCOPE_IDENTITY()
END
GO