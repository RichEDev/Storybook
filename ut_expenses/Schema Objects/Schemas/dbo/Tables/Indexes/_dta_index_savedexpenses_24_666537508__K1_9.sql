CREATE NONCLUSTERED INDEX [_dta_index_savedexpenses_24_666537508__K1_9] ON [dbo].[savedexpenses]
(
	[expenseid] ASC
)
INCLUDE ( 	[total]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]