ALTER TABLE [dbo].[invoiceStatusType]
    ADD CONSTRAINT [DF_invoiceStatusType_archived] DEFAULT ((0)) FOR [archived];

