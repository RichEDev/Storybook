ALTER TABLE [dbo].[emailNotifications]
    ADD CONSTRAINT [DF_emailNotifications_customerType] DEFAULT ((1)) FOR [customerType];

