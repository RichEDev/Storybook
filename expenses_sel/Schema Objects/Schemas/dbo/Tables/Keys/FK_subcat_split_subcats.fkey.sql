ALTER TABLE [dbo].[subcat_split]
    ADD CONSTRAINT [FK_subcat_split_subcats] FOREIGN KEY ([primarysubcatid]) REFERENCES [dbo].[subcats] ([subcatid]) ON DELETE CASCADE ON UPDATE NO ACTION;

