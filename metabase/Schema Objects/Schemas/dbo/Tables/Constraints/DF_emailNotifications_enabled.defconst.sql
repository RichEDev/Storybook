ALTER TABLE [dbo].[emailNotifications]
    ADD CONSTRAINT [DF_emailNotifications_enabled] DEFAULT ((0)) FOR [enabled];

