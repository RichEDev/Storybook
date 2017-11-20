ALTER TABLE [dbo].[ESRElementSubcats]
    ADD CONSTRAINT [FK_subcats_esrElementSubcats] FOREIGN KEY ([subcatID]) REFERENCES [dbo].[subcats] ([subcatid]) ON DELETE CASCADE ON UPDATE NO ACTION;

