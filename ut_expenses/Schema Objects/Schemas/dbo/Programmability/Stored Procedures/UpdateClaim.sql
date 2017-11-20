CREATE PROCEDURE dbo.UpdateClaim @name NVARCHAR(50)
	,@description NVARCHAR(2000)
	,@modifiedOn DATETIME
	,@userId INT
	,@claimId INT
AS
BEGIN
	UPDATE claims_base
	SET NAME = @name
		,description = @description
		,ModifiedOn = @modifiedon
		,ModifiedBy = @userid
	WHERE claimid = @claimid;

	RETURN 0;
END
