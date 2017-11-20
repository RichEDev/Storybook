CREATE NONCLUSTERED INDEX [_dta_index_signoffs_24_661577395__K8_K3_2] ON [dbo].[signoffs]
(
	[stage] ASC,
	[signofftype] ASC
)
INCLUDE ( 	[groupid]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]