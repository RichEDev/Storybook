CREATE NONCLUSTERED INDEX [IX_savedexpenses_tempallow_primaryitem] ON [dbo].[savedexpenses]
(
	[tempallow] ASC,
	[primaryitem] ASC
)
INCLUDE ([claimid] ) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]