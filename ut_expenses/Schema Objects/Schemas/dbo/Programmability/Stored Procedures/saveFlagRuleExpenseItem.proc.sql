CREATE PROCEDURE [dbo].[saveFlagRuleExpenseItem]
	-- Add the parameters for the stored procedure here
	@flagID INT,
	@subcatIDs  IntPk readonly,
	@employeeID INT,
	@delegateID INT,
	@performItemRoleExpenseCheck bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--see if it would duplicate items and expense items
	if @performItemRoleExpenseCheck = 1 and (select count(flags.flagID) from flags
		left join flagAssociatedRoles on flagAssociatedRoles.flagID = flags.flagID
		left join flagAssociatedExpenseItems on flagAssociatedExpenseItems.flagID = flags.flagID
		where active = 1 and flagtype = (select flagtype from flags where flagid = @flagid) and (expenseiteminclusiontype = 1 or subCatID in (select c1 from @subcatIDs)) and (itemroleinclusiontype = 1 or roleID in (select roleID from flagAssociatedRoles where flagID = @flagid)) and flags.flagID <> @flagid) > 0
		begin
			return -1	
		end
	
    INSERT INTO dbo.flagAssociatedExpenseItems ( flagID, subCatID ) select @flagID, c1 from @subcatIDs
    
    declare @description nvarchar(max)
    declare @subcat nvarchar(50)
    declare @flagDescription nvarchar(2000)
    
    set @flagDescription = (select description from flags where flagID = @flagID)
    declare flag_cursor cursor for select subcat from subcats where subcatid in (select c1 from @subcatIDs)

	open flag_cursor

	fetch next from flag_cursor into @subcat
	while @@fetch_status = 0
		BEGIN
			set @description = @flagDescription + ' - Expense item ' + @subcat + ' associated to the flag rule.'
			exec addInsertEntryToAuditLog @employeeID, @delegateID, 33, @flagID, @description, null;
			fetch next from flag_cursor into @subcat
		end
	close flag_cursor
	deallocate flag_cursor
    
    return 0
END





GO