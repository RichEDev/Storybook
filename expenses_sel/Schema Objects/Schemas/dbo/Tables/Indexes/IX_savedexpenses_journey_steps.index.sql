﻿CREATE NONCLUSTERED INDEX [IX_savedexpenses_journey_steps]
    ON [dbo].[savedexpenses_journey_steps]([expenseid] ASC)
    INCLUDE([start_location]) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];

