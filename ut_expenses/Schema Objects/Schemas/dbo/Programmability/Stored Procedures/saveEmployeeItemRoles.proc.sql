CREATE PROCEDURE [dbo].[saveEmployeeItemRoles]
	@employeeID int,
	@roleID int,
@CUemployeeID INT,
@CUdelegateID INT,
@startDate DateTime,
@endDate DateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @order int;
	declare @title1 nvarchar(500);
	select @title1 = username from employees where employeeid=@employeeID;
	declare @title2 nvarchar(500);
	select @title2 = rolename from item_roles where itemroleid=@roleID;
	declare @recordTitle nvarchar(2000);
	
	set @order = (select isnull(max([order]),0) from employee_roles where employeeid = @employeeID)	
	if @order = 0
		set @order = 1

	-- if exists update the entry
	if exists(select * from employee_roles where itemroleid = @roleID and employeeid = @employeeID)
	BEGIN

	declare @oldStartDate dateTime
	select @oldstartdate = startdate from employee_roles where itemroleid = @roleID and employeeid = @employeeID
	declare @oldEndDate dateTime
	select @oldEndDate = enddate from employee_roles where itemroleid = @roleID and employeeid = @employeeID

	update employee_roles 
	set startdate = @startDate, enddate = @endDate
	where itemroleid = @roleID and employeeid = @employeeID

	set @recordTitle = (select @title2 + ' item role for user ' + @title1 + '. ');

	if @oldStartDate <> @startDate or (@oldStartDate is null and @startDate is not null) or (@oldEndDate is not null and @startDate is null)
	exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '4B8E0B15-0F55-4E0E-81C1-612E6130DD8E', @oldStartDate, @startDate, @recordtitle, null;
	
	if @oldEndDate <> @endDate  or (@oldEndDate is null and @endDate is not null) or (@oldEndDate is not null and @endDate is null)
	exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '77F08892-038A-48BC-9F65-C9DB412F588E', @oldEndDate, @endDate, @recordtitle, null;

	END

	ELSE

	BEGIN
    -- Insert statements for procedure here
	insert into employee_roles (employeeid, itemroleid, [order], startdate, enddate) values (@employeeID, @roleID, @order, @startDate, @endDate)

	declare @title3 nvarchar(500) = '';
	if @startDate is not null
	select @title3 = 'Start date: ' + CONVERT(varchar(20), @startDate)  + ' ';
	
	declare @title4 nvarchar(500) = '';
	if @endDate is not null
	select @title4 = 'End date: ' + CONVERT(varchar(20), @endDate);

	set @recordTitle = (select @title2 + ' item role for user ' + @title1 + '. ' + @title3 + @title4);

	if @CUemployeeID > 0
	BEGIN	
		exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeID, @recordTitle, null;
	END
END
END