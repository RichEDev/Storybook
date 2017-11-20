




-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[deleteEmployeeItemRole]
	@employeeID int,
	@roleID INT,
	@date DATETIME,
	@userid int,
@CUemployeeID INT,
@CUdelegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


	declare @olddate datetime;
	declare @olduserid int;
	declare @title1 nvarchar(500);
	declare @title2 nvarchar(500);
	declare @recordtitle nvarchar(2000);
	select @title1 = username, @olduserid = modifiedby, @olddate = modifiedon from employees where employeeid = @employeeID;
	select @title2 = rolename from item_roles where itemroleid=@roleID;
	set @recordTitle = (select @title2 + ' item role for user ' + @title1);

	delete from employee_roles where employeeid = @employeeID and itemroleid = @roleID

	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeID, @recordTitle, null;

	UPDATE employees SET modifiedon = @date, modifiedby = @userid WHERE employeeid = @employeeID

	exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeID, 'e27c6957-0435-4177-b1a6-b56459466c40', @olddate, @date, @title1, null;
	exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeID, '3c749b9d-6e8f-4711-96dc-48c8aaf8abc8', @olduserid, @userid, @title1, null;
	
	EXEC clearDisallowedAddItems
END





