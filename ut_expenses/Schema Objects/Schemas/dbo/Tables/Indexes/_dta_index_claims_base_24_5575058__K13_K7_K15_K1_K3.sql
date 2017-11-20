CREATE NONCLUSTERED INDEX [_dta_index_claims_base_24_5575058__K13_K7_K15_K1_K3] ON [dbo].[claims_base]
(
	[checkerid] ASC,
	[paid] ASC,
	[submitted] ASC,
	[claimid] ASC,
	[employeeid] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]