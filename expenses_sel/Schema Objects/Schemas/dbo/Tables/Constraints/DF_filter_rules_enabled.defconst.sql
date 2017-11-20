ALTER TABLE [dbo].[filter_rules]
    ADD CONSTRAINT [DF_filter_rules_enabled] DEFAULT ((0)) FOR [enabled];

