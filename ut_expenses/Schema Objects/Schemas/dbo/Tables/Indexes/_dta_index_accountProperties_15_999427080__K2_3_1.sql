CREATE NONCLUSTERED INDEX [_dta_index_accountProperties_15_999427080__K2_3] ON [dbo].[accountProperties] 
(
	[stringKey] ASC
)
INCLUDE ( [stringValue]) WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]