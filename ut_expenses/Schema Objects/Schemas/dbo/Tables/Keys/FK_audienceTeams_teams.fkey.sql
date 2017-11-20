ALTER TABLE [dbo].[audienceTeams]
    ADD CONSTRAINT [FK_audienceTeams_teams] FOREIGN KEY ([teamID]) REFERENCES [dbo].[teams] ([teamid]) ON DELETE CASCADE ON UPDATE CASCADE;

