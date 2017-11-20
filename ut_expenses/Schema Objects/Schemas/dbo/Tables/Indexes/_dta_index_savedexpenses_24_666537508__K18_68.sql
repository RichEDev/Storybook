CREATE NONCLUSTERED INDEX [_dta_index_savedexpenses_24_666537508__K18_68] ON [dbo].[savedexpenses]
(
	[claimid] ASC
)
INCLUDE ( 	[itemtype]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]