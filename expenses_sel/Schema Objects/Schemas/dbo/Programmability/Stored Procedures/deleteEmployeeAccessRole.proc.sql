CREATE PROCEDURE [dbo].[deleteEmployeeAccessRole]
	@employeeID int,
	@accessRoleID INT,
	@subAccountID int,
	@date DATETIME,
	@userid int,
@CUemployeeID INT,
@CUdelegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @hasCheckAndPay BIT
	DECLARE @count int
	select @count = count(*) from accessRoleElementDetails inner join employeeAccessRoles on employeeAccessRoles.accessRoleid = accessRoleElementDetails.roleID where accessRoleElementDetails.elementID = 14 and employeeAccessRoles.accessRoleID = @accessRoleID AND dbo.employeeAccessRoles.employeeID = @employeeID;
	if @count > 0
	begin
		select @count = count(*) from accessRoleElementDetails inner join employeeAccessRoles on employeeAccessRoles.accessRoleid = accessRoleElementDetails.roleID where accessRoleElementDetails.elementID = 14 and employeeAccessRoles.accessRoleID <> @accessRoleID AND dbo.employeeAccessRoles.employeeID = @employeeID
		IF @count > 0
			SET @hasCheckAndPay = 1;
		ELSE
			SET @hasCheckAndPay = 0;
		
		IF @hasCheckAndPay = 0
		BEGIN
			--budget holders
			SELECT @count = COUNT(*) FROM signoffs INNER JOIN budgetholders ON budgetholders.budgetholderid = signoffs.relid WHERE signofftype = 1 AND budgetholders.employeeid = @employeeID
			IF @count > 0
				RETURN -1
			--employees
			SELECT @count = COUNT(*) FROM signoffs WHERE signofftype = 2 AND relid = @employeeID;
			IF @count > 0
				RETURN -1
			--teams
			SELECT @count = COUNT(*) FROM signoffs INNER JOIN teams ON teams.teamid = signoffs.relid inner JOIN teamemps ON teamemps.teamid = teams.teamid WHERE signofftype = 3 AND teamemps.employeeid = @employeeID
			IF @count > 0
				RETURN -1
			
		END
	END	

	declare @olddate datetime;
	declare @olduserid int;
	declare @title1 nvarchar(500);
	declare @title2 nvarchar(500);
	declare @recordtitle nvarchar(2000);
	select @title1 = username, @olduserid = modifiedby, @olddate = modifiedon from employees where employeeid = @employeeID;
	select @title2 = rolename from accessRoles where roleID=@accessRoleID;
	set @recordTitle = (select @title2 + ' access role for user ' + @title1);

    -- Insert statements for procedure here
	delete from employeeAccessRoles where employeeid = @employeeID and accessRoleID = @accessRoleID and subAccountID = @subAccountID;
	
	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeID, @recordTitle, @subAccountID;


	UPDATE employees SET modifiedon = @date, modifiedby = @userid WHERE employeeid = @employeeID;

	exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeID, 'e27c6957-0435-4177-b1a6-b56459466c40', @olddate, @date, @title1, @subAccountID;
	exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeID, '3c749b9d-6e8f-4711-96dc-48c8aaf8abc8', @olduserid, @userid, @title1, @subAccountID;
	return 0;
END
