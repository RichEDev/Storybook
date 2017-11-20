	CREATE TABLE [dbo].[EmployeeSelectedClaimApprover](
	 [EmployeeSelectedClaimApproverId] [int] IDENTITY(1,1) NOT NULL,
	 [EmployeeId] [int] NOT NULL,
	 [ApproverId] [int] NOT NULL,
	 [SelectedDate] DATETIME NOT NULL
	 );