ALTER TABLE [dbo].[invoices]
    ADD CONSTRAINT [DF__invoice_d__PO Ma__77BE66AD] DEFAULT ((0)) FOR [poMaxValue];

