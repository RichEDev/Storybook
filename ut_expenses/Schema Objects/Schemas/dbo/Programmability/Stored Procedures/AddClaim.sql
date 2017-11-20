CREATE PROCEDURE dbo.AddClaim 
	  @employeeId INT
	,@claimNumber INT
	,@claimName NVARCHAR(50)
	,@description NVARCHAR(2000)
	,@baseCurrency INT
	,@createdOn DATETIME
	,@userId INT, 
	 @identity int OUTPUT 
AS
BEGIN
	INSERT INTO [dbo].claims_base (
		employeeid
		,claimno
		,[name]
		,[description]
		,currencyid
		,createdon
		,createdby
		)
	VALUES (
		@employeeId
		,@claimNumber
		,@claimName
		,@description
		,@baseCurrency
		,@createdOn
		,@userId
		);

	SELECT @identity = @@identity
	return @@identity;
END
