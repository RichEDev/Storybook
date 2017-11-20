ALTER TABLE [dbo].[tables_base]
    ADD CONSTRAINT [FK_tables_base_fields_base] FOREIGN KEY ([primarykey]) REFERENCES [dbo].[fields_base] ([fieldid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

