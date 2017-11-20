ALTER TABLE [dbo].[filter_rule_values]
    ADD CONSTRAINT [DF_filter_rules_filtertype] DEFAULT ((0)) FOR [filterid];

