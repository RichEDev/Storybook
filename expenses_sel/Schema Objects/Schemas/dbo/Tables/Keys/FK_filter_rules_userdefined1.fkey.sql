ALTER TABLE [dbo].[filter_rules]
    ADD CONSTRAINT [FK_filter_rules_userdefined1] FOREIGN KEY ([childuserdefineid]) REFERENCES [dbo].[userdefined] ([userdefineid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

