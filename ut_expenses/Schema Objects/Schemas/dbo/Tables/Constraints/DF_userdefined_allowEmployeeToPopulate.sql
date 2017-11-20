ALTER TABLE [dbo].[userdefined]
	ADD CONSTRAINT [DF_userdefined_allowEmployeeToPopulate]
	DEFAULT (0)
	FOR [allowEmployeeToPopulate]
