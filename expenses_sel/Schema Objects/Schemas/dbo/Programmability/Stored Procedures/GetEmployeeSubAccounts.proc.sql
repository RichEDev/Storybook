CREATE PROCEDURE GetEmployeeSubAccounts 

	@CurrentSubAccountID int,
	@EmployeeID int 
AS
BEGIN
	SELECT Distinct accountsSubAccounts.subAccountID, [Description] FROM accountsSubAccounts 
	INNER JOIN employeeAccessRoles ON employeeAccessRoles.subAccountID = accountsSubAccounts.subAccountID
	WHERE employeeAccessRoles.employeeID = @EmployeeID AND archived = 0 AND accountsSubAccounts.subAccountID <> @CurrentSubAccountID
END
