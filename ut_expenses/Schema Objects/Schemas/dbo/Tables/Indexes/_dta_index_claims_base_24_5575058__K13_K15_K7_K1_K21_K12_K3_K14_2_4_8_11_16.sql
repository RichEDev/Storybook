CREATE NONCLUSTERED INDEX [_dta_index_claims_base_24_5575058__K13_K15_K7_K1_K21_K12_K3_K14_2_4_8_11_16] ON [dbo].[claims_base]
(
	[checkerid] ASC,
	[submitted] ASC,
	[paid] ASC,
	[claimid] ASC,
	[currencyid] ASC,
	[teamid] ASC,
	[employeeid] ASC,
	[stage] ASC
)
INCLUDE ( 	[claimno],
	[approved],
	[datesubmitted],
	[status],
	[name]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]