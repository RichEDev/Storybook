CREATE NONCLUSTERED INDEX [IX_ClaimApproverDetails_CheckerId_ClaimId_SavedExpenseId_ClaimApproverId] ON [dbo].[ClaimApproverDetails]
(
	[CheckerId] ASC,
	[ClaimId] ASC,
	[SavedExpenseId] ASC,
	[ClaimApproverDetailId] ASC
)
INCLUDE ( 	[ClaimantId],
	[CreatedOn],
	[ClaimAmount]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]