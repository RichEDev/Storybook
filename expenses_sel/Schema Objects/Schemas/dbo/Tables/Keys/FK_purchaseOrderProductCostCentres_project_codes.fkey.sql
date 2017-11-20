ALTER TABLE [dbo].[purchaseOrderProductCostCentres]
    ADD CONSTRAINT [FK_purchaseOrderProductCostCentres_project_codes] FOREIGN KEY ([projectCodeID]) REFERENCES [dbo].[project_codes] ([projectcodeid]) ON DELETE CASCADE ON UPDATE CASCADE;

