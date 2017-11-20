ALTER TABLE [dbo].[productDetails]
    ADD CONSTRAINT [FK_product_details_employees] FOREIGN KEY ([notifyId]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

