ALTER TABLE [dbo].[emailNotificationLink]
    ADD CONSTRAINT [FK_emailNotificationLink_emailNotificationLink] FOREIGN KEY ([teamID]) REFERENCES [dbo].[teams] ([teamid]) ON DELETE CASCADE ON UPDATE NO ACTION;

