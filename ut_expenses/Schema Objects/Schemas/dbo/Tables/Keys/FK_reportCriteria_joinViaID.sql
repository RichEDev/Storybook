ALTER TABLE [dbo].[reportCriteria]
	ADD CONSTRAINT [FK_reportCriteria_joinViaID]
	FOREIGN KEY (joinViaID)
	REFERENCES [joinVia] (joinViaID)
