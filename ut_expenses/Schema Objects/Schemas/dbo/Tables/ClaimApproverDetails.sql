CREATE TABLE [dbo].[ClaimApproverDetails](
	[ClaimApproverDetailId] [int] IDENTITY(1,1) NOT NULL,
	[ClaimantId] [int] NOT NULL,
	[CheckerId] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ClaimAmount] [decimal](18, 2) NOT NULL,
	[ClaimId] [int] NOT NULL,
	[SavedExpenseId] [int] NULL,
 CONSTRAINT [PK_ClaimApproverDetails] PRIMARY KEY CLUSTERED 
(
	[ClaimApproverDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ClaimApproverDetails]  WITH CHECK ADD  CONSTRAINT [FK_ClaimApproverDetails_claims_base] FOREIGN KEY([ClaimId])
REFERENCES [dbo].[claims_base] ([claimid])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ClaimApproverDetails] CHECK CONSTRAINT [FK_ClaimApproverDetails_claims_base]
GO

ALTER TABLE [dbo].[ClaimApproverDetails]  WITH CHECK ADD  CONSTRAINT [FK_ClaimApproverDetails_employees] FOREIGN KEY([ClaimantId])
REFERENCES [dbo].[employees] ([employeeid])
GO

ALTER TABLE [dbo].[ClaimApproverDetails] CHECK CONSTRAINT [FK_ClaimApproverDetails_employees]
GO

ALTER TABLE [dbo].[ClaimApproverDetails]  WITH CHECK ADD  CONSTRAINT [FK_ClaimApproverDetails_employees1] FOREIGN KEY([CheckerId])
REFERENCES [dbo].[employees] ([employeeid])
GO

ALTER TABLE [dbo].[ClaimApproverDetails] CHECK CONSTRAINT [FK_ClaimApproverDetails_employees1]
GO

ALTER TABLE [dbo].[ClaimApproverDetails]  WITH CHECK ADD  CONSTRAINT [FK_ClaimApproverDetails_savedexpenses] FOREIGN KEY([SavedExpenseId])
REFERENCES [dbo].[savedexpenses] ([expenseid])
GO

ALTER TABLE [dbo].[ClaimApproverDetails] CHECK CONSTRAINT [FK_ClaimApproverDetails_savedexpenses]
GO