ALTER TABLE [dbo].[reportcolumns_flatfile]
    ADD CONSTRAINT [FK_reportcolumns_flatfile_reportcolumns] FOREIGN KEY ([reportcolumnid]) REFERENCES [dbo].[reportcolumns] ([reportcolumnid]) ON DELETE CASCADE ON UPDATE NO ACTION;

