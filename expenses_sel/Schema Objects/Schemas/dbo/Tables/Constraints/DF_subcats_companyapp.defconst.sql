ALTER TABLE [dbo].[subcats]
    ADD CONSTRAINT [DF_subcats_companyapp] DEFAULT ((0)) FOR [companyapp];

