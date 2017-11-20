ALTER TABLE [dbo].[employees]
    ADD CONSTRAINT [DF_employees_passwordMethod] DEFAULT ((4)) FOR [passwordMethod];

