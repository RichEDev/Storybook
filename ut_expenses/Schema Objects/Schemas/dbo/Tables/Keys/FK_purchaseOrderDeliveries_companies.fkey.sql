ALTER TABLE [dbo].[purchaseOrderDeliveries]
    ADD CONSTRAINT [FK_purchaseOrderDeliveries_companies] FOREIGN KEY ([locationID]) REFERENCES [dbo].[companies] ([companyid]) ON DELETE CASCADE ON UPDATE CASCADE;

