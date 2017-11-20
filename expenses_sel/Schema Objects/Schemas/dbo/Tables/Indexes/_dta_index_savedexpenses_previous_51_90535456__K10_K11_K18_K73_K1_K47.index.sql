﻿CREATE NONCLUSTERED INDEX [_dta_index_savedexpenses_previous_51_90535456__K10_K11_K18_K73_K1_K47]
    ON [dbo].[savedexpenses_previous]([subcatid] ASC, [date] ASC, [claimid] ASC, [mileageid] ASC, [expenseid] ASC, [carid] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];

