ALTER TABLE [dbo].[fields_base]  WITH CHECK ADD  CONSTRAINT [FK_fields_base_fields_base1] FOREIGN KEY([associatedFieldForDuplicateChecking])
REFERENCES [dbo].[fields_base] ([fieldid])
GO

ALTER TABLE [dbo].[fields_base] CHECK CONSTRAINT [FK_fields_base_fields_base1]
GO