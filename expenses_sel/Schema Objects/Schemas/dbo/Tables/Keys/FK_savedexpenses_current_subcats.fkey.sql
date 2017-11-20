ALTER TABLE [dbo].[savedexpenses_current]
    ADD CONSTRAINT [FK_savedexpenses_current_subcats] FOREIGN KEY ([subcatid]) REFERENCES [dbo].[subcats] ([subcatid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

