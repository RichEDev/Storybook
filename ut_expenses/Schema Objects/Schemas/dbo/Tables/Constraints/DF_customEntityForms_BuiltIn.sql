ALTER TABLE [dbo].[customEntityForms]
	ADD CONSTRAINT [DF_customEntityForms_BuiltIn] DEFAULT (0) FOR [BuiltIn];