ALTER TABLE [dbo].[flags]
    ADD CONSTRAINT [DF_flags_active] DEFAULT ((1)) FOR [active];

