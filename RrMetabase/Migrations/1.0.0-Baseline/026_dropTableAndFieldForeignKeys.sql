-- <Migration ID="b87dd2bc-14c4-419e-92e5-6375ba2225e8" />
GO
ALTER TABLE [dbo].[tables_base] DROP CONSTRAINT [FK_tables_base_fields_base]
GO
ALTER TABLE [dbo].[tables_base] DROP CONSTRAINT [FK_tables_base_fields_base1]
GO
ALTER TABLE [dbo].[tables_base] DROP CONSTRAINT [FK_tables_base_tables_base]
GO
ALTER TABLE [dbo].[fields_base] DROP CONSTRAINT [FK_fields_base_fields_base1]
GO
ALTER TABLE [dbo].[fields_base] DROP CONSTRAINT [FK_fields_base_fields_base]
GO