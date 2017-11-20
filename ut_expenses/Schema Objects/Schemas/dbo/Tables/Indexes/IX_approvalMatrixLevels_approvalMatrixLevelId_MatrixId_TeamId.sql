CREATE NONCLUSTERED INDEX [IX_approvalMatrixLevels_approvalMatrixLevelId_MatrixId_TeamId] ON [dbo].[approvalMatrixLevels]
(
	[approvalMatrixId] ASC,
	[approverTeamId] ASC,
	[approvalMatrixLevelId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]