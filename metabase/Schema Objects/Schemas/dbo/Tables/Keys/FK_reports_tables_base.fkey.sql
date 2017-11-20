ALTER TABLE [dbo].[reports]
    ADD CONSTRAINT [FK_reports_tables_base] FOREIGN KEY ([basetable]) REFERENCES [dbo].[tables_base] ([tableid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

