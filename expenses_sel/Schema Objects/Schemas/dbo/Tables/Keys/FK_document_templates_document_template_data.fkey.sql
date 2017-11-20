ALTER TABLE [dbo].[document_template_data]
    ADD CONSTRAINT [FK_document_templates_document_template_data] FOREIGN KEY ([documentid]) REFERENCES [dbo].[document_templates] ([documentid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

