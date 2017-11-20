CREATE PROCEDURE [dbo].[ApiBatchSaveEmployeeAccessRole] @list ApiBatchSaveEmployeeAccessRoleType READONLY
AS
BEGIN
	DECLARE @employeeid INT
	DECLARE @accessRoleID INT
	DECLARE @subAccountID INT

	SET @subAccountID = (
			SELECT TOP 1 subAccountID
			FROM accountsSubAccounts
			WHERE archived = 0
			ORDER BY subAccountID
			)

	INSERT employeeAccessRoles (
		employeeID
		,accessRoleID
		,subAccountID
		)
	SELECT l.employeeid
		,l.accessRoleID
		,@subAccountID
	FROM @list l
	INNER JOIN accessRoles ar ON ar.roleID = l.accessRoleID
	LEFT JOIN employeeAccessRoles ear ON ear.employeeID = l.employeeid
	WHERE ear.employeeID IS NULL

	RETURN 0;
END