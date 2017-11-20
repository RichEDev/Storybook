ALTER TABLE [dbo].[audienceTeams]
    ADD CONSTRAINT [FK_audienceTeams_audiences] FOREIGN KEY ([audienceID]) REFERENCES [dbo].[audiences] ([audienceID]) ON DELETE CASCADE ON UPDATE CASCADE;

