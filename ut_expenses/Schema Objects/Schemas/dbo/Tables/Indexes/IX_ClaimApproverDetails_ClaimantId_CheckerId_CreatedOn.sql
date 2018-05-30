CREATE NONCLUSTERED INDEX IX_ClaimApproverDetails_ClaimantId_CheckerId_CreatedOn ON [dbo].[ClaimApproverDetails]
(
	[ClaimantId] ASC,
	[CheckerId] ASC,
	[CreatedOn] ASC
)
INCLUDE ([ClaimAmount]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]