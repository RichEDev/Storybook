CREATE PROCEDURE GetClaimCountByEmployeeAndClaimName @employeeId INT
	,@claimName NVARCHAR(50)
AS
BEGIN
	SELECT count(claimid)
	FROM dbo.claims_base
	WHERE employeeid = @employeeId
		AND NAME = @claimName

	return 0;
END

