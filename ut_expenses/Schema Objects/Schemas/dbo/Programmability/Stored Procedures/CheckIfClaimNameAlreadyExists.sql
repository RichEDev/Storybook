CREATE PROCEDURE dbo.CheckIfClaimNameAlreadyExists @employeeId INT
	,@name NVARCHAR(50)
	,@claimId INT
AS
BEGIN
	SELECT count(claimid)
	FROM claims_base
	WHERE employeeid = @employeeId
		AND NAME = @name
		AND claimid <> @claimId;

	RETURN 0;
END