CREATE NONCLUSTERED INDEX [_dta_index_savedexpenses_26_666537508__K18_K83_K55_K34_9_10_38_42_68] ON [dbo].[savedexpenses]
(
	[claimid] ASC,
	[itemCheckerId] ASC,
	[primaryitem] ASC,
	[tempallow] ASC
)
INCLUDE ( 	[total],
	[subcatid],
	[normalreceipt],
	[receiptattached],
	[itemtype]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]