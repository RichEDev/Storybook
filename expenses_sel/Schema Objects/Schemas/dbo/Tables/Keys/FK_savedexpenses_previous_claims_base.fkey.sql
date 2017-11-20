ALTER TABLE [dbo].[savedexpenses_previous]
    ADD CONSTRAINT [FK_savedexpenses_previous_claims_base] FOREIGN KEY ([claimid]) REFERENCES [dbo].[claims_base] ([claimid]) ON DELETE CASCADE ON UPDATE NO ACTION;

