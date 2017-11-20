ALTER TABLE [dbo].[customEntityForms]
	ADD CONSTRAINT [DF_customEntityForms_hideAttachments]
	DEFAULT (0)
	FOR [hideAttachments]
