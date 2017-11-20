CREATE NONCLUSTERED INDEX [_dta_index_savedexpensesFlags_8_1838253321__K2_K3_K11_K1] ON [dbo].[savedexpensesFlags] 
(
	[expenseid] ASC,
	[flagID] ASC,
	[stepNumber] ASC,
	[flaggedItemId] ASC
)WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
