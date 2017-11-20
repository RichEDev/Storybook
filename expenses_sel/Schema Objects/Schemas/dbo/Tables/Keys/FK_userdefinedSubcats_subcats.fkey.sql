ALTER TABLE [dbo].[userdefinedSubcats]
    ADD CONSTRAINT [FK_userdefinedSubcats_subcats] FOREIGN KEY ([subcatid]) REFERENCES [dbo].[subcats] ([subcatid]) ON DELETE CASCADE ON UPDATE NO ACTION;

