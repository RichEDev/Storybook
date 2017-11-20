ALTER TABLE [dbo].[fields_base]
    ADD CONSTRAINT [FK_fields_base_tables_base1] FOREIGN KEY ([lookuptable]) REFERENCES [dbo].[tables_base] ([tableid]) ON DELETE NO ACTION ON UPDATE SET NULL;

