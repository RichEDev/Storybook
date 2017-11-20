



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[saveEmployeeItemRoles]
	@employeeID int,
	@roleID int,
@CUemployeeID INT,
@CUdelegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @order int;
	
	set @order = (select isnull(max([order]),0) from employee_roles where employeeid =  22848)	
	if @order = 0
		set @order = 1

    -- Insert statements for procedure here
	insert into employee_roles (employeeid, itemroleid, [order]) values (@employeeID, @roleID, @order)
		
	declare @title1 nvarchar(500);
	select @title1 = username from employees where employeeid=@employeeID;
	declare @title2 nvarchar(500);
	select @title2 = rolename from item_roles where itemroleid=@roleID;
	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select @title2 + ' item role for user ' + @title1);
	if @CUemployeeID > 0
	BEGIN	
		exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeID, @recordTitle, null;
	END
END






 
