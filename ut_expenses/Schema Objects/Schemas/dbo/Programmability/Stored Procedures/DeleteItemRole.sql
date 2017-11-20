CREATE PROCEDURE DeleteItemRole 
	@itemroleID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	if (select count(roleid) from flagAssociatedRoles where roleID = @itemroleID) > 0
		return -1
		
    delete from additems where employeeid in (select employeeid from employee_roles where itemroleid = @itemroleID) and subcatid in (select subcatid from rolesubcats where roleid = @itemroleID)
    delete from employee_roles where itemroleid = @itemroleID
    delete from item_roles where itemroleid = @itemroleID
    return 0
END
GO