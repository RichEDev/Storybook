ALTER TABLE [dbo].[purchaseOrders]
    ADD CONSTRAINT [FK_purchaseOrders_currencies] FOREIGN KEY ([currencyID]) REFERENCES [dbo].[currencies] ([currencyid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

