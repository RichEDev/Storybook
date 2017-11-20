ALTER TABLE [dbo].[customFields]
    ADD CONSTRAINT [DF_customFields_valuelist] DEFAULT ((0)) FOR [valuelist];

