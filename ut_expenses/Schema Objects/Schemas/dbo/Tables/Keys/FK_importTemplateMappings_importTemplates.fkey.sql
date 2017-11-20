ALTER TABLE [dbo].[importTemplateMappings]
    ADD CONSTRAINT [FK_importTemplateMappings_importTemplates] FOREIGN KEY ([templateID]) REFERENCES [dbo].[importTemplates] ([templateID]) ON DELETE CASCADE ON UPDATE NO ACTION;

