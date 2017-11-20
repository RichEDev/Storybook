ALTER TABLE [dbo].[EmployeeSelectedClaimApprover] 
ADD  CONSTRAINT [DF_EmployeeSelectedClaimApprover_SelectedDate]  
DEFAULT (getdate()) FOR [SelectedDate]
