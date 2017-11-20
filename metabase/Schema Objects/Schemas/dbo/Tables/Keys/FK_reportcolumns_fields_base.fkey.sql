ALTER TABLE [dbo].[reportcolumns]
    ADD CONSTRAINT [FK_reportcolumns_fields_base] FOREIGN KEY ([fieldID]) REFERENCES [dbo].[fields_base] ([fieldid]) ON DELETE CASCADE ON UPDATE NO ACTION;

