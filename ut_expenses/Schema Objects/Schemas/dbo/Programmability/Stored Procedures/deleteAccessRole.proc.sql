
CREATE PROCEDURE [dbo].[deleteAccessRole] (@accessRoleID INT, @CUemployeeID INT, @CUdelegateID INT)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Insert statements for procedure here
	declare @recordTitle nvarchar(2000);
	select @recordTitle = rolename FROM accessRoles WHERE roleID=@accessRoleID;

	-- secondary accessrole is performed by cascade delete
	DELETE from accessRolesLink WHERE primaryAccessRoleID = @accessRoleID;
	
	DELETE FROM accessRoles WHERE roleID=@accessRoleID;

	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 6, @accessRoleID, @recordTitle, null;
END
