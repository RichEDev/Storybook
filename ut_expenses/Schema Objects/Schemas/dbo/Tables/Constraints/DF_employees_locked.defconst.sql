ALTER TABLE [dbo].[employees] 
	ADD  CONSTRAINT [DF_employees_locked] DEFAULT 0 FOR [locked]
