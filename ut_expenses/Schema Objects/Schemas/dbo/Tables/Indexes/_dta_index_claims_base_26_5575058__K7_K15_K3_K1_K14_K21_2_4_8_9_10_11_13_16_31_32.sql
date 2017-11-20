CREATE NONCLUSTERED INDEX [_dta_index_claims_base_26_5575058__K7_K15_K3_K1_K14_K21_2_4_8_9_10_11_13_16_31_32] ON [dbo].[claims_base]
(
	[paid] ASC,
	[submitted] ASC,
	[employeeid] ASC,
	[claimid] ASC,
	[stage] ASC,
	[currencyid] ASC
)
INCLUDE ( 	[claimno],
	[approved],
	[datesubmitted],
	[datepaid],
	[description],
	[status],
	[checkerid],
	[name],
	[ReferenceNumber],
	[splitApprovalStage]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]