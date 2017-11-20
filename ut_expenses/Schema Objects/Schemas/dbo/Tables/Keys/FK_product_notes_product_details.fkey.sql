ALTER TABLE [dbo].[productNotes]
    ADD CONSTRAINT [FK_product_notes_product_details] FOREIGN KEY ([productID]) REFERENCES [dbo].[productDetails] ([productId]) ON DELETE CASCADE ON UPDATE NO ACTION;

