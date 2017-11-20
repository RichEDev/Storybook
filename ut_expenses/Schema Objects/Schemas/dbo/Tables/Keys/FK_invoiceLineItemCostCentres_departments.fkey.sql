ALTER TABLE [dbo].[invoiceLineItemCostCentres]
    ADD CONSTRAINT [FK_invoiceLineItemCostCentres_departments] FOREIGN KEY ([departmentId]) REFERENCES [dbo].[departments] ([departmentid]) ON DELETE CASCADE ON UPDATE CASCADE;

