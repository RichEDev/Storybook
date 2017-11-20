CREATE NONCLUSTERED INDEX [_dta_index_savedexpenses_26_666537508__K18_K34_K55] ON [dbo].[savedexpenses]
(
	[claimid] ASC,
	[tempallow] ASC,
	[primaryitem] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]