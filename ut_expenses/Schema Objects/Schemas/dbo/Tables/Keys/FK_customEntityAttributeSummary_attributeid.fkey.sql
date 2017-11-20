ALTER TABLE [dbo].[customEntityAttributeSummary]
    ADD CONSTRAINT [FK_customEntityAttributeSummary_attributeid] FOREIGN KEY ([attributeid]) REFERENCES [dbo].[customEntityAttributes] ([attributeid]) ON DELETE CASCADE ON UPDATE NO ACTION;

