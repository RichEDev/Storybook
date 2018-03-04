CREATE NONCLUSTERED INDEX [idx_savedexpenses_vat_with_expenseid_subcatid_date_claimid]
ON [dbo].[savedexpenses] ([vat])
INCLUDE ([expenseid],[subcatid],[date],[claimid])