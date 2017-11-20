ALTER TABLE [dbo].[mobileAPIVersion]
	ADD CONSTRAINT [FK_mobileAPIVersion_mobileAPITypes] 
	FOREIGN KEY ([mobileAPITypeID])
	REFERENCES [dbo].[mobileAPITypes] ([API_TypeId]) ON DELETE CASCADE
