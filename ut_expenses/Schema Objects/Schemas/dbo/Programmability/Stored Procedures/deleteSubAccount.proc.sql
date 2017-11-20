CREATE PROCEDURE [dbo].[deleteSubAccount]
(
	@subAccountId INT, 
	@employeeId INT,
	@delegateID INT
)
AS
BEGIN
DECLARE @description nvarchar(50);

select @description = description from accountsSubAccounts where SubAccountId = @subAccountId;

delete from accountsSubAccounts where SubAccountId = @subAccountId;

exec addDeleteEntryToAuditLog @employeeId, @delegateID, 132, @subAccountId, @description, @subAccountId;
END