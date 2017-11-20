CREATE NONCLUSTERED INDEX [_dta_index_savedexpenses_24_666537508__K18_K83_K1_10_34_68_85] ON [dbo].[savedexpenses]
(
	[claimid] ASC,
	[itemCheckerId] ASC,
	[expenseid] ASC
)
INCLUDE ( 	[subcatid],
	[tempallow],
	[itemtype],
	[itemCheckerTeamId]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]