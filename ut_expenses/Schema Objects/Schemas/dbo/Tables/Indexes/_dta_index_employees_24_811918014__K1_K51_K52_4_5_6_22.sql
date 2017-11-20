CREATE NONCLUSTERED INDEX [_dta_index_employees_24_811918014__K1_K51_K52_4_5_6_22] ON [dbo].[employees]
(
	[employeeid] ASC,
	[groupidcc] ASC,
	[groupidpc] ASC
)
INCLUDE ( 	[title],
	[firstname],
	[surname],
	[groupid]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]