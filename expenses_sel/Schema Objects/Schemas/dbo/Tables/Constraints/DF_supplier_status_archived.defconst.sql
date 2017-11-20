ALTER TABLE [dbo].[supplier_status]
    ADD CONSTRAINT [DF_supplier_status_archived] DEFAULT ((0)) FOR [archived];

