ALTER TABLE [dbo].[locales]
    ADD CONSTRAINT [DF_locales_active] DEFAULT ((0)) FOR [active];

