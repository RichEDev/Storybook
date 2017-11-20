ALTER TABLE [dbo].[claims_base]
    ADD CONSTRAINT [DF_claims_status] DEFAULT ((0)) FOR [status];

