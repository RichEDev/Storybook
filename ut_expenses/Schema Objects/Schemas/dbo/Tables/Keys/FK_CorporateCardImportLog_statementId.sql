ALTER TABLE [dbo].[CorporateCardImportLog]
	ADD CONSTRAINT [FK_CorporateCardImportLog_statementId]
	FOREIGN KEY (StatementId)
	REFERENCES [Card_statements_base] (StatementId) ON DELETE CASCADE
