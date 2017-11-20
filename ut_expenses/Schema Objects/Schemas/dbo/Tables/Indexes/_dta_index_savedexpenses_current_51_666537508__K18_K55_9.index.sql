﻿CREATE NONCLUSTERED INDEX [_dta_index_savedexpenses_51_666537508__K18_K55_9]
    ON [dbo].[savedexpenses]([claimid] ASC, [primaryitem] ASC)
    INCLUDE([total]) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];

