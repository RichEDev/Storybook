ALTER TABLE [dbo].[ESRElementFields]
    ADD CONSTRAINT [FK_ESRElementFields_reportcolumns] FOREIGN KEY ([reportColumnID]) REFERENCES [dbo].[reportcolumns] ([reportcolumnid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

