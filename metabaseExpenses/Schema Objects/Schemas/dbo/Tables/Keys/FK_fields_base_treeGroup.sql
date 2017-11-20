ALTER TABLE [dbo].[fields_base]  ADD  CONSTRAINT [FK_fields_base_treeGroup] FOREIGN KEY([treeGroup])
REFERENCES [dbo].[TreeGroup] ([Id])
GO
