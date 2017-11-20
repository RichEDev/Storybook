CREATE PROCEDURE [dbo].[SMApiUpdateApiDetails]
	@apiDetailsId int,
	@employeeId int,
	@certificateInfo nvarchar(max),
	@generationTime bigint,
	@expiryTime datetime
AS
BEGIN
	UPDATE [dbo].[ApiDetails] 
	SET EmployeeId = @employeeId,
		CertificateInfo = @certificateInfo,
		GenerationTime = @generationTime,
		ExpiryTime = @expiryTime
	WHERE ApiDetailsId = @apiDetailsId;
	RETURN @apiDetailsId;
END


