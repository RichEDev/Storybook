ALTER TABLE [dbo].[employees]
    ADD CONSTRAINT [DF_employees_logonCount] DEFAULT ((0)) FOR [logonCount];

