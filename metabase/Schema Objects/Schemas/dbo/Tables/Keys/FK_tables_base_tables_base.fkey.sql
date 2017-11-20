ALTER TABLE [dbo].[tables_base]
    ADD CONSTRAINT [FK_tables_base_tables_base] FOREIGN KEY ([userdefined_table]) REFERENCES [dbo].[tables_base] ([tableid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

