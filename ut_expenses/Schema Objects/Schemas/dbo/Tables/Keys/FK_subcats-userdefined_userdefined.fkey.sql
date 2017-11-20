ALTER TABLE [dbo].[subcats_userdefined]
    ADD CONSTRAINT [FK_subcats-userdefined_userdefined] FOREIGN KEY ([userdefineid]) REFERENCES [dbo].[userdefined] ([userdefineid]) ON DELETE CASCADE ON UPDATE NO ACTION;

