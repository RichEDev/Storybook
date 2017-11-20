CREATE NONCLUSTERED INDEX [_dta_index_claims_base_51_5575058__K3_K15_K7_K4_K1_8_9_10_16_21]
    ON [dbo].[claims_base]([employeeid] ASC, [submitted] ASC, [paid] ASC, [approved] ASC, [claimid] ASC)
    INCLUDE([currencyid], [datepaid], [datesubmitted], [description], [name]) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];

