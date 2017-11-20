ALTER TABLE [dbo].[customEntityForms]
	ADD CONSTRAINT [DF_customEntityForms_showSaveAndDuplicate]
	DEFAULT ((1))
	FOR [showSaveAndDuplicate]
