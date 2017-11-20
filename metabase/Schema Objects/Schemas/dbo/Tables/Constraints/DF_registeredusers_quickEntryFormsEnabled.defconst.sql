ALTER TABLE [dbo].[registeredusers]
    ADD CONSTRAINT [DF_registeredusers_quickEntryFormsEnabled] DEFAULT ((1)) FOR [quickEntryFormsEnabled];

