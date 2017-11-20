CREATE NONCLUSTERED INDEX [_dta_index_costcodes_26_85575343__K1_K11_K12_K10] ON [dbo].[costcodes]
(
	[costcodeid] ASC,
	[OwnerTeamId] ASC,
	[OwnerBudgetHolderId] ASC,
	[OwnerEmployeeId] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]