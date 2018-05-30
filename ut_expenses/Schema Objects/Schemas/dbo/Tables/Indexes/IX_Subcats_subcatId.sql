CREATE NONCLUSTERED INDEX IX_Subcats_subcatId ON [dbo].[subcats]
(
	[subcatid] ASC
)
INCLUDE ( 	[categoryid],
	[description],
	[subcat],
	[calculation],
	[shortsubcat],
	[fromapp],
	[toapp]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
