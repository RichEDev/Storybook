CREATE NONCLUSTERED INDEX [_dta_index_claims_base_15_5575058__K1_15] ON [dbo].[claims_base] 
(
	[claimid] ASC
)
INCLUDE ( [submitted]) WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
