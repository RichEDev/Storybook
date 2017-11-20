﻿CREATE NONCLUSTERED INDEX [IX_savedexpenses_itemtype_claimid_normalreceipt_expenseid_tempallow_subcatid]
    ON [dbo].[savedexpenses]([itemtype] ASC, [claimid] ASC, [normalreceipt] ASC, [expenseid] ASC, [tempallow] ASC, [subcatid] ASC)
    INCLUDE([total]) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];

