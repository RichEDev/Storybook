CREATE NONCLUSTERED INDEX [idx_CostCodes_CostCodeId_CostCode_Archived] ON [dbo].[costcodes]
(
	[costcode] ASC,
	[costcodeid] ASC,
	[archived] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]