ALTER TABLE [dbo].[claims_base]
    ADD CONSTRAINT [DF_claims_submitted] DEFAULT ((0)) FOR [submitted];

