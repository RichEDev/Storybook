ALTER TABLE [dbo].[rolesubcats]
    ADD CONSTRAINT [FK_rolesubcats_subcats] FOREIGN KEY ([subcatid]) REFERENCES [dbo].[subcats] ([subcatid]) ON DELETE CASCADE ON UPDATE NO ACTION;

