ALTER TABLE [dbo].[subcat_split]
    ADD CONSTRAINT [FK_subcat_split_subcats1] FOREIGN KEY ([splitsubcatid]) REFERENCES [dbo].[subcats] ([subcatid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

