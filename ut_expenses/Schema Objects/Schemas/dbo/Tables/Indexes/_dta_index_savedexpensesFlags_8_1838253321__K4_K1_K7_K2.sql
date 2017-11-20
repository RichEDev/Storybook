CREATE NONCLUSTERED INDEX [_dta_index_savedexpensesFlags_8_1838253321__K4_K1_K7_K2] ON [dbo].[savedexpensesFlags] 
(
	[flagType] ASC,
	[flaggedItemId] ASC,
	[duplicateExpenseID] ASC,
	[expenseid] ASC
)WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
