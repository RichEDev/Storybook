CREATE PROCEDURE [dbo].[authenticateMobileDevice] 
	@pairingKey nvarchar(30),
	@serialKey nvarchar(200),
	@employeeID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	declare @count int;
	declare @archived bit;
	declare @active bit;
	
	if exists (select employeeid from employees where employeeid = @employeeID)
	begin
		select @archived = archived, @active = active from employees where employeeId = @employeeID;
	
		if @archived = 1
			return -2; -- MobileReturnCode for EmployeeArchived
			
		if @active = 0
			return -3; --MobileReturnCode for EmployeeNotActivated
	end
	else
	begin
		return -4; -- MobileReturnCode for EmployeeUnknown
	end
    
	set @count = (select count(*) from mobileDevices where pairingKey = @pairingKey and deviceSerialKey = @serialKey and employeeID = @employeeID)
	if @count > 0
		return 0; -- MobileReturnCode for Success
	else
		return 1; -- MobileReturnCode for AuthenticationFailed
END
