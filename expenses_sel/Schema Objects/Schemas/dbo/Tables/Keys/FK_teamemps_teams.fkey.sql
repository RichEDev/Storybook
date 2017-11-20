ALTER TABLE [dbo].[teamemps]
    ADD CONSTRAINT [FK_teamemps_teams] FOREIGN KEY ([teamid]) REFERENCES [dbo].[teams] ([teamid]) ON DELETE CASCADE ON UPDATE CASCADE;

