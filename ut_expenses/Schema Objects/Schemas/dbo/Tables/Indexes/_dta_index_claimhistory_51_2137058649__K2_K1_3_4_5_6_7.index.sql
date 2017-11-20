CREATE NONCLUSTERED INDEX [_dta_index_claimhistory_51_2137058649__K2_K1_3_4_5_6_7]
    ON [dbo].[claimhistory]([claimid] ASC, [claimhistoryid] ASC)
    INCLUDE([comment], [datestamp], [employeeid], [refnum], [stage]) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];

