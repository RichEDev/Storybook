CREATE NONCLUSTERED INDEX [_dta_index_savedexpenses_journey_steps_47_1077891207__K1_5]
    ON [dbo].[savedexpenses_journey_steps]([expenseid] ASC)
    INCLUDE([num_miles]) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];

