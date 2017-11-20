ALTER TABLE [dbo].[productDetails]
    ADD CONSTRAINT [DF_productdetails_archived] DEFAULT ((0)) FOR [Archived];

