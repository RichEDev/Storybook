ALTER TABLE [dbo].[userdefinedClaims]
    ADD CONSTRAINT [FK_userdefinedClaims_claims_base] FOREIGN KEY ([claimid]) REFERENCES [dbo].[claims_base] ([claimid]) ON DELETE CASCADE ON UPDATE NO ACTION;

