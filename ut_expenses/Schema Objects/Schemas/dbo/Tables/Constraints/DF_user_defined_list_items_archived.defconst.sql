ALTER TABLE [dbo].[userdefined_list_items]
	ADD CONSTRAINT [DF_userdefined_list_items_archived]
	DEFAULT 0
	FOR [archived]
