ALTER TABLE [dbo].[reportcolumns_flatfile]
    ADD CONSTRAINT [FK_reportcolumns_flatfile_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE NO ACTION;

