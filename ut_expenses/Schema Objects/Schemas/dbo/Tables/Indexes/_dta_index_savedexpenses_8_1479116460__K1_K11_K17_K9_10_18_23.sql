CREATE NONCLUSTERED INDEX [_dta_index_savedexpenses_8_1479116460__K1_K11_K17_K9_10_18_23] ON [dbo].[savedexpenses] 
(
	[expenseid] ASC,
	[date] ASC,
	[refnum] ASC,
	[total] ASC
)
INCLUDE ( [subcatid],
[claimid],
[currencyid]) WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
