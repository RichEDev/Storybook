CREATE NONCLUSTERED INDEX [IX_savedexpenses_costcodes_expenseid_savedcostcodeid_costcodeid] ON [dbo].[savedexpenses_costcodes] 
(
	[expenseid] ASC,
	[savedcostcodeid] ASC,
	[costcodeid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]