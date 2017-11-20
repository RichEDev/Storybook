CREATE PROCEDURE [dbo].[addEmployeeRole] (@employeeid int, @itemroleid int, @order int,
@CUemployeeID INT,
@CUdelegateID INT) 
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    -- Insert statements for procedure here
	insert into employee_roles (employeeid, itemroleid, [order]) values (@employeeid, @itemroleid, @order);
		
	declare @title1 nvarchar(500);
	select @title1 = username from employees where employeeid=@employeeid;
	declare @title2 nvarchar(500);
	select @title2 = rolename from item_roles where itemroleid=@itemroleid;
	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select @title2 + ' item role for user ' + @title1);
	exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, @recordTitle, null;
END
