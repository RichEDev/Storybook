ALTER TABLE [dbo].[claimhistory]
    ADD CONSTRAINT [FK_claimhistory_claims_base] FOREIGN KEY ([claimid]) REFERENCES [dbo].[claims_base] ([claimid]) ON DELETE CASCADE ON UPDATE NO ACTION;

