ALTER TABLE [dbo].[EmployeeSelectedClaimApprover]
    ADD CONSTRAINT [PK_EmployeeSelectedClaimApprover] 
	PRIMARY KEY CLUSTERED ([EmployeeSelectedClaimApproverId] ASC) 
	WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);