CREATE NONCLUSTERED INDEX [_dta_index_savedexpenses_15_666537508__K1_9_10_11_18_62] ON [dbo].[savedexpenses] 
	(
		[expenseid] ASC
	)
	INCLUDE ( [total],
	[subcatid],
	[date],
	[claimid],
	[basecurrency]) WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]