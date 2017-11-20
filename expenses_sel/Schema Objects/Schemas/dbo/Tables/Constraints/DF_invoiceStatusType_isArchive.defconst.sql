ALTER TABLE [dbo].[invoiceStatusType]
    ADD CONSTRAINT [DF_invoiceStatusType_isArchive] DEFAULT ((0)) FOR [isArchive];

