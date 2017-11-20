CREATE PROC [dbo].[MergeDuplicateEmployees] @duplicateEmps DuplicateEmployees READONLY
AS

	declare @usernameToKeep nvarchar(50);
	declare @usernameToRemove nvarchar(50);
	declare @empIdToKeep int;
	declare @empIdToRemove int;
	declare @updateDetails bit;

	declare @position nvarchar(250);
	declare @payroll nvarchar(50);
	declare @telNo nvarchar(50);
	declare @faxNo nvarchar(50);
	declare @mobileNo nvarchar(50);

	DECLARE tmpCursor CURSOR FOR
	SELECT usernameToKeep, usernameToDrop, updateDetails FROM @duplicateEmps
	OPEN tmpCursor
	FETCH NEXT FROM tmpCursor INTO @usernameToKeep, @usernameToRemove, @updateDetails
	WHILE @@FETCH_STATUS = 0
	BEGIN

		--Set the employee ID values 
		set @empIdToKeep = (select employeeid from employees where username = @usernameToKeep);
		set @empIdToRemove= (select employeeid from employees where username = @usernameToRemove);

		update contract_details set contractOwner = @empIdToKeep where contractOwner = @empIdToRemove;
		update contract_notification set employeeid = @empIdToKeep where employeeid = @empIdToRemove;
		update productDetails set notifyid = @empIdToKeep where notifyId = @empIdToRemove;
		update productLicences set notifyid = @empIdToKeep where notifyId = @empIdToRemove;
		update teams set teamleaderid = @empIdToKeep where teamleaderId = @empIdToRemove;
		update teamemps set employeeid = @empIdToKeep where employeeid = @empIdToRemove;

		IF @updateDetails = 1
		BEGIN
			--get values of the employee to rem,ove to update the employee record to keep
			select @position = position, @payroll = payroll, @telNo = telno, @faxNo = faxno, @mobileNo = mobileno from employees where employeeid = @empIdToRemove;

			update employees set position =@position, payroll = @payroll, telno = @telNo, faxno = @faxNo, mobileno = @mobileNo where employeeid = @empIdToKeep;
		END
		
		--Remove the employee from the database 
		delete from employeeAccessRoles where employeeID = @empIdToRemove;
		delete from employees where employeeid = @empIdToRemove
		
		FETCH NEXT FROM tmpCursor INTO @usernameToKeep, @usernameToRemove, @updateDetails
	END
	CLOSE tmpCursor
	DEALLOCATE tmpCursor

