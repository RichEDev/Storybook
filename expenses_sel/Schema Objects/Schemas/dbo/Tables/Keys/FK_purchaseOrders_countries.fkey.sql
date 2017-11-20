ALTER TABLE [dbo].[purchaseOrders]
    ADD CONSTRAINT [FK_purchaseOrders_countries] FOREIGN KEY ([countryID]) REFERENCES [dbo].[countries] ([countryid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

