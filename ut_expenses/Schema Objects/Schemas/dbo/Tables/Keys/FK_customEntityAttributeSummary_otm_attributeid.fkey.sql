ALTER TABLE [dbo].[customEntityAttributeSummary]
    ADD CONSTRAINT [FK_customEntityAttributeSummary_otm_attributeid] FOREIGN KEY ([otm_attributeid]) REFERENCES [dbo].[customEntityAttributes] ([attributeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

