ALTER TABLE [dbo].[supplier_details]
    ADD CONSTRAINT [DF_supplier_details_isReseller] DEFAULT ((0)) FOR [isReseller];

