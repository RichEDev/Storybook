ALTER TABLE [dbo].[filter_rules]
    ADD CONSTRAINT [FK_filter_rules_userdefined] FOREIGN KEY ([paruserdefineid]) REFERENCES [dbo].[userdefined] ([userdefineid]) ON DELETE CASCADE ON UPDATE NO ACTION;

