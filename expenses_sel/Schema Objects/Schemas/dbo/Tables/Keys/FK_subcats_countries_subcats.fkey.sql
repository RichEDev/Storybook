ALTER TABLE [dbo].[subcats_countries]
    ADD CONSTRAINT [FK_subcats_countries_subcats] FOREIGN KEY ([subcatid]) REFERENCES [dbo].[subcats] ([subcatid]) ON DELETE CASCADE ON UPDATE NO ACTION;

