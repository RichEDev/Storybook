CREATE NONCLUSTERED INDEX IX_RoleSubCats_RoleId_SubCatId ON [dbo].[rolesubcats]
(
	[roleid] ASC,
	[subcatid] ASC
)
INCLUDE ( 	[maximum],
	[receiptmaximum]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]