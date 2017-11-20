CREATE PROCEDURE GetClaimantIdsMatchesForClaimName @claimName NVARCHAR(255)
AS
BEGIN
	SELECT DISTINCT [claims].employeeid
	FROM [claims]
	INNER JOIN [employees] ON [employees].[employeeid] = [claims].[employeeid]
	WHERE ([claims].[name] LIKE @claimName)
		AND (
			[employees].[username] NOT LIKE @claimName
			OR [employees].[username] IS NULL
			)
END
