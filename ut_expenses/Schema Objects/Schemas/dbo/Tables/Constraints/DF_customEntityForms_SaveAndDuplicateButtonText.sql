ALTER TABLE [dbo].[customEntityForms]
	ADD CONSTRAINT [DF_customEntityForms_SaveAndDuplicateButtonText]
	DEFAULT (N'Save and Duplicate')
	FOR [SaveAndDuplicateButtonText]
