


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[saveESRAssignmentNumber]
	@esrAssignID int,
	@employeeID int,
	@assignmentNumber nvarchar(30),
	@active bit,
	@primaryAssignment bit,
	@earliestassignmentstartdate DateTime,
	@finalassignmentenddate DateTime,
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

    if @esrAssignID = 0
		begin
			insert into esr_assignments (employeeid, AssignmentNumber, Active, PrimaryAssignment, EarliestAssignmentStartDate, FinalAssignmentEndDate, CreatedOn, CreatedBy) values (@employeeID, @assignmentNumber, @active, @primaryAssignment, @earliestassignmentstartdate, @finalassignmentenddate, @date, @userid);
			set @esrAssignID = scope_identity();
			
			if @CUemployeeID > 0
			Begin
				exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @esrAssignID, @recordTitle, null;
			end
		end
	else
		begin
			declare @oldassignmentNumber nvarchar(30);
			declare @oldactive bit;
			declare @oldprimaryAssignment bit;
			declare @oldearliestassignmentstartdate DateTime;
			declare @oldfinalassignmentenddate DateTime;
			select @oldassignmentNumber = assignmentNumber, @oldactive = active, @oldprimaryAssignment = primaryAssignment, @oldearliestassignmentstartdate = earliestassignmentstartdate, @oldfinalassignmentenddate = finalassignmentenddate from esr_assignments where esrAssignID = @esrAssignID;

			update esr_assignments set AssignmentNumber = @assignmentNumber, Active = @active, PrimaryAssignment = @primaryAssignment, EarliestAssignmentStartDate = @earliestassignmentstartdate, FinalAssignmentEndDate = @finalassignmentenddate, modifiedon = @date, modifiedby = @userid where esrAssignID = @esrAssignID;
			
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
			end
		end

	return @esrAssignID;
END





 
