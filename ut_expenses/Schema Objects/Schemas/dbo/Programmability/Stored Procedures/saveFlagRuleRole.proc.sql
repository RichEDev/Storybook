CREATE PROCEDURE [dbo].[saveFlagRuleRole]
	-- Add the parameters for the stored procedure here
	@flagID INT,
	@roleIDs IntPk readonly,
	@employeeID INT,
	@delegateID INT,
	@performItemRoleExpenseCheck bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	if @performItemRoleExpenseCheck = 1 and (select count(flags.flagID) from flags
		left join flagAssociatedRoles on flagAssociatedRoles.flagID = flags.flagID
		left join flagAssociatedExpenseItems on flagAssociatedExpenseItems.flagID = flags.flagID
		where active = 1 and flagtype = (select flagtype from flags where flagid = @flagid) and (itemroleInclusiontype = 1 or roleID in (select c1 from @roleIDs)) and (expenseiteminclusiontype = 1 or subcatID in (select subcatID from flagAssociatedExpenseItems where flagID = @flagid)) and flags.flagID <> @flagid) > 0
		begin
			return -1	
		end
    INSERT INTO dbo.flagAssociatedRoles ( flagID, roleID ) select @flagID, c1 from @roleIDs
    
    declare @description nvarchar(max)
    declare @rolename nvarchar(50)
    declare @flagDescription nvarchar(2000)
    
    set @flagDescription = (select description from flags where flagID = @flagID)
    declare flag_cursor cursor for select rolename from item_roles where itemroleid in (select c1 from @roleIDs)

	open flag_cursor

	fetch next from flag_cursor into @rolename
	while @@fetch_status = 0
		BEGIN
			set @description = @flagDescription + ' - Item role ' + @rolename + ' associated to the flag rule.'
			exec addInsertEntryToAuditLog @employeeID, @delegateID, 33, @flagID, @description, null;
			fetch next from flag_cursor into @rolename
		end
	close flag_cursor
	deallocate flag_cursor
END

GO