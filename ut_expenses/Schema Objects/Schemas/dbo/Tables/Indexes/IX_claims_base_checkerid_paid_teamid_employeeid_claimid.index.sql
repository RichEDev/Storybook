CREATE NONCLUSTERED INDEX [IX_claims_base_checkerid_paid_teamid_employeeid_claimid]
    ON [dbo].[claims_base]([checkerid] ASC, [paid] ASC, [teamid] ASC, [employeeid] ASC, [claimid] ASC)
    INCLUDE([approved], [claimno], [currencyid], [name], [stage], [status]) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];

