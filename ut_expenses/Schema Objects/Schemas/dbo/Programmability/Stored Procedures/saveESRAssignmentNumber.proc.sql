CREATE PROCEDURE [dbo].[saveESRAssignmentNumber]
	@esrAssignID int,
	@employeeID int,
	@assignmentNumber nvarchar(30),
	@active bit,
	@primaryAssignment bit,
	@earliestassignmentstartdate DateTime,
	@finalassignmentenddate DateTime,
	@supervisorAssignmentNumber nvarchar(30),
	@SignOffOwner varchar(39),
	@date DateTime,
	@userid int,
	@CUemployeeID INT,
	@CUdelegateID INT	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'ESR Assignment Number ' + @assignmentNumber);

	declare @supervisorAssignmentId bigint = null;
	declare @supervisorEmployeeNumber nvarchar(30) = null;
	declare @supervisorEsrAssignId int = null;
	declare @supervisorFullName nvarchar(240) = null;
	declare @supervisorPersonId bigint = null;

	if @supervisorAssignmentNumber is not null and @supervisorAssignmentNumber <> ''
	begin
		select top 1 @supervisorAssignmentId = AssignmentID, @supervisorEmployeeNumber = EmployeeNumber, @supervisorEsrAssignId = esrAssignID, @supervisorFullName = dbo.getEmployeeFullName(esr_assignments.employeeid), @supervisorPersonId = esr_assignments.ESRPersonId from esr_assignments 
		inner join employees on esr_assignments.employeeid = employees.employeeid
		where AssignmentNumber = @supervisorAssignmentNumber;
	end

    if @esrAssignID = 0
		begin
			if exists (select AssignmentNumber from esr_assignments where AssignmentNumber = @assignmentNumber)
				return -1;

			insert into esr_assignments (employeeid, AssignmentNumber, Active, PrimaryAssignment, EarliestAssignmentStartDate, FinalAssignmentEndDate, SupervisorAssignmentId, SupervisorAssignmentNumber, SupervisorEmployeeNumber, SupervisorEsrAssignId, SupervisorFullName, SupervisorPersonId, SignOffOwner, CreatedOn, CreatedBy) 
			values (@employeeID, @assignmentNumber, @active, @primaryAssignment, @earliestassignmentstartdate, @finalassignmentenddate, @supervisorAssignmentId, @supervisorAssignmentNumber, @supervisorEmployeeNumber, @supervisorEsrAssignId, @supervisorFullName, @supervisorPersonId, @SignOffOwner, @date, @userid);
			set @esrAssignID = scope_identity();
			
			if @CUemployeeID > 0
			Begin
				exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @esrAssignID, @recordTitle, null;
			end
		end
	else
		begin
			if exists (select AssignmentNumber from esr_assignments where AssignmentNumber = @assignmentNumber and employeeid <> @employeeID)
				return -1;

			declare @oldassignmentNumber nvarchar(30);
			declare @oldactive bit;
			declare @oldprimaryAssignment bit;
			declare @oldearliestassignmentstartdate DateTime;
			declare @oldfinalassignmentenddate DateTime;
			declare @oldsupervisorAssignmentNumber nvarchar(30);

			select @oldassignmentNumber = assignmentNumber, @oldactive = active, @oldprimaryAssignment = primaryAssignment, @oldearliestassignmentstartdate = earliestassignmentstartdate, @oldfinalassignmentenddate = finalassignmentenddate, @oldsupervisorAssignmentNumber = SupervisorAssignmentNumber from esr_assignments where esrAssignID = @esrAssignID;

			update esr_assignments set AssignmentNumber = @assignmentNumber, Active = @active, PrimaryAssignment = @primaryAssignment, EarliestAssignmentStartDate = @earliestassignmentstartdate, FinalAssignmentEndDate = @finalassignmentenddate, SupervisorAssignmentId = @supervisorAssignmentId, SupervisorAssignmentNumber = @supervisorAssignmentNumber, SupervisorEmployeeNumber = @supervisorEmployeeNumber, SupervisorEsrAssignId = @supervisorEsrAssignId, SupervisorFullName = @supervisorFullName, SupervisorPersonId = @supervisorPersonId, SignOffOwner = @SignOffOwner, modifiedon = @date, modifiedby = @userid where esrAssignID = @esrAssignID;
			
			if @CUemployeeID > 0
			Begin
				if @oldassignmentNumber <> @assignmentNumber
					exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @esrAssignID, 'c23858b8-7730-440e-b481-c43fe8a1dbef', @oldassignmentNumber, @assignmentNumber, @recordtitle, null;
				if @oldactive <> @active
					exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @esrAssignID, 'cb54066c-84f0-4653-a2d1-cfd22b86d0c5', @oldactive, @active, @recordtitle, null;
				if @oldprimaryAssignment <> @primaryAssignment
					exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @esrAssignID, 'fec46ed7-57f9-4c51-9916-ec92834371c3', @oldprimaryAssignment, @primaryAssignment, @recordtitle, null;
				if @oldearliestassignmentstartdate <> @earliestassignmentstartdate
					exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @esrAssignID, 'c53828af-99ff-463f-93f9-2721df44e5f2', @oldearliestassignmentstartdate, @earliestassignmentstartdate, @recordtitle, null;
				if @oldfinalassignmentenddate <> @finalassignmentenddate
					exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @esrAssignID, '36eb4bb6-f4d5-414c-9106-ee62db01d902', @oldfinalassignmentenddate, @finalassignmentenddate, @recordtitle, null;
				if @oldsupervisorAssignmentNumber <> @supervisorAssignmentNumber
					exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @esrAssignID, 'ED828976-F992-4ADC-B461-27EE83EBFDC8', @oldsupervisorAssignmentNumber, @supervisorAssignmentNumber, @recordtitle, null;
			end
		end

	return @esrAssignID;
END
