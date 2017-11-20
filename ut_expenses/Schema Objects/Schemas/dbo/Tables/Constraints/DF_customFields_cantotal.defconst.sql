ALTER TABLE [dbo].[customFields]
    ADD CONSTRAINT [DF_customFields_cantotal] DEFAULT ((0)) FOR [cantotal];

