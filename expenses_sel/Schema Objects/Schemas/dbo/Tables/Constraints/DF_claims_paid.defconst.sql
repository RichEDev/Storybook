ALTER TABLE [dbo].[claims_base]
    ADD CONSTRAINT [DF_claims_paid] DEFAULT ((0)) FOR [paid];

