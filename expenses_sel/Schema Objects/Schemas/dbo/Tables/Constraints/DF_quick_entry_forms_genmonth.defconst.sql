ALTER TABLE [dbo].[quick_entry_forms]
    ADD CONSTRAINT [DF_quick_entry_forms_genmonth] DEFAULT (0) FOR [genmonth];

