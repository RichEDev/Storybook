ALTER TABLE [dbo].[defineditems]
    ADD CONSTRAINT [FK_defineditems_userdefined] FOREIGN KEY ([userdefineid]) REFERENCES [dbo].[userdefined] ([userdefineid]) ON DELETE CASCADE ON UPDATE CASCADE;

