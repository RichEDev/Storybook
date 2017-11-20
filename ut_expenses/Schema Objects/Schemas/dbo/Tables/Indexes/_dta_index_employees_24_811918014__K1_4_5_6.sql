CREATE NONCLUSTERED INDEX [_dta_index_employees_24_811918014__K1_4_5_6] ON [dbo].[employees]
(
	[employeeid] ASC
)
INCLUDE ( 	[title],
	[firstname],
	[surname]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]