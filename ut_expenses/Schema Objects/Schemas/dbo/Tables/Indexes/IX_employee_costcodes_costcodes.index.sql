CREATE NONCLUSTERED INDEX [IX_employee_costcodes_costcodes] ON [dbo].[employee_costcodes]
(
	[costcodeid] ASC
)
INCLUDE ( 	[employeeid]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
