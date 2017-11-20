CREATE NONCLUSTERED INDEX [_dta_index_savedexpensesFlags_8_1838253321__K1_K2] ON [dbo].[savedexpensesFlags] 
(
	[flaggedItemId] ASC,
	[expenseid] ASC
)WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]