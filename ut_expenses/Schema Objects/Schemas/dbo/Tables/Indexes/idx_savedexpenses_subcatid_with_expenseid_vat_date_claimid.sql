CREATE NONCLUSTERED INDEX [idx_savedexpenses_subcatid_with_expenseid_vat_date_claimid]
ON [dbo].[savedexpenses] ([subcatid])
INCLUDE ([expenseid],[vat],[date],[claimid])