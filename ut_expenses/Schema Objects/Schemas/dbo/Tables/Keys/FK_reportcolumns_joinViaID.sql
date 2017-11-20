ALTER TABLE [dbo].[reportcolumns]
	ADD CONSTRAINT [FK_reportcolumns_joinViaID]
	FOREIGN KEY (joinViaID)
	REFERENCES [JoinVia] (JoinViaID)
