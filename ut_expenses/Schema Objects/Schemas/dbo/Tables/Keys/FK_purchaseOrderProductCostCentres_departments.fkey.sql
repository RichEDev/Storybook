ALTER TABLE [dbo].[purchaseOrderProductCostCentres]
    ADD CONSTRAINT [FK_purchaseOrderProductCostCentres_departments] FOREIGN KEY ([departmentID]) REFERENCES [dbo].[departments] ([departmentid]) ON DELETE CASCADE ON UPDATE CASCADE;

