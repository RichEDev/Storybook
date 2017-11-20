ALTER TABLE [dbo].[customEntityAttributeListItems]
	ADD CONSTRAINT [DF_customEntityAttributeListItems_archived]
	DEFAULT 0
	FOR [archived]
