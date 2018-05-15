-- <Migration ID="255f9224-47e2-449c-8478-5c76740cd0fd" />
GO
ALTER TABLE [dbo].[tables_base]  WITH CHECK ADD  CONSTRAINT [FK_tables_base_fields_base] FOREIGN KEY([primarykey])
REFERENCES [dbo].[fields_base] ([fieldid])
GO

ALTER TABLE [dbo].[tables_base] CHECK CONSTRAINT [FK_tables_base_fields_base]
GO

ALTER TABLE [dbo].[tables_base]  WITH CHECK ADD  CONSTRAINT [FK_tables_base_fields_base1] FOREIGN KEY([keyfield])
REFERENCES [dbo].[fields_base] ([fieldid])
GO

ALTER TABLE [dbo].[tables_base] CHECK CONSTRAINT [FK_tables_base_fields_base1]
GO

ALTER TABLE [dbo].[tables_base]  WITH CHECK ADD  CONSTRAINT [FK_tables_base_tables_base] FOREIGN KEY([userdefined_table])
REFERENCES [dbo].[tables_base] ([tableid])
GO

ALTER TABLE [dbo].[tables_base] CHECK CONSTRAINT [FK_tables_base_tables_base]
GO

ALTER TABLE [dbo].[fields_base]  WITH CHECK ADD  CONSTRAINT [FK_fields_base_fields_base1] FOREIGN KEY([associatedFieldForDuplicateChecking])
REFERENCES [dbo].[fields_base] ([fieldid])
GO

ALTER TABLE [dbo].[fields_base] CHECK CONSTRAINT [FK_fields_base_fields_base1]
GO

ALTER TABLE [dbo].[fields_base]  WITH CHECK ADD  CONSTRAINT [FK_fields_base_fields_base] FOREIGN KEY([lookupfield])
REFERENCES [dbo].[fields_base] ([fieldid])
GO

ALTER TABLE [dbo].[fields_base] CHECK CONSTRAINT [FK_fields_base_fields_base]
GO