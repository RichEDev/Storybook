ALTER TABLE [dbo].[subcats_userdefined]
    ADD CONSTRAINT [FK_subcats-userdefined_subcats] FOREIGN KEY ([subcatid]) REFERENCES [dbo].[subcats] ([subcatid]) ON DELETE CASCADE ON UPDATE NO ACTION;

