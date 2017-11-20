ALTER TABLE [dbo].[claims_base]
    ADD CONSTRAINT [DF_claims_approved] DEFAULT ((0)) FOR [approved];

