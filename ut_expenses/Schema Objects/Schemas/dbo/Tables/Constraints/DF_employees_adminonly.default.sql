ALTER TABLE [dbo].[employees]
	ADD CONSTRAINT [DF_employees_adminonly] DEFAULT 0 FOR [adminonly];