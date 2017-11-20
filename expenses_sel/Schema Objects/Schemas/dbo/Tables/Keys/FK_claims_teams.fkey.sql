ALTER TABLE [dbo].[claims_base]
    ADD CONSTRAINT [FK_claims_teams] FOREIGN KEY ([teamid]) REFERENCES [dbo].[teams] ([teamid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

