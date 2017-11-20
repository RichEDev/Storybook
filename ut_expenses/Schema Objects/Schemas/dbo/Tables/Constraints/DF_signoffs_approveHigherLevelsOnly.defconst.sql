ALTER TABLE [dbo].[signoffs]
	ADD CONSTRAINT [DF_signoffs_approveHigherLevelsOnly]
	DEFAULT 0
	FOR [approveHigherLevelsOnly]
