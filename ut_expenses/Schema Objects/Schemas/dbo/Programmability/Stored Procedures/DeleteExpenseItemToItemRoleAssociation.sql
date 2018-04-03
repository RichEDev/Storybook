CREATE PROCEDURE [dbo].[DeleteExpenseItemToItemRoleAssociation]
	@rolesubcatid int
AS
BEGIN
	DELETE FROM rolesubcats WHERE rolesubcatid = @rolesubcatid
	EXEC clearDisallowedAddItems
END