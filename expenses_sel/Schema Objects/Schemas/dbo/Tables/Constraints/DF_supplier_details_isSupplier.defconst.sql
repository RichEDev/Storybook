ALTER TABLE [dbo].[supplier_details]
    ADD CONSTRAINT [DF_supplier_details_isSupplier] DEFAULT ((0)) FOR [isSupplier];

