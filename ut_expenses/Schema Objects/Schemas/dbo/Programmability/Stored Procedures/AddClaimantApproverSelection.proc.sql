CREATE PROCEDURE [dbo].[AddClaimantApproverSelection] 
	@employeeId int,
	@approverId int
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT * FROM dbo.EmployeeSelectedClaimApprover WHERE EmployeeId = @employeeId AND ApproverId = @approverId)
		BEGIN
			UPDATE DBO.EmployeeSelectedClaimApprover SET SelectedDate = GETDATE();
		END
	ELSE
		BEGIN
		INSERT INTO [dbo].[EmployeeSelectedClaimApprover]
			   ([EmployeeId]
			   ,[ApproverId])
		 VALUES
			   (@employeeId
			   ,@approverId);
		END
     return 0;
END
