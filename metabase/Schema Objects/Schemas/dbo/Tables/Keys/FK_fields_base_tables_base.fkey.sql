ALTER TABLE [dbo].[fields_base]
    ADD CONSTRAINT [FK_fields_base_tables_base] FOREIGN KEY ([tableid]) REFERENCES [dbo].[tables_base] ([tableid]) ON DELETE CASCADE ON UPDATE NO ACTION;

