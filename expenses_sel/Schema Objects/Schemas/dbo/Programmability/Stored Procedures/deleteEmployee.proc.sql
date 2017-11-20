CREATE PROCEDURE [dbo].[deleteEmployee]
	@employeeid int,
	@CUemployeeID INT,
	@CUdelegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @archived bit
	declare @count int

	select @archived = archived from employees where employeeid = @employeeid;
	if @archived = 0
		return 4;

	set @count = (select count(*) from signoffs where (signofftype = 2 and relid = @employeeid) or (holidaytype = 2 and relid = @employeeid))
	if @count > 0
		return 1;

	set @count = (select count(*) from floats where ([float] - (select sum(amount) from float_allocations where floatid = floats.floatid)) > 0 and employeeid = @employeeid and paid = 1)
	if @count > 0
		return 2

	set @count = (select count(*) from budgetholders where employeeid=@employeeid)
	if @count > 0
		return 3

	set @count = (select count(*) from contract_details where contractOwner=@employeeid)
	if @count > 0
		return 5

	set @count = (select count(*) from contract_audience  where audienceType=0 and accessId=@employeeid)
	if @count > 0
		return 6

	set @count = (select count(*) from attachment_audience where audienceType=0 and accessId=@employeeid)
	if @count > 0
		return 7

	set @count = (select count(*) from contract_notification where employeeId=@employeeid)
	if @count > 0
		return 8

	set @count = (select count(*) from teams where teamleaderid=@employeeid)
	if @count > 0
		return 9
	set @count = (select count(*) from contract_history where employeeId=@employeeid)
	if @count > 0
		return 11
	
	set @count = (select count(*) from audienceEmployees where employeeid=@employeeid)
	if @count > 0
		return 13;

	DECLARE @tmpTeam int;
	DECLARE teams_cursor CURSOR FOR
		select teamid from teamemps where employeeid = @employeeid
		OPEN teams_cursor
		FETCH NEXT FROM teams_cursor INTO @tmpTeam
		WHILE @@FETCH_STATUS = 0
		BEGIN
			set @count = (select count(*) from teamemps where teamid = @tmpTeam)
			if @count = 1
				return 10
			FETCH NEXT FROM teams_cursor INTO @tmpTeam
		END
	CLOSE teams_cursor
	DEALLOCATE teams_cursor


	declare @recordTitle nvarchar(2000);
	select @recordTitle = username from employees where employeeid = @employeeid;

	update reports set employeeid = (SELECT stringValue FROM accountProperties WHERE stringKey='mainadministrator') where employeeid = @employeeid;
	exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 48, @employeeid, 'b3a00791-e61c-4c7f-8ebe-860b42837efb', @employeeid, null, @recordtitle, null;

	delete from float_allocations where floatid in (select floatid from [floats] where employeeid = @employeeid);
	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 7, @employeeid, @recordTitle, null;

	delete from [floats] where employeeid = @employeeid;
	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 7, @employeeid, @recordTitle, null;

	delete from savedexpenses_current where claimid in (select claimid from claims where employeeid = @employeeid);
	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 100, @employeeid, @recordTitle, null;

	delete from savedexpenses_previous where claimid in (select claimid from claims where employeeid = @employeeid);
	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 100, @employeeid, @recordTitle, null;

	delete from claims_base where employeeid = @employeeid;
	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 100, @employeeid, @recordTitle, null;

	delete from logon_trace where employeeid = @employeeid;
	
	--delete from [employees_bankdetails] where employeeid = @employeeid;
	--exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, @recordTitle, null;

	exec deleteEmployeeAccessRoles @employeeid;

	delete from employees where employeeid = @employeeid;
	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, @recordTitle, null;
END
