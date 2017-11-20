ALTER TABLE [dbo].[contract_productdetails]
    ADD CONSTRAINT [DF_Contract_ProductDetails_ArchivedStatus] DEFAULT ((0)) FOR [archiveStatus];

