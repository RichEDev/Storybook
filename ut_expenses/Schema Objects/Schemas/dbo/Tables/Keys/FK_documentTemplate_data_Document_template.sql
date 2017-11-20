ALTER TABLE [dbo].[document_template_data]
	ADD CONSTRAINT [FK_documentTemplate_data_Document_template]
	FOREIGN KEY (documentid)
	REFERENCES [document_templates] (documentid)
