CREATE PROCEDURE [dbo].[saveEmployeeAccessRoles]
	@employeeID int,
	@subAccountID int,
	@accessRoleID int,
@CUemployeeID INT,
@CUdelegateID INT
AS 
	BEGIN
		if not exists (select * from employeeAccessRoles where employeeID = @employeeID and accessRoleID = @accessRoleID and subAccountID = @subAccountID)
		begin
			INSERT INTO [dbo].[employeeAccessRoles] (employeeID, accessRoleID, subAccountID) VALUES (@employeeID, @accessRoleID, @subAccountID);

			declare @title1 nvarchar(500);
			select @title1 = username from employees where employeeid=@employeeID;
			declare @title2 nvarchar(500);
			select @title2 = rolename from accessRoles where roleID=@accessRoleID;
			declare @recordTitle nvarchar(2000);
			set @recordTitle = (select @title2 + ' access role for user ' + @title1);
			
			if @CUemployeeID > 0
			BEGIN
				exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeID, @recordTitle, @subAccountId;
			END
		end
	END
