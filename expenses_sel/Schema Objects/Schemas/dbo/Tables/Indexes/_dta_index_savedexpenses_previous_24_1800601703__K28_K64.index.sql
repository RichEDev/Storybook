﻿CREATE NONCLUSTERED INDEX [_dta_index_savedexpenses_previous_24_1800601703__K28_K64]
    ON [dbo].[savedexpenses_previous]([convertedtotal] ASC, [transactionid] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];

