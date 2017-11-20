ALTER TABLE [dbo].[filter_rule_values]
    ADD CONSTRAINT [FK_filter_rule_values_filter_rules] FOREIGN KEY ([filterid]) REFERENCES [dbo].[filter_rules] ([filterid]) ON DELETE CASCADE ON UPDATE NO ACTION;

