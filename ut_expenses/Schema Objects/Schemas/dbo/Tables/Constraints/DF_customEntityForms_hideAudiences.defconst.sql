ALTER TABLE [dbo].[customEntityForms]
	ADD CONSTRAINT [DF_customEntityForms_hideAudiences]
	DEFAULT (0)
	FOR [hideAudiences]
