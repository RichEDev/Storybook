ALTER TABLE [dbo].[invoiceLineItemCostCentres]
    ADD CONSTRAINT [FK_invoiceLineItemCostCentres_project_codes] FOREIGN KEY ([projectCodeId]) REFERENCES [dbo].[project_codes] ([projectcodeid]) ON DELETE CASCADE ON UPDATE CASCADE;

