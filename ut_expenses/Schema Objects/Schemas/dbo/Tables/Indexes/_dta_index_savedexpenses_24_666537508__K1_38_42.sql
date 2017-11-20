CREATE NONCLUSTERED INDEX [_dta_index_savedexpenses_24_666537508__K1_38_42] ON [dbo].[savedexpenses]
(
	[expenseid] ASC
)
INCLUDE ( 	[normalreceipt],
	[receiptattached]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]