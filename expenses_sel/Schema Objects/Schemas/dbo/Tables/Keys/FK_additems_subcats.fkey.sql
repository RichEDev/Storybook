ALTER TABLE [dbo].[additems]
    ADD CONSTRAINT [FK_additems_subcats] FOREIGN KEY ([subcatid]) REFERENCES [dbo].[subcats] ([subcatid]) ON DELETE CASCADE ON UPDATE NO ACTION;

