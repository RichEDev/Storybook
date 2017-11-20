ALTER TABLE [dbo].[subcats]
    ADD CONSTRAINT [FK_subcats_pdcats] FOREIGN KEY ([pdcatid]) REFERENCES [dbo].[pdcats] ([pdcatid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

