ALTER TABLE [dbo].[savedexpenses_current]
    ADD CONSTRAINT [FK_savedexpenses_current_claims_base] FOREIGN KEY ([claimid]) REFERENCES [dbo].[claims_base] ([claimid]) ON DELETE CASCADE ON UPDATE NO ACTION;

