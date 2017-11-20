CREATE NONCLUSTERED INDEX [_dta_index_savedexpensesFlags_15_525728097__K2_8] ON [dbo].[savedexpensesFlags] 
(
	[expenseid] ASC
)
INCLUDE ( [flagColour]) WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
