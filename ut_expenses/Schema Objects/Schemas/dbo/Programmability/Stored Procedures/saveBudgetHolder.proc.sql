CREATE PROCEDURE [dbo].[saveBudgetHolder] 
	@budgetholderid int,
	@budgetholder nvarchar(50),
	@description nvarchar(4000),
	@date DateTime,
	@userid INT,	
	@employeeid int,
	@delegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

declare @count int
    if @budgetholderid > 0
		begin
			set @count = (select count(*) from budgetholders where budgetholder = @budgetholder and budgetholderid <> @budgetholderid)
			if @count > 0
				return -1
				
			declare @oldBudgetHolder nvarchar(50)
			declare @oldemployeeid int
	declare @oldDescription nvarchar(4000)
	
	select @oldBudgetHolder = budgetholder, @oldDescription = description, @oldemployeeid = employeeid from budgetholders where [budgetholderid] = @budgetholderid;
	
	update budgetholders set budgetholder = @budgetholder, description = @description, employeeid = @employeeid, ModifiedOn = @date, ModifiedBy = @userid where budgetholderid = @budgetholderid
	
	
	if @userID > 0
	BEGIN
		if @budgetholder <> @oldbudgetholder
			exec addUpdateEntryToAuditLog @userID, @delegateID, 1, @budgetholderid, '28E88047-145A-45E5-AFEB-3F18A4683BA5', @oldbudgetholder, @budgetholder, @budgetholder, null;
		if @description <> @oldDescription
			exec addUpdateEntryToAuditLog @userID, @delegateID, 1, @budgetholderid, '7A470F7D-D081-4D64-B663-2A0AC2A7C221', @oldDescription, @description, @budgetholder, null;
		if @employeeid <> @oldemployeeid
			begin
				declare @oldusername nvarchar(500)
				declare @username nvarchar(500)
				
				select @username = username from employees where employeeid = @employeeid
				select @oldusername = username from employees where employeeid = @oldemployeeid
				exec addUpdateEntryToAuditLog @userID, @delegateID, 1, @budgetholderid, '4B9B73B6-789A-4272-86A0-05950A1095B3', @oldusername, @username, @budgetholder, null;
			end
			
		end
		end
	else
		begin
			set @count = (select count(*) from budgetholders where budgetholder = @budgetholder);
			if @count > 0
				return -1
				
			insert into budgetholders (budgetholder, description, employeeid, CreatedOn, CreatedBy) values (@budgetholder,@description,@employeeid, @date, @userid);
			set @budgetholderid =  scope_identity();
			
			if @userID > 0
			BEGIN
				EXEC addInsertEntryToAuditLog @userID, @delegateID, 11, @budgetholderid, @budgetholder, null
			END
		end
		
	return @budgetholderid
END