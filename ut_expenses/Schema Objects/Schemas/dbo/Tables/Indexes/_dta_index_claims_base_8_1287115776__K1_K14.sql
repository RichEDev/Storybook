CREATE NONCLUSTERED INDEX [_dta_index_claims_base_8_1287115776__K1_K14] ON [dbo].[claims_base] 
(
	[claimid] ASC,
	[name] ASC
)WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
